using System.Security.Claims;
using GisanParkGolf_Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GisanParkGolf_Core.Security;

namespace GisanParkGolf_Core.Services.Account
{
    public class Playerservice : IPlayerservice
    {
        private readonly MyDbContext _db;
        private readonly PasswordHasher _hasher;
        private readonly string? _superAdminHash;

        public Playerservice(MyDbContext db, PasswordHasher hasher, IConfiguration configuration)
        {
            _db = db;
            _hasher = hasher;

            // appsettings.json 또는 환경변수에서 superadmin 해시를 읽음
            // 설정 키: "SuperAdmin:PasswordHash" (또는 환경변수 SUPERADMIN__PASSWORDHASH / SUPERADMIN_PASSWORD_HASH 등)
            _superAdminHash = configuration["SuperAdmin:PasswordHash"]
                              ?? configuration["SUPERADMIN_PASSWORD_HASH"];
        }

        public bool IsUserIdExist(string userId)
        {
            return _db.Players.Any(u => u.UserId == userId);
        }

        public async Task<ClaimsPrincipal?> AuthenticateUserAsync(string userId, string password)
        {
            // 1. superadmin 특별 처리: DB에 없고 구성에 해시가 있는 경우, 구성된 해시로 검증
            if (userId.Equals("superadmin", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(_superAdminHash) && _hasher.Verify(password, _superAdminHash))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, "superadmin"),
                        new Claim("DisplayName", "MasterAdmin"),
                        new Claim("IsAdmin", "true")
                    };
                    var identity = new ClaimsIdentity(claims, "Identity.Application");
                    return new ClaimsPrincipal(identity);
                }
                // superadmin 계정은 DB에 없고 구성 해시도 없거나 검증 실패하면 로그인 안됨
                return null;
            }

            // 2. 일반 사용자 처리 (DB에 저장된 해시 검증)
            var user = await _db.Players.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user != null)
            {
                bool passwordMatches = _hasher.Verify(password, user.UserPassword);

                if (passwordMatches)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId),
                        new Claim(ClaimTypes.Name, user.UserName ?? user.UserId),
                        new Claim("UserNumber", user.UserNumber.ToString()),
                        new Claim("UserGender", user.UserGender.ToString()),
                        new Claim("UserWClass", user.UserWClass ?? string.Empty),
                        new Claim("UserClass", user.UserClass.ToString()),
                        new Claim("UserAddress", user.UserAddress ?? string.Empty),
                        new Claim("UserAddress2", user.UserAddress2 ?? string.Empty),
                        new Claim("UserRegistrationDate", user.UserRegistrationDate.ToString("o")),
                        new Claim("IsAdmin", (user.UserClass == 1).ToString().ToLower()),
                        new Claim("IsManager", (user.UserClass == 2).ToString().ToLower()),
                        new Claim("IsMember", (user.UserClass == 3).ToString().ToLower())
                    };
                    var identity = new ClaimsIdentity(claims, "Identity.Application");
                    return new ClaimsPrincipal(identity);
                }
            }

            // 3. 로그인 실패
            return null;
        }
    }
}