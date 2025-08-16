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

        // �ڽ� ����Ʈ (CourseName, HoleCount�� ����ִ� ViewModel)
        public List<CourseViewModel> Courses { get; set; } = new();

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

                // ��ġ �ɼ� ����
                GenderSort = HttpContext.Session.GetString("GenderSort") ?? "false";
                Handicapped = HttpContext.Session.GetString("Handicapped") ?? "false";
                AgeSort = HttpContext.Session.GetString("AgeSort") ?? "false";
                AwardSort = HttpContext.Session.GetString("AwardSort") ?? "false";

                // ����忡 �ش��ϴ� �ڽ� ��������
                var game = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
                if (game != null)
                {
                    Courses = await _context.Courses
                        .Where(c => c.StadiumCode == game.StadiumCode)
                        .Select(c => new CourseViewModel { CourseName = c.CourseName, HoleCount = c.HoleCount })
                        .ToListAsync();
                }
            }
        }

        public int MaxHoleNumber
        {
            get
            {
                return Courses.Any() ? Courses.Max(c => c.HoleCount) : 1;
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
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
            {
                TempData["ErrorTitle"] = "����";
                TempData["ErrorMessage"] = "��ȸ ������ ã�� �� �����ϴ�.";
                return Page();
            }

            var participants = await _context.GameParticipants
                .Where(p => p.GameCode == gameCode && !p.IsCancelled)
                .Include(p => p.User!)
                .ThenInclude(u => u.Handicap!)
                .ToListAsync();

            var awardDict = _context.GameAwardHistories
                .Where(a => participants.Select(p => p.UserId).Contains(a.UserId))
                .GroupBy(a => a.UserId)
                .ToDictionary(g => g.Key ?? "", g => g.Count());

            bool useGenderSort = GenderSort == "true";
            bool useHandicap = Handicapped == "true";
            int maxPerHole = 4;
            var courses = await _context.Courses.Where(c => c.StadiumCode == game.StadiumCode).ToListAsync();

            var assignmentResults = new List<CourseAssignmentResultViewModel>();
            var unassigned = new List<ParticipantViewModel>();

            if (useGenderSort)
            {
                // ��/�� �и�
                var maleParticipants = participants.Where(p => p.User?.UserGender == 1).ToList();
                var femaleParticipants = participants.Where(p => p.User?.UserGender == 2).ToList();

                // �ڵ�ĸ ����� ���� ����
                if (useHandicap)
                {
                    maleParticipants = maleParticipants.OrderByDescending(p => p.User?.Handicap?.AgeHandicap ?? 0).ToList();
                    femaleParticipants = femaleParticipants.OrderByDescending(p => p.User?.Handicap?.AgeHandicap ?? 0).ToList();
                }

                // ������
                var maleTeams = maleParticipants
                    .Select((p, i) => new { p, idx = i / maxPerHole })
                    .GroupBy(x => x.idx)
                    .Select(g => g.Select(x => x.p).ToList())
                    .ToList();

                // ������
                var femaleTeams = femaleParticipants
                    .Select((p, i) => new { p, idx = i / maxPerHole })
                    .GroupBy(x => x.idx)
                    .Select(g => g.Select(x => x.p).ToList())
                    .ToList();

                // ������ ���� ����, ������ �� ������ ������ ���� (�ڽ�/Ȧ �������)
                var allTeams = new List<(string gender, List<GameParticipant> team)>();
                allTeams.AddRange(maleTeams.Select(t => ("��", t)));
                allTeams.AddRange(femaleTeams.Select(t => ("��", t)));

                // �ο� ���� �� �켱 + ����
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
                // ���� ȥ�� �ڵ� ���� ����(�ڵ�ĸ �� �ٸ� �ɼ� ����)
                IEnumerable<GameParticipant> sortedParticipants = participants;
                if (useHandicap)
                    sortedParticipants = sortedParticipants.OrderByDescending(p => p.User?.Handicap?.AgeHandicap ?? 0);

                var teams = sortedParticipants
                    .Select((p, i) => new { p, idx = i / maxPerHole })
                    .GroupBy(x => x.idx)
                    .Select(g => g.Select(x => x.p).ToList())
                    .ToList();

                // ���� ����
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
                                // �Ʒ� �߰�: ���� ���� ������ ���� ä��
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

            this.AssignmentResults = assignmentResults;
            //TempData["AssignmentResults"] = JsonConvert.SerializeObject(assignmentResults);
            //TempData["UnassignedParticipants"] = JsonConvert.SerializeObject(unassigned); 
            //���� ������� ������ �������� ������
            HttpContext.Session.SetString("AssignmentResults", JsonConvert.SerializeObject(assignmentResults));
            HttpContext.Session.SetString("UnassignedParticipants", JsonConvert.SerializeObject(unassigned));

            HttpContext.Session.SetString("GenderSort", GenderSort);
            HttpContext.Session.SetString("Handicapped", Handicapped);
            HttpContext.Session.SetString("AgeSort", AgeSort);
            HttpContext.Session.SetString("AwardSort", AwardSort);

            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        // ���� �ؽ�Ʈ ��ȯ �Լ�
        private string GetAgeGroupText(int age, int gender)
        {
            age = PersonInfoCalculator.CalculateAge(age, gender); // ���� �ڵ� 1�� ����
            if (age < 40) return "û��";
            if (age < 60) return "�߳�";
            return "���";
        }

        // ���� ���� �ڵ鷯(�ִ� �ο� �ʰ� ����)
        public IActionResult OnPostForceAssignParticipant(string userId, string courseName, int holeNumber, string gameCode)
        {
            // 1. �̹��� �ο� ����Ʈ ��������
            var unassignedJson = HttpContext.Session.GetString("UnassignedParticipants");
            var UnassignedParticipants = !string.IsNullOrEmpty(unassignedJson)
                ? JsonConvert.DeserializeObject<List<ParticipantViewModel>>(unassignedJson)
                : new List<ParticipantViewModel>();

            // 2. ���� ������ ������ ã��
            var participant = (UnassignedParticipants ?? new List<ParticipantViewModel>())
                .FirstOrDefault(p => p.UserId == userId);
            if (participant == null)
            {
                TempData["ErrorMessage"] = "�ش� �����ڸ� ã�� �� �����ϴ�.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            // 3. ���� AssignmentResults ��������
            var resultsJson = HttpContext.Session.GetString("AssignmentResults");
            var AssignmentResults = !string.IsNullOrEmpty(resultsJson)
                ? JsonConvert.DeserializeObject<List<CourseAssignmentResultViewModel>>(resultsJson)
                : new List<CourseAssignmentResultViewModel>();

            // 4. �ش� �ڽ�/Ȧ�� ���� ���� �ο� ��
            var assignedCount = (AssignmentResults ?? new List<CourseAssignmentResultViewModel>())
                .Count(a => a.CourseName == courseName && a.HoleNumber == holeNumber.ToString());

            // 5. ������ ���� (�ִ� �ο� ����)
            var teamNumber = assignedCount + 1;
            AssignmentResults?.Add(new CourseAssignmentResultViewModel
            {
                CourseName = courseName,
                HoleNumber = holeNumber.ToString(),
                TeamNumber = (assignedCount + 1).ToString(),
                UserName = participant.Name,
                UserId = participant.UserId,
                GenderText = participant.GenderText,
                // ���� ����ؼ� �ֱ� (������ �⺻��)
                AgeGroupText = participant.AgeGroupText ?? "Ȯ�κҰ�",
                HandicapValue = participant.HandicapValue,
                AwardCount = participant.AwardCount
            });

            // 6. �̹��� �ο����� ����
            UnassignedParticipants?.Remove(participant);

            // 7. �ٽ� TempData�� ����
            //TempData["AssignmentResults"] = JsonConvert.SerializeObject(AssignmentResults);
            //TempData["UnassignedParticipants"] = JsonConvert.SerializeObject(UnassignedParticipants);
            HttpContext.Session.SetString("AssignmentResults", JsonConvert.SerializeObject(AssignmentResults));
            HttpContext.Session.SetString("UnassignedParticipants", JsonConvert.SerializeObject(UnassignedParticipants));
            TempData["SuccessMessage"] = $"������ {courseName} {holeNumber}Ȧ�� �����Ǿ����ϴ�!";

            // 8. ��� ������ �̵�
            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }
    }
}