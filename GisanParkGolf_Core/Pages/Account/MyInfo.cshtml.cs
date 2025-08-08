using GisanParkGolf_Core.Data; // SYS_User ���� �ִ� ��
using GisanParkGolf_Core.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using T_Engine; // Cryptography�� ���� �߰�

namespace GisanParkGolf_Core.Pages.Account
{
    [Authorize] // �ڡڡ� �α����� ����ڸ� ���� ����! �ڡڡ�
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
            // ���� �α����� ������� ID�� �����´�.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Challenge(); // �α��� �ȵ����� �α��� ��������
            }

            var user = await _context.Players.FindAsync(userId);
            if (user == null)
            {
                return NotFound("����� ������ ã�� �� �����ϴ�.");
            }

            // DB���� �о�� ������ InputModel�� ä���ִ´�.
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
                return Page(); // ��ȿ�� �˻� ���� ��, �������� �ٽ� ������
            }

            var userId = User.FindFirstValue(ClaimTypes.Name);
            var userToUpdate = await _context.Players.FindAsync(userId);

            if (userToUpdate == null)
            {
                return NotFound();
            }

            // InputModel�� ������ DB���� ������ userToUpdate ��ü�� �Ӽ��� ����
            userToUpdate.UserName = Input.UserName;
            userToUpdate.UserNumber = int.Parse(Input.UserNumber);
            userToUpdate.UserGender = int.Parse(Input.UserGender);
            userToUpdate.UserAddress = Input.UserAddress;
            userToUpdate.UserAddress2 = Input.UserAddress2;
            userToUpdate.UserNote = Input.UserNote;

            // �� ��й�ȣ�� �ԷµǾ����� Ȯ��
            if (!string.IsNullOrEmpty(Input.NewPassword))
            {
                userToUpdate.UserPassword = _crypt.GetEncoding("ParkGolf", Input.NewPassword);
            }

            try
            {
                await _context.SaveChangesAsync(); // �ڡڡ� EF Core�� �����ϰ� DB�� ����! �ڡڡ�
            }
            catch (DbUpdateConcurrencyException)
            {
                // ���ü� ���� ó�� (�ٸ� ����� ���ÿ� �������� ���)
                ModelState.AddModelError(string.Empty, "�ٸ� ����ڿ� ���� ������ ����Ǿ����ϴ�. �������� ���ΰ�ħ ���ּ���.");
                return Page();
            }

            // TempData�� ���� �޽����� �����ϰ�, ����� ��� �������� �����̷�Ʈ
            TempData["SuccessMessage"] = "����� ������ ���������� �����Ǿ����ϴ�.";
            return RedirectToPage("/Index"); // �Ǵ� �ٸ� ���ϴ� ������
        }
    }
}