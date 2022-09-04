using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Entities.Models
{
    public class AccessTokenConfig
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PrivateKey { get; set; }
        public int TokenExpiryMinute { get; set; }
        public int RefreshTokenExpiryMinute { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }

    public class GeneratedToken
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiryDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryDate { get; set; }
        public int UserID { get; set; }
        public int statusCode { get; set; }
        public string ErrorStatus { get; set; }
    }

    public class ValidateAccessTokenConfig
    {
        public string Token { get; set; }
        public bool IsValidateExpiry { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string PrivateKey { get; set; }
    }

    public class ValidateToken
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }

}
