using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GisanParkGolf_V1._0.Pages.Account
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
            public string UserID { get; set; } = string.Empty;

            [Required(ErrorMessage = "��й�ȣ�� �Է��Ͽ� �ֽʽÿ�.")]
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

            var user = await _dbContext.SYS_Users.SingleOrDefaultAsync(u => u.UserId == Input.UserID);

            // ��й�ȣ �� �񱳴� ����, ���� ������ ���� ���
            if (user != null && user.UserPassword == Input.Password)
            {
                // ���� ��Ű �߱�
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserId),
                    new Claim("DisplayName", user.UserName ?? user.UserId)
                    // �ʿ��� �߰� Claim
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

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