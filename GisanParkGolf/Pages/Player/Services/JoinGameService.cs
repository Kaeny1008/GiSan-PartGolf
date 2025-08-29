using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using GisanParkGolf.Data;
using GisanParkGolf.Helpers;
using GisanParkGolf.Pages.Player.ViewModels;
using GisanParkGolf.ViewModels.Player;
using GiSanParkGolf.Pages.Player;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace GisanParkGolf.Services.Player
{
    public class JoinGameService : IJoinGameService
    {
        public class JoinGameResult
        {
            public bool Success { get; set; }
            public string? ErrorMessage { get; set; }
        }

        private readonly MyDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JoinGameService(MyDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedList<Game>> GetGamesAsync(
            string? searchField, string? searchQuery, int pageIndex, int pageSize, string? status = null)
        {
            var query = _dbContext.Games.AsQueryable();

            var statusCode = GameStatusHelper.ToStatusCode(status);
            query = query.Where(g => g.GameStatus == statusCode);

            // 검색조건
            if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrEmpty(searchField))
            {
                switch (searchField)
                {
                    case "GameName":
                        query = query.Where(g => (g.GameName ?? "").Contains(searchQuery));
                        break;
                    case "StadiumName":
                        query = query.Where(g => (g.StadiumName ?? "").Contains(searchQuery));
                        break;
                    case "GameHost":
                        query = query.Where(g => (g.GameHost ?? "").Contains(searchQuery));
                        break;
                }
            }
            var ordered = query.OrderByDescending(g => g.GameDate);
            return await PaginatedList<Game>.CreateAsync(ordered, pageIndex, pageSize);
        }

        public async Task<JoinGameResult> JoinGameAsync(string gameCode, ClaimsPrincipal user)
        {
            string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return new JoinGameResult { Success = false, ErrorMessage = "로그인 정보가 없습니다." };

            var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
                return new JoinGameResult { Success = false, ErrorMessage = "해당 대회를 찾을 수 없습니다." };

            // 기존 참가 엔터티 찾기 (취소 포함)
            var participant = await _dbContext.GameParticipants
                .FirstOrDefaultAsync(gp => gp.GameCode == gameCode && gp.UserId == userId);

            if (participant != null)
            {
                if (participant.Approval != null)
                {
                    // 관리자에 의해 참가취소 승인된 참가자
                    return new JoinGameResult { Success = false, ErrorMessage = "관리자가 취소요청을 승인하여 재 참가 요청 할 수 없습니다." };
                }

                if (participant.IsCancelled)
                {
                    // 참가 취소 이력이 있으면 복구
                    participant.IsCancelled = false;
                    participant.CancelDate = null;
                    participant.CancelReason = null;
                    participant.Approval = null;
                    participant.JoinDate = DateTime.Now;
                    participant.JoinIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                    participant.JoinStatus = "Join";
                    // 참가자 수 증가
                    game.ParticipantNumber += 1;

                    try
                    {
                        await _dbContext.SaveChangesAsync();
                        await LogGameJoinHistoryAsync(
                            gameCode,
                            userId,
                            "Join",
                            userId,
                            "참가자 요청",
                            participant.JoinId,
                            "참가 취소 이력 복구(재참가)"
                        );
                        return new JoinGameResult { Success = true };
                    }
                    catch (Exception ex)
                    {
                        return new JoinGameResult { Success = false, ErrorMessage = "DB 오류: " + ex.Message };
                    }
                }
                else
                {
                    // 이미 참가중인 경우
                    return new JoinGameResult { Success = false, ErrorMessage = "이미 참가 신청하셨습니다." };
                }
            }
            else
            {
                // 신규 참가
                var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                string joinId = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 12);

                var join = new GameParticipant
                {
                    JoinId = joinId,
                    GameCode = gameCode,
                    UserId = userId,
                    JoinDate = DateTime.Now,
                    JoinIp = ip,
                    JoinStatus = "Join",
                    IsCancelled = false,
                    CancelDate = null,
                    CancelReason = null,
                    Approval = null
                };
                _dbContext.GameParticipants.Add(join);

                game.ParticipantNumber += 1;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    await LogGameJoinHistoryAsync(
                        gameCode,
                        userId,
                        "Join",
                        userId,
                        null,
                        joinId,
                        "신규 참가"
                    );
                    return new JoinGameResult { Success = true };
                }
                catch (Exception ex)
                {
                    return new JoinGameResult { Success = false, ErrorMessage = "DB 오류: " + ex.Message };
                }
            }
        }

        public async Task<JoinGameResult> LeaveGameAsync(string gameCode, ClaimsPrincipal user)
        {
            string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return new JoinGameResult { Success = false, ErrorMessage = "로그인 정보가 없습니다." };

            var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
                return new JoinGameResult { Success = false, ErrorMessage = "해당 대회를 찾을 수 없습니다." };

            if (game.GameStatus != GameStatusHelper.ToStatusCode("모집중"))
            {
                return new JoinGameResult { Success = false, ErrorMessage = "모집중인 대회가 아닙니다.\n관리자에게 참가취소 요청을 하십시오." };
            }

            // 참가 엔터티 찾기 (GameCode + UserId로)
            var joinEntity = await _dbContext.GameParticipants
                .FirstOrDefaultAsync(gp => gp.GameCode == gameCode && gp.UserId == userId && !gp.IsCancelled);

            if (joinEntity == null)
                return new JoinGameResult { Success = false, ErrorMessage = "참가 내역이 없습니다." };

            joinEntity.JoinStatus = "Cancel";
            joinEntity.IsCancelled = true;
            joinEntity.CancelDate = DateTime.Now;
            joinEntity.CancelReason = "참가자 요청";
            joinEntity.Approval = null;

            // 참가자 수 감소 (0보다 작아지지 않도록 방어 코드)
            if (game.ParticipantNumber > 0)
                game.ParticipantNumber -= 1;

            try
            {
                await _dbContext.SaveChangesAsync();
                await LogGameJoinHistoryAsync(
                    gameCode,
                    userId,
                    "Cancel",
                    userId,
                    "참가자 요청",
                    joinEntity.JoinId,
                    "참가자가 직접 참가취소"
                );
                return new JoinGameResult { Success = true };
            }
            catch (Exception ex)
            {
                return new JoinGameResult { Success = false, ErrorMessage = "DB 오류: " + ex.Message };
            }
        }

        public IQueryable<GameParticipant> GameParticipants
            => _dbContext.GameParticipants.AsQueryable();

        public async Task LogGameJoinHistoryAsync(
            string gameCode,
            string userId,
            string actionType,
            string actionBy,
            string? cancelReason = null,
            string? participantId = null,
            string? memo = null)
        {
            var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var history = new GameJoinHistory
            {
                GameCode = gameCode,
                UserId = userId,
                ActionType = actionType,
                ActionDate = DateTime.Now,
                ActionBy = actionBy,
                CancelReason = cancelReason,
                ParticipantId = participantId,
                Memo = memo,
                ActionIp = ip
            };
            _dbContext.GameJoinHistories.Add(history);
            await _dbContext.SaveChangesAsync();
        }

        // [MyGame페이지 관련 추가] 내 대회 목록 조회 (검색/페이징)
        public async Task<PaginatedList<MyGameListModel>> GetMyGameListAsync(
            string userId, string? searchField, string? searchQuery, int pageIndex, int pageSize)
        {
            var query = _dbContext.GameParticipants
                .Include(gp => gp.Game)
                .Where(gp => gp.UserId == userId)
                .Select(gp => new MyGameListModel
                {
                    GameCode = gp.GameCode,
                    GameName = gp.Game != null ? gp.Game.GameName : "",
                    StadiumName = gp.Game != null ? gp.Game.StadiumName : "",
                    GameHost = gp.Game != null ? gp.Game.GameHost : "",
                    GameDate = gp.Game != null ? gp.Game.GameDate : DateTime.MinValue,
                    StartRecruiting = gp.Game != null ? gp.Game.StartRecruiting : DateTime.MinValue,
                    EndRecruiting = gp.Game != null ? gp.Game.EndRecruiting : DateTime.MinValue,
                    GameStatus = gp.Game != null ? gp.Game.GameStatus : "",
                    IsCancelled = gp.IsCancelled,
                    Approval = gp.Approval
                });

            if (!string.IsNullOrWhiteSpace(searchQuery) && !string.IsNullOrWhiteSpace(searchField))
            {
                var kw = searchQuery.Trim().ToLower();
                switch (searchField)
                {
                    case "GameName":
                        query = query.Where(g => (g.GameName ?? "").ToLower().Contains(kw));
                        break;
                    case "GameHost":
                        query = query.Where(g => (g.GameHost ?? "").ToLower().Contains(kw));
                        break;
                    case "StadiumName":
                        query = query.Where(g => (g.StadiumName ?? "").ToLower().Contains(kw));
                        break;
                }
            }

            query = query.OrderByDescending(g => g.GameDate);
            return await PaginatedList<MyGameListModel>.CreateAsync(query.AsNoTracking(), pageIndex, pageSize);
        }

        private async Task AddAssignmentHistoryAsync(string gameCode, string userId, string changeType, object detailsObj)
        {
            var participant = await _dbContext.GameParticipants
                .Include(gp => gp.Game)
                .FirstOrDefaultAsync(gp => gp.GameCode == gameCode && gp.UserId == userId);

            //if (participant == null || participant.Game == null)
            //    return null;

            var jsonDetails = JsonConvert.SerializeObject(detailsObj ?? new { });
            var history = new GameAssignmentHistory
            {
                GameCode = gameCode,
                ChangedBy = participant?.User?.UserName ?? "",
                ChangeType = changeType,
                Details = jsonDetails,
                ChangedAt = DateTime.Now
            };
            _dbContext.Add(history);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<MyGameDetailViewModel?> GetMyGameInformationAsync(string gameCode, string? userId)
        {
            var participant = await _dbContext.GameParticipants
                .Include(gp => gp.Game)
                .FirstOrDefaultAsync(gp => gp.GameCode == gameCode && gp.UserId == userId);

            if (participant == null || participant.Game == null)
                return null;

            var game = participant.Game;

            // 본인 배정정보 조회
            var assignment = await _dbContext.GameUserAssignments
                .Where(x => x.GameCode == gameCode && x.UserId == userId)
                .FirstOrDefaultAsync();

            // 전체 배정 결과 조회
            var allAssignments = await _dbContext.GameUserAssignments
                .Where(x => x.GameCode == gameCode)
                .Include(x => x.User) // User 테이블이 있으면
                .OrderBy(x => x.CourseName).ThenBy(x => x.HoleNumber).ThenBy(x => x.CourseOrder)
                .Select(x => new GameAssignmentResultViewModel
                {
                    UserName = x.User != null ? x.User.UserName : x.UserId,
                    UserId = x.UserId,
                    CourseName = x.CourseName,
                    HoleNumber = x.HoleNumber,
                    TeamNumber = x.TeamNumber,
                    CourseOrder = x.CourseOrder
                })
                .ToListAsync();

            // PlayModeToText 등은 필요에 따라 변환 로직 추가
            return new MyGameDetailViewModel
            {
                GameCode = game.GameCode,
                GameName = game.GameName,
                GameDate = game.GameDate,
                StadiumName = game.StadiumName,
                PlayMode = game.PlayMode, // 변환 함수 필요시 적용
                GameHost = game.GameHost,
                HoleMaximum = game.HoleMaximum,
                StartRecruiting = game.StartRecruiting,
                EndRecruiting = game.EndRecruiting,
                GameNote = game.GameNote,
                ParticipantNumber = game.ParticipantNumber,
                IsCancelled = participant.IsCancelled ? 1 : 0,
                CancelDate = participant.CancelDate,
                CancelReason = participant.CancelReason,
                Approval = participant.Approval,
                AssignmentStatus = game.GameStatus,
                AssignedCourseName = assignment?.CourseName,
                AssignedHoleNumber = assignment?.HoleNumber,
                AssignedTeamNumber = assignment?.TeamNumber,
                AssignedCourseOrder = assignment?.CourseOrder,
                AllAssignments = allAssignments,
                GameStatus = game.GameStatus
            };
        }

        public async Task<JoinGameResult> MyGameCancelAsync(string gameCode, string? userId, string cancelReason)
        {
            if (string.IsNullOrEmpty(userId))
                return new JoinGameResult { Success = false, ErrorMessage = "로그인 정보가 없습니다." };

            var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
                return new JoinGameResult { Success = false, ErrorMessage = "해당 대회를 찾을 수 없습니다." };

            // 대회 모집중 상태 체크
            if (game.GameStatus != GameStatusHelper.ToStatusCode("모집중"))
                return new JoinGameResult { Success = false, ErrorMessage = "모집중인 대회가 아닙니다." };

            var participant = await _dbContext.GameParticipants
                .FirstOrDefaultAsync(gp => gp.GameCode == gameCode && gp.UserId == userId && !gp.IsCancelled);

            if (participant == null)
                return new JoinGameResult { Success = false, ErrorMessage = "참가 내역이 없습니다." };

            // 트랜잭션 시작
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                participant.IsCancelled = true;
                participant.CancelDate = DateTime.Now;
                participant.CancelReason = cancelReason;
                participant.JoinStatus = "Cancel";
                participant.Approval = userId;

                if (game.ParticipantNumber > 0)
                    game.ParticipantNumber -= 1;

                await _dbContext.SaveChangesAsync();

                //await LogGameJoinHistoryAsync(
                //    gameCode,
                //    userId,
                //    "Cancel",
                //    userId,
                //    cancelReason,
                //    participant.JoinId,
                //    "참가자가 직접 참가취소"
                //);

                await AddAssignmentHistoryAsync(gameCode, userId, "Cancel", new
                {
                    UserId = userId,
                    ActionType = "Cancel",
                    ActionBy = userId,
                    CancelReason = cancelReason,
                    ParticipantId = participant.JoinId,
                    Memo = "참가자가 직접 참가취소"
                });

                await transaction.CommitAsync();

                return new JoinGameResult { Success = true };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new JoinGameResult { Success = false, ErrorMessage = "DB 오류: " + ex.Message };
            }
        }

        public async Task<JoinGameResult> MyGameCancelRequestlAsync(string gameCode, string? userId, string cancelReason)
        {
            if (string.IsNullOrEmpty(userId))
                return new JoinGameResult { Success = false, ErrorMessage = "로그인 정보가 없습니다." };

            var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
                return new JoinGameResult { Success = false, ErrorMessage = "해당 대회를 찾을 수 없습니다." };

            var participant = await _dbContext.GameParticipants
                .FirstOrDefaultAsync(gp => gp.GameCode == gameCode && gp.UserId == userId && !gp.IsCancelled);

            if (participant == null)
                return new JoinGameResult { Success = false, ErrorMessage = "참가 내역이 없습니다." };

            // 트랜잭션 시작
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                participant.IsCancelled = true;
                participant.CancelDate = DateTime.Now;
                participant.CancelReason = cancelReason;
                participant.JoinStatus = "Cancel";
                participant.Approval = null;

                if (game.ParticipantNumber > 0)
                    game.ParticipantNumber -= 1;

                await _dbContext.SaveChangesAsync();

                // 관리자(슈퍼관리자 포함) 전체 조회
                var adminUsers = await _dbContext.Players
                    .Where(m => m.UserClass <= 1)
                    .ToListAsync();

                // 관리자에게 알림 생성
                var message = $"{userId}님이 [{game.GameName}] 대회 참가취소를 요청했습니다.";

                foreach (var admin in adminUsers)
                {
                    var notification = new Notification
                    {
                        UserId = admin.UserId,       // 각 관리자에게 알림
                        Title = "참가취소",
                        Message = message,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                        LinkUrl = "/Admin/AdminGameParticipants",
                        Type = NotificationTypes.CancelRequest
                    };
                    _dbContext.Notifications.Add(notification);
                }
                await _dbContext.SaveChangesAsync();

                //await LogGameJoinHistoryAsync(
                //    gameCode,
                //    userId,
                //    "Cancel",
                //    userId,
                //    cancelReason,
                //    participant.JoinId,
                //    "참가자가 직접 참가취소 요청"
                //);

                await AddAssignmentHistoryAsync(gameCode, userId, "CancelRequest", new
                {
                    UserId = userId,
                    ActionType = "Cancel",
                    ActionBy = userId,
                    CancelReason = cancelReason,
                    ParticipantId = participant.JoinId,
                    Memo = "참가자가 직접 참가취소 요청"
                });

                await transaction.CommitAsync();

                return new JoinGameResult { Success = true };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new JoinGameResult { Success = false, ErrorMessage = "DB 오류: " + ex.Message };
            }
        }

        public async Task<JoinGameResult> MyGameRejoinAsync(string gameCode, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new JoinGameResult { Success = false, ErrorMessage = "로그인 정보가 없습니다." };

            var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
                return new JoinGameResult { Success = false, ErrorMessage = "해당 대회를 찾을 수 없습니다." };

            // 기존 참가 엔터티 찾기 (취소된 것만)
            var participant = await _dbContext.GameParticipants
                .FirstOrDefaultAsync(gp => gp.GameCode == gameCode && gp.UserId == userId && gp.IsCancelled);

            if (participant == null)
                return new JoinGameResult { Success = false, ErrorMessage = "재참가 가능한 이력이 없습니다." };

            // 관리자 승인 취소된 경우 재참가 불가
            if (!string.IsNullOrEmpty(participant.Approval))
            {
                return new JoinGameResult { Success = false, ErrorMessage = "관리자가 취소요청을 승인하여 재참가 할 수 없습니다." };
            }

            // 트랜잭션 시작
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // 재참가 처리
                participant.IsCancelled = false;
                participant.CancelDate = null;
                participant.CancelReason = null;
                participant.Approval = null;
                participant.JoinDate = DateTime.Now;
                participant.JoinIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                participant.JoinStatus = "Join";

                // 참가자 수 증가
                game.ParticipantNumber += 1;

                await _dbContext.SaveChangesAsync();

                //await LogGameJoinHistoryAsync(
                //    gameCode,
                //    userId,
                //    "Join",
                //    userId,
                //    "참가자 요청",
                //    participant.JoinId,
                //    "참가 취소 이력 복구(재참가)"
                //);

                await AddAssignmentHistoryAsync(gameCode, userId, "Rejoin", new
                {
                    UserId = userId,
                    ActionType = "Rejoin",
                    ActionBy = userId,
                    ParticipantId = participant.JoinId,
                    Memo = "참가 취소 이력 복구(재참가)"
                });

                await transaction.CommitAsync();

                return new JoinGameResult { Success = true };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new JoinGameResult { Success = false, ErrorMessage = "DB 오류: " + ex.Message };
            }
        }

        public async Task<JoinGameResult> MyGameRejoinRequestAsync(string gameCode, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new JoinGameResult { Success = false, ErrorMessage = "로그인 정보가 없습니다." };

            var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
                return new JoinGameResult { Success = false, ErrorMessage = "해당 대회를 찾을 수 없습니다." };

            // 기존 참가 엔터티 찾기 (취소된 것만)
            var participant = await _dbContext.GameParticipants
                .FirstOrDefaultAsync(gp => gp.GameCode == gameCode && gp.UserId == userId && gp.IsCancelled);

            if (participant == null)
                return new JoinGameResult { Success = false, ErrorMessage = "재참가 가능한 이력이 없습니다." };

            // 트랜잭션 시작
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // 재참가 요청 처리
                participant.Approval = userId;

                // 참가자 수 증가
                game.ParticipantNumber += 1;

                await _dbContext.SaveChangesAsync();

                // 관리자(슈퍼관리자 포함) 전체 조회
                var adminUsers = await _dbContext.Players
                    .Where(m => m.UserClass <= 1)
                    .ToListAsync();

                // 관리자에게 알림 생성
                var message = $"{userId}님이 [{game.GameName}] 대회 재참가를 요청했습니다.";

                foreach (var admin in adminUsers)
                {
                    var notification = new Notification
                    {
                        UserId = admin.UserId,
                        Title = "재참가",
                        Message = message,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                        LinkUrl = "/Admin/AdminGameParticipants",
                        Type = NotificationTypes.RejoinRequest
                    };
                    _dbContext.Notifications.Add(notification);
                }
                await _dbContext.SaveChangesAsync();

                //await LogGameJoinHistoryAsync(
                //    gameCode,
                //    userId,
                //    "Join",
                //    userId,
                //    "참가자 요청",
                //    participant.JoinId,
                //    "참가 취소 이력 복구(재참가)"
                //);

                await AddAssignmentHistoryAsync(gameCode, userId, "RejoinRequest", new
                {
                    UserId = userId,
                    ActionType = "Rejoin",
                    ActionBy = userId,
                    ParticipantId = participant.JoinId,
                    Memo = "참가 취소 이력 복구(재참가) 요청"
                });

                await transaction.CommitAsync();

                return new JoinGameResult { Success = true };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new JoinGameResult { Success = false, ErrorMessage = "DB 오류: " + ex.Message };
            }
        }

        public async Task<bool> IsAssignmentLockedAsync(string gameCode)
        {
            var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            return game != null && game.GameStatus == "Assigned";
        }

        public async Task<PaginatedList<AssignmentResultModel>> GetAssignmentResultAsync(
            string gameCode,
            string? searchField,
            string? searchQuery,
            int pageIndex,
            int pageSize
        )
        {
            var query = _dbContext.GameUserAssignments
                .Where(x => x.GameCode == gameCode);

            // 검색조건
            if (!string.IsNullOrWhiteSpace(searchQuery) && !string.IsNullOrWhiteSpace(searchField))
            {
                var kw = searchQuery.Trim();
                switch (searchField)
                {
                    case "UserName":
                        query = query.Where(a => (a.User != null ? a.User.UserName : a.UserId ?? "").Contains(kw));
                        break;
                    case "UserId":
                        query = query.Where(a => (a.UserId ?? "").Contains(kw));
                        break;
                    case "TeamNumber":
                        query = query.Where(a => (a.TeamNumber ?? "").Contains(kw));
                        break;
                        // 필요하면 코스명, 홀번호 등도 추가
                }
            }

            var projected = query
                .OrderBy(x => x.CourseName ?? "")
                .ThenBy(x => x.HoleNumber ?? "")
                .ThenBy(x => x.CourseOrder)
                .Select(x => new AssignmentResultModel
                {
                    UserName = x.User != null && !string.IsNullOrEmpty(x.User.UserName) ? x.User.UserName : (x.UserId ?? ""),
                    UserId = x.UserId ?? "",
                    CourseName = x.CourseName ?? "",
                    HoleNumber = x.HoleNumber ?? "",
                    TeamNumber = x.TeamNumber ?? "",
                    CourseOrder = x.CourseOrder
                });

            return await PaginatedList<AssignmentResultModel>.CreateAsync(projected, pageIndex, pageSize);
        }
    }
}