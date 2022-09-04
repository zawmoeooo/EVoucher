using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Entities.ResponseModels
{
    public class PromoCodeResponse : ResponseBase
    {
        public string status { get; set; }
        public string promo_code { get; set; }
    }

    public class GeneratePromoCodeResponse : ResponseBase
    {
        public bool PromoCodeGenerated { get; set; }
    }
}
