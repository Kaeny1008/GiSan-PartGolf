using GisanParkGolf.Data;
using GisanParkGolf.Helpers;
using GisanParkGolf.Pages.Player.ViewModels;
using System.Linq;

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
        return allResults.FirstOrDefault(r => r.UserId == userId);
    }

    public List<GameResultViewModel> GetAllResults(string gameCode)
    {
        var groupedResults = _dbContext.GameResultScores
            .Where(r => r.GameCode == gameCode)
            .GroupBy(r => r.UserId)
            .Select(g => new GameResultViewModel
            {
                UserId = g.Key,
                UserName = _dbContext.Players
                    .Where(p => p.UserId == g.Key)
                    .Select(p => p.UserName)
                    .FirstOrDefault() ?? "Unknown",
                TotalScore = g.Sum(r => r.Score ?? 0),
                Rank = 0,
                Award = null
            })
            .ToList();

        var rankedResults = groupedResults
            .OrderBy(r => r.TotalScore)
            .Select((r, index) =>
            {
                r.Rank = index + 1;
                r.Award = GetAward(index + 1);
                return r;
            })
            .ToList();

        return rankedResults;
    }

    public List<HoleResultViewModel> GetHoleResults(string gameCode, string userId)
    {
        var results = _dbContext.GameResultScores
            .Where(r => r.GameCode == gameCode && r.UserId == userId)
            .OrderBy(r => r.HoleId)
            .Select(r => new HoleResultViewModel
            {
                CourseName = r.Course != null ? r.Course.CourseName : "Unknown",
                HoleId = r.HoleId,
                HoleName = r.Hole != null ? r.Hole.HoleName : $"Hole {r.HoleId}",
                Score = r.Score ?? 0
            })
            .ToList();

        // ������ �α� ���
        //foreach (var result in results)
        //{
        //    Console.WriteLine($"CourseName: {result.CourseName}, HoleId: {result.HoleId}, Score: {result.Score}, HoleName: {result.HoleName}");
        //}

        return results;
    }

    public List<CourseViewModel> GetCourses(string gameCode)
    {
        // GameCode�� ���� StadiumCode�� ������
        var stadiumCode = _dbContext.Games
            .Where(g => g.GameCode == gameCode)
            .Select(g => g.StadiumCode)
            .FirstOrDefault();

        if (stadiumCode == null)
        {
            return new List<CourseViewModel>(); // StadiumCode�� ������ �� ����Ʈ ��ȯ
        }

        // StadiumCode�� ������� �ڽ� ���� ��������
        return _dbContext.Courses
            .Where(c => c.StadiumCode == stadiumCode)
            .Select(c => new CourseViewModel
            {
                CourseName = c.CourseName,
                HoleCount = c.HoleCount
            })
            .ToList();
    }

    private string? GetAward(int rank)
    {
        return rank switch
        {
            1 => "1��",
            2 => "2��",
            3 => "3��",
            _ => null
        };
    }

    public Task<PaginatedList<GameResultViewModel>> GetFilteredResults(
        string gameCode, string? searchField, string? searchQuery, int pageIndex, int pageSize)
    {
        var groupedResults = _dbContext.GameResultScores
            .Where(r => r.GameCode == gameCode)
            .GroupBy(r => r.UserId)
            .Select(g => new GameResultViewModel
            {
                UserId = g.Key,
                UserName = _dbContext.Players
                    .Where(p => p.UserId == g.Key)
                    .Select(p => p.UserName)
                    .FirstOrDefault() ?? "Unknown",
                TotalScore = g.Sum(r => r.Score ?? 0),
                Rank = 0,
                Award = null
            })
            .ToList();

        // ���� ���
        groupedResults = groupedResults
            .OrderBy(r => r.TotalScore)
            .Select((r, i) => { r.Rank = i + 1; return r; })
            .ToList();

        // �˻� ����
        if (!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(searchQuery))
        {
            groupedResults = searchField switch
            {
                "UserName" => groupedResults.Where(r => r.UserName.Contains(searchQuery)).ToList(),
                "Rank" => groupedResults.Where(r => r.Rank.ToString().Contains(searchQuery)).ToList(),
                _ => groupedResults
            };
        }

        // ���� ����¡ ó��
        var pagedResults = groupedResults
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalCount = groupedResults.Count;

        // ���� ����� Task�� �����ؼ� ��ȯ
        return Task.FromResult(new PaginatedList<GameResultViewModel>(pagedResults, totalCount, pageIndex, pageSize));
    }

    public int GetTotalPages(string gameCode, string? searchField, string? searchQuery, int pageSize)
    {
        var groupedResults = _dbContext.GameResultScores
            .Where(r => r.GameCode == gameCode)
            .GroupBy(r => r.UserId)
            .Select(g => new GameResultViewModel
            {
                UserId = g.Key,
                UserName = _dbContext.Players
                    .Where(p => p.UserId == g.Key)
                    .Select(p => p.UserName)
                    .FirstOrDefault() ?? "Unknown",
                TotalScore = g.Sum(r => r.Score ?? 0)
            })
            .ToList();

        if (!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(searchQuery))
        {
            groupedResults = searchField switch
            {
                "UserName" => groupedResults.Where(r => r.UserName.Contains(searchQuery)).ToList(),
                "Rank" => groupedResults.Where(r => r.Rank.ToString().Contains(searchQuery)).ToList(),
                _ => groupedResults
            };
        }

        return (int)Math.Ceiling(groupedResults.Count / (double)pageSize);
    }
}