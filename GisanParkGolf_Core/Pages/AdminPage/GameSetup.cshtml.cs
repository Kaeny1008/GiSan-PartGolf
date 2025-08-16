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

        // 정렬 옵션
        [BindProperty]
        public string GenderSort { get; set; } = "false";

        [BindProperty]
        public string Handicapped { get; set; } = "false";

        [BindProperty]
        public string AgeSort { get; set; } = "false";

        [BindProperty]
        public string AwardSort { get; set; } = "false";

        // 코스배치 결과를 저장할 리스트
        public List<CourseAssignmentResultViewModel> AssignmentResults { get; set; } = new();

        // 코스 리스트 (CourseName, HoleCount가 들어있는 ViewModel)
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

                // 배치 옵션 복원
                GenderSort = HttpContext.Session.GetString("GenderSort") ?? "false";
                Handicapped = HttpContext.Session.GetString("Handicapped") ?? "false";
                AgeSort = HttpContext.Session.GetString("AgeSort") ?? "false";
                AwardSort = HttpContext.Session.GetString("AwardSort") ?? "false";

                // 경기장에 해당하는 코스 가져오기
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
                TempData["ErrorTitle"] = "오류";
                TempData["ErrorMessage"] = "참가자를 찾을 수 없습니다.";
                return RedirectToPage();
            }

            // 어떤 관리자가 했는지 기록
            // 관리자만 입력하면 승인됨.
            participant.Approval = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";

            _context.GameParticipants.Update(participant);
            await _context.SaveChangesAsync();

            TempData["SuccessTitle"] = "취소 승인 완료";
            TempData["SuccessMessage"] = "취소 승인이 완료되었습니다.";

            return RedirectToPage(new { gameCode = participant.GameCode });
        }

        public async Task<IActionResult> OnPostRejoinAsync(string id)
        {
            var participant = await _context.GameParticipants
                .FirstOrDefaultAsync(p => p.UserId == id);
            if (participant == null)
            {
                TempData["ErrorTitle"] = "오류";
                TempData["ErrorMessage"] = "재참가 처리에 실패했습니다.";
                return RedirectToPage();
            }

            participant.IsCancelled = false;
            participant.CancelDate = null;
            participant.CancelReason = null;
            participant.Approval = null;

            _context.GameParticipants.Update(participant);
            await _context.SaveChangesAsync();

            TempData["SuccessTitle"] = "재참가 완료";
            TempData["SuccessMessage"] = "해당 참가자를 재참가 처리하였습니다.";

            return RedirectToPage(new { gameCode = participant.GameCode });
        }

        public async Task<IActionResult> OnPostCourseAssignmentAsync(string gameCode)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
            {
                TempData["ErrorTitle"] = "오류";
                TempData["ErrorMessage"] = "대회 정보를 찾을 수 없습니다.";
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
                // 남/여 분리
                var maleParticipants = participants.Where(p => p.User?.UserGender == 1).ToList();
                var femaleParticipants = participants.Where(p => p.User?.UserGender == 2).ToList();

                // 핸디캡 적용시 각각 정렬
                if (useHandicap)
                {
                    maleParticipants = maleParticipants.OrderByDescending(p => p.User?.Handicap?.AgeHandicap ?? 0).ToList();
                    femaleParticipants = femaleParticipants.OrderByDescending(p => p.User?.Handicap?.AgeHandicap ?? 0).ToList();
                }

                // 남자팀
                var maleTeams = maleParticipants
                    .Select((p, i) => new { p, idx = i / maxPerHole })
                    .GroupBy(x => x.idx)
                    .Select(g => g.Select(x => x.p).ToList())
                    .ToList();

                // 여자팀
                var femaleTeams = femaleParticipants
                    .Select((p, i) => new { p, idx = i / maxPerHole })
                    .GroupBy(x => x.idx)
                    .Select(g => g.Select(x => x.p).ToList())
                    .ToList();

                // 남자팀 먼저 배정, 남자팀 다 배정후 여자팀 배정 (코스/홀 순서대로)
                var allTeams = new List<(string gender, List<GameParticipant> team)>();
                allTeams.AddRange(maleTeams.Select(t => ("남", t)));
                allTeams.AddRange(femaleTeams.Select(t => ("여", t)));

                // 인원 많은 팀 우선 + 랜덤
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
                                Name = p.User?.UserName ?? p.UserId ?? "이름없음",
                                UserId = p.UserId ?? "",
                                GenderText = p.User?.UserGender == 1 ? "남" : "여",
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
                            UserName = p.User?.UserName ?? p.UserId ?? "이름없음",
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
                // 기존 혼합 자동 배정 로직(핸디캡 등 다른 옵션 적용)
                IEnumerable<GameParticipant> sortedParticipants = participants;
                if (useHandicap)
                    sortedParticipants = sortedParticipants.OrderByDescending(p => p.User?.Handicap?.AgeHandicap ?? 0);

                var teams = sortedParticipants
                    .Select((p, i) => new { p, idx = i / maxPerHole })
                    .GroupBy(x => x.idx)
                    .Select(g => g.Select(x => x.p).ToList())
                    .ToList();

                // 랜덤 섞기
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
                                Name = p.User?.UserName ?? p.UserId ?? "이름없음",
                                UserId = p.UserId ?? "",
                                GenderText = p.User?.UserGender == 1 ? "남" : "여",
                                // 아래 추가: 실제 값이 있으면 직접 채움
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
                            UserName = p.User?.UserName ?? p.UserId ?? "이름없음",
                            UserId = p.UserId ?? "",
                            GenderText = p.User?.UserGender == 1 ? "남" : "여",
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
            //값이 사라지는 문제로 세션으로 변경함
            HttpContext.Session.SetString("AssignmentResults", JsonConvert.SerializeObject(assignmentResults));
            HttpContext.Session.SetString("UnassignedParticipants", JsonConvert.SerializeObject(unassigned));

            HttpContext.Session.SetString("GenderSort", GenderSort);
            HttpContext.Session.SetString("Handicapped", Handicapped);
            HttpContext.Session.SetString("AgeSort", AgeSort);
            HttpContext.Session.SetString("AwardSort", AwardSort);

            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        // 나이 텍스트 변환 함수
        private string GetAgeGroupText(int age, int gender)
        {
            age = PersonInfoCalculator.CalculateAge(age, gender); // 성별 코드 1로 가정
            if (age < 40) return "청년";
            if (age < 60) return "중년";
            return "장년";
        }

        // 강제 배정 핸들러(최대 인원 초과 무시)
        public IActionResult OnPostForceAssignParticipant(string userId, string courseName, int holeNumber, string gameCode)
        {
            // 1. 미배정 인원 리스트 가져오기
            var unassignedJson = HttpContext.Session.GetString("UnassignedParticipants");
            var UnassignedParticipants = !string.IsNullOrEmpty(unassignedJson)
                ? JsonConvert.DeserializeObject<List<ParticipantViewModel>>(unassignedJson)
                : new List<ParticipantViewModel>();

            // 2. 강제 배정할 참가자 찾기
            var participant = (UnassignedParticipants ?? new List<ParticipantViewModel>())
                .FirstOrDefault(p => p.UserId == userId);
            if (participant == null)
            {
                TempData["ErrorMessage"] = "해당 참가자를 찾을 수 없습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            // 3. 기존 AssignmentResults 가져오기
            var resultsJson = HttpContext.Session.GetString("AssignmentResults");
            var AssignmentResults = !string.IsNullOrEmpty(resultsJson)
                ? JsonConvert.DeserializeObject<List<CourseAssignmentResultViewModel>>(resultsJson)
                : new List<CourseAssignmentResultViewModel>();

            // 4. 해당 코스/홀의 현재 배정 인원 수
            var assignedCount = (AssignmentResults ?? new List<CourseAssignmentResultViewModel>())
                .Count(a => a.CourseName == courseName && a.HoleNumber == holeNumber.ToString());

            // 5. 강제로 배정 (최대 인원 무시)
            var teamNumber = assignedCount + 1;
            AssignmentResults?.Add(new CourseAssignmentResultViewModel
            {
                CourseName = courseName,
                HoleNumber = holeNumber.ToString(),
                TeamNumber = (assignedCount + 1).ToString(),
                UserName = participant.Name,
                UserId = participant.UserId,
                GenderText = participant.GenderText,
                // 직접 계산해서 넣기 (없으면 기본값)
                AgeGroupText = participant.AgeGroupText ?? "확인불가",
                HandicapValue = participant.HandicapValue,
                AwardCount = participant.AwardCount
            });

            // 6. 미배정 인원에서 제거
            UnassignedParticipants?.Remove(participant);

            // 7. 다시 TempData에 저장
            //TempData["AssignmentResults"] = JsonConvert.SerializeObject(AssignmentResults);
            //TempData["UnassignedParticipants"] = JsonConvert.SerializeObject(UnassignedParticipants);
            HttpContext.Session.SetString("AssignmentResults", JsonConvert.SerializeObject(AssignmentResults));
            HttpContext.Session.SetString("UnassignedParticipants", JsonConvert.SerializeObject(UnassignedParticipants));
            TempData["SuccessMessage"] = $"강제로 {courseName} {holeNumber}홀에 배정되었습니다!";

            // 8. 결과 페이지 이동
            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }
    }
}