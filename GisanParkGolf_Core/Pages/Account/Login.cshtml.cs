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
            [Required(ErrorMessage = "ID�� �Է��Ͽ� �ֽʽÿ�.")]
            public string USER_ID { get; set; } = string.Empty;

            [Required(ErrorMessage = "��й�ȣ�� �Է��Ͽ� �ֽʽÿ�.")]
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
                    ModelState.AddModelError(string.Empty, "�α��ο� �����߽��ϴ�.\n���̵� �Ǵ� ��й�ȣ�� Ȯ���ϼ���.");
                }
            }

            return Page();
        }
    }
}