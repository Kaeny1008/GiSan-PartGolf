namespace GisanParkGolf_Core.Services
{
    public class UserService : IUserService
    {
        private readonly MyDbContext _db;

        public UserService(MyDbContext db)
        {
            _db = db;
        }

        public bool IsUserIdExist(string userId)
        {
            return _db.SYS_Users.Any(u => u.UserId == userId);
        }
    }
}
