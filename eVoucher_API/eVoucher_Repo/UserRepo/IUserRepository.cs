using eVoucher_Entities.RequestModels;
using eVoucher_Entities.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVoucher_Repo.UserRepo
{
    public interface IUserRepository
    {
        public UserResponse Login(LoginRequest _request);
        public registerResponse Register(registerResponse _request);
        public RefreshTokenResponseWithError RefreshToken(RefreshTokenRequest _request, string token);
    }
}
