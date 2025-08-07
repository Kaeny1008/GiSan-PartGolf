using GisanParkGolf_Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using T_Engine;

namespace GiSanParkGolf.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class EditPlayerModel : PageModel
    {
        private readonly MyDbContext _context;
        private readonly Cryptography _cryptography;

        public EditPlayerModel(MyDbContext context, Cryptography cryptography)
        {
            _context = context;
            _cryptography = cryptography;
        }

        // [BindProperty]는 cshtml의 form 데이터와 이 모델을 자동으로 연결해주는 마법
        [BindProperty]
        public Player_InputModel Player { get; set; } = new Player_InputModel();

        // 페이지가 처음 로드될 때 (GET 요청)
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.SYS_Users.FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            Player = new Player_InputModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserNumber = user.UserNumber.ToString(),
                UserGender = user.UserGender.ToString(),
                UserAddress = user.UserAddress,
                UserAddress2 = user.UserAddress2,
                UserNote = user.UserNote,
                IsApproved = (user.UserWClass == "승인"), // "승인" 문자열을 true/false로 변환
                UserClass = user.UserClass
            };

            return Page();
        }

        // "수정하기" 버튼을 눌러 폼 데이터가 제출될 때 (POST 요청)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userToUpdate = await _context.SYS_Users.FirstOrDefaultAsync(u => u.UserId == Player.UserId);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            // 입력된 값으로 DB 모델의 데이터를 업데이트
            userToUpdate.UserName = Player.UserName;
            userToUpdate.UserNumber = int.Parse(Player.UserNumber);
            userToUpdate.UserGender = int.Parse(Player.UserGender);
            userToUpdate.UserAddress = Player.UserAddress;
            userToUpdate.UserAddress2 = Player.UserAddress2;
            userToUpdate.UserNote = Player.UserNote;
            userToUpdate.UserClass = Player.UserClass;
            userToUpdate.UserWClass = Player.IsApproved ? "승인" : "승인대기";

            // 비밀번호 필드가 비어있지 않다면, 비밀번호도 업데이트
            if (!string.IsNullOrEmpty(Player.NewPassword))
            {
                userToUpdate.UserPassword = _cryptography.GetEncoding("ParkGolf", Player.NewPassword);
            }

            _context.Attach(userToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 동시성 오류 처리 (다른 사람이 먼저 수정했을 경우)
                if (!_context.SYS_Users.Any(e => e.UserId == Player.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // 수정 완료 후, 목록 페이지로 리다이렉트
            TempData["SuccessMessage"] = "선수 정보가 성공적으로 수정되었습니다.";
            return RedirectToPage("./PlayerManagement");
        }
    }
}