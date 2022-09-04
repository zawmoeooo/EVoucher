using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using eVoucher_Repo;
using eVoucher_Entities.RequestModels;
using Newtonsoft.Json;
using eVoucher_Entities.ResponseModels;

namespace PromoCodeGeneration.Controllers
{
    [ApiController]
    public class PromoCodeController : ControllerBase
    {
        private readonly IRepositories repo;
        private readonly ILogger log;
        //private readonly IDistributedCache distributedCache;

        public PromoCodeController(IRepositories _repo, ILogger<PromoCodeController> _log)
        {
            repo = _repo;
            log = _log;
        }

        [Route("api/PromoCode/GeneratePromoCode")]
        [HttpPost]
        public IActionResult GeneratePromoCode(GeneratePromoCodeRequest _request)
        {
            string APIName = "GeneratePromoCode";
            log.LogInformation($"{APIName}\r\njson={JsonConvert.SerializeObject(_request)}");
            try
            {
                var response = repo.Estore.GeneratePromoCode(_request);
                if (response.StatusCode == 200)
                {
                    log.LogInformation($"{APIName}\r\n Success ");
                    return Ok(response);
                }
                else
                {
                    log.LogError($"{APIName}\r\nStautsCode:{response.StatusCode}\r\nErrorType:{response.ErrorType}" +
                                 $"\r\nErrorMsg:{response.ErrorMessage}");
                    return StatusCode(response.StatusCode, response.GetError());
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName}\r\n{e}");
                return StatusCode(500, new Error("internal-error", e.Message));
            }
        }
    }
}
