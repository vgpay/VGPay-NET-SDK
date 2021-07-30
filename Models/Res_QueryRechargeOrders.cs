using System;

namespace VGPay.sdk.Models
{
    /// <summary>
    /// 充币申请单查询
    /// </summary>
    public class Res_QueryRechargeOrders
    {
        /// <summary>
        /// 平台订单号
        /// </summary>
        /// <value></value>
        public string orderNo { get; set; }
        /// <summary>
        /// 商户端订单号
        /// </summary>
        /// <value></value>
        public string outOrderNo { get; set; }
        /// <summary>
        /// 商户端用户标识
        /// </summary>
        /// <value></value>
        public string paymentUserId { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        /// <value></value>
        public decimal amount { get; set; }
        /// <summary>
        /// 用户实际支付金额
        /// </summary>
        /// <value></value>
        public decimal actualPaymentAmount { get; set; }
        /// <summary>
        /// 状态(0:未充币; 1:待收款; 2:欠额支付; 3: 超额支付; 4:完成; 5:手动完成; 6 : 关闭/撤销)
        /// </summary>
        /// <value></value>
        public int status { get; set; }
        /// <summary>
        /// 充币地址
        /// </summary>
        /// <value></value>
        public string address { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        /// <value></value>
        public string coin { get; set; }
        /// <summary>
        /// 链交易ID
        /// </summary>
        /// <value></value>
        public string txId { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        /// <value></value>
        public long creationTime { get; set; }
        /// <summary>
        /// 支付到账时间
        /// </summary>
        /// <value></value>
        public long receivedTime { get; set; }
    }
}