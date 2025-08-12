using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.ViewModels.AdminPage;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace GiSanParkGolf.Services.AdminPage
{
    public class GameSetupService : IGameSetupService
    {
        private readonly MyDbContext _dbContext;

        public GameSetupService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<GameListViewModel> GetGames(string keyword)
        {
            var key = keyword ?? "";
            return _dbContext.Games
                .Where(g => (g.GameName ?? "").Contains(key))
                .Select(g => new GameListViewModel
                {
                    GameCode = g.GameCode,
                    GameName = g.GameName,
                    GameDate = g.GameDate,
                    StadiumName = g.StadiumName,
                    GameHost = g.GameHost,
                    GameStatus = g.GameStatus,
                    ParticipantNumber = g.Participants.Count(),
                    HoleMaximum = g.HoleMaximum,
                    PlayMode = g.PlayMode,
                    Note = g.GameNote
                })
                .ToList();
        }

        public GameListViewModel? GetGameInformation(string code)
        {
            return _dbContext.Games
                .Where(g => g.GameCode == code)
                .Select(g => new GameListViewModel
                {
                    GameCode = g.GameCode,
                    GameName = g.GameName,
                    GameDate = g.GameDate,
                    StadiumName = g.StadiumName,
                    GameHost = g.GameHost,
                    GameStatus = g.GameStatus,
                    ParticipantNumber = g.Participants.Count(),
                    HoleMaximum = g.HoleMaximum,
                    PlayMode = g.PlayMode,
                    Note = g.GameNote
                })
                .FirstOrDefault();
        }

        public List<GameJoinUserListViewModel> GetGameUserList(string code)
        {
            return (
                from p in _dbContext.GameParticipants
                where p.GameCode == code
                join u in _dbContext.Players on p.UserId equals u.UserId into uj
                from u in uj.DefaultIfEmpty()
                join h in _dbContext.PlayerHandicaps on p.UserId equals h.UserId into hj
                from h in hj.DefaultIfEmpty()
                let awardCount = _dbContext.GameAwardHistories.Count(a => a.UserId == p.UserId)
                select new GameJoinUserListViewModel
                {
                    UserId = p.UserId,
                    UserName = u != null ? u.UserName : "",
                    UserNumber = u != null ? u.UserNumber : 0,
                    UserGender = u != null ? u.UserGender : 0,
                    AgeHandicap = h != null ? h.AgeHandicap : 0,
                    AwardsSummary = awardCount > 0 ? $"{awardCount}회 수상" : "수상 내역 없음",
                    IsCancelled = p.IsCancelled,
                    JoinStatus = p.JoinStatus
                }
            ).ToList();
        }

        //public List<AssignedPlayerViewModel> GetAssignmentResult(string code)
        //{
        //    // 해당 게임의 배정 결과 반환(예시)
        //    return _dbContext.AssignedPlayers
        //        .Where(a => a.GameCode == code)
        //        .Select(a => new AssignedPlayerViewModel
        //        {
        //            PlayerId = a.PlayerId,
        //            PlayerName = a.Player.Name,
        //            AssignmentStatus = a.AssignmentStatus,
        //            // 필요시 추가 필드
        //        })
        //        .ToList();
        //}
    }
}