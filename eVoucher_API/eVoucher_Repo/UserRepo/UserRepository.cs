using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVoucher_Entities.EntityModels;
using eVoucher_Entities.RequestModels;
using eVoucher_Entities.ResponseModels;
using eVoucher_Entities.Models;
using eVoucher_Repo.Helper;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace eVoucher_Repo.UserRepo
{
    public class UserRepository : IUserRepository
    {
        private eVoucherContext db_Evoucher;
        private readonly IConfiguration configuration;

        public UserRepository(eVoucherContext _db_Evoucher, IConfiguration _configuration)
        {
            db_Evoucher = _db_Evoucher;
            configuration = _configuration;
        }

        public UserResponse Login(LoginRequest _request)
        {
            UserResponse response = new UserResponse();
            string hashedPassword = StringHelper.GenerateHash(_request.password);

            var user = (from u in db_Evoucher.Tblusers
                        where u.Usename == _request.username && u.Password == hashedPassword && u.Status == 1
                        select u).FirstOrDefault();
            if (user != null)
            {
                AccessTokenConfig ATConfig = new AccessTokenConfig
                {
                    Audience = configuration["Audience"],
                    Issuer = configuration["Issuer"],
                    PrivateKey = configuration["RSAPrivateKey"],
                    TokenExpiryMinute = Int32.Parse(configuration["TokenExpiryInMin"]),
                    RefreshTokenExpiryMinute = Int32.Parse(configuration["RefreshTokenExpiryInMin"]),
                    UserId = user.Id,
                    UserName = string.IsNullOrEmpty(user.Displayname) ? "" : user.Displayname
                };
                GeneratedToken generatedToken = AccessTokenHelper.GenerateToken(ATConfig);
                if (String.IsNullOrEmpty(generatedToken.ErrorStatus))
                {
                    response.AccessToken = generatedToken.AccessToken;
                    response.AccessTokenExpireInMinutes = Int32.Parse(configuration["TokenExpiryInMin"]);
                    response.RefreshToken = generatedToken.RefreshToken;
                    response.RefreshTokenExpireInMinutes = Int32.Parse(configuration["RefreshTokenExpiryInMin"]);
                    response.UserId = user.Id;
                    deleteUserAccessTokenbyUserID(user.Id);
                    saveAccessToken(generatedToken);
                }
                else
                {
                    response.ErrorStatus = generatedToken.ErrorStatus;
                }
            }
            else
            {
                response.ErrorStatus = "Invalid user name or password!";
            }
            return response;
        }

        private void saveAccessToken(GeneratedToken _request)
        {
            TblaccessToken at = new TblaccessToken();
            at.AccessToken = _request.AccessToken;
            at.AccessTokenExpiry = _request.AccessTokenExpiryDate;
            at.RefreshToken = _request.RefreshToken;
            at.RefreshTokenExpiry = _request.RefreshTokenExpiryDate;
            at.UserId = _request.UserID;
            db_Evoucher.TblaccessTokens.Add(at);
            db_Evoucher.SaveChanges();
        }

        private void deleteUserAccessToken(string refreshToken, int UserID)
        {
            var user = (from u in db_Evoucher.TblaccessTokens
                        where u.UserId == UserID && u.RefreshToken == refreshToken
                        select u).FirstOrDefault();
            if (user != null)
            {
                db_Evoucher.TblaccessTokens.Remove(user);
                db_Evoucher.SaveChanges();
            }

        }

        private void deleteUserAccessTokenbyUserID(int UserID)
        {
            var user = (from u in db_Evoucher.TblaccessTokens
                        where u.UserId == UserID
                        select u).FirstOrDefault();
            if (user != null)
            {
                db_Evoucher.TblaccessTokens.Remove(user);
                db_Evoucher.SaveChanges();
            }
        }
        

        public RefreshTokenResponseWithError RefreshToken(RefreshTokenRequest _request, string token)
        {
            RefreshTokenResponseWithError response = new RefreshTokenResponseWithError();
            if (string.IsNullOrEmpty(token))
            {
                response.statusCode = 401;
                response.ErrorMessage = "Invalid token.";
                return response;
            }
            ValidateAccessTokenConfig validateAccessTokenConfig = new ValidateAccessTokenConfig
            {
                Audience = configuration["Audience"],
                Issuer = configuration["Issuer"],
                PrivateKey = configuration["RSAPrivateKey"],
                IsValidateExpiry = false,
                Token = token
            };

            var validatedToken = AccessTokenHelper.CheckValidToken(validateAccessTokenConfig);
            if (validatedToken.IsValid)
            {
                var refreshtoken = (from rt in db_Evoucher.TblaccessTokens
                                    where rt.RefreshToken == _request.RefreshToken
                                    && rt.UserId == validatedToken.UserID
                                    && rt.RefreshTokenExpiry > DateTime.Now
                                    select rt).FirstOrDefault();
                if (refreshtoken != null && !string.IsNullOrEmpty(refreshtoken.RefreshToken))
                {
                    AccessTokenConfig ATConfig = new AccessTokenConfig
                    {
                        Audience = configuration["Audience"],
                        Issuer = configuration["Issuer"],
                        PrivateKey = configuration["RSAPrivateKey"],
                        TokenExpiryMinute = Int32.Parse(configuration["TokenExpiryInMin"]),
                        RefreshTokenExpiryMinute = Int32.Parse(configuration["RefreshTokenExpiryInMin"]),
                        UserId = validatedToken.UserID,
                        UserName = validatedToken.UserName
                    };
                    GeneratedToken generatedToken = AccessTokenHelper.GenerateToken(ATConfig);
                    if (String.IsNullOrEmpty(generatedToken.ErrorStatus))
                    {
                        response.AccessToken = generatedToken.AccessToken;
                        response.AccessTokenExpireInMinutes = Int32.Parse(configuration["TokenExpiryInMin"]);
                        response.RefreshToken = generatedToken.RefreshToken;
                        response.RefreshTokenExpireInMinutes = Int32.Parse(configuration["RefreshTokenExpiryInMin"]);
                        response.statusCode = 200;
                        deleteUserAccessToken(_request.RefreshToken, validatedToken.UserID);
                        saveAccessToken(generatedToken);
                    }
                    else
                    {
                        response.statusCode = generatedToken.statusCode;
                        response.ErrorMessage = generatedToken.ErrorStatus;
                    }
                }
                else
                {
                    response.statusCode = 401;
                    response.ErrorMessage = "Invalid or Expired Refresh Token.";
                }
            }
            else
            {
                response.statusCode = 401;
                response.ErrorMessage = "Invalid or Expired Access Token.";
            }
            return response;
        }


        public registerResponse Register(registerResponse _request)
        {
            registerResponse response = new registerResponse();
            string hashedPassword = StringHelper.GenerateHash(_request.password);
            Tbluser u = new Tbluser();
            u.Status = 1;
            u.Displayname = "Zaw Moe Oo";
            u.Password = hashedPassword;
            u.Usename = _request.username;
            db_Evoucher.Tblusers.Add(u);
            db_Evoucher.SaveChanges();
            response.username = _request.username;
            response.password = hashedPassword;
            return response;
        }

    }
}
