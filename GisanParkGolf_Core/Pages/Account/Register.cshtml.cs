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
                ErrorMessage = "�� �Է��� Ȯ���ϼ���.";
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
                UserWClass = "���δ��",
                UserClass = 3,
                UserRegistrationDate = DateTime.Now
            };

            // ��й�ȣ ��ȣȭ
            var crypt = new Cryptography();
            string encryptedPassword = crypt.GetEncoding("ParkGolf", NewUser.UserPassword);
            NewUser.UserPassword = encryptedPassword;

            _dbContext.SYS_Users.Add(NewUser);

            try
            {
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "���� ���δ�� �Ǿ����ϴ�.\n������ ���� �Ϸ� �� ��� �Ͻ� �� �ֽ��ϴ�.";
                return RedirectToPage("/Account/Login"); // ���� �� �̵�
            }
            catch (Exception ex)
            {
                ErrorMessage = "���� �� ������ �߻��߽��ϴ�: " + ex.Message;
                return Page(); // ���� �� ���� ���������� ���� �޽��� ǥ��
            }
        }
    }
}
