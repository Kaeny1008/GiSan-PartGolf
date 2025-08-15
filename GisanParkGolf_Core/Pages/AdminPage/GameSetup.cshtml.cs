using DocumentFormat.OpenXml.Wordprocessing;
using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.Services.AdminPage;
using GisanParkGolf_Core.ViewModels.AdminPage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        public int TotalCount { get; set; }
        public int CancelledCount { get; set; }
        public int JoinedCount { get; set; }

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

        // ���� �ɼ�
        [BindProperty]
        public string GenderSort { get; set; } = "false";

        [BindProperty]
        public string Handicapped { get; set; } = "false";

        [BindProperty]
        public string AgeSort { get; set; } = "false";

        [BindProperty]
        public string AwardSort { get; set; } = "false";

        // �ڽ���ġ ����� ������ ����Ʈ
        public List<CourseAssignmentResultViewModel> AssignmentResults { get; set; } = new();

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

                var query = _context.GameParticipants
                    .Include(gp => gp.Game)
                    .AsQueryable();
                TotalCount = await query.CountAsync();
                CancelledCount = await _context.GameParticipants.CountAsync(gp => gp.IsCancelled);
                JoinedCount = await _context.GameParticipants.CountAsync(gp => !gp.IsCancelled);

                if (TempData.ContainsKey("AssignmentResults"))
                {
                    var json = TempData["AssignmentResults"] as string;
                    if (!string.IsNullOrEmpty(json))
                    {
                        AssignmentResults = JsonConvert.DeserializeObject<List<CourseAssignmentResultViewModel>>(json) ??
                            new List<CourseAssignmentResultViewModel>();
                    }
                }

                // ��ġ �ɼ� ����
                if (TempData.ContainsKey("GenderSort"))
                    GenderSort = TempData["GenderSort"] as string ?? "false";
                if (TempData.ContainsKey("Handicapped"))
                    Handicapped = TempData["Handicapped"] as string ?? "false";
                if (TempData.ContainsKey("AgeSort"))
                    AgeSort = TempData["AgeSort"] as string ?? "false";
                if (TempData.ContainsKey("AwardSort"))
                    AwardSort = TempData["AwardSort"] as string ?? "false";
            }
        }

        public async Task<IActionResult> OnPostApproveCancelAsync(string id)
        {             
            var participant = await _context.GameParticipants
                .FirstOrDefaultAsync(p => p.UserId == id);
            if (participant == null)
            {
                TempData["ErrorTitle"] = "����";
                TempData["ErrorMessage"] = "�����ڸ� ã�� �� �����ϴ�.";
                return RedirectToPage();
            }

            // � �����ڰ� �ߴ��� ���
            // �����ڸ� �Է��ϸ� ���ε�.
            participant.Approval = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";

            _context.GameParticipants.Update(participant);
            await _context.SaveChangesAsync();

            TempData["SuccessTitle"] = "��� ���� �Ϸ�";
            TempData["SuccessMessage"] = "��� ������ �Ϸ�Ǿ����ϴ�.";

            return RedirectToPage(new { gameCode = participant.GameCode });
        }

        public async Task<IActionResult> OnPostRejoinAsync(string id)
        {
            var participant = await _context.GameParticipants
                .FirstOrDefaultAsync(p => p.UserId == id);
            if (participant == null)
            {
                TempData["ErrorTitle"] = "����";
                TempData["ErrorMessage"] = "������ ó���� �����߽��ϴ�.";
                return RedirectToPage();
            }

            participant.IsCancelled = false;
            participant.CancelDate = null;
            participant.CancelReason = null;
            participant.Approval = null;

            _context.GameParticipants.Update(participant);
            await _context.SaveChangesAsync();

            TempData["SuccessTitle"] = "������ �Ϸ�";
            TempData["SuccessMessage"] = "�ش� �����ڸ� ������ ó���Ͽ����ϴ�.";

            return RedirectToPage(new { gameCode = participant.GameCode });
        }

        public async Task<IActionResult> OnPostCourseAssignmentAsync(string gameCode)
        {
            // 1. ���� ���� ��ȸ
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
            {
                TempData["ErrorTitle"] = "����";
                TempData["ErrorMessage"] = "���� ������ ã�� �� �����ϴ�.";
                return Page();
            }

            // 2. ������ ����Ʈ �ҷ�����
            var participants = await _context.GameParticipants
                .Where(p => p.GameCode == gameCode && !p.IsCancelled)
                .Include(p => p.User!)
                .ThenInclude(u => u.Handicap!)
                .ToListAsync();

            // 3. ������ Dictionary �غ�
            var awardDict = _context.GameAwardHistories
                .Where(a => participants.Select(p => p.UserId).Contains(a.UserId))
                .GroupBy(a => a.UserId)
                .ToDictionary(g => g.Key ?? "", g => g.Count());

            // 4. �ɼ� �� �������� (string �� bool ��ȯ)
            bool useGenderSort = GenderSort == "true";
            bool useHandicap = Handicapped == "true";
            bool useAgeSort = AgeSort == "true";
            bool useAwardSort = AwardSort == "true";

            // 5. �ɼǿ� ���� ����
            var query = participants.Select(p => new
            {
                UserName = p.User?.UserName ?? p.UserId ?? "�̸�����",
                UserId = p.UserId ?? "",
                Gender = p.User?.UserGender ?? 0,
                Age = p.User?.UserNumber ?? 0,
                Handicap = p.User?.Handicap?.AgeHandicap ?? 0,
                AwardCount = awardDict.TryGetValue(p.UserId ?? "", out var cnt) ? cnt : 0
            });

            // ���� ���� �ʿ�� ThenBy/ThenByDescending ��� (���⼱ ���� ����)
            if (useGenderSort)
                query = query.OrderBy(x => x.Gender);
            if (useAgeSort)
                query = query.OrderBy(x => x.Age);
            if (useHandicap)
                query = query.OrderByDescending(x => x.Handicap);
            if (useAwardSort)
                query = query.OrderByDescending(x => x.AwardCount);

            var sortedParticipants = query.ToList();

            // 6. �ڽ�/Ȧ/�� ����
            var courses = await _context.Courses.Where(c => c.StadiumCode == game.StadiumCode).ToListAsync();
            int teamNumber = 1, holeNumber = 1;
            int courseIdx = 0;
            var results = new List<CourseAssignmentResultViewModel>();

            foreach (var p in sortedParticipants)
            {
                var course = courses[courseIdx % courses.Count];
                results.Add(new CourseAssignmentResultViewModel
                {
                    CourseName = course.CourseName,
                    HoleNumber = holeNumber.ToString(),
                    TeamNumber = teamNumber.ToString(),
                    UserName = p.UserName,
                    UserId = p.UserId,
                    GenderText = p.Gender == 1 ? "��" : "��",
                    AgeGroupText = GetAgeGroupText(p.Age),
                    HandicapValue = p.Handicap,
                    AwardCount = p.AwardCount
                });

                teamNumber++;
                if (teamNumber > 4)
                {
                    teamNumber = 1;
                    holeNumber++;
                    if (holeNumber > course.HoleCount)
                    {
                        holeNumber = 1;
                        courseIdx++;
                    }
                }
            }

            this.AssignmentResults = results;

            TempData["AssignmentResults"] = JsonConvert.SerializeObject(results);

            // 7. ��ġ �ɼ��� TempData�� ���� (�����̷�Ʈ �� �ɼ� ����)
            TempData["GenderSort"] = GenderSort;
            TempData["Handicapped"] = Handicapped;
            TempData["AgeSort"] = AgeSort;
            TempData["AwardSort"] = AwardSort;

            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        // ���ɴ� �ؽ�Ʈ ��ȯ �Լ�
        private string GetAgeGroupText(int age)
        {
            if (age < 40) return "û��";
            if (age < 60) return "�߳�";
            return "���";
        }
    }
}