using GisanParkGolf.Data;
using GisanParkGolf.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GisanParkGolf.Services.Player
{
    public class PlayerService : IPlayerService
    {
        private readonly MyDbContext _dbContext;

        public PlayerService(MyDbContext context)
        {
            _dbContext = context;
        }

        public async Task<PaginatedList<Member>> GetPlayersAsync(string? searchField, string? searchQuery, bool readyUserOnly, int pageIndex, int pageSize)
        {
            var query = GetFilteredPlayersQuery(searchField, searchQuery, readyUserOnly);
            var orderedQuery = query.OrderByDescending(u => u.UserRegistrationDate);

            return await PaginatedList<Member>.CreateAsync(orderedQuery, pageIndex, pageSize);
        }

        public async Task<List<Member>> GetPlayersForExcelAsync(string? searchField, string? searchQuery, bool readyUserOnly)
        {
            var query = GetFilteredPlayersQuery(searchField, searchQuery, readyUserOnly);
            return await query.OrderByDescending(u => u.UserRegistrationDate).ToListAsync();
        }

        // 여러 곳에서 사용되는 필터링 로직을 private 메서드로 추출하여 코드 중복 방지
        private IQueryable<Member> GetFilteredPlayersQuery(string? searchField, string? searchQuery, bool readyUserOnly)
        {
            IQueryable<Member> PlayersIQ = _dbContext.Players.AsQueryable();

            if (readyUserOnly)
            {
                PlayersIQ = PlayersIQ.Where(u => u.UserWClass == "승인대기");
            }

            if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrEmpty(searchField))
            {
                switch (searchField)
                {
                    case "UserName":
                        PlayersIQ = PlayersIQ.Where(u => u.UserName.Contains(searchQuery));
                        break;
                    case "UserId":
                        PlayersIQ = PlayersIQ.Where(u => u.UserId.Contains(searchQuery));
                        break;
                }
            }
            return PlayersIQ;
        }

        public async Task ApproveReadyUsersAsync()
        {
            // 대기 상태(ReadyUserOnly=true)인 사용자 모두 승인(Status 변경)
            var users = _dbContext.Players.Where(u => u.UserWClass == UserStatus.Pending).ToList();
            foreach (var user in users)
            {
                user.UserWClass = UserStatus.Approved;
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}