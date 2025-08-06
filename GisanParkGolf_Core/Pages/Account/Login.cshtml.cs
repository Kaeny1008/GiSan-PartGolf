using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq; // Linq ���ӽ����̽��� �߰��ؾ� �մϴ�.
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
            // --- ������� ���� ---
            if (!ModelState.IsValid)
            {
                // ModelState�� �ִ� ��� ���� �޽����� �����ɴϴ�.
                var allErrors = ModelState.Values.SelectMany(v => v.Errors);

                // ���� �޽������� �ϳ��� ���ڿ��� ��Ĩ�ϴ�. (�ٹٲ����� ����)
                ErrorMessage = string.Join("\n", allErrors.Select(e => e.ErrorMessage));

                // Page()�� ��ȯ�ϸ�, ErrorMessage�� ���Ե� �������� �ٽ� �������˴ϴ�.
                return Page();
            }
            // --- ������� ���� ---

            var crypt = new Cryptography();
            string encryptedPassword = crypt.GetEncoding("ParkGolf", Input.USER_PASSWORD);

            // ������ ������ ���� �ϵ��ڵ�
            if (Input.USER_ID == "supervisor" && encryptedPassword == "JfdGnVeo6PR5KwhI3yVlQjhyfM5lT77e")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "������"),
                    new Claim("DisplayName", "MasterAdmin"),
                    new Claim("IsAdmin", "true")
                    // �ʿ��� �߰� Claim
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                await HttpContext.SignInAsync("Identity.Application", principal);

                return RedirectToPage("/Index");
            }

            var user = await _dbContext.SYS_Users.SingleOrDefaultAsync(u => u.UserId == Input.USER_ID);

            // ���� �α��� ����
            if (user != null && user.UserPassword == encryptedPassword)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserId),
                    new Claim("DisplayName", user.UserName ?? user.UserId)
                    // �ʿ��� �߰� Claim
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                await HttpContext.SignInAsync("Identity.Application", principal);
                
                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "�α��ο� �����߽��ϴ�.\n���̵� �Ǵ� ��й�ȣ�� Ȯ���ϼ���.";
                return Page();
            }
        }
    }
}