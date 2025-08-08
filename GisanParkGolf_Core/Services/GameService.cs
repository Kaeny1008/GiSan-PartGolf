using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GisanParkGolf_Core.Services
{
    public class GameService : IGameService
    {
        private readonly MyDbContext _dbContext;

        public GameService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedList<Game>> GetGamesAsync(string? searchField, string? searchQuery, int pageIndex, int pageSize)
        {
            var query = _dbContext.Games.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery) && !string.IsNullOrWhiteSpace(searchField))
            {
                searchQuery = searchQuery.Trim();

                switch (searchField)
                {
                    case "GameName":
                        query = query.Where(g => (g.GameName ?? "").Contains(searchQuery));
                        break;
                    case "GameHost":
                        query = query.Where(g => (g.GameHost ?? "").Contains(searchQuery));
                        break;
                    case "StadiumName":
                        query = query.Where(g => (g.StadiumName ?? "").Contains(searchQuery));
                        break;
                }
            }

            var orderedQuery = query.OrderByDescending(g => g.GameDate);
            return await PaginatedList<Game>.CreateAsync(orderedQuery.AsNoTracking(), pageIndex, pageSize);
        }

        public async Task<Game?> GetGameByIdAsync(string gameCode)
        {
            return await _dbContext.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
        }

        public async Task CreateGameAsync(Game game)
        {
            // 게임 코드 생성 로직 (예시)
            game.GameCode = DateTime.Now.ToString("yyMMdd"); // 실제로는 더 정교한 방법 필요
            _dbContext.Games.Add(game);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateGameAsync(Game game)
        {
            _dbContext.Games.Update(game);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateGameStatusAsync(string gameCode, string status)
        {
            var game = await _dbContext.Games.FindAsync(gameCode);
            if (game != null)
            {
                game.GameStatus = status;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}