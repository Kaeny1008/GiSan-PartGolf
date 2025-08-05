using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using T_Engine;

namespace GisanParkGolf_Core.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly MyDbContext _dbContext;

        public LoginModel(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ErrorMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required(ErrorMessage = "ID를 입력하여 주십시오.")]
            public string UserID { get; set; } = string.Empty;

            [Required(ErrorMessage = "비밀번호를 입력하여 주십시오.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var crypt = new Cryptography();
            string encryptedPassword = crypt.GetEncoding("ParkGolf", Input.Password);

            var user = await _dbContext.SYS_Users.SingleOrDefaultAsync(u => u.UserId == Input.UserID);

            if (user != null && user.UserPassword == encryptedPassword)
            {
                // 인증 쿠키 발급
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserId),
                    new Claim("DisplayName", user.UserName ?? user.UserId)
                    // 필요한 추가 Claim
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Identity.Application", principal);

                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "로그인에 실패했습니다. 아이디 또는 비밀번호를 확인하세요.";
                return Page();
            }
        }
    }
}