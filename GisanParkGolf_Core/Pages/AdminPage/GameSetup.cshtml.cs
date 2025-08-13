using DocumentFormat.OpenXml.Wordprocessing;
using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.Services.AdminPage;
using GisanParkGolf_Core.ViewModels.AdminPage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiSanParkGolf.Pages.AdminPage
{
    public class GameSetupModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly MyDbContext _context;

        public GameSetupModel(MyDbContext context, IGameService gameService)
        {
            _gameService = gameService;
            _context = context;
            Competitions = new PaginatedList<CompetitionViewModel>(new List<CompetitionViewModel>(), 0, 1, 10);
            Participants = new PaginatedList<ParticipantViewModel>(new List<ParticipantViewModel>(), 0, 1, 10);
        }

        public PaginatedList<CompetitionViewModel> Competitions { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string? SearchField { get; set; } = "GameName";

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        public PaginatedList<ParticipantViewModel> Participants { get; set; }

        [BindProperty(SupportsGet = true)]
        public int ParticipantPageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int ParticipantPageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public string? ParticipantSearchQuery { get; set; }

        public async Task OnGetAsync(string? gameCode)
        {
            Competitions = await _gameService.GetCompetitionsAsync(SearchField, SearchQuery, PageIndex, PageSize);

            if (!string.IsNullOrEmpty(gameCode))
            {
                Participants = await _gameService.GetParticipantsAsync(
                    gameCode,
                    ParticipantSearchQuery,
                    ParticipantPageIndex,
                    ParticipantPageSize
                );
            }
        }

        //public async Task<JsonResult> OnGetCompetitionDetailAsync(string gameCode)
        //{
        //    var game = await _gameService.GetGameByIdAsync(gameCode);
        //    if (game == null)
        //        return new JsonResult(new { success = false, message = "대회를 찾을 수 없습니다." });

        //    // 필요에 따라 ViewModel로 변환
        //    var detail = new
        //    {
        //        game.GameCode,
        //        game.GameName,
        //        GameDate = game.GameDate.ToString("yyyy-MM-dd"),
        //        Status = GameStatusHelper.ToDisplay(game.GameStatus),
        //        StadiumName = game.Stadium?.StadiumName ?? "(미지정)",
        //        game.ParticipantNumber,
        //        game.GameHost,
        //        PlayMode = PlayModeHelper.ToKorDisplay(game.PlayMode),
        //        game.GameNote
        //    };

        //    return new JsonResult(new { success = true, data = detail });
        //}

        //public async Task<JsonResult> OnGetParticipantsAsync(
        //    string gameCode,
        //    int pageIndex = 1,
        //    int pageSize = 10,
        //    string? searchQuery = null)
        //{
        //    // 참가자 쿼리
        //    var query = _context.GameParticipants
        //        .Include(p => p.User)
        //        .Where(p => p.GameCode == gameCode);

        //    // 검색어 필터 (이름, UserId, JoinId 등 원하는 필드에 적용)
        //    if (!string.IsNullOrWhiteSpace(searchQuery))
        //    {
        //        query = query.Where(p =>
        //            (p.User != null && p.User.UserName.Contains(searchQuery)) ||
        //            (p.UserId != null && p.UserId.Contains(searchQuery)) ||
        //            (p.JoinId != null && p.JoinId.Contains(searchQuery))
        //        );
        //    }

        //    // ParticipantViewModel로 변환
        //    var projected = query
        //        .OrderBy(p => p.JoinDate)
        //        .Select(p => new ParticipantViewModel
        //        {
        //            JoinId = p.JoinId ?? "",
        //            UserId = p.UserId ?? "",
        //            Name = p.User != null ? p.User.UserName : "(알수없음)",
        //            JoinDate = p.JoinDate,
        //            JoinStatus = p.JoinStatus,
        //            IsCancelled = p.IsCancelled,
        //            CancelDate = p.CancelDate,
        //            CancelReason = p.CancelReason,
        //            Approval = p.Approval
        //        });

        //    // 페이징 적용
        //    var paged = await PaginatedList<ParticipantViewModel>.CreateAsync(projected, pageIndex, pageSize);

        //    // 결과 반환
        //    return new JsonResult(new
        //    {
        //        success = true,
        //        data = paged,
        //        pageIndex = paged.PageIndex,
        //        totalPages = paged.TotalPages,
        //        totalCount = paged.TotalCount
        //    });
        //}
    }
}