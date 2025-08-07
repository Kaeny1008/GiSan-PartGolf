using GisanParkGolf_Core.Services; // IUserService를 위해 추가
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
        // ★★★ DBContext 대신, 우리가 만든 로그인 전문가를 주입받는다! ★★★
        private readonly IUserService _userService;

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ErrorMessage { get; set; } = string.Empty;

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
            if (!ModelState.IsValid)
            {
                // 모델 유효성 검사 실패 시, 에러 메시지를 합쳐서 보여준다.
                ErrorMessage = string.Join("\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Page();
            }

            // ★★★ '로그인 전문가'에게 인증을 요청한다 ★★★
            var principal = await _userService.AuthenticateUserAsync(Input.USER_ID, Input.USER_PASSWORD);

            if (principal != null)
            {
                // 인증 성공! 전문가가 만들어준 신분증(principal)으로 로그인 처리
                await HttpContext.SignInAsync("Identity.Application", principal);
                return RedirectToPage("/Index");
            }
            else
            {
                // 인증 실패! 친구가 만든 에러 메시지를 그대로 사용
                ErrorMessage = "로그인에 실패했습니다.\n아이디 또는 비밀번호를 확인하세요.";
                return Page();
            }
        }
    }
}