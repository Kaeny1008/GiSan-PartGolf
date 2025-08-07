using GisanParkGolf_Core.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using T_Engine;

namespace GisanParkGolf_Core.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly MyDbContext _dbContext;

        public RegisterModel(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public SYS_Users? NewUser { get; set; }

        [BindProperty]
        public Register_InputModel Input { get; set; } = new Register_InputModel();

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "폼 입력을 확인하세요.";
                return Page();
            }

            NewUser = new SYS_Users
            {
                UserId = Input.UserId,
                UserPassword = Input.Password,
                UserName = Input.UserName,
                UserNumber = int.Parse(Input.UserNumber),
                UserGender = int.Parse(Input.UserGender),
                UserAddress = Input.UserAddress,
                UserAddress2 = Input.UserAddress2,
                UserNote = Input.UserNote,
                UserWClass = "승인대기",
                UserClass = 3,
                UserRegistrationDate = DateTime.Now
            };

            // 비밀번호 암호화
            var crypt = new Cryptography();
            string encryptedPassword = crypt.GetEncoding("ParkGolf", NewUser.UserPassword);
            NewUser.UserPassword = encryptedPassword;

            _dbContext.SYS_Users.Add(NewUser);

            try
            {
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "가입 승인대기 되었습니다.\n관리자 승인 완료 후 사용 하실 수 있습니다.";
                return RedirectToPage("/Account/Login"); // 성공 시 이동
            }
            catch (Exception ex)
            {
                ErrorMessage = "가입 중 오류가 발생했습니다: " + ex.Message;
                return Page(); // 실패 시 현재 페이지에서 에러 메시지 표시
            }
        }
    }
}
