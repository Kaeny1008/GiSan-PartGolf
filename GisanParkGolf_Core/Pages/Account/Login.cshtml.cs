using GisanParkGolf_Core.Services; // IUserService�� ���� �߰�
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
        // �ڡڡ� DBContext ���, �츮�� ���� �α��� �������� ���Թ޴´�! �ڡڡ�
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
            [Required(ErrorMessage = "ID�� �Է��Ͽ� �ֽʽÿ�.")]
            public string USER_ID { get; set; } = string.Empty;

            [Required(ErrorMessage = "��й�ȣ�� �Է��Ͽ� �ֽʽÿ�.")]
            [DataType(DataType.Password)]
            public string USER_PASSWORD { get; set; } = string.Empty;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // �� ��ȿ�� �˻� ���� ��, ���� �޽����� ���ļ� �����ش�.
                ErrorMessage = string.Join("\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Page();
            }

            // �ڡڡ� '�α��� ������'���� ������ ��û�Ѵ� �ڡڡ�
            var principal = await _userService.AuthenticateUserAsync(Input.USER_ID, Input.USER_PASSWORD);

            if (principal != null)
            {
                // ���� ����! �������� ������� �ź���(principal)���� �α��� ó��
                await HttpContext.SignInAsync("Identity.Application", principal);
                return RedirectToPage("/Index");
            }
            else
            {
                // ���� ����! ģ���� ���� ���� �޽����� �״�� ���
                ErrorMessage = "�α��ο� �����߽��ϴ�.\n���̵� �Ǵ� ��й�ȣ�� Ȯ���ϼ���.";
                return Page();
            }
        }
    }
}