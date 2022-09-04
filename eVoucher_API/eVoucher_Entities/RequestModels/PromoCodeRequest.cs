using eVoucher_Entities.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Entities.RequestModels
{
    public class PromoCodeRequest
    {
        public string promo_code { get; set; }
        public string phone_no { get; set; }
    }
}
