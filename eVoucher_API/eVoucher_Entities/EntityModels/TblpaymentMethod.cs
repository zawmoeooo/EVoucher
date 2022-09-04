using System;
using System.Collections.Generic;

namespace eVoucher_Entities.EntityModels
{
    public partial class TblpaymentMethod
    {
        public int Id { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public int DiscountPercentage { get; set; }
        public sbyte Status { get; set; }
    }
}
