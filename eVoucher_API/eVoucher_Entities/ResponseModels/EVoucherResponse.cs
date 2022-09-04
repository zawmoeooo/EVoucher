using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Entities.ResponseModels
{
    public class EVoucherResponse
    {
    }

    public class newEvoucherResponse : ResponseBase
    {
        public string Evoucher_No { get; set; }
    }

    public class GetEVoucherListingResponse
    {
        public string voucher_no { get; set; }
        public string title { get; set; }
        public DateTime? expirydate { get; set; }
        public decimal amount { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public string status { get; set; }
    }

    public class GetEVoucherDetailResponse
    {
        public string voucher_No { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime expiry_date { get; set; }
        public string image { get; set; }
        public decimal amount { get; set; }
        public string payment_method { get; set; }
        public decimal price { get; set; }
        public int discount { get; set; }
        public int quantity { get; set; }
        public int max_limit { get; set; }
        public string buy_type { get; set; }
        public int gift_per_user_limit { get; set; }
        public string status { get; set; }
    }
}
