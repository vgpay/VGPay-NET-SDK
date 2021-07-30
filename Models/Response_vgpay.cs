using System;

namespace VGPay.sdk.Models
{
    /// <summary>
    /// 响应基类
    /// </summary>
    public class Response_vgpay<T>
    {
        /// <summary>
        /// 状态: true成功，false失败
        /// </summary>
        /// <value></value>
        public bool isSuccess { get; set; }
        /// <summary>
        /// 错误码
        /// </summary>
        /// <value></value>
        public int code { get; set; }
        /// <summary>
        /// 错误信息详细
        /// </summary>
        /// <value></value>
        public string msg { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        /// <value></value>
        public string mac { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        /// <value></value>
        public T data{get;set;}
    }
}
