using GisanParkGolf_Core.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // �ڡڡ� List<string>�� ����ϱ� ���� �߰�! �ڡڡ�
using System.ComponentModel.DataAnnotations;
using System.Linq; // �ڡڡ� ToLower()�� ����ϱ� ���� �߰�! �ڡڡ�
using System.Threading.Tasks;
using T_Engine;

namespace GisanParkGolf_Core.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly MyDbContext _dbContext;
        private readonly Cryptography _crypt;

        private readonly List<string> _reservedUserIds = new List<string>
        {
            "admin", "administrator", "root", "manager", "supervisor",
            "system", "master", "webmaster", "sysadmin", "������", "���"
        };

        public RegisterModel(MyDbContext dbContext)
        {
            _dbContext = dbContext;
            _crypt = new Cryptography();
        }

        [BindProperty]
        public Register_InputModel Input { get; set; } = new Register_InputModel();

        public void OnGet()
        {
            // �������� ó�� �ε�� �� ����Ǵ� ��
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_reservedUserIds.Contains(Input.UserId.ToLower()))
            {
                // ������ ���̵���, ������ �߰��ϰ� �������� �ٽ� ������
                ModelState.AddModelError("Input.UserId", "�� ���̵�� ����� �� �����ϴ�.");
                return Page();
            }

            // DB�� �Ȱ��� ���̵� �̹� �ִ��� Ȯ��
            bool isUserExists = await _dbContext.SYS_Users.AnyAsync(u => u.UserId == Input.UserId);
            if (isUserExists)
            {
                // �̹� ���̵� �����ϸ�, ������ �߰��ϰ� �������� �ٽ� ������
                ModelState.AddModelError("Input.UserId", "�̹� ��� ���� ���̵��Դϴ�.");
                return Page();
            }

            // ��� �˻縦 ���������, �� ����ڸ� ����
            var newUser = new SYS_Users
            {
                UserId = Input.UserId,
                UserPassword = _crypt.GetEncoding("ParkGolf", Input.Password),
                UserName = Input.UserName,
                UserNumber = int.Parse(Input.UserNumber),
                UserGender = int.Parse(Input.UserGender),
                UserAddress = Input.UserAddress,
                UserAddress2 = Input.UserAddress2,
                UserNote = Input.UserNote,
                UserWClass = "���δ��",
                UserClass = 3,
                UserRegistrationDate = DateTime.Now
            };

            _dbContext.SYS_Users.Add(newUser);

            try
            {
                // ��������� �����ͺ��̽��� �񵿱�� ����
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "ȸ�������� �Ϸ�Ǿ����ϴ�.\n������ ���� �� �α����Ͽ� �ֽñ� �ٶ��ϴ�.";
                return RedirectToPage("/Account/Login");
            }
            catch (DbUpdateException)
            {
                // �����ͺ��̽� ���� �� ������ �߻����� ���
                ModelState.AddModelError(string.Empty, "���� ó�� �� ������ �߻��߽��ϴ�. ��� �� �ٽ� �õ����ּ���.");
                return Page();
            }
        }
    }
}