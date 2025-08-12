using GisanParkGolf_Core.Data;
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
                        query = query.Where(gp => gp.Game.GameName.Contains(SearchQuery));
                        break;
                    case "UserId":
                        query = query.Where(gp => gp.UserId.Contains(SearchQuery));
                        break;
                }
            }

            if (SearchField == "CancelledOnly")
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
    }
}