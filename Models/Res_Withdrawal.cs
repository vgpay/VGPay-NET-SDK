using System;

namespace VGPay.sdk.Models
{
    /// <summary>
    /// 提币申请
    /// </summary>
    public class Res_Withdrawal
    {
        /// <summary>
        /// 平台提币单号
        /// </summary>
        /// <value></value>
        public string withdrawalNo { get; set; }
        /// <summary>
        /// 提币费用
        /// </summary>
        /// <value></value>
        public decimal fee { get; set; }
        /// <summary>
        /// 商户端提币单号
        /// </summary>
        /// <value></value>
        public string outWithdrawalNo { get; set; }
    }
}