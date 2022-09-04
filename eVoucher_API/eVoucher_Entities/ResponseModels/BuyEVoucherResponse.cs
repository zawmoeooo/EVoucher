using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Entities.ResponseModels
{
    public class BuyEVoucherResponse : ResponseBase
    {
        public string purchase_id { get; set; }
        public bool IsPurchaseSuccess { get; set; }
        public string ErrorResponse { get; set; }

    }

    public class GetPurchaseHistoryResponse
    {
        public string status { get; set; }
        public string PromoCode { get; set; }
        public string QR_Image_Path { get; set; }
        public DateTime Purchase_Date { get; set; }
    }
}
