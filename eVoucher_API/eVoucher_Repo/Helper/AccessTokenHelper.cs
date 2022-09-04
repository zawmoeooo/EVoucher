using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using eVoucher_Entities.Models;

namespace eVoucher_Repo.Helper
{
    public class AccessTokenHelper
    {
        public static GeneratedToken GenerateToken(AccessTokenConfig tokenConfig)
        {
            GeneratedToken gToken = new GeneratedToken();


            var privateKey = tokenConfig.PrivateKey;
            if (privateKey != "")
            {
                try
                {
                    RSACryptoServiceProvider rsaService = new RSACryptoServiceProvider();
                    rsaService.FromXmlString(privateKey);
                    var ExpiryDate = DateTime.Now.AddMinutes(tokenConfig.TokenExpiryMinute);
                    var refreshToken = GenerateRefreshToken();

                    var authClaims = new[]
                    {
                        new Claim("UserID", tokenConfig.UserId.ToString()),
                        new Claim("UserName", tokenConfig.UserName)
                    };

                    var jwttoken = new JwtSecurityToken(
                        issuer: tokenConfig.Issuer,
                        audience: tokenConfig.Audience,
                        expires: ExpiryDate,
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(new RsaSecurityKey(rsaService), SecurityAlgorithms.RsaSha256)
                        );
                    gToken.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwttoken);
                    gToken.AccessTokenExpiryDate = ExpiryDate;
                    gToken.RefreshTokenExpiryDate = DateTime.Now.AddMinutes(tokenConfig.RefreshTokenExpiryMinute);
                    gToken.RefreshToken = refreshToken;
                    gToken.UserID = tokenConfig.UserId;
                }
                catch (Exception e)
                {
                    gToken.statusCode = 500;
                    gToken.ErrorStatus = "Error occur while generate token." + e.Message;
                }
            }
            else
            {
                gToken.statusCode = 404;
                gToken.ErrorStatus = "Private Key can't be empty.";
            }

            return gToken;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public static ValidateToken CheckValidToken(ValidateAccessTokenConfig _request)
        {
            ValidateToken validToken = new ValidateToken();
            RSACryptoServiceProvider privateKey = new RSACryptoServiceProvider();
            privateKey.FromXmlString(_request.PrivateKey);

            _request.Token = _request.Token.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

            TokenValidationParameters para =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _request.Issuer,
                ValidAudience = _request.Audience,
                IssuerSigningKey = new RsaSecurityKey(privateKey),
                ValidateLifetime = _request.IsValidateExpiry,
                ClockSkew = TimeSpan.FromMinutes(0) //0 minute tolerance for the expiration date
            };
            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            try
            {
                var payload = handler.ValidateToken(_request.Token, para, out validatedToken);
                Int32.TryParse(payload.Claims.Where(c => c.Type == "UserID").Select(c => c.Value).SingleOrDefault(), out int userId);
                var userName = payload.Claims.Where(c => c.Type == "UserName").Select(c => c.Value).SingleOrDefault();

                validToken.UserID = userId;
                validToken.UserName = userName;
                validToken.IsValid = true;


            }
            catch (Exception e)
            {
                validToken.IsValid = false;
                validToken.ErrorMessage = e.Message;
            }
            return validToken;
        }
    }
}
