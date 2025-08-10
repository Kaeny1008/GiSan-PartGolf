using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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

        [BindProperty(SupportsGet = true)]
        public string? SearchGameName { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SearchUserId { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ShowCancelledOnly { get; set; }

        public async Task OnGetAsync()
        {
            var query = _dbContext.GameParticipants
                .Include(gp => gp.Game)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchGameName))
                query = query.Where(gp => gp.Game.GameName.Contains(SearchGameName));
            if (!string.IsNullOrEmpty(SearchUserId))
                query = query.Where(gp => gp.UserId.Contains(SearchUserId));
            if (ShowCancelledOnly)
                query = query.Where(gp => gp.IsCancelled);

            Participants = await query
                .OrderByDescending(gp => gp.JoinDate)
                .Take(200) // 최대 200건만
                .ToListAsync();

            TotalCount = await _dbContext.GameParticipants.CountAsync();
            CancelledCount = await _dbContext.GameParticipants.CountAsync(gp => gp.IsCancelled);
            JoinedCount = await _dbContext.GameParticipants.CountAsync(gp => !gp.IsCancelled);
        }
    }
}