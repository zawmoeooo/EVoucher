using eVoucher_Entities.RequestModels;
using eVoucher_Entities.ResponseModels;
using eVoucher_Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using Newtonsoft.Json;
using eVoucher_Repo.Helper;

namespace eVoucher_API.Controllers
{
    [ApiController]
    public class EvoucherController : ControllerBase
    {
        private readonly ILogger log;
        private readonly IRepositories repo;
        public EvoucherController(ILogger<EvoucherController> _log, IRepositories _repo)
        {
            log = _log;
            repo = _repo;
        }

        [Route("api/Evoucher/CreateNewEvoucher")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateNewEvoucher(NewEvoucherRequest _request)
        {
            var APIName = "CreateNewEvoucher";
            log.LogInformation($"{APIName}\r\njson={JsonConvert.SerializeObject(_request)}");
            try
            {
                if (ModelState.IsValid)
                {
                    var response = repo.Evoucher.CreateNewEvoucher(_request);
                    if (response.StatusCode == 200)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(response.StatusCode, new Error(response.ErrorType, response.ErrorMessage));
                    }

                }
                else
                {
                    return StatusCode(400, new Error("bad_request", JsonConvert.SerializeObject(ModelState.Values.Select(e => e.Errors).ToList())));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName} Error\r\n{e}");
                return StatusCode(500, new Error("internal_error", e.Message));
            }
        }

        [Route("api/Evoucher/UpdateEvoucher")]
        [HttpPost]
        [Authorize]
        public IActionResult UpdateEvoucher(UpdateEvoucherRequest _request)
        {
            var APIName = "UpdateEvoucher";
            log.LogInformation($"{APIName}\r\njson={JsonConvert.SerializeObject(_request)}");
            try
            {
                if (ModelState.IsValid)
                {
                    var response = repo.Evoucher.UpdateEVoucher(_request);
                    if (response.StatusCode == 200)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(response.StatusCode, new Error(response.ErrorType, response.ErrorMessage));
                    }

                }
                else
                {
                    return StatusCode(400, new Error("bad_request", JsonConvert.SerializeObject(ModelState.Values.Select(e => e.Errors).ToList())));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName} Error\r\n{e}");
                return StatusCode(500, new Error("internal_error", e.Message));
            }
        }

        [Route("api/Evoucher/UpdateEvoucherStatus")]
        [HttpPost]
        [Authorize]
        public IActionResult UpdateEvoucherStatus(UpdateStatusRequest _request)
        {
            var APIName = "UpdateEvoucherStatus";
            log.LogInformation($"{APIName}\r\njson={JsonConvert.SerializeObject(_request)}");
            try
            {
                if (ModelState.IsValid)
                {
                    var response = repo.Evoucher.UpdateStatus(_request);
                    if (response.StatusCode == 200)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(response.StatusCode, new Error(response.ErrorType, response.ErrorMessage));
                    }

                }
                else
                {
                    return StatusCode(400, new Error("bad_request", JsonConvert.SerializeObject(ModelState.Values.Select(e => e.Errors).ToList())));
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
