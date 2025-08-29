using GisanParkGolf.Data;
using GisanParkGolf.Pages.Player.ViewModels;

public class GameResultService : IGameResutService
{
    private readonly MyDbContext _dbContext;

    public GameResultService(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public GameResultViewModel? GetMyResult(string gameCode, string userId)
    {
        var allResults = GetAllResults(gameCode);

        // null 반환
        return allResults.FirstOrDefault(r => r.UserId == userId);
    }

    public List<GameResultViewModel> GetAllResults(string gameCode)
    {
        // 사용자별 점수 계산
        var groupedResults = _dbContext.GameResultScores
            .Where(r => r.GameCode == gameCode)
            .GroupBy(r => r.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                UserName = _dbContext.Players
                    .Where(p => p.UserId == g.Key)
                    .Select(p => p.UserName)
                    .FirstOrDefault(),
                TotalScore = g.Sum(r => r.Score ?? 0) // 점수 합산
            })
            .ToList();

        // 순위 계산
        var rankedResults = groupedResults
            .OrderBy(r => r.TotalScore) // 점수가 낮을수록 높은 순위
            .Select((r, index) => new GameResultViewModel
            {
                UserId = r.UserId,
                UserName = r.UserName,
                TotalScore = r.TotalScore,
                Rank = index + 1, // 순위는 1부터 시작
                Award = GetAward(index + 1) // 순위에 따른 상 계산
            })
            .ToList();

        return rankedResults;
    }

    private string? GetAward(int rank)
    {
        // 순위에 따른 상 설정 (예시)
        return rank switch
        {
            1 => "Gold Medal",
            2 => "Silver Medal",
            3 => "Bronze Medal",
            _ => null
        };
    }
}