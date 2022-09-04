using System;
using System.Collections.Generic;

namespace eVoucher_Entities.EntityModels
{
    public partial class TblaccessToken
    {
        public int Id { get; set; }
        public string AccessToken { get; set; } = null!;
        public DateTime AccessTokenExpiry { get; set; }
        public string RefreshToken { get; set; } = null!;
        public DateTime RefreshTokenExpiry { get; set; }
        public int UserId { get; set; }
    }
}
