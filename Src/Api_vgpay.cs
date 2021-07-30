using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using VGPay.sdk.Models;

namespace VGPay.sdk
{
    /// <summary>
    /// vgpay接口对接程序
    /// </summary>
    public class Api_vgpay
    {
        /// <summary>
        /// 日志接口
        /// </summary>
        public readonly ILogger _logger;
        /// <summary>
        /// 密钥
        /// </summary>
        private readonly string _secret_key;
        /// <summary>
        /// 商户id
        /// </summary>
        private readonly string _businessId;
        /// <summary>
        /// rest客户端
        /// </summary>
        private readonly RestClient _client;
        /// <summary>
        /// 日志事件ID
        /// </summary>
        /// <returns></returns>
        private readonly EventId eventId = new EventId(0, "(api)vgpay");
        /// <summary>
        /// 临时变量
        /// </summary>
        /// <returns></returns>
        private StringBuilder str = new StringBuilder();
        /// <summary>
        /// 临时变量
        /// </summary>
        /// <returns></returns>
        private StringBuilder str1 = new StringBuilder();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="logger">日志接口</param>
        /// <param name="baseUrl">域名</param>
        /// <param name="secret_key">密钥</param>
        /// <param name="businessId">商户ID</param>
        public Api_vgpay(ILogger logger, string baseUrl, string secret_key, string businessId)
        {
            this._logger = logger ?? NullLogger.Instance;
            this._secret_key = secret_key;
            this._businessId = businessId;
            this._client = new RestClient(baseUrl);
        }

        /// <summary>
        /// 请求web数据
        /// </summary>
        /// <param name="client">web请求控件</param>
        /// <param name="method">请求类型</param>
        /// <param name="url">接口地址</param>
        /// <param name="map">输入数据</param>
        /// <typeparam name="T">输出类型</typeparam>
        /// <returns>返回数据</returns>
        private Response_vgpay<T> Request<T>(Method method, string url, Dictionary<string, string> map)
        {
            map.Add("businessId", this._businessId);
            map.Add("timeStamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            string md5 = GetMac(map);
            map.Add("mac", md5);
            str.Clear();
            foreach (var item in map)
            {
                str.Append($"{item.Key}={item.Value}&");
            }
            string namevalue = str.ToString().TrimEnd('&');
            RestRequest request = new RestRequest(url + "?" + namevalue, method);
            try
            {
                IRestResponse<Response_vgpay<T>> asyncHandle = this._client.Execute<Response_vgpay<T>>(request);
                this._logger.LogTrace(eventId, $"method:{method.ToString()},url:{url},参数:{JsonConvert.SerializeObject(map)},结果:{asyncHandle.Content}");
                if (asyncHandle.Data == null)
                {
                    Response_vgpay<T> t = default(Response_vgpay<T>);
                    return t;
                }
                if (asyncHandle.Data.code == 200 && !VerifyMac(asyncHandle.Content, asyncHandle.Data.mac))
                {
                    asyncHandle.Data.isSuccess = false;
                    asyncHandle.Data.code = 401;
                }
                return asyncHandle.Data;
            }
            catch (Exception ex)
            {
                this._logger.LogError(eventId, ex, $"method:{method.ToString()},url:{url},参数:{JsonConvert.SerializeObject(map)},异常:{ex.Message}");
            }
            return default(Response_vgpay<T>);
        }

        /// <summary>
        /// 获取mac值
        /// </summary>
        /// <param name="map">参数</param>
        /// <returns></returns>
        public string GetMac(Dictionary<string, string> map)
        {
            str.Clear();
            List<KeyValuePair<string, string>> order = map.OrderBy(P => P.Key).ToList();
            foreach (var item in order)
            {
                if (string.IsNullOrWhiteSpace(item.Value))
                {
                    continue;
                }
                str.Append($"{item.Key}={item.Value}&");
            }
            str.Append($"secretKey={this._secret_key}");
            string md5 = Md5Hash(str.ToString().TrimEnd('&'));
            str.Clear();
            return md5;
        }

        /// <summary>
        /// 校验mac值
        /// </summary>
        /// <param name="json">返回字符串</param>
        /// <param name="model">返回对象</param>
        /// <returns></returns>
        private bool VerifyMac(string json, string mac)
        {
            try
            {
                str1.Clear();
                JToken token = JObject.Parse(json)["data"];
                if (token.Type == JTokenType.Array)
                {
                    foreach (JObject item in token.Children())
                    {
                        IOrderedEnumerable<JProperty> prop = item.Properties().OrderBy(P => P.Name);
                        foreach (var item1 in prop)
                        {
                            if (string.IsNullOrWhiteSpace(item[item1.Name].ToString()))
                            {
                                continue;
                            }
                            str1.Append($"{item1.Name}={item[item1.Name].ToString()}&");
                        }
                    }
                }
                else if (token.Type == JTokenType.Object)
                {
                    IOrderedEnumerable<JProperty> prop = ((JObject)token).Properties().OrderBy(P => P.Name);
                    foreach (var item in prop)
                    {
                        if (string.IsNullOrWhiteSpace(token[item.Name].ToString()))
                        {
                            continue;
                        }
                        str1.Append($"{item.Name}={token[item.Name].ToString()}&");
                    }
                }
                str1.Append($"secretKey={this._secret_key}");
                string md5 = Md5Hash(str1.ToString());
                str1.Clear();
                return mac == md5;
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(eventId, ex, $"验证返回值出错:{json},mac:{mac}");
            }
            return true;
        }

        /// <summary>
        /// md5加密 32位 小写
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <returns></returns>
        public string Md5Hash(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 充币申请
        /// </summary>
        /// <param name="outOrderNo">商户端订单号.由商家自定义，64个字符以内，仅支持字母、数字、下划线且需保证在商户端不重复</param>
        /// <param name="paymentUserId">商户端用户标识,64个字符以内</param>
        /// <param name="coin">币种标识(BTC、ETH、ERC20_USDT、TRC20_USDT)</param>
        /// <param name="amount">支付数量,取值范围：[0.00001, ∞ ]</param>
        /// <param name="ordertype">订单类型(0、消费 1、充值)</param>
        /// <param name="productName">商品名称</param>
        /// <param name="exData">商户扩展数据</param>
        /// <returns></returns>
        public Response_vgpay<Res_Payment> RegUserInfo(string outOrderNo, string paymentUserId, string coin, decimal amount, string ordertype, string productName, string exData)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("outOrderNo", outOrderNo);
            if (!string.IsNullOrWhiteSpace(paymentUserId))
            {
                map.Add("paymentUserId", paymentUserId);
            }
            map.Add("coin", coin);
            map.Add("amount", amount.ToString());
            map.Add("orderType", ordertype);
            if (!string.IsNullOrWhiteSpace(productName))
            {
                map.Add("productName", productName);
            }
            if (!string.IsNullOrWhiteSpace(exData))
            {
                map.Add("exData", exData);
            }
            return Request<Res_Payment>(Method.POST, "/api/v3/Payment", map);
        }

        /// <summary>
        /// 撤销充币申请
        /// </summary>
        /// <param name="outOrderNo">商户端订单号.由商家自定义，64个字符以内，仅支持字母、数字、下划线且需保证在商户端不重复</param>
        /// <param name="orderNo">商户端用户标识,64个字符以内</param>       
        /// <returns></returns>
        public Response_vgpay<Res_CancelPayment> CancelPayment(string outOrderNo, string orderNo)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(outOrderNo))
            {
                map.Add("outOrderNo", outOrderNo);
            }
            if (!string.IsNullOrWhiteSpace(orderNo))
            {
                map.Add("orderNo", orderNo);
            }
            return Request<Res_CancelPayment>(Method.POST, "/api/v3/CancelPayment", map);
        }

        /// <summary>
        /// 充币申请单查询
        /// </summary>
        /// <param name="outOrderNo">非必填，商户端订单号.由商家自定义，64个字符以内，仅支持字母、数字、下划线且需保证在商户端不重复</param>
        /// <param name="orderNo">非必填，平台订单号,64个字符以内,outOrderNo和orderNo不能同时为空</param>
        /// <param name="paymentUserId">非必填，商户端用户标识,64个字符以内</param>
        /// <param name="coin">非必填，币种标识(BTC、ETH、ERC20_USDT、TRC20_USDT)</param>
        /// <param name="txId">非必填，链交易ID</param>       
        /// <returns></returns>
        public Response_vgpay<List<Res_QueryRechargeOrders>> QueryRechargeOrders(string outOrderNo, string orderNo, string paymentUserId, string coin, string txId)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(outOrderNo))
            {
                map.Add("outOrderNo", outOrderNo);
            }
            if (!string.IsNullOrWhiteSpace(orderNo))
            {
                map.Add("orderNo", orderNo);
            }
            if (!string.IsNullOrWhiteSpace(paymentUserId))
            {
                map.Add("paymentUserId", paymentUserId);
            }
            if (!string.IsNullOrWhiteSpace(coin))
            {
                map.Add("coin", coin);
            }
            if (!string.IsNullOrWhiteSpace(txId))
            {
                map.Add("txId", txId);
            }
            return Request<List<Res_QueryRechargeOrders>>(Method.GET, "/api/v3/QueryRechargeOrders", map);
        }

        /// <summary>
        /// 提币申请
        /// </summary>
        /// <param name="outWithdrawalNo">商户端提币单号.由商家自定义，64个字符以内，仅支持字母、数字、下划线且需保证在商户端不重复</param>
        /// <param name="withdrawalUserId">商户端用户标识,64个字符以内</param>     
        /// <param name="coin">币种标识(BTC、ETH、ERC20_USDT、TRC20_USDT)</param>
        /// <param name="amount">提币数量</param>   
        /// <param name="address">提币地址</param> 
        /// <returns></returns>
        public Response_vgpay<Res_Withdrawal> Withdrawal(string outWithdrawalNo, string withdrawalUserId, string coin, decimal amount, string address)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("outWithdrawalNo", outWithdrawalNo);
            if (!string.IsNullOrWhiteSpace(withdrawalUserId))
            {
                map.Add("withdrawalUserId", withdrawalUserId);
            }
            map.Add("coin", coin);
            map.Add("amount", amount.ToString());
            map.Add("address", address);
            return Request<Res_Withdrawal>(Method.POST, "/api/v3/Withdrawal", map);
        }

        /// <summary>
        /// 提币申请单查询
        /// </summary>
        /// <param name="outWithdrawalNo">商户端提币单号.由商家自定义，64个字符以内，仅支持字母、数字、下划线且需保证在商户端不重复</param>
        /// <param name="withdrawalNo">平台提币单号,64个字符以内,outWithdrawalNo和withdrawalNo不能同时为空</param>  
        /// <param name="withdrawalUserId">商户端用户标识,64个字符以内</param>     
        /// <param name="coin">币种标识(BTC、ETH、ERC20_USDT、TRC20_USDT)</param>        
        /// <param name="txId">链交易ID</param> 
        /// <returns></returns>
        public Response_vgpay<Res_QueryWithdrawalOrders> QueryWithdrawalOrders(string outWithdrawalNo, string withdrawalNo, string withdrawalUserId, string coin, string txId)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(outWithdrawalNo))
            {
                map.Add("outWithdrawalNo", outWithdrawalNo);
            }
            if (!string.IsNullOrWhiteSpace(withdrawalNo))
            {
                map.Add("withdrawalNo", withdrawalNo);
            }
            if (!string.IsNullOrWhiteSpace(withdrawalUserId))
            {
                map.Add("withdrawalUserId", withdrawalUserId);
            }
            if (!string.IsNullOrWhiteSpace(coin))
            {
                map.Add("coin", coin);
            }
            if (!string.IsNullOrWhiteSpace(txId))
            {
                map.Add("txId", txId);
            }
            return Request<Res_QueryWithdrawalOrders>(Method.GET, "/api/v3/QueryWithdrawalOrders", map);
        }

        /// <summary>
        ///  币价市场
        /// </summary>
        /// <returns></returns>
        public Response_vgpay<List<Res_Market>> MarketInfo()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            return Request<List<Res_Market>>(Method.GET, "/api/v3/Market", map);
        }

    }
}