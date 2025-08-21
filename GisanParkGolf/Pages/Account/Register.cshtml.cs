using GisanParkGolf.Data;
using GisanParkGolf.Security;
using GisanParkGolf.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GisanParkGolf.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly MyDbContext _dbContext;
        private readonly PasswordHasher _hasher;

        private readonly List<string> _reservedUserIds = new List<string>
        {
            "admin", "administrator", "root", "manager", "supervisor",
            "system", "master", "webmaster", "sysadmin", "관리자", "운영자"
        };

        public RegisterModel(MyDbContext dbContext, PasswordHasher hasher)
        {
            _dbContext = dbContext;
            _hasher = hasher;
        }

        [BindProperty]
        public Register_InputModel Input { get; set; } = new Register_InputModel();

        public void OnGet()
        {
            // 페이지가 처음 로드될 때 실행되는 곳
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_reservedUserIds.Contains(Input.UserId.ToLower()))
            {
                // 금지된 아이디라면, 에러를 추가하고 페이지를 다시 보여줌
                ModelState.AddModelError("Input.UserId", "이 아이디는 사용할 수 없습니다.");
                return Page();
            }

            // DB에 똑같은 아이디가 이미 있는지 확인
            bool isUserExists = await _dbContext.Players.AnyAsync(u => u.UserId == Input.UserId);
            if (isUserExists)
            {
                // 이미 아이디가 존재하면, 에러를 추가하고 페이지를 다시 보여줌
                ModelState.AddModelError("Input.UserId", "이미 사용 중인 아이디입니다.");
                return Page();
            }

            // 모든 검사를 통과했으니, 새 사용자를 생성
            var newUser = new Player
            {
                UserId = Input.UserId,
                UserPassword = _hasher.Hash(Input.Password),
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

            _dbContext.Players.Add(newUser);

            try
            {
                // 변경사항을 데이터베이스에 비동기로 저장
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "회원가입이 완료되었습니다.\n관리자 승인 후 로그인하여 주시기 바랍니다.";
                return RedirectToPage("/Account/Login");
            }
            catch (DbUpdateException)
            {
                // 데이터베이스 저장 중 오류가 발생했을 경우
                ModelState.AddModelError(string.Empty, "가입 처리 중 오류가 발생했습니다. 잠시 후 다시 시도해주세요.");
                return Page();
            }
        }
    }
}