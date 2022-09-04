using System;
using System.Collections.Generic;

namespace eVoucher_Entities.EntityModels
{
    public partial class TblEvoucher
    {
        public int Id { get; set; }
        public string VoucherNo { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? ImagePath { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal Price { get; set; }
        public int? Discount { get; set; }
        public int Quantity { get; set; }
        public int MaxLimit { get; set; }
        public string BuyType { get; set; } = null!;
        public int GiftPerUserLimit { get; set; }
        public sbyte? Status { get; set; }
    }
}
