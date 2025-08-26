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
                        Title = "���� ��� ����",
                        Message = $"ȸ������ {(string.IsNullOrEmpty(gameName) ? "" : $"[{gameName}] ")}���� ��Ұ� ���εǾ����ϴ�.",
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
                        Memo = "�����ڰ� ������� ����"
                    });

                    await transaction.CommitAsync();

                    TempData["SuccessTitle"] = "��� ���� �Ϸ�";
                    TempData["SuccessMessage"] = "��� ������ �Ϸ�Ǿ����ϴ�.";
                    if (participant.Game?.GameStatus == "Assigned")
                    {
                        TempData["SuccessMessage"] += "\n�ڽ���ġ�� �Ϸ� �Ǿ� �����Ƿ� <strong style='color:red; font-size:1.2em;'>�ڽ� ���ġ</strong>�� �ʿ��մϴ�.";
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
                    TempData["ErrorTitle"] = "����";
                    TempData["ErrorMessage"] = "��� ���� ó���� �����߽��ϴ�.";
                    await transaction.RollbackAsync();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorTitle"] = "����";
                TempData["ErrorMessage"] = $"��� ���� �� ������ �߻��߽��ϴ�: {ex.Message}";
                // ���⼭ �α�(��: _logger.LogError(ex, ...))
                await transaction.RollbackAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejoinAsync(string id)
        {
            // Ʈ����� ����
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

                    // ������ �˸� �߰�
                    var gameName = participant.Game?.GameName ?? "";
                    var notification = new Notification
                    {
                        UserId = participant.UserId ?? "",
                        Type = NotificationTypes.RejoinApproved,
                        Title = "������ ó�� �Ϸ�",
                        Message = $"ȸ������ {(string.IsNullOrEmpty(gameName) ? "" : $"[{gameName}] ")} ��ȸ�� �ٽ� ���� ó�� �Ǿ����ϴ�.",
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
                        Memo = "�����ڰ� ������ ����"
                    });

                    await transaction.CommitAsync();

                    TempData["SuccessTitle"] = "������ �Ϸ�";
                    TempData["SuccessMessage"] = "�ش� �����ڸ� ������ ó���Ͽ����ϴ�.";
                    if (participant.Game?.GameStatus == "Assigned")
                    {
                        TempData["SuccessMessage"] += "\n�ڽ���ġ�� �Ϸ� �Ǿ� �����Ƿ� <strong style='color:red; font-size:1.2em;'>�ڽ� ���ġ</strong>�� �ʿ��մϴ�.";
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
                    TempData["ErrorTitle"] = "����";
                    TempData["ErrorMessage"] = "������ ó���� �����߽��ϴ�.";
                    await transaction.RollbackAsync();
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorTitle"] = "����";
                TempData["ErrorMessage"] = $"������ ó�� �� ������ �߻��߽��ϴ�: {ex.Message}";
                // �ʿ�� ���� �α� �߰�
            }
            return RedirectToPage();
        }
    }
}