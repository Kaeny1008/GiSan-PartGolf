using Microsoft.AspNetCore.Mvc;
using GisanParkGolf.Services.Account;

namespace GisanParkGolf.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IPlayerservice _Playerservice;
        public AccountController(IPlayerservice Playerservice)
        {
            _Playerservice = Playerservice;
        }

        [HttpGet("checkid")]
        public IActionResult CheckId(string userId)
        {
            bool isExist = _Playerservice.IsUserIdExist(userId);
            return new JsonResult(new { isExist });
        }
    }
}