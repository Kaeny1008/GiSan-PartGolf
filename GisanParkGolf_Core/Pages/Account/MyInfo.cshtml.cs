using GisanParkGolf_Core.Data; // SYS_User 모델이 있는 곳
using GisanParkGolf_Core.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using T_Engine; // Cryptography를 위해 추가

namespace GisanParkGolf_Core.Pages.Account
{
    [Authorize] // ★★★ 로그인한 사용자만 접근 가능! ★★★
    public class MyInfoModel : PageModel
    {
        private readonly MyDbContext _context;
        private readonly Cryptography _crypt;

        public MyInfoModel(MyDbContext context, Cryptography crypt)
        {
            _context = context;
            _crypt = crypt;
        }

        [BindProperty]
        public MyInfo_InputModel Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // 현재 로그인한 사용자의 ID를 가져온다.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Challenge(); // 로그인 안됐으면 로그인 페이지로
            }

            var user = await _context.Players.FindAsync(userId);
            if (user == null)
            {
                return NotFound("사용자 정보를 찾을 수 없습니다.");
            }

            // DB에서 읽어온 정보를 InputModel에 채워넣는다.
            Input = new MyInfo_InputModel
            {
                UserId = user.UserId,
                UserName = user.UserName ?? "",
                UserNumber = user.UserNumber.ToString(),
                UserGender = user.UserGender.ToString(),
                UserAddress = user.UserAddress,
                UserAddress2 = user.UserAddress2,
                UserNote = user.UserNote
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // 유효성 검사 실패 시, 페이지를 다시 보여줌
            }

            var userId = User.FindFirstValue(ClaimTypes.Name);
            var userToUpdate = await _context.Players.FindAsync(userId);

            if (userToUpdate == null)
            {
                return NotFound();
            }

            // InputModel의 값으로 DB에서 가져온 userToUpdate 객체의 속성을 변경
            userToUpdate.UserName = Input.UserName;
            userToUpdate.UserNumber = int.Parse(Input.UserNumber);
            userToUpdate.UserGender = int.Parse(Input.UserGender);
            userToUpdate.UserAddress = Input.UserAddress;
            userToUpdate.UserAddress2 = Input.UserAddress2;
            userToUpdate.UserNote = Input.UserNote;

            // 새 비밀번호가 입력되었는지 확인
            if (!string.IsNullOrEmpty(Input.NewPassword))
            {
                userToUpdate.UserPassword = _crypt.GetEncoding("ParkGolf", Input.NewPassword);
            }

            try
            {
                await _context.SaveChangesAsync(); // ★★★ EF Core가 안전하게 DB에 저장! ★★★
            }
            catch (DbUpdateConcurrencyException)
            {
                // 동시성 오류 처리 (다른 사람이 동시에 수정했을 경우)
                ModelState.AddModelError(string.Empty, "다른 사용자에 의해 정보가 변경되었습니다. 페이지를 새로고침 해주세요.");
                return Page();
            }

            // TempData에 성공 메시지를 저장하고, 사용자 목록 페이지로 리다이렉트
            TempData["SuccessMessage"] = "사용자 정보가 성공적으로 수정되었습니다.";
            return RedirectToPage("/Index"); // 또는 다른 원하는 페이지
        }
    }
}