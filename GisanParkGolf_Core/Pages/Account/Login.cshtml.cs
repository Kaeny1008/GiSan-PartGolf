using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
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
            [Required(ErrorMessage = "ID�� �Է��Ͽ� �ֽʽÿ�.")]
            public string USER_ID { get; set; } = string.Empty;

            [Required(ErrorMessage = "��й�ȣ�� �Է��Ͽ� �ֽʽÿ�.")]
            [DataType(DataType.Password)]
            public string USER_PASSWORD { get; set; } = string.Empty;
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
            string encryptedPassword = crypt.GetEncoding("ParkGolf", Input.USER_PASSWORD);

            var user = await _dbContext.SYS_USERS.SingleOrDefaultAsync(u => u.USER_ID == Input.USER_ID);

            if (user != null && user.USER_PASSWORD == encryptedPassword)
            {
                // ���� ��Ű �߱�
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.USER_ID),
                    new Claim("DisplayName", user.USER_NAME ?? user.USER_ID)
                    // �ʿ��� �߰� Claim
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Identity.Application", principal);

                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "�α��ο� �����߽��ϴ�. ���̵� �Ǵ� ��й�ȣ�� Ȯ���ϼ���.";
                return Page();
            }
        }
    }
}