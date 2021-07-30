using System;

namespace VGPay.sdk.Models
{
    /// <summary>
    /// 充币申请
    /// </summary>
    public class Res_Payment
    {
        /// <summary>
        /// 充币地址
        /// </summary>
        /// <value></value>
        public string address { get; set; }
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
    }
}