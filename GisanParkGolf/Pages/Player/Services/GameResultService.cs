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

        // null ��ȯ
        return allResults.FirstOrDefault(r => r.UserId == userId);
    }

    public List<GameResultViewModel> GetAllResults(string gameCode)
    {
        // ����ں� ���� ���
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
                TotalScore = g.Sum(r => r.Score ?? 0) // ���� �ջ�
            })
            .ToList();

        // ���� ���
        var rankedResults = groupedResults
            .OrderBy(r => r.TotalScore) // ������ �������� ���� ����
            .Select((r, index) => new GameResultViewModel
            {
                UserId = r.UserId,
                UserName = r.UserName,
                TotalScore = r.TotalScore,
                Rank = index + 1, // ������ 1���� ����
                Award = GetAward(index + 1) // ������ ���� �� ���
            })
            .ToList();

        return rankedResults;
    }

    private string? GetAward(int rank)
    {
        // ������ ���� �� ���� (����)
        return rank switch
        {
            1 => "Gold Medal",
            2 => "Silver Medal",
            3 => "Bronze Medal",
            _ => null
        };
    }
}