using GisanParkGolf_Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GiSanParkGolf.Pages.AdminPage
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

        public async Task<IActionResult> OnPostApproveCancelAsync(string id)
        {
            var participant = await _dbContext.GameParticipants.FirstOrDefaultAsync(p => p.UserId == id);
            if (participant != null && participant.IsCancelled && string.IsNullOrEmpty(participant.Approval))
            {
                participant.Approval = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";
                await _dbContext.SaveChangesAsync();
                TempData["SuccessTitle"] = "취소 승인 완료";
                TempData["SuccessMessage"] = "취소 승인이 완료되었습니다.";
            }
            else
            {
                TempData["ErrorTitle"] = "오류";
                TempData["ErrorMessage"] = "취소 승인 처리에 실패했습니다.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejoinAsync(string id)
        {
            var participant = await _dbContext.GameParticipants.FirstOrDefaultAsync(p => p.UserId == id && p.IsCancelled);
            if (participant != null)
            {
                participant.IsCancelled = false;
                participant.CancelDate = null;
                participant.CancelReason = null;
                participant.Approval = null;
                await _dbContext.SaveChangesAsync();

                TempData["SuccessTitle"] = "재참가 완료";
                TempData["SuccessMessage"] = "해당 참가자를 재참가 처리하였습니다.";
            }
            else
            {
                TempData["ErrorTitle"] = "오류";
                TempData["ErrorMessage"] = "재참가 처리에 실패했습니다.";
            }
            return RedirectToPage();
        }
    }
}