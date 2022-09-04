using eVoucher_Entities.RequestModels;
using eVoucher_Entities.ResponseModels;
using eVoucher_Repo;
using eVoucher_Repo.Helper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using static eVoucher_Entities.RequestModels.BuyEvoucherRequest;

namespace eStore_API.Controllers
{
    [ApiController]
    public class EStoreController : ControllerBase
    {
        private readonly IRepositories repo;
        private readonly ILogger log;
        private readonly IDistributedCache cache;
        public EStoreController(ILogger<EStoreController> _log, IRepositories _repo, IDistributedCache _cache)
        {
            log = _log;
            repo = _repo;
            cache = _cache;
        }

        [Route("api/EStore/GenerateAccessToken")]
        [HttpPost]
        public IActionResult GenerateAccessToken(EStoreAccessTokenRequest _request)
        {
            var APIName = "GenerateAccessToken";
            try
            {
                log.LogInformation($"{APIName}\r\n");
                var response = repo.Estore.GenerateToken(_request);
                if (response.StatusCode == 200)
                {
                    log.LogInformation($"Success");
                    return Ok(response);
                }
                else
                {
                    log.LogError($"{APIName} Error\r\n{response.ErrorMessage}");
                    return StatusCode(response.StatusCode, response.GetError());
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName} Error\r\n{e}");
                return StatusCode(500, new Error("internal_error", e.Message));
            }
        }

        [Route("api/EStore/RefreshToken")]
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
                    return StatusCode(response.statusCode, new Error(response.statusCode.ToString(), response.ErrorMessage));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName} Error\r\n{e}");
                return StatusCode(500, new Error("internal_error", e.Message));
            }
        }

        [Route("api/EStore/GetPaymentMethod")]
        [HttpPost]
        [Authorize]
        public IActionResult GetPaymentMethod()
        {
            var APIName = "GetPaymentMethod";
            log.LogInformation($"{APIName}");
            try
            {
                var paymentListfromcache = cache.GetString("PaymentMethodList");
                if (string.IsNullOrEmpty(paymentListfromcache))
                {
                    var response = repo.Estore.GetPaymentMethodList();
                    if (response != null && response.Count > 0)
                    {
                        cache.SetString("PaymentMethodList", JsonConvert.SerializeObject(response));
                        log.LogInformation($"{APIName}\r\n Get List Count :{response.Count}");
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(404, new Error("not-found", "No payment methods."));
                    }
                }
                else
                {
                    var response = JsonConvert.DeserializeObject<List<PaymentMethodListResponse>>(paymentListfromcache);

                    log.LogInformation($"{APIName}\r\n Get List from cache Count :{response.Count}");
                    return Ok(response);
                }

            }
            catch (Exception e)
            {
                log.LogError($"{APIName} Error\r\n{e}");
                return StatusCode(500, new Error("internal_error", e.Message));
            }
        }

        [Route("api/EStore/GetStoreEvoucherList")]
        [HttpPost]
        [Authorize]
        public IActionResult GetStoreEvoucherList()
        {
            var APIName = "GetStoreEvoucherList";
            log.LogInformation($"{APIName}");
            try
            {
                var response = repo.Estore.GetVoucherList();
                if (response != null && response.Count > 0)
                {
                    cache.SetString("GetStoreEvoucherList", JsonConvert.SerializeObject(response));
                    log.LogInformation($"{APIName}\r\n Get EVoucher List Count :{response.Count}");
                    return Ok(response);
                }
                else
                {
                    return StatusCode(404, new Error("not-found", "No Evouche."));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName} Error\r\n{e}");
                return StatusCode(500, new Error("internal_error", e.Message));
            }
        }

        [Route("api/EStore/GetEvoucherDetail")]
        [HttpPost]
        [Authorize]
        public IActionResult GetEvoucherDetail(EvoucherDetailRequest _request)
        {
            var APIName = "GetStoreEvoucherList";
            log.LogInformation($"{APIName}\r\njson={JsonConvert.SerializeObject(_request)}");
            try
            {
                var response = repo.Estore.GetEVoucherDetail(_request);
                if (response != null)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(404, new Error("not-found", "No Evouche found."));
                }

            }
            catch (Exception e)
            {
                log.LogError($"{APIName} Error\r\n{e}");
                return StatusCode(500, new Error("internal_error", e.Message));
            }
        }

        [Route("api/EStore/CheckPromoCode")]
        [HttpPost]
        [Authorize()]
        public IActionResult CheckPromoCode(PromoCodeRequest _request)
        {
            string APIName = "CheckPromoCode";
            log.LogInformation($"{APIName}\r\njson={JsonConvert.SerializeObject(_request)}");
            try
            {
                var response = repo.Estore.CheckPromoCode(_request);
                if (response != null)
                {
                    log.LogInformation($"{APIName}\r\n Check PromoCode Success ");
                    return Ok(response);
                }
                else
                {
                    log.LogError($"{APIName}\r\nStautsCode:404\r\nErrorType:Record Not Found");
                    return NotFound(new Error("not-found", "Record Not Found"));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName}\r\n{e}");
                return StatusCode(500, new Error("internal-error", e.Message));
            }
        }

        [Route("api/EStore/BuyEvoucher")]
        [HttpPost]
        [Authorize()]
        public IActionResult BuyEvoucher(BuyEvoucherRequest _request)
        {
            string APIName = "BuyEvoucher";
            log.LogInformation($"{APIName}\r\njson={JsonConvert.SerializeObject(_request)}");
            try
            {
                if(IsCreditCardInfoValid(_request.CardNumber, _request.ExpiryDate, _request.CVV))
                {
                    var response = repo.Estore.BuyEvoucher(_request);
                    if (response.StatusCode == 200)
                    {
                        var generatePromoJobId = BackgroundJob.Enqueue(() => repo.Estore.ScheduleGeneratePromoCode(new GeneratePromoCodeRequest
                        {
                            Purchase_id = response.purchase_id
                        }));

                        log.LogInformation($"{APIName}\r\n Buy Success ");
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(response.StatusCode, new Error(response.ErrorType, response.ErrorMessage));
                    }
                }
                else
                {
                    return StatusCode(400, new Error("bad-request", "Card information is not valid."));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName}\r\n{e}");
                return StatusCode(500, new Error("internal-error", e.Message));
            }
        }

        [Route("api/EStore/GetPurchaseHistoryList")]
        [HttpPost]
        [Authorize()]
        public IActionResult GetPurchaseHistoryList()
        {
            string APIName = "GetPurchaseHistoryList";
            log.LogInformation($"{APIName}");
            try
            {
                var response = repo.Estore.GetPurchaseHistory();

                if (response != null && response.Count > 0)
                {
                    log.LogInformation($"{APIName}\r\n Get List Count :{response.Count}");
                    return Ok(response);
                }
                else
                {
                    log.LogError($"{APIName}\r\nNo Record Found");
                    return NotFound(new Error("not-found", "No Record Found"));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName}\r\n{e}");
                return StatusCode(500, new Error("internal-error", e.Message));
            }
        }

        private static bool IsCreditCardInfoValid(string cardNo, string expiryDate, string cvv)
        {
            var cardCheck = new Regex(@"^(1298|1267|4512|4567|8901|8933)([\-\s]?[0-9]{4}){3}$");
            var monthCheck = new Regex(@"^(0[1-9]|1[0-2])$");
            var yearCheck = new Regex(@"^20[0-9]{2}$");
            var cvvCheck = new Regex(@"^\d{3}$");

            if (!cardCheck.IsMatch(cardNo)) // <1>check card number is valid
                return false;
            if (!cvvCheck.IsMatch(cvv)) // <2>check cvv is valid as "999"
                return false;

            var dateParts = expiryDate.Split('/'); //expiry date in from MM/yyyy            
            if (!monthCheck.IsMatch(dateParts[0]) || !yearCheck.IsMatch(dateParts[1])) // <3 - 6>
                return false; // ^ check date format is valid as "MM/yyyy"

            var year = int.Parse(dateParts[1]);
            var month = int.Parse(dateParts[0]);
            var lastDateOfExpiryMonth = DateTime.DaysInMonth(year, month); //get actual expiry date
            var cardExpiry = new DateTime(year, month, lastDateOfExpiryMonth, 23, 59, 59);

            //check expiry greater than today & within next 6 years <7, 8>>
            return (cardExpiry > DateTime.Now && cardExpiry < DateTime.Now.AddYears(6));
        }
    }
}
