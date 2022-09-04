using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using eVoucher_Entities.RequestModels;
using eVoucher_Repo;
using eVoucher_Entities.ResponseModels;

namespace eVoucher_API.Controllers
{

    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger log;
        private readonly IRepositories repo;
        public UserController(ILogger<UserController> _log, IRepositories _repo)
        {
            log = _log;
            repo = _repo;
        }

        [Route("api/User/LoginService")]
        [HttpPost]
        public IActionResult LoginService(LoginRequest _request)
        {
            var APIName = "LoginService";
            try
            {
                log.LogInformation($"{APIName}\r\n");
                var response = repo.User.Login(_request);
                if (String.IsNullOrEmpty(response.ErrorStatus))
                {
                    log.LogInformation($"Login Success");
                    return Ok(response);
                }
                else
                {
                    log.LogError($"{APIName} Error\r\n{response.ErrorStatus}");
                    return NotFound(new Error("Un-authorized", response.ErrorStatus));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName} Error\r\n{e}");
                return StatusCode(500, new Error("internal_error", e.Message));
            }
        }

        [Route("api/User/Register")]
        [HttpPost]
        public IActionResult Register(registerResponse _request)
        {
            try
            {
                log.LogInformation($"Register\r\n");
                var response = repo.User.Register(_request);
                if (String.IsNullOrEmpty(response.ErrorStatus))
                {
                    return Ok(response);
                }
                else
                {
                    log.LogError($"LoginService Error\r\n{response.ErrorStatus}");
                    return NotFound(new Error("Un-authorized", response.ErrorStatus));
                }
            }
            catch (Exception e)
            {
                log.LogError($"Register Error\r\n{e}");
                return StatusCode(500, new Error("internal_error", e.Message));
            }
        }

        [Route("api/User/RefreshToken")]
        [HttpPost]
        public IActionResult RefreshToken(RefreshTokenRequest _request)
        {
            var APIName = "RefreshToken";
            try
            {
                log.LogInformation($"{APIName}\r\n");
                Request.Headers.TryGetValue("Authorization", out var accessToken);
                var response = repo.User.RefreshToken(_request, accessToken);

                if (response.statusCode == 200)
                {
                    log.LogInformation($"{APIName} Success");
                    RefreshTokenResponse returnRespone = new RefreshTokenResponse
                    {
                        AccessToken = response.AccessToken,
                        AccessTokenExpireInMinutes = response.AccessTokenExpireInMinutes,
                        RefreshToken = response.RefreshToken,
                        RefreshTokenExpireInMinutes = response.RefreshTokenExpireInMinutes
                    };
                    return Ok(returnRespone);
                }
                else
                {
                    log.LogError($"{APIName}\r\nStautsCode:{response.statusCode}\r\nErrorMessage:{response.ErrorMessage}");
                    return StatusCode(response.statusCode, new Error(response.statusCode.ToString(),response.ErrorMessage));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName} Error\r\n{e}");
                return StatusCode(500, new Error("internal_error", e.Message));
            }
        }
    }
}
