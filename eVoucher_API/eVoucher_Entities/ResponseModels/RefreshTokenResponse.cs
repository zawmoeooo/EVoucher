using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Entities.ResponseModels
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }
        public int AccessTokenExpireInMinutes { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshTokenExpireInMinutes { get; set; }
    }

    public class RefreshTokenResponseWithError
    {
        public string AccessToken { get; set; }
        public int AccessTokenExpireInMinutes { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshTokenExpireInMinutes { get; set; }
        public int statusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
