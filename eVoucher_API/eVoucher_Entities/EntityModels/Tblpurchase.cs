using System;
using System.Collections.Generic;

namespace eVoucher_Entities.EntityModels
{
    public partial class Tblpurchase
    {
        public string PurchaseId { get; set; } = null!;
        public int Id { get; set; }
        public string VoucherNo { get; set; } = null!;
        public string? BuyerName { get; set; }
        public string BuyerPhone { get; set; } = null!;
        public DateTime? ExpiryDate { get; set; }
        public string? BuyType { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public decimal? Price { get; set; }
        public int? Discount { get; set; }
        public int? Quantity { get; set; }
        public decimal Total { get; set; }
        public DateTime PurchaseDate { get; set; }
        public short? Status { get; set; }
    }
}
