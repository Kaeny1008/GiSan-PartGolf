using DocumentFormat.OpenXml.InkML;
using GiSanParkGolf.Pages.AdminPage;
using GiSanParkGolf.Pages.PlayerPage;
using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.ViewModels.AdminPage;
using Microsoft.EntityFrameworkCore;

namespace GisanParkGolf_Core.Services.AdminPage
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
            return await _dbContext.Games
                .Include(g => g.Stadium)
                .FirstOrDefaultAsync(g => g.GameCode == gameCode);
        }

        public async Task CreateGameAsync(Game game)
        {
            // 게임 코드 생성: 날짜 + GUID 일부 (중복 거의 없음, 가독성도 어느 정도)
            var datePart = DateTime.Now.ToString("yyMMdd");
            var guidPart = Guid.NewGuid().ToString("N").Substring(0, 6); // 6자리
            game.GameCode = $"{datePart}-{guidPart}";
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

        // ############## 아래부터 코스배치관련 메서드 ##############
        // 대회 목록 불러오기
        // ... 기존 using 및 namespace ...

        public async Task<PaginatedList<CompetitionViewModel>> GetCompetitionsAsync(
            string? searchField, string? searchQuery, int pageIndex, int pageSize)
        {
            var query = _dbContext.Games.Include(g => g.Stadium).AsQueryable();

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
                        query = query.Where(g => g.Stadium != null && (g.Stadium.StadiumName ?? "").Contains(searchQuery));
                        break;
                }
            }

            var orderedQuery = query.OrderByDescending(g => g.GameDate);

            // 1. 대회 목록 페이징
            var pagedGames = await PaginatedList<Game>.CreateAsync(
                orderedQuery.AsNoTracking(), pageIndex, pageSize);

            // 2. 각 대회별로 참가자/코스배치 인원 수 집계
            var competitionList = new List<CompetitionViewModel>();
            foreach (var g in pagedGames)
            {
                // 참가자 전체 리스트
                var participants = await _dbContext.GameParticipants
                    .Where(p => p.GameCode == g.GameCode)
                    .Include(p => p.User)
                    .ToListAsync();

                // 코스배치 인원 수 (GameUserAssignments)
                int assignmentCount = await _dbContext.GameUserAssignments
                    .CountAsync(a => a.GameCode == g.GameCode);

                // 취소 안된 참가자 수 (IsCancelled == false)
                int joinedCount = participants.Count(p => !p.IsCancelled);

                // 수상경력 정보 (기존)
                var participantAwards = participants
                    .Select(p => new ParticipantAwardInfo
                    {
                        UserId = p.UserId,
                        UserName = p.User?.UserName ?? p.UserId ?? "이름없음",
                        AwardHistories = _dbContext.GameAwardHistories
                            .Where(a => a.UserId == p.UserId)
                            .OrderByDescending(a => a.AwardDate)
                            .ToList()
                    }).ToList();

                competitionList.Add(new CompetitionViewModel
                {
                    GameCode = g.GameCode,
                    GameName = g.GameName,
                    GameDate = g.GameDate,
                    Status = g.GameStatus,
                    StadiumName = g.Stadium == null ? "(미지정)" : g.Stadium.StadiumName,
                    TotalParticipants = participants.Count,
                    JoinedCount = joinedCount,         // 추가됨
                    AssignmentCount = assignmentCount, // 추가됨
                    GameHost = g.GameHost,
                    PlayMode = PlayModeHelper.ToKorDisplay(g.PlayMode),
                    GameNote = g.GameNote,
                    ParticipantAwards = participantAwards
                });
            }

            // 3. CompetitionViewModel 리스트를 PaginatedList로 반환
            return new PaginatedList<CompetitionViewModel>(
                competitionList, pagedGames.TotalCount, pagedGames.PageIndex, pagedGames.PageSize);
        }

        // 참가자 목록 불러오기
        public async Task<PaginatedList<ParticipantViewModel>> GetParticipantsAsync(
            string gameCode, string? searchQuery, int pageIndex, int pageSize)
        {
            var query = _dbContext.GameParticipants
                .Include(p => p.User)
                .Where(p => p.GameCode == gameCode);

            if (!string.IsNullOrWhiteSpace(searchQuery))
                query = query.Where(p =>
                    (p.User != null && p.User.UserName.Contains(searchQuery)) ||
                    (p.UserId != null && p.UserId.Contains(searchQuery)) ||
                    (p.JoinId != null && p.JoinId.Contains(searchQuery))
                );

            var projected = query.OrderBy(p => p.JoinDate)
                .Select(p => new ParticipantViewModel
                {
                    JoinId = p.JoinId ?? "",
                    UserId = p.UserId ?? "",
                    Name = p.User != null ? p.User.UserName : "(알수없음)",
                    JoinDate = p.JoinDate,
                    JoinStatus = p.JoinStatus,
                    IsCancelled = p.IsCancelled,
                    CancelDate = p.CancelDate,
                    CancelReason = p.CancelReason,
                    Approval = p.Approval
                });

            return await PaginatedList<ParticipantViewModel>.CreateAsync(projected, pageIndex, pageSize);
        }
    }
}