using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly MyDbContext _context;

        public PlayerService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<SYS_Users>> GetPlayersAsync(string? searchField, string? searchQuery, bool readyUserOnly, int pageIndex, int pageSize)
        {
            var query = GetFilteredUsersQuery(searchField, searchQuery, readyUserOnly);
            var orderedQuery = query.OrderByDescending(u => u.UserRegistrationDate);

            return await PaginatedList<SYS_Users>.CreateAsync(orderedQuery, pageIndex, pageSize);
        }

        public async Task<List<SYS_Users>> GetPlayersForExcelAsync(string? searchField, string? searchQuery, bool readyUserOnly)
        {
            var query = GetFilteredUsersQuery(searchField, searchQuery, readyUserOnly);
            return await query.OrderByDescending(u => u.UserRegistrationDate).ToListAsync();
        }

        // 여러 곳에서 사용되는 필터링 로직을 private 메서드로 추출하여 코드 중복 방지
        private IQueryable<SYS_Users> GetFilteredUsersQuery(string? searchField, string? searchQuery, bool readyUserOnly)
        {
            IQueryable<SYS_Users> usersIQ = _context.SYS_Users.AsQueryable();

            if (readyUserOnly)
            {
                usersIQ = usersIQ.Where(u => u.UserWClass == "승인");
            }

            if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrEmpty(searchField))
            {
                switch (searchField)
                {
                    case "UserName":
                        usersIQ = usersIQ.Where(u => u.UserName.Contains(searchQuery));
                        break;
                    case "UserId":
                        usersIQ = usersIQ.Where(u => u.UserId.Contains(searchQuery));
                        break;
                }
            }
            return usersIQ;
        }
    }
}