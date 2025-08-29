using GisanParkGolf.Data;
using GisanParkGolf.Pages.Admin.ViewModels;
using GisanParkGolf.Pages.Manager.ViewModels;

namespace GisanParkGolf.Pages.Admin.Services
{
    public class GameResultFinalizeService : IGameResultFinalizeService
    {
        private readonly MyDbContext _dbContext;

        public GameResultFinalizeService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<GameInfoViewModel> GetScoreConfirmedGames()
        {
            return _dbContext.Games
                .Where(g => g.GameStatus == "Score Confirmed" || g.GameStatus == "GameEnd")
                .OrderByDescending(g => g.GameDate)
                .Select(g => new GameInfoViewModel
                {
                    GameCode = g.GameCode,
                    GameName = g.GameName,
                    GameDate = g.GameDate
                })
                .ToList();
        }

        public List<ParticipantResultViewModel> GetGameResults(string gameCode)
        {
            return _dbContext.GameResultScores
                .Where(r => r.GameCode == gameCode)
                .GroupBy(r => new
                {
                    r.UserId,
                    UserName = r.User != null ? r.User.UserName : "Unknown" // r.User null 처리
                })
                .Select(g => new ParticipantResultViewModel
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.UserName,
                    UserScore = g.Sum(r => r.Score ?? 0), // 총 점수 계산
                    ScoreDetails = g.Select(r => new ScoreDetailViewModel
                    {
                        CourseName = r.Course != null ? r.Course.CourseName : "", // 코스 이름
                        HoleName = r.Hole != null ? r.Hole.HoleName : "", // 홀 번호
                        Score = r.Score ?? 0 // 해당 코스 점수
                    }).OrderBy(d => d.HoleName).ToList() // 홀 번호로 정렬
                })
                .ToList();
        }

        public void EndGame(string gameCode)
        {
            var game = _dbContext.Games.FirstOrDefault(g => g.GameCode == gameCode);
            if (game == null)
            {
                throw new InvalidOperationException("대회를 찾을 수 없습니다.");
            }

            game.GameStatus = "GameEnd"; // game_status를 "GameEnd"로 업데이트
            _dbContext.SaveChanges();
        }
    }
}