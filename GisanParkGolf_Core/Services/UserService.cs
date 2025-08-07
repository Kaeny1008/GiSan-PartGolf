using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using T_Engine;

namespace GisanParkGolf_Core.Services
{
    public class UserService : IUserService
    {
        private readonly MyDbContext _db;
        private readonly Cryptography _crypt; // 암호화 모듈도 주입받자!

        public UserService(MyDbContext db, Cryptography crypt)
        {
            _db = db;
            _crypt = crypt;
        }

        public bool IsUserIdExist(string userId)
        {
            return _db.SYS_Users.Any(u => u.UserId == userId);
        }

        // ★★★ 실제 로그인 로직을 여기에 구현! ★★★
        public async Task<ClaimsPrincipal?> AuthenticateUserAsync(string userId, string password)
        {
            string encryptedPassword = _crypt.GetEncoding("ParkGolf", password);

            // 1. 슈퍼관리자 계정 특별 처리
            if (userId == "superadmin" && encryptedPassword == "JfdGnVeo6PR5KwhI3yVlQjhyfM5lT77e")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "superadmin"), // 슈퍼관리자 ID
                    new Claim("DisplayName", "MasterAdmin"),
                    new Claim("IsAdmin", "true")
                };
                var identity = new ClaimsIdentity(claims, "Identity.Application");
                return new ClaimsPrincipal(identity);
            }

            // 2. 일반 사용자 처리
            var user = await _db.SYS_Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user != null && user.UserPassword == encryptedPassword)
            {
                bool isAdmin = (user.UserClass == 1);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserId),
                    new Claim("DisplayName", user.UserName ?? user.UserId),
                    new Claim("IsAdmin", isAdmin.ToString().ToLower())
                };
                var identity = new ClaimsIdentity(claims, "Identity.Application");
                return new ClaimsPrincipal(identity);
            }

            // 3. 로그인 실패
            return null;
        }
    }
}