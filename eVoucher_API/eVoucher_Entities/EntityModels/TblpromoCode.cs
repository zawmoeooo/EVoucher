using System;
using System.Collections.Generic;

namespace eVoucher_Entities.EntityModels
{
    public partial class TblpromoCode
    {
        public string PromoCode { get; set; } = null!;
        public string VoucherNo { get; set; } = null!;
        public string QrImage { get; set; } = null!;
        public string? OwnerName { get; set; }
        public string OwnerPhone { get; set; } = null!;
        public double VoucherAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public sbyte Status { get; set; }
        public string purchase_id { get; set; }
    }
}
