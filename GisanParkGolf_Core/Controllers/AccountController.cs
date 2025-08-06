using Microsoft.AspNetCore.Mvc;
using GisanParkGolf_Core.Services;

namespace GisanParkGolf_Core.Controllers
{
    [ApiController]
    [Route("api/account")] // <-- 이 부분도 체크!
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("checkid")] // <-- 이렇게 하면 /api/account/checkid
        public IActionResult CheckId(string userId)
        {
            bool isExist = _userService.IsUserIdExist(userId);
            return new JsonResult(new { isExist });
        }
    }
}