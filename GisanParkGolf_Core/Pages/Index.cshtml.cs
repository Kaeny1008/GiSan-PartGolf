using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace GisanParkGolf_Core.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly MyDbContext _db;

        public string UserName { get; set; } = "�մ�";

        public IndexModel(MyDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var userId = User.Identity.Name; // �α��ν� ����� �� (UserId�� �����ؾ� ��)
                var user = _db.SYS_USERS.FirstOrDefault(u => u.USER_ID == userId);
                if (user != null)
                {
                    UserName = user.USER_NAME;
                }
            }
        }
    }
}
