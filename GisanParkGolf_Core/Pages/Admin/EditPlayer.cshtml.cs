using GisanParkGolf_Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using T_Engine;

namespace GisanParkGolf_Core.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class EditPlayerModel : PageModel
    {
        private readonly MyDbContext _context;
        private readonly Cryptography _crypt;

        public EditPlayerModel(MyDbContext context, Cryptography crypt)
        {
            _context = context;
            _crypt = crypt;
        }

        [BindProperty]
        public Player_InputModel Input { get; set; } = new Player_InputModel();

        // ★★★ 드롭다운 메뉴를 채울 선택지 목록 ★★★
        public SelectList? UserClassOptions { get; set; }
        public SelectList? ApprovalStatusOptions { get; set; }

        private void PopulateUserClassOptions(object? selectedClass = null)
        {
            // 실제 프로젝트의 회원 등급에 맞게 수정하세요.
            var classes = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Administrator" },
                new SelectListItem { Value = "2", Text = "Manager" },
                new SelectListItem { Value = "3", Text = "Member" }
            };
            UserClassOptions = new SelectList(classes, "Value", "Text", selectedClass);
        }

        // ★★★ "승인", "승인대기" 문자열로 드롭다운을 만드는 헬퍼 ★★★
        private void PopulateApprovalStatusOptions(object? selectedStatus = null)
        {
            var statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "승인", Text = "승인" },
                new SelectListItem { Value = "승인대기", Text = "승인대기" }
            };
            ApprovalStatusOptions = new SelectList(statuses, "Value", "Text", selectedStatus);
        }

        // ★★★ 페이지가 처음 열릴 때 실행 (정보 불러오기) ★★★
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.SYS_Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // DB 데이터를 Input 모델에 채워넣기
            Input = new Player_InputModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserNumber = user.UserNumber.ToString(),
                UserGender = user.UserGender.ToString(),
                UserAddress = user.UserAddress,
                UserAddress2 = user.UserAddress2,
                UserNote = user.UserNote,
                UserWClass = user.UserWClass ?? string.Empty,
                UserClass = user.UserClass
            };

            PopulateUserClassOptions(user.UserClass);
            PopulateApprovalStatusOptions(user.UserWClass);

            return Page();
        }

        // ★★★ '수정하기' 버튼을 눌렀을 때 실행 (정보 저장하기) ★★★
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // 유효성 검사 실패 시, 드롭다운을 다시 채워서 페이지를 보여줌
                PopulateUserClassOptions(Input.UserClass);
                PopulateApprovalStatusOptions(Input.UserWClass);
                return Page();
            }

            var userToUpdate = await _context.SYS_Users.FindAsync(Input.UserId);

            if (userToUpdate == null)
            {
                return NotFound();
            }

            // Input 모델의 값으로 DB에서 가져온 객체를 업데이트
            userToUpdate.UserName = Input.UserName;
            userToUpdate.UserNumber = int.Parse(Input.UserNumber);
            userToUpdate.UserGender = int.Parse(Input.UserGender);
            userToUpdate.UserAddress = Input.UserAddress;
            userToUpdate.UserAddress2 = Input.UserAddress2;
            userToUpdate.UserNote = Input.UserNote;
            userToUpdate.UserClass = Input.UserClass;
            userToUpdate.UserWClass = Input.UserWClass;

            // 새 비밀번호가 입력되었을 때만 암호화하여 변경
            if (!string.IsNullOrEmpty(Input.NewPassword))
            {
                userToUpdate.UserPassword = _crypt.GetEncoding("ParkGolf", Input.NewPassword);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"'{Input.UserName}'님의 정보가 성공적으로 수정되었습니다.";
            return RedirectToPage("/Admin/PlayerManagement"); // 수정 후 회원 목록 페이지로 이동
        }
    }
}