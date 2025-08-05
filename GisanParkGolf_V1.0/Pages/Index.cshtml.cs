using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace GisanParkGolf_V1._0.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly MyDbContext _db;

        public string UserName { get; set; } = "손님";

        public IndexModel(MyDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var userId = User.Identity.Name; // 로그인시 저장된 값 (UserId와 동일해야 함)
                var user = _db.SYS_Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    UserName = user.UserName;
                }
            }
        }
    }
}
