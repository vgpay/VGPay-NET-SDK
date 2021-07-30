using System;

namespace VGPay.sdk.Models
{
    /// <summary>
    /// 提币申请单查询
    /// </summary>
    public class Res_QueryWithdrawalOrders
    {
        /// <summary>
        /// 商户端提币单号.由商家自定义，64个字符以内，仅支持字母、数字、下划线且需保证在商户端不重复。
        /// </summary>
        /// <value></value>
        public string outWithdrawalNo { get; set; }
        /// <summary>
        /// 平台提币单号
        /// </summary>
        /// <value></value>
        public string withdrawalNo { get; set; }
        /// <summary>
        /// 商户端用户唯一标识
        /// </summary>
        /// <value></value>
        public string withdrawalUserId { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        /// <value></value>
        public decimal amount { get; set; }
        /// <summary>
        /// 提币费
        /// </summary>
        /// <value></value>
        public decimal fee { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        /// <value></value>
        public string coin { get; set; }
        /// <summary>
        /// 状态(0:待商家审核; 1:待运营审核; 2:待放行; 3: 转账中; 4:已拒绝; 5:已完成;
        /// </summary>
        /// <value></value>
        public int status { get; set; }
        /// <summary>
        /// 充币地址
        /// </summary>
        /// <value></value>
        public string address { get; set; }
         /// <summary>
        /// 链交易ID
        /// </summary>
        /// <value></value>
        public string txId { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        /// <value></value>
        public string creationTime { get; set; }
    }
}