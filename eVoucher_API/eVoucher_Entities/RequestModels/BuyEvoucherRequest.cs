using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Entities.RequestModels
{
    public class BuyEvoucherRequest
    {
        [Required]
        public string VoucherNo { get; set; }
        [Required]
        public string BuyerName { get; set; }
        [Required]
        public string BuyerPhone { get; set; }
        [Required]
        public string BuyType { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string ExpiryDate { get; set; }
        [Required]
        public string CVV { get; set; }
        [Required]
        [Range(1, 2000,
        ErrorMessage = "Value for {0} must be greater than 0.")]
        public int Quantity { get; set; }
    }

    public class GeneratePromoCodeRequest
    {
        public string Purchase_id { get; set; }
    }

    public class BuyEVoucherRequest
    {
        [Required]
        public string VoucherNo { get; set; }
        [Required]
        public string BuyerName { get; set; }
        [Required]
        public string BuyerPhone { get; set; }
        [Required]
        public string BuyType { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        [Range(1, 10000,
        ErrorMessage = "Value for {0} must be greater than 0.")]
        public int Quantity { get; set; }

    }

}
