using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Entities.ResponseModels
{
    public class PaymentMethodListResponse
    {
        public string payment_method { get; set; }
        public int discount_percentage { get; set; }
        public string Status { get; set; }
    }
}
