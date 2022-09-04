using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Entities.ResponseModels
{
    public class UserResponse
    {
        public string AccessToken { get; set; }
        public int AccessTokenExpireInMinutes { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshTokenExpireInMinutes { get; set; }

        public string ErrorStatus;
        public int UserId;
    }

    public class registerResponse
    {
        public string username { get; set; }
        public string password { get; set; }

        public string ErrorStatus;
    }
}
