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

        // [BindProperty]�� cshtml�� form �����Ϳ� �� ���� �ڵ����� �������ִ� ����
        [BindProperty]
        public Player_InputModel Player { get; set; } = new Player_InputModel();

        // �������� ó�� �ε�� �� (GET ��û)
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
                IsApproved = (user.UserWClass == "����"), // "����" ���ڿ��� true/false�� ��ȯ
                UserClass = user.UserClass
            };

            return Page();
        }

        // "�����ϱ�" ��ư�� ���� �� �����Ͱ� ����� �� (POST ��û)
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

            // �Էµ� ������ DB ���� �����͸� ������Ʈ
            userToUpdate.UserName = Player.UserName;
            userToUpdate.UserNumber = int.Parse(Player.UserNumber);
            userToUpdate.UserGender = int.Parse(Player.UserGender);
            userToUpdate.UserAddress = Player.UserAddress;
            userToUpdate.UserAddress2 = Player.UserAddress2;
            userToUpdate.UserNote = Player.UserNote;
            userToUpdate.UserClass = Player.UserClass;
            userToUpdate.UserWClass = Player.IsApproved ? "����" : "���δ��";

            // ��й�ȣ �ʵ尡 ������� �ʴٸ�, ��й�ȣ�� ������Ʈ
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
                // ���ü� ���� ó�� (�ٸ� ����� ���� �������� ���)
                if (!_context.SYS_Users.Any(e => e.UserId == Player.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // ���� �Ϸ� ��, ��� �������� �����̷�Ʈ
            TempData["SuccessMessage"] = "���� ������ ���������� �����Ǿ����ϴ�.";
            return RedirectToPage("./PlayerManagement");
        }
    }
}