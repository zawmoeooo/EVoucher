using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Entities.RequestModels
{
    public class NewEvoucherRequest
    {
        //public string voucher_No { get; set; }
        [Required]
        public string title { get; set; }
        public string description { get; set; }
        public DateTime expiry_date { get; set; }
        public string base64image { get; set; }
        [Required]
        [Range(0, 10000)]
        public decimal amount { get; set; }
        public string payment_method { get; set; }
        [Required]
        public decimal price { get; set; }
        public int discount { get; set; }
        [Required]
        [Range(0, 999)]
        public int quantity { get; set; }
        [Required]
        public int max_limit { get; set; }
        [Required]
        public string buy_type { get; set; }
        [Required]
        [Range(0, 999)]
        public int gift_per_user_limit { get; set; }
        public string status { get; set; }
    }

    public class UpdateEvoucherRequest
    {
        public string voucher_No { get; set; }
        [Required]
        public string title { get; set; }
        public string description { get; set; }
        public DateTime expiry_date { get; set; }
        public string base64image { get; set; }
        [Required]
        [Range(0, 10000)]
        public decimal amount { get; set; }
        public string payment_method { get; set; }
        [Required]
        public decimal price { get; set; }
        public int discount { get; set; }
        [Required]
        [Range(0, 999)]
        public int quantity { get; set; }
        [Required]
        public int max_limit { get; set; }
        [Required]
        public string buy_type { get; set; }
        [Required]
        [Range(0, 999)]
        public int gift_per_user_limit { get; set; }
        public string status { get; set; }
    }

    public class UpdateStatusRequest
    {
        public string evoucher_no { get; set; }
        public string status { get; set; }
    }

    public class EvoucherDetailRequest
    {
        public string evoucher_no { get; set; }
    }
}
