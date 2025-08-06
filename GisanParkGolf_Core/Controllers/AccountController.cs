using Microsoft.AspNetCore.Mvc;
using GisanParkGolf_Core.Services;

namespace GisanParkGolf_Core.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("checkid")]
        public IActionResult CheckId(string userId)
        {
            bool isExist = _userService.IsUserIdExist(userId);
            return new JsonResult(new { isExist });
        }
    }
}