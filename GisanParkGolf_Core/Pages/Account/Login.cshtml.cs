using GisanParkGolf_Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "ID를 입력하여 주십시오.")]
            public string USER_ID { get; set; } = string.Empty;

            [Required(ErrorMessage = "비밀번호를 입력하여 주십시오.")]
            [DataType(DataType.Password)]
            public string USER_PASSWORD { get; set; } = string.Empty;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var principal = await _userService.AuthenticateUserAsync(Input.USER_ID, Input.USER_PASSWORD);

                if (principal != null)
                {
                    await HttpContext.SignInAsync("Identity.Application", principal);
                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "로그인에 실패했습니다.\n아이디 또는 비밀번호를 확인하세요.");
                }
            }

            return Page();
        }
    }
}