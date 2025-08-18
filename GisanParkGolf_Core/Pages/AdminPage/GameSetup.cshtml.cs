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
            Assignments = new PaginatedList<CourseAssignmentResultViewModel>(new List<CourseAssignmentResultViewModel>(), 0, 1, 10);
        }

        public PaginatedList<CompetitionViewModel> Competitions { get; set; }
        public PaginatedList<ParticipantViewModel> Participants { get; set; }
        public PaginatedList<CourseAssignmentResultViewModel> Assignments { get; set; }
        public List<CourseAssignmentResultViewModel> AssignmentResults { get; set; } = new();
        public List<CourseViewModel> Courses { get; set; } = new();

        public int TotalCount { get; set; }
        public int CancelledCount { get; set; }
        public int JoinedCount { get; set; }
        public int MaxHoleNumber => Courses.Any() ? Courses.Max(c => c.HoleCount) : 1;

        // �ɼ�
        [BindProperty(SupportsGet = true)] public int PageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)] public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public string? SearchField { get; set; } = "GameName";
        [BindProperty(SupportsGet = true)] public string? SearchQuery { get; set; }
        [BindProperty(SupportsGet = true)] public int ParticipantPageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int ParticipantPageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)] public string? ParticipantSearchQuery { get; set; }
        [BindProperty(SupportsGet = true)] public int AssignmentPageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int AssignmentPageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)] public string? AssignmentSearchQuery { get; set; }
        [BindProperty] public string? GenderSort { get; set; }
        [BindProperty] public string? Handicapped { get; set; }
        [BindProperty] public string? AgeSort { get; set; }
        [BindProperty] public string? AwardSort { get; set; }

        // ���� GET
        public async Task OnGetAsync(string? gameCode)
        {
            // gameCode�� ������ ���� Ŭ���� (���/�� ����)
            if (string.IsNullOrEmpty(gameCode))
            {
                ClearAssignmentSession();
            }

            // ��ȸ ��� ��ȸ
            Competitions = await _gameService.GetCompetitionsAsync(SearchField, SearchQuery, PageIndex, PageSize);

            // gameCode�� ������ �ش� ��ȸ �� ������ �ε�
            if (!string.IsNullOrEmpty(gameCode))
            {
                await LoadGameDetailDataAsync(gameCode);
            }
        }

        // �� ������ �ε� (������/�ڽ�/��ġ���)
        private async Task LoadGameDetailDataAsync(string gameCode)
        {
            Participants = await _gameService.GetParticipantsAsync(
                gameCode, ParticipantSearchQuery, ParticipantPageIndex, ParticipantPageSize);

            var query = _context.GameParticipants.Include(gp => gp.Game).AsQueryable();
            TotalCount = await query.CountAsync();
            CancelledCount = await _context.GameParticipants.CountAsync(gp => gp.IsCancelled);
            JoinedCount = await _context.GameParticipants.CountAsync(gp => !gp.IsCancelled);

            // �ɼ� ���� ����ȭ
            GenderSort = HttpContext.Session.GetString("GenderSort");
            Handicapped = HttpContext.Session.GetString("Handicapped");
            AgeSort = HttpContext.Session.GetString("AgeSort");
            AwardSort = HttpContext.Session.GetString("AwardSort");

            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game != null)
            {
                Courses = await _context.Courses
                    .Where(c => c.StadiumCode == game.StadiumCode)
                    .Select(c => new CourseViewModel { CourseName = c.CourseName, HoleCount = c.HoleCount })
                    .ToListAsync();

                // �ɼ� ���� ������ DB�� ����� ���Ӽ������� ����
                if (string.IsNullOrEmpty(GenderSort) || string.IsNullOrEmpty(Handicapped) ||
                    string.IsNullOrEmpty(AgeSort) || string.IsNullOrEmpty(AwardSort))
                {
                    if (!string.IsNullOrEmpty(game.GameSetting))
                    {
                        try
                        {
                            var setting = JsonConvert.DeserializeObject<dynamic>(game.GameSetting);
                            if (setting != null)
                            {
                                GenderSort ??= setting.GenderSort ?? "false";
                                Handicapped ??= setting.Handicapped ?? "false";
                                AgeSort ??= setting.AgeSort ?? "false";
                                AwardSort ??= setting.AwardSort ?? "false";
                            }
                        }
                        catch { /* �Ľ� ���� ���� */ }
                    }
                }
            }

            // ��ġ ���: ���ǿ� ������ ���ǿ���, ������ DB����
            var assignmentResultsJson = HttpContext.Session.GetString("AssignmentResults");
            if (!string.IsNullOrEmpty(assignmentResultsJson))
            {
                AssignmentResults = JsonConvert.DeserializeObject<List<CourseAssignmentResultViewModel>>(assignmentResultsJson) ?? new();
            }
            else
            {
                var dbResults = await _context.GameUserAssignments
                    .Include(a => a.Handicap)
                    .Include(a => a.User)
                    .Where(a => a.GameCode == gameCode)
                    .ToListAsync();

                var userIds = dbResults.Select(r => r.UserId ?? "")
                                       .Where(id => !string.IsNullOrEmpty(id))
                                       .Distinct().ToList();

                var awardDict = await _context.GameAwardHistories
                    .Where(a => !string.IsNullOrEmpty(a.UserId) && userIds.Contains(a.UserId))
                    .GroupBy(a => a.UserId)
                    .ToDictionaryAsync(g => g.Key ?? "", g => g.Count());

                AssignmentResults = dbResults.Select(r => new CourseAssignmentResultViewModel
                {
                    CourseName = r.CourseName,
                    HoleNumber = r.HoleNumber,
                    TeamNumber = r.TeamNumber,
                    UserId = r.UserId ?? "",
                    UserName = r.User?.UserName ?? r.UserId ?? "",
                    GenderText = r.User?.UserGender == 1 ? "��" : r.User?.UserGender == 2 ? "��" : "",
                    AgeGroupText = GetAgeGroupText(r.User?.UserNumber ?? 0, r.User?.UserGender ?? 0),
                    HandicapValue = r.Handicap?.AgeHandicap ?? 0,
                    AwardCount = awardDict.TryGetValue(r.UserId ?? "", out var cnt) ? cnt : 0
                }).ToList();
            }

            IEnumerable<CourseAssignmentResultViewModel> filteredResults = AssignmentResults;

            if (!string.IsNullOrWhiteSpace(AssignmentSearchQuery))
            {
                var q = AssignmentSearchQuery.Trim();
                filteredResults = filteredResults.Where(r =>
                    (r.UserName != null && r.UserName.Contains(q)) ||
                    (r.UserId != null && r.UserId.Contains(q)) ||
                    (r.CourseName != null && r.CourseName.Contains(q))
                );
            }

            int totalCount = filteredResults.Count();
            var pagedResults = filteredResults
                .OrderBy(r => r.CourseName)
                .ThenBy(r => int.TryParse(r.HoleNumber, out var hn) ? hn : 0)
                .ThenBy(r => int.TryParse(r.TeamNumber, out var tn) ? tn : 0)
                .Skip((AssignmentPageIndex - 1) * AssignmentPageSize)
                .Take(AssignmentPageSize)
                .ToList();

            Assignments = new PaginatedList<CourseAssignmentResultViewModel>(
                pagedResults, totalCount, AssignmentPageIndex, AssignmentPageSize
            );
        }

        // ���� ��� ����
        public async Task<IActionResult> OnPostApproveCancelAsync(string id)
        {
            var participant = await _context.GameParticipants.FirstOrDefaultAsync(p => p.UserId == id);
            if (participant == null)
            {
                TempData["ErrorTitle"] = "����";
                TempData["ErrorMessage"] = "�����ڸ� ã�� �� �����ϴ�.";
                return RedirectToPage();
            }
            participant.Approval = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";
            _context.GameParticipants.Update(participant);
            await _context.SaveChangesAsync();

            // �ڽ���ġ ��� ���� �ʱ�ȭ
            ClearAssignmentSession();

            TempData["SuccessTitle"] = "��� ���� �Ϸ�";
            TempData["SuccessMessage"] = "��ҵ� �����ڸ� �ݿ��Ϸ��� �ڽ���ġ�� �ٽ� �����ϼ���.";
            return RedirectToPage(new { gameCode = participant.GameCode });
        }

        // ������ ������ ó��
        public async Task<IActionResult> OnPostRejoinAsync(string id)
        {
            var participant = await _context.GameParticipants.FirstOrDefaultAsync(p => p.UserId == id);
            if (participant == null)
            {
                TempData["ErrorTitle"] = "����";
                TempData["ErrorMessage"] = "������ ������ ã�� �� �����ϴ�.";
                return RedirectToPage();
            }
            participant.IsCancelled = false;
            participant.CancelDate = null;
            participant.CancelReason = null;
            participant.Approval = null;

            _context.GameParticipants.Update(participant);
            await _context.SaveChangesAsync();

            TempData["SuccessTitle"] = "������ �Ϸ�";
            TempData["SuccessMessage"] = "������ ó���Ǿ����ϴ�. �ڽ���ġ�� �ٽ� �����ؾ� �ݿ��˴ϴ�.";
            return RedirectToPage(new { gameCode = participant.GameCode });
        }

        // �ڽ� ��ġ ����
        public async Task<IActionResult> OnPostCourseAssignmentAsync(string gameCode)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
            {
                TempData["ErrorTitle"] = "����";
                TempData["ErrorMessage"] = "��ȸ ������ ã�� �� �����ϴ�.";
                return Page();
            }

            var participants = await _context.GameParticipants
                .Where(p => p.GameCode == gameCode && !p.IsCancelled)
                .Include(p => p.User)
                .ThenInclude(u => u.Handicap)
                .ToListAsync();

            var userIds = participants.Select(p => p.UserId ?? "")
                                     .Where(id => !string.IsNullOrEmpty(id))
                                     .Distinct().ToList();

            var awardDict = await _context.GameAwardHistories
                .Where(a => !string.IsNullOrEmpty(a.UserId) && userIds.Contains(a.UserId))
                .GroupBy(a => a.UserId)
                .ToDictionaryAsync(g => g.Key ?? "", g => g.Count());

            bool useGenderSort = GenderSort == "true";
            bool useHandicap = Handicapped == "true";
            int maxPerHole = 4;
            var courses = await _context.Courses.Where(c => c.StadiumCode == game.StadiumCode).ToListAsync();

            var assignmentResults = new List<CourseAssignmentResultViewModel>();
            var unassigned = new List<ParticipantViewModel>();

            // ���� �˰��� ����, null ���� �߰�
            if (useGenderSort)
            {
                var maleParticipants = participants.Where(p => p.User?.UserGender == 1).ToList();
                var femaleParticipants = participants.Where(p => p.User?.UserGender == 2).ToList();

                if (useHandicap)
                {
                    maleParticipants = maleParticipants.OrderByDescending(p => p.User?.Handicap?.AgeHandicap ?? 0).ToList();
                    femaleParticipants = femaleParticipants.OrderByDescending(p => p.User?.Handicap?.AgeHandicap ?? 0).ToList();
                }
                else
                {
                    maleParticipants = maleParticipants.OrderBy(_ => Guid.NewGuid()).ToList();
                    femaleParticipants = femaleParticipants.OrderBy(_ => Guid.NewGuid()).ToList();
                }

                var maleTeams = maleParticipants
                    .Select((p, i) => new { p, idx = i / maxPerHole })
                    .GroupBy(x => x.idx)
                    .Select(g => g.Select(x => x.p).ToList())
                    .ToList();

                var femaleTeams = femaleParticipants
                    .Select((p, i) => new { p, idx = i / maxPerHole })
                    .GroupBy(x => x.idx)
                    .Select(g => g.Select(x => x.p).ToList())
                    .ToList();

                var allTeams = new List<(string gender, List<GameParticipant> team)>();
                allTeams.AddRange(maleTeams.Select(t => ("��", t)));
                allTeams.AddRange(femaleTeams.Select(t => ("��", t)));

                allTeams = allTeams.OrderByDescending(t => t.team.Count)
                                   .ThenBy(_ => Guid.NewGuid())
                                   .ToList();

                int teamIdx = 0;
                int totalAvailableSlots = courses.Sum(c => c.HoleCount);

                foreach (var t in allTeams)
                {
                    if (teamIdx >= totalAvailableSlots)
                    {
                        foreach (var p in t.team)
                        {
                            unassigned.Add(new ParticipantViewModel
                            {
                                Name = p.User?.UserName ?? p.UserId ?? "�̸�����",
                                UserId = p.UserId ?? "",
                                GenderText = t.gender,
                                HandicapValue = p.User?.Handicap?.AgeHandicap ?? 0,
                                AgeGroupText = GetAgeGroupText(p.User?.UserNumber ?? 0, p.User?.UserGender ?? 0),
                                AwardCount = awardDict.TryGetValue(p.UserId ?? "", out var cnt) ? cnt : 0
                            });
                        }
                        continue;
                    }
                    int courseIdx = 0;
                    int holeIdx = teamIdx + 1;
                    int acc = 0;
                    foreach (var c in courses)
                    {
                        acc += c.HoleCount;
                        if (teamIdx < acc)
                        {
                            courseIdx = courses.IndexOf(c);
                            holeIdx = c.HoleCount - (acc - teamIdx - 1);
                            break;
                        }
                    }
                    var course = courses[courseIdx];

                    for (int i = 0; i < t.team.Count; i++)
                    {
                        var p = t.team[i];
                        assignmentResults.Add(new CourseAssignmentResultViewModel
                        {
                            CourseName = course.CourseName,
                            HoleNumber = holeIdx.ToString(),
                            TeamNumber = (i + 1).ToString(),
                            UserName = p.User?.UserName ?? p.UserId ?? "�̸�����",
                            UserId = p.UserId ?? "",
                            GenderText = t.gender,
                            AgeGroupText = GetAgeGroupText(p.User?.UserNumber ?? 0, p.User?.UserGender ?? 0),
                            HandicapValue = p.User?.Handicap?.AgeHandicap ?? 0,
                            AwardCount = awardDict.TryGetValue(p.UserId ?? "", out var cnt) ? cnt : 0
                        });
                    }
                    teamIdx++;
                }
            }
            else
            {
                IEnumerable<GameParticipant> sortedParticipants = participants;
                if (useHandicap)
                {
                    sortedParticipants = sortedParticipants.OrderByDescending(p => p.User?.Handicap?.AgeHandicap ?? 0);
                }
                else
                {
                    sortedParticipants = sortedParticipants.OrderBy(_ => Guid.NewGuid());
                }

                var teams = sortedParticipants
                        .Select((p, i) => new { p, idx = i / maxPerHole })
                        .GroupBy(x => x.idx)
                        .Select(g => g.Select(x => x.p).ToList())
                        .ToList();

                teams = teams.OrderBy(_ => Guid.NewGuid()).ToList();

                int teamIdx = 0;
                int totalAvailableSlots = courses.Sum(c => c.HoleCount);

                foreach (var t in teams)
                {
                    if (teamIdx >= totalAvailableSlots)
                    {
                        foreach (var p in t)
                        {
                            unassigned.Add(new ParticipantViewModel
                            {
                                Name = p.User?.UserName ?? p.UserId ?? "�̸�����",
                                UserId = p.UserId ?? "",
                                GenderText = p.User?.UserGender == 1 ? "��" : "��",
                                HandicapValue = p.User?.Handicap?.AgeHandicap ?? 0,
                                AgeGroupText = GetAgeGroupText(p.User?.UserNumber ?? 0, p.User?.UserGender ?? 0),
                                AwardCount = awardDict.TryGetValue(p.UserId ?? "", out var cnt) ? cnt : 0
                            });
                        }
                        continue;
                    }
                    int courseIdx = 0;
                    int holeIdx = teamIdx + 1;
                    int acc = 0;
                    foreach (var c in courses)
                    {
                        acc += c.HoleCount;
                        if (teamIdx < acc)
                        {
                            courseIdx = courses.IndexOf(c);
                            holeIdx = c.HoleCount - (acc - teamIdx - 1);
                            break;
                        }
                    }
                    var course = courses[courseIdx];

                    for (int i = 0; i < t.Count; i++)
                    {
                        var p = t[i];
                        assignmentResults.Add(new CourseAssignmentResultViewModel
                        {
                            CourseName = course.CourseName,
                            HoleNumber = holeIdx.ToString(),
                            TeamNumber = (i + 1).ToString(),
                            UserName = p.User?.UserName ?? p.UserId ?? "�̸�����",
                            UserId = p.UserId ?? "",
                            GenderText = p.User?.UserGender == 1 ? "��" : "��",
                            AgeGroupText = GetAgeGroupText(p.User?.UserNumber ?? 0, p.User?.UserGender ?? 0),
                            HandicapValue = p.User?.Handicap?.AgeHandicap ?? 0,
                            AwardCount = awardDict.TryGetValue(p.UserId ?? "", out var cnt) ? cnt : 0
                        });
                    }
                    teamIdx++;
                }
            }

            HttpContext.Session.SetString("AssignmentResults", JsonConvert.SerializeObject(assignmentResults));
            HttpContext.Session.SetString("UnassignedParticipants", JsonConvert.SerializeObject(unassigned));
            HttpContext.Session.SetString("GenderSort", GenderSort ?? "false");
            HttpContext.Session.SetString("Handicapped", Handicapped ?? "false");
            HttpContext.Session.SetString("AgeSort", AgeSort ?? "false");
            HttpContext.Session.SetString("AwardSort", AwardSort ?? "false");

            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        // �̹��� �ο� ���� ����
        public IActionResult OnPostForceAssignParticipant(string userId, string courseName, int holeNumber, string gameCode)
        {
            var unassignedJson = HttpContext.Session.GetString("UnassignedParticipants");
            List<ParticipantViewModel> UnassignedParticipants = !string.IsNullOrEmpty(unassignedJson)
                ? JsonConvert.DeserializeObject<List<ParticipantViewModel>>(unassignedJson) ?? new List<ParticipantViewModel>()
                : new List<ParticipantViewModel>();

            var participant = UnassignedParticipants.FirstOrDefault(p => p.UserId == userId);
            if (participant == null)
            {
                TempData["ErrorMessage"] = "�ش� �����ڸ� ã�� �� �����ϴ�.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            var resultsJson = HttpContext.Session.GetString("AssignmentResults");
            List<CourseAssignmentResultViewModel> AssignmentResults = !string.IsNullOrEmpty(resultsJson)
                ? JsonConvert.DeserializeObject<List<CourseAssignmentResultViewModel>>(resultsJson) ?? new List<CourseAssignmentResultViewModel>()
                : new List<CourseAssignmentResultViewModel>();

            var assignedCount = AssignmentResults.Count(a => a.CourseName == courseName && a.HoleNumber == holeNumber.ToString());

            AssignmentResults.Add(new CourseAssignmentResultViewModel
            {
                CourseName = courseName,
                HoleNumber = holeNumber.ToString(),
                TeamNumber = (assignedCount + 1).ToString(),
                UserName = participant.Name,
                UserId = participant.UserId,
                GenderText = participant.GenderText,
                AgeGroupText = participant.AgeGroupText ?? "Ȯ�κҰ�",
                HandicapValue = participant.HandicapValue,
                AwardCount = participant.AwardCount
            });

            UnassignedParticipants.Remove(participant);

            HttpContext.Session.SetString("AssignmentResults", JsonConvert.SerializeObject(AssignmentResults));
            HttpContext.Session.SetString("UnassignedParticipants", JsonConvert.SerializeObject(UnassignedParticipants));
            TempData["SuccessMessage"] = $"�������� {courseName} {holeNumber}Ȧ�� �����Ǿ����ϴ�!";

            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        // ��ġ ��� ����
        public async Task<IActionResult> OnPostSaveAssignmentResultAsync(string gameCode)
        {
            var assignmentResultsJson = HttpContext.Session.GetString("AssignmentResults");
            var assignmentResults = !string.IsNullOrEmpty(assignmentResultsJson)
                ? JsonConvert.DeserializeObject<List<CourseAssignmentResultViewModel>>(assignmentResultsJson)
                : new List<CourseAssignmentResultViewModel>();

            if (assignmentResults == null || assignmentResults.Count == 0)
            {
                TempData["ErrorMessage"] = "������ ��ġ ����� �����ϴ�.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            // �ɼ� ����
            var gameSettingObj = new
            {
                GenderSort = HttpContext.Session.GetString("GenderSort") ?? "false",
                Handicapped = HttpContext.Session.GetString("Handicapped") ?? "false",
                AgeSort = HttpContext.Session.GetString("AgeSort") ?? "false",
                AwardSort = HttpContext.Session.GetString("AwardSort") ?? "false"
            };
            var settingJson = JsonConvert.SerializeObject(gameSettingObj);

            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game != null)
            {
                game.GameSetting = settingJson;
                game.GameStatus = "Assigned"; // ���� ����
                _context.Games.Update(game);
                await _context.SaveChangesAsync();
            }

            var existingAssignments = await _context.GameUserAssignments
                .Where(a => a.GameCode == gameCode)
                .ToListAsync();

            if (existingAssignments.Any())
            {
                _context.GameUserAssignments.RemoveRange(existingAssignments);
                await _context.SaveChangesAsync();
            }

            foreach (var r in assignmentResults)
            {
                var assignment = new GameUserAssignment
                {
                    GameCode = gameCode,
                    UserId = r.UserId,
                    CourseName = r.CourseName,
                    HoleNumber = r.HoleNumber,
                    TeamNumber = r.TeamNumber,
                    AgeHandicap = r.HandicapValue,
                    AssignmentStatus = "Assigned",
                    AssignedDate = DateTime.Now
                };
                _context.GameUserAssignments.Add(assignment);
            }
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"��ġ ����� ���������� ����Ǿ����ϴ�.";
            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        // ���ɴ� �ؽ�Ʈ ��ȯ
        private string GetAgeGroupText(int age, int gender)
        {
            age = PersonInfoCalculator.CalculateAge(age, gender);
            if (age < 40) return "û��";
            if (age < 60) return "�߳�";
            return "���";
        }

        private void ClearAssignmentSession()
        {
            HttpContext.Session.Remove("AssignmentResults");
            HttpContext.Session.Remove("UnassignedParticipants");
            HttpContext.Session.Remove("GenderSort");
            HttpContext.Session.Remove("Handicapped");
            HttpContext.Session.Remove("AgeSort");
            HttpContext.Session.Remove("AwardSort");
        }
    }
}