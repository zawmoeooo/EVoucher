using System;
using System.Collections.Generic;

namespace eVoucher_Entities.EntityModels
{
    public partial class Tbluser
    {
        public int Id { get; set; }
        public string Usename { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Displayname { get; set; }
        public short? Status { get; set; }
    }
}
