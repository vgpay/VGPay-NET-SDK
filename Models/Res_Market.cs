using System;

namespace VGPay.sdk.Models
{
    /// <summary>
    /// 提币申请
    /// </summary>
    public class Res_Market
    {
        /// <summary>
        /// 交易对
        /// </summary>
        public string symbol { get; set; }
        /// <summary>
        /// 兑入价格
        /// </summary>
        public decimal inPrice { get; set; }
        /// <summary>
        /// 兑出价格
        /// </summary>
        public decimal outPrice { get; set; }
        /// <summary>
        /// 单笔最大兑入数量
        /// </summary>
        public decimal maxInQuantity { get; set; }
        /// <summary>
        /// 单笔最大兑出数量
        /// </summary>
        public decimal maxOutQuantity { get; set; }
    }
}