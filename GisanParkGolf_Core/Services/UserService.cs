using System.Security.Claims;
using GisanParkGolf_Core.Data;
using Microsoft.EntityFrameworkCore;
using T_Engine;

namespace GisanParkGolf_Core.Services
{
    public class UserService : IUserService
    {
        private readonly MyDbContext _db;
        private readonly Cryptography _crypt;

        public UserService(MyDbContext db, Cryptography crypt)
        {
            _db = db;
            _crypt = crypt;
        }

        public bool IsUserIdExist(string userId)
        {
            return _db.SYS_Users.Any(u => u.UserId == userId);
        }

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
                bool isManager = (user.UserClass == 2);
                bool isMember = (user.UserClass == 3);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId), 
                    new Claim(ClaimTypes.Name, user.UserName ?? user.UserId), 
                    new Claim("UserNumber", user.UserNumber.ToString()),
                    new Claim("UserGender", user.UserGender.ToString()),
                    new Claim("UserWClass", user.UserWClass ?? string.Empty),
                    new Claim("UserClass", user.UserClass.ToString()),
                    new Claim("UserAddress", user.UserAddress),
                    new Claim("UserAddress2", user.UserAddress2 ?? string.Empty),
                    new Claim("UserRegistrationDate", user.UserRegistrationDate.ToString("o")),
                    new Claim("IsAdmin", (user.UserClass == 1).ToString().ToLower()),
                    new Claim("IsManager", (user.UserClass == 2).ToString().ToLower()),
                    new Claim("IsMember", (user.UserClass == 3).ToString().ToLower())
                };
                var identity = new ClaimsIdentity(claims, "Identity.Application");
                return new ClaimsPrincipal(identity);
            }

            // 3. 로그인 실패
            return null;
        }
    }
}