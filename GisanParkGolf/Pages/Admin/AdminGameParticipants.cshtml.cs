using DocumentFormat.OpenXml.Spreadsheet;
using GisanParkGolf.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace GiSanParkGolf.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminGameParticipantsModel : PageModel
    {
        private readonly MyDbContext _dbContext;

        public AdminGameParticipantsModel(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<GameParticipant> Participants { get; set; } = new();
        public int TotalCount { get; set; }
        public int CancelledCount { get; set; }
        public int JoinedCount { get; set; }
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchField { get; set; } = "GameName";
        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public async Task OnGetAsync()
        {
            var query = _dbContext.GameParticipants
                .Include(gp => gp.Game)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchQuery))
            {
                switch (SearchField)
                {
                    case "GameName":
                        query = query.Where(gp => gp.Game != null
                                               && gp.Game.GameName != null
                                               && gp.Game.GameName.Contains(SearchQuery));
                        break;
                    case "UserId":
                        query = query.Where(gp => gp.UserId != null
                                               && gp.UserId.Contains(SearchQuery));
                        break;
                }
            }

            var isCancelRequested = Request.Query["IsCancelRequested"] == "true";
            if (isCancelRequested)
            {
                query = query.Where(gp => gp.IsCancelled);
            }

            TotalCount = await query.CountAsync();
            CancelledCount = await _dbContext.GameParticipants.CountAsync(gp => gp.IsCancelled);
            JoinedCount = await _dbContext.GameParticipants.CountAsync(gp => !gp.IsCancelled);

            TotalPages = (int)Math.Ceiling((double)TotalCount / PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (PageIndex < 1) PageIndex = 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            Participants = await query
                .OrderByDescending(gp => gp.JoinDate)
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        private async Task AddAssignmentHistoryAsync(string gameCode, string userId, string changeType, object detailsObj)
        {
            var jsonDetails = JsonConvert.SerializeObject(detailsObj ?? new { });
            var history = new GameAssignmentHistory
            {
                GameCode = gameCode,
                ChangedBy = userId,
                ChangeType = changeType,
                Details = jsonDetails,
                ChangedAt = DateTime.Now
            };
            _dbContext.Add(history);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IActionResult> OnPostApproveCancelAsync(string id)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var participant = await _dbContext.GameParticipants
                    .Include(p => p.Game)
                    .FirstOrDefaultAsync(p => p.UserId == id);

                if (participant != null && participant.IsCancelled && string.IsNullOrEmpty(participant.Approval))
                {
                    participant.Approval = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";
                    await _dbContext.SaveChangesAsync();

                    var gameName = participant.Game?.GameName ?? "";
                    var notification = new Notification
                    {
                        UserId = participant.UserId ?? "",
                        Type = NotificationTypes.CancelApproved,
                        Title = "참가 취소 승인",
                        Message = $"회원님의 {(string.IsNullOrEmpty(gameName) ? "" : $"[{gameName}] ")}참가 취소가 승인되었습니다.",
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                    };
                    _dbContext.Notifications.Add(notification);
                    await _dbContext.SaveChangesAsync();

                    await AddAssignmentHistoryAsync(participant.Game?.GameCode ?? "", participant.UserId ?? "", "CancelApproval", new
                    {
                        UserId = participant.UserId ?? "",
                        ActionType = NotificationTypes.CancelApproved,
                        ActionBy = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        ParticipantId = participant.JoinId,
                        Memo = "관리자가 참가취소 승인"
                    });

                    await transaction.CommitAsync();

                    TempData["SuccessTitle"] = "취소 승인 완료";
                    TempData["SuccessMessage"] = "취소 승인이 완료되었습니다.";
                    if (participant.Game?.GameStatus == "Assigned")
                    {
                        TempData["SuccessMessage"] += "\n코스배치가 완료 되어 있으므로 <strong style='color:red; font-size:1.2em;'>코스 재배치</strong>가 필요합니다.";
                    }

                    if (participant.Game != null && participant.Game.AssignmentLocked)
                    {
                        participant.Game.AssignmentLocked = false;
                        participant.Game.GameStatus = "Assigning";
                        _dbContext.Games.Update(participant.Game);
                        await _dbContext.SaveChangesAsync();
                    }

                    return RedirectToPage("/Admin/GameSetup", new { gameCode = participant.GameCode, tab = "tab-course" });
                }
                else
                {
                    TempData["ErrorTitle"] = "오류";
                    TempData["ErrorMessage"] = "취소 승인 처리에 실패했습니다.";
                    await transaction.RollbackAsync();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorTitle"] = "오류";
                TempData["ErrorMessage"] = $"취소 승인 중 오류가 발생했습니다: {ex.Message}";
                // 여기서 로깅(예: _logger.LogError(ex, ...))
                await transaction.RollbackAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejoinAsync(string id)
        {
            // 트랜잭션 시작
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var participant = await _dbContext.GameParticipants
                    .Include(p => p.Game)
                    .FirstOrDefaultAsync(p => p.UserId == id && p.IsCancelled);

                if (participant != null)
                {
                    participant.IsCancelled = false;
                    participant.CancelDate = null;
                    participant.CancelReason = null;
                    participant.Approval = null;
                    _dbContext.GameParticipants.Update(participant);
                    await _dbContext.SaveChangesAsync();

                    // 재참가 알림 추가
                    var gameName = participant.Game?.GameName ?? "";
                    var notification = new Notification
                    {
                        UserId = participant.UserId ?? "",
                        Type = NotificationTypes.RejoinApproved,
                        Title = "재참가 처리 완료",
                        Message = $"회원님이 {(string.IsNullOrEmpty(gameName) ? "" : $"[{gameName}] ")} 대회에 다시 참가 처리 되었습니다.",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    _dbContext.Notifications.Add(notification);
                    await _dbContext.SaveChangesAsync();

                    await AddAssignmentHistoryAsync(participant.Game?.GameCode ?? "", participant.UserId ?? "", "RejoinApproval", new
                    {
                        UserId = participant.UserId ?? "",
                        ActionType = NotificationTypes.RejoinApproved,
                        ActionBy = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        ParticipantId = participant.JoinId,
                        Memo = "관리자가 재참가 승인"
                    });

                    await transaction.CommitAsync();

                    TempData["SuccessTitle"] = "재참가 완료";
                    TempData["SuccessMessage"] = "해당 참가자를 재참가 처리하였습니다.";
                    if (participant.Game?.GameStatus == "Assigned")
                    {
                        TempData["SuccessMessage"] += "\n코스배치가 완료 되어 있으므로 <strong style='color:red; font-size:1.2em;'>코스 재배치</strong>가 필요합니다.";
                    }

                    if (participant.Game != null && participant.Game.AssignmentLocked)
                    {
                        participant.Game.AssignmentLocked = false;
                        participant.Game.GameStatus = "Assigning";
                        _dbContext.Games.Update(participant.Game);
                        await _dbContext.SaveChangesAsync();
                    }

                    return RedirectToPage("/Admin/GameSetup", new { gameCode = participant.GameCode, tab = "tab-course" });
                }
                else
                {
                    TempData["ErrorTitle"] = "오류";
                    TempData["ErrorMessage"] = "재참가 처리에 실패했습니다.";
                    await transaction.RollbackAsync();
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorTitle"] = "오류";
                TempData["ErrorMessage"] = $"재참가 처리 중 오류가 발생했습니다: {ex.Message}";
                // 필요시 예외 로깅 추가
            }
            return RedirectToPage();
        }
    }
}