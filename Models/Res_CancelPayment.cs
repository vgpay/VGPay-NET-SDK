using System;

namespace VGPay.sdk.Models
{
    /// <summary>
    /// 撤销充币申请
    /// </summary>
    public class Res_CancelPayment
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
    }
}