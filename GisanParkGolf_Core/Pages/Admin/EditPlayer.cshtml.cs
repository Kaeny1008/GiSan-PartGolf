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

        // �ڡڡ� ��Ӵٿ� �޴��� ä�� ������ ��� �ڡڡ�
        public SelectList? UserClassOptions { get; set; }
        public SelectList? ApprovalStatusOptions { get; set; }

        private void PopulateUserClassOptions(object? selectedClass = null)
        {
            // ���� ������Ʈ�� ȸ�� ��޿� �°� �����ϼ���.
            var classes = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Administrator" },
                new SelectListItem { Value = "2", Text = "Manager" },
                new SelectListItem { Value = "3", Text = "Member" }
            };
            UserClassOptions = new SelectList(classes, "Value", "Text", selectedClass);
        }

        // �ڡڡ� "����", "���δ��" ���ڿ��� ��Ӵٿ��� ����� ���� �ڡڡ�
        private void PopulateApprovalStatusOptions(object? selectedStatus = null)
        {
            var statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "����", Text = "����" },
                new SelectListItem { Value = "���δ��", Text = "���δ��" }
            };
            ApprovalStatusOptions = new SelectList(statuses, "Value", "Text", selectedStatus);
        }

        // �ڡڡ� �������� ó�� ���� �� ���� (���� �ҷ�����) �ڡڡ�
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

            // DB �����͸� Input �𵨿� ä���ֱ�
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

        // �ڡڡ� '�����ϱ�' ��ư�� ������ �� ���� (���� �����ϱ�) �ڡڡ�
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // ��ȿ�� �˻� ���� ��, ��Ӵٿ��� �ٽ� ä���� �������� ������
                PopulateUserClassOptions(Input.UserClass);
                PopulateApprovalStatusOptions(Input.UserWClass);
                return Page();
            }

            var userToUpdate = await _context.SYS_Users.FindAsync(Input.UserId);

            if (userToUpdate == null)
            {
                return NotFound();
            }

            // Input ���� ������ DB���� ������ ��ü�� ������Ʈ
            userToUpdate.UserName = Input.UserName;
            userToUpdate.UserNumber = int.Parse(Input.UserNumber);
            userToUpdate.UserGender = int.Parse(Input.UserGender);
            userToUpdate.UserAddress = Input.UserAddress;
            userToUpdate.UserAddress2 = Input.UserAddress2;
            userToUpdate.UserNote = Input.UserNote;
            userToUpdate.UserClass = Input.UserClass;
            userToUpdate.UserWClass = Input.UserWClass;

            // �� ��й�ȣ�� �ԷµǾ��� ���� ��ȣȭ�Ͽ� ����
            if (!string.IsNullOrEmpty(Input.NewPassword))
            {
                userToUpdate.UserPassword = _crypt.GetEncoding("ParkGolf", Input.NewPassword);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"'{Input.UserName}'���� ������ ���������� �����Ǿ����ϴ�.";
            return RedirectToPage("/Admin/PlayerManagement"); // ���� �� ȸ�� ��� �������� �̵�
        }
    }
}