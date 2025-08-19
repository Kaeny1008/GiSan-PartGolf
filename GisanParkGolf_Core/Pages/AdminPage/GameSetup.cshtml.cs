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

        // ------------------- ViewModel Properties -------------------
        public PaginatedList<CompetitionViewModel> Competitions { get; set; }
        public PaginatedList<ParticipantViewModel> Participants { get; set; }
        public PaginatedList<CourseAssignmentResultViewModel> Assignments { get; set; }
        public List<CourseAssignmentResultViewModel> AssignmentResults { get; set; } = new();
        public List<CourseViewModel> Courses { get; set; } = new();

        public int TotalCount { get; set; }
        public int CancelledCount { get; set; }
        public int JoinedCount { get; set; }
        public int MaxHoleNumber => Courses.Any() ? Courses.Max(c => c.HoleCount) : 1;
        public int AssignableCount { get; set; }

        // ------------------- Page Parameters -------------------
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
        [BindProperty(SupportsGet = true)] public string? Tab { get; set; }
        [BindProperty(SupportsGet = true)] public string? GameCode { get; set; }

        // ------------------- Utility: Session/DB Handling -------------------
        private List<CourseAssignmentResultViewModel> GetAssignmentResults(string gameCode)
        {
            var assignmentResultsJson = HttpContext.Session.GetString("AssignmentResults");
            List<CourseAssignmentResultViewModel> results = !string.IsNullOrEmpty(assignmentResultsJson)
                ? JsonConvert.DeserializeObject<List<CourseAssignmentResultViewModel>>(assignmentResultsJson) ?? new List<CourseAssignmentResultViewModel>()
                : new List<CourseAssignmentResultViewModel>();

            if (results == null || results.Count == 0)
            {
                // DB���� �ҷ�����
                var dbResults = _context.GameUserAssignments
                    .Include(a => a.Handicap)
                    .Include(a => a.User)
                    .Where(a => a.GameCode == gameCode)
                    .ToList();

                var userIds = dbResults.Select(r => r.UserId ?? "")
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Distinct().ToList();

                var awardDict = _context.GameAwardHistories
                    .Where(a => !string.IsNullOrEmpty(a.UserId) && userIds.Contains(a.UserId))
                    .GroupBy(a => a.UserId)
                    .ToDictionary(g => g.Key ?? "", g => g.Count());

                results = dbResults.Select(r => new CourseAssignmentResultViewModel
                {
                    CourseName = r.CourseName,
                    HoleNumber = r.HoleNumber,
                    TeamNumber = r.TeamNumber ?? "",
                    CourseOrder = r.CourseOrder ?? 0,
                    UserId = r.UserId ?? "",
                    UserName = r.User?.UserName ?? r.UserId ?? "",
                    GenderText = r.User?.UserGender == 1 ? "��" : r.User?.UserGender == 2 ? "��" : "",
                    AgeGroupText = GetAgeGroupText(r.User?.UserNumber ?? 0, r.User?.UserGender ?? 0),
                    HandicapValue = r.Handicap?.AgeHandicap ?? 0,
                    AwardCount = awardDict.TryGetValue(r.UserId ?? "", out var cnt) ? cnt : 0
                }).ToList();
            }
            return results;
        }

        private List<ParticipantViewModel> GetUnassignedParticipants()
        {
            var unassignedJson = HttpContext.Session.GetString("UnassignedParticipants");
            return !string.IsNullOrEmpty(unassignedJson)
                ? JsonConvert.DeserializeObject<List<ParticipantViewModel>>(unassignedJson) ?? new List<ParticipantViewModel>()
                : new List<ParticipantViewModel>();
        }

        private async Task SaveAssignmentResultsAsync(List<CourseAssignmentResultViewModel> results, string gameCode)
        {
            await AssignTeamNumbers(results, gameCode); // ���ѹ� ����
            HttpContext.Session.SetString("AssignmentResults", JsonConvert.SerializeObject(results ?? new List<CourseAssignmentResultViewModel>()));
        }

        private void SaveAssignmentResults(List<CourseAssignmentResultViewModel> results)
        {
            HttpContext.Session.SetString("AssignmentResults", JsonConvert.SerializeObject(results ?? new List<CourseAssignmentResultViewModel>()));
        }

        private void SaveUnassignedParticipants(List<ParticipantViewModel> participants)
        {
            HttpContext.Session.SetString("UnassignedParticipants", JsonConvert.SerializeObject(participants ?? new List<ParticipantViewModel>()));
        }

        private void RenumberCourseOrders(List<CourseAssignmentResultViewModel> results)
        {
            var grouped = results
                .GroupBy(r => new { r.CourseName, r.HoleNumber })
                .ToList();

            foreach (var group in grouped)
            {
                int idx = 1;
                // CourseOrder ���� ����!
                foreach (var item in group.OrderBy(r => r.CourseOrder))
                {
                    item.CourseOrder = idx;
                    idx++;
                }
            }
        }

        // ------------------- GET -------------------
        public async Task OnGetAsync(string? gameCode)
        {
            if (string.IsNullOrEmpty(gameCode)) ClearAssignmentSession();

            Competitions = await _gameService.GetCompetitionsAsync(SearchField, SearchQuery, PageIndex, PageSize);

            if (!string.IsNullOrEmpty(gameCode)) await LoadGameDetailDataAsync(gameCode);
        }

        private async Task LoadGameDetailDataAsync(string gameCode)
        {
            // ������, ���
            Participants = await _gameService.GetParticipantsAsync(
                gameCode, ParticipantSearchQuery, ParticipantPageIndex, ParticipantPageSize);

            // �����ڼ� ���� ����
            JoinedCount = await _context.GameParticipants.CountAsync(gp => !gp.IsCancelled);

            var query = _context.GameParticipants.Include(gp => gp.Game).AsQueryable();
            TotalCount = await query.CountAsync();
            CancelledCount = await _context.GameParticipants.CountAsync(gp => gp.IsCancelled);
            JoinedCount = await _context.GameParticipants.CountAsync(gp => !gp.IsCancelled);

            // ��ġ �ɼ� ���� -> ����
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

                // �ɼǰ� ������ DB���� �ݿ�
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
                        catch { /* ���� */ }
                    }
                }
            }

            // �ڽ���ġ ���(����/DB)
            AssignmentResults = GetAssignmentResults(gameCode);

            // �˻�/����¡
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
                .ThenBy(r => r.CourseOrder)
                .Skip((AssignmentPageIndex - 1) * AssignmentPageSize)
                .Take(AssignmentPageSize)
                .ToList();

            Assignments = new PaginatedList<CourseAssignmentResultViewModel>(
                pagedResults, totalCount, AssignmentPageIndex, AssignmentPageSize
            );
        }

        // ------------------- �ڵ鷯 (POST) -------------------
        public async Task<IActionResult> OnPostCancelAssignmentAsync(string gameCode, string userId, string? cancelReason)
        {
            if (string.IsNullOrWhiteSpace(cancelReason))
            {
                TempData["ErrorMessage"] = "��� ������ �ݵ�� �Է��ؾ� �մϴ�.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            // 1. ���� �����Ϳ��� ��ġ ����
            var assignmentResults = GetAssignmentResults(gameCode);
            assignmentResults.RemoveAll(r => r.UserId == userId);
            RenumberCourseOrders(assignmentResults);

            // 2. DB���� �ڽ���ġ ��� ����
            var userAssignments = await _context.GameUserAssignments
                .Where(a => a.GameCode == gameCode && a.UserId == userId)
                .ToListAsync();
            if (userAssignments.Any())
            {
                _context.GameUserAssignments.RemoveRange(userAssignments);
                await _context.SaveChangesAsync();
            }

            // 3. ������ ���� ��� ó�� (�������)
            var participant = await _context.GameParticipants
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.GameCode == gameCode && p.UserId == userId);
            if (participant != null)
            {
                participant.IsCancelled = true;
                participant.CancelDate = DateTime.Now;
                participant.CancelReason = cancelReason;
                participant.Approval = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";
                _context.GameParticipants.Update(participant);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString($"CancelReason_{userId}", cancelReason ?? "");
            }

            // 4. ���� ������Ʈ
            SaveAssignmentResults(assignmentResults);
            SaveUnassignedParticipants(GetUnassignedParticipants() ?? new List<ParticipantViewModel>());

            TempData["SuccessMessage"] = "�����ڰ� ��� ó���Ǿ����ϴ�. (�ڽ���ġ �� ���� ��Ұ� DB�� ��� �ݿ�)";
            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        public IActionResult OnPostForceAssignParticipant(string userId, string courseName, int holeNumber, string gameCode)
        {
            var unassigned = GetUnassignedParticipants();
            var participant = unassigned.FirstOrDefault(p => p.UserId == userId);
            if (participant == null)
            {
                TempData["ErrorMessage"] = "�ش� �����ڸ� ã�� �� �����ϴ�.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            var assignmentResults = GetAssignmentResults(gameCode);

            // ���� �ش� �ڽ�/Ȧ ������ ���� ����
            var sameHoleList = assignmentResults
                .Where(r => r.CourseName == courseName && r.HoleNumber == holeNumber.ToString())
                .OrderBy(r => r.CourseOrder)
                .ToList();

            assignmentResults.RemoveAll(r => r.CourseName == courseName && r.HoleNumber == holeNumber.ToString());

            // ���� ������ �� ���� ū CourseOrder ��
            int maxCourseOrder = sameHoleList.Count > 0 ? sameHoleList.Max(x => x.CourseOrder) : 0;

            // ���������ڸ� "������ ����"���� �߰�
            sameHoleList.Add(new CourseAssignmentResultViewModel
            {
                CourseName = courseName,
                HoleNumber = holeNumber.ToString(),
                CourseOrder = maxCourseOrder + 1, // �ٷ� ����!
                UserName = participant.Name,
                UserId = participant.UserId,
                GenderText = participant.GenderText,
                AgeGroupText = participant.AgeGroupText ?? "Ȯ���ʿ�",
                HandicapValue = participant.HandicapValue,
                AwardCount = participant.AwardCount
            });

            assignmentResults.AddRange(sameHoleList);
            unassigned.Remove(participant);

            // ���� ������(�ڽ�/Ȧ���� 1����)
            RenumberCourseOrders(assignmentResults);

            // �ڽ���>Ȧ��ȣ>������� ����
            assignmentResults = assignmentResults
                .OrderBy(r => r.CourseName)
                .ThenBy(r => int.TryParse(r.HoleNumber, out var hn) ? hn : 0)
                .ThenBy(r => r.CourseOrder)
                .ToList();

            // ���ѹ� �Ҵ�(�ڽ�/Ȧ���� �ϳ�, �ʰ����)
            AssignTeamNumbersAllowOverflow(assignmentResults);

            SaveAssignmentResults(assignmentResults);
            SaveUnassignedParticipants(unassigned);

            TempData["SuccessMessage"] = $"���������ڰ� {courseName} {holeNumber}Ȧ�� ������ ������ �����Ǿ����ϴ�!";
            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        // ���ѹ� �Ҵ�(�ڽ�/Ȧ���� �ϳ�, �ʰ����)
        private static void AssignTeamNumbersAllowOverflow(List<CourseAssignmentResultViewModel> assignmentResults)
        {
            var grouped = assignmentResults
                .GroupBy(r => new { r.CourseName, r.HoleNumber })
                .ToList();

            int teamSeq = 1;
            foreach (var group in grouped)
            {
                string currentTeamNumber = $"T{teamSeq:D2}";
                foreach (var item in group.OrderBy(r => r.CourseOrder))
                {
                    item.TeamNumber = currentTeamNumber;
                }
                teamSeq++;
            }
        }

        public async Task<IActionResult> OnPostSaveAssignmentResultAsync(string gameCode)
        {
            var assignmentResults = GetAssignmentResults(gameCode);

            // 1. ��ġ�ɼ� ���� (���� ���� ����)
            var gameSettingObj = new
            {
                //GenderSort = HttpContext.Session.GetString("GenderSort") ?? "false",
                //Handicapped = HttpContext.Session.GetString("Handicapped") ?? "false",
                //AgeSort = HttpContext.Session.GetString("AgeSort") ?? "false",
                //AwardSort = HttpContext.Session.GetString("AwardSort") ?? "false"
                GenderSort = this.GenderSort ?? "false",
                Handicapped = this.Handicapped ?? "false",
                AgeSort = this.AgeSort ?? "false",
                AwardSort = this.AwardSort ?? "false"
            };
            var settingJson = JsonConvert.SerializeObject(gameSettingObj);

            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game != null)
            {
                game.GameSetting = settingJson;
                game.GameStatus = "Assigned";
                _context.Games.Update(game);
                await _context.SaveChangesAsync();
            }

            // 2. ���� DB �������� �ҷ�����
            var prevAssignments = await _context.GameUserAssignments
                .Where(a => a.GameCode == gameCode)
                .ToListAsync();

            // 3. ���ǿ� �����ִ� ���������� ���Ͽ� ����� ã��
            var assignedUserIds = assignmentResults.Select(r => r.UserId).ToHashSet();
            var cancelledUsers = prevAssignments
                .Where(pa => !assignedUserIds.Contains(pa.UserId))
                .Select(pa => pa.UserId)
                .Distinct()
                .ToList();

            // 4. ��� �̷� �����
            foreach (var userId in cancelledUsers)
            {
                var participant = await _context.GameParticipants
                    .FirstOrDefaultAsync(p => p.GameCode == gameCode && p.UserId == userId);

                if (participant != null)
                {
                    var cancelReason = HttpContext.Session.GetString($"CancelReason_{userId}") ?? "�ڽ� ��ġ ���";
                    participant.IsCancelled = true;
                    participant.CancelDate = DateTime.Now;
                    participant.CancelReason = !string.IsNullOrWhiteSpace(cancelReason) ? cancelReason : "�ڽ� ��ġ ���";
                    participant.Approval = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";
                    _context.GameParticipants.Update(participant);
                }
            }
            await _context.SaveChangesAsync();

            // 5. ���� ���� ����/�� ���� ����
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
                    CourseOrder = r.CourseOrder,
                    TeamNumber = r.TeamNumber,
                    AgeHandicap = r.HandicapValue,
                    AssignmentStatus = "Assigned",
                    AssignedDate = DateTime.Now
                };
                _context.GameUserAssignments.Add(assignment);
            }
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"�ڽ���ġ ����� ���������� ����Ǿ����ϴ�.";
            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        // --- �������, ������ �� ���� ����� �״�� ---
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
            ClearAssignmentSession();

            TempData["SuccessTitle"] = "��� ���� �Ϸ�";
            TempData["SuccessMessage"] = "������ ��Ұ� ���εǾ����ϴ�. �ڽ���ġ ����� �ʿ�.";
            return RedirectToPage(new { gameCode = participant.GameCode });
        }

        public async Task<IActionResult> OnPostRejoinAsync(string id)
        {
            var participant = await _context.GameParticipants.FirstOrDefaultAsync(p => p.UserId == id);
            if (participant == null)
            {
                TempData["ErrorTitle"] = "����";
                TempData["ErrorMessage"] = "�����ڸ� ã�� �� �����ϴ�.";
                return RedirectToPage();
            }
            participant.IsCancelled = false;
            participant.CancelDate = null;
            participant.CancelReason = null;
            participant.Approval = null;

            _context.GameParticipants.Update(participant);
            await _context.SaveChangesAsync();

            TempData["SuccessTitle"] = "������ �Ϸ�";
            TempData["SuccessMessage"] = "������ ó���Ǿ����ϴ�.\n�ݵ�� �ڽ���ġ�� �ٽ� �����ؾ� �����ڰ� �����˴ϴ�.";
            return RedirectToPage(new { gameCode = participant.GameCode, tab = "tab-course" });
        }

        // ------------------- �ڽ���ġ �ɼ�/���� �κ� -------------------
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
                            CourseOrder = i + 1,
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
                            CourseOrder = i + 1,
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

            // === ���ѹ� ���� ===
            await AssignTeamNumbers(assignmentResults, gameCode);

            // �ٽ� ���� (Ȥ�� ���� ���� ������ �ص� ��)
            HttpContext.Session.SetString("AssignmentResults", JsonConvert.SerializeObject(assignmentResults));

            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        private async Task AssignTeamNumbers(List<CourseAssignmentResultViewModel> assignmentResults, string gameCode)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            int maxPerHole = 4; // �⺻��
            if (!string.IsNullOrEmpty(game?.GameSetting))
            {
                var setting = JsonConvert.DeserializeObject<dynamic>(game.GameSetting);
                maxPerHole = setting?.MaxPerHole ?? 4;
            }

            int teamSeq = 1;
            int memberCount = 0;
            string currentTeamNumber = $"T{teamSeq:D2}";

            for (int i = 0; i < assignmentResults.Count; i++)
            {
                assignmentResults[i].TeamNumber = currentTeamNumber;
                assignmentResults[i].CourseOrder = memberCount + 1;

                memberCount++;
                if (memberCount >= maxPerHole)
                {
                    teamSeq++;
                    currentTeamNumber = $"T{teamSeq:D2}";
                    memberCount = 0;
                }
            }
        }

        // ------------------- ��Ÿ ��ƿ/���� -------------------
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