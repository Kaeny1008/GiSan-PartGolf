﻿using GisanParkGolf_Core.Data;
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
                // DB에서 불러오기
                var dbResults = _context.GameUserAssignments
                    .Include(a => a.Handicap)
                    .Include(a => a.User)
                    .Include(a => a.Game)
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
                    GenderText = r.User?.UserGender == 1 ? "남" : r.User?.UserGender == 2 ? "여" : "",
                    AgeGroupText = GetAgeGroupText(r.User?.UserNumber ?? 0, r.User?.UserGender ?? 0),
                    HandicapValue = r.Handicap?.AgeHandicap ?? 0,
                    AwardCount = awardDict.TryGetValue(r.UserId ?? "", out var cnt) ? cnt : 0,
                    GameName = r.Game?.GameName ?? "",
                    GameDate = r.Game?.GameDate.ToString("yyyy-MM-dd"),
                    StadiumName = r.Game?.StadiumName ?? ""
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
            await AssignTeamNumbers(results, gameCode); // 팀넘버 배정
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
                // CourseOrder 기준 정렬!
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
            // 참가자, 통계
            Participants = await _gameService.GetParticipantsAsync(
                gameCode, ParticipantSearchQuery, ParticipantPageIndex, ParticipantPageSize);

            // 참가자수 변수 저장
            JoinedCount = await _context.GameParticipants.CountAsync(gp => !gp.IsCancelled);

            var query = _context.GameParticipants.Include(gp => gp.Game).AsQueryable();
            TotalCount = await query.CountAsync();
            CancelledCount = await _context.GameParticipants.CountAsync(gp => gp.IsCancelled);
            JoinedCount = await _context.GameParticipants.CountAsync(gp => !gp.IsCancelled);

            // 배치 옵션 세션 -> 변수
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

                // 옵션값 없으면 DB설정 반영
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
                        catch { /* 무시 */ }
                    }
                }
            }

            // 코스배치 결과(세션/DB)
            AssignmentResults = GetAssignmentResults(gameCode);

            // 검색/페이징
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

        // ------------------- 핸들러 (POST) -------------------
        public async Task<IActionResult> OnPostCancelAssignmentAsync(string gameCode, string userId, string? cancelReason)
        {
            if (string.IsNullOrWhiteSpace(cancelReason))
            {
                TempData["ErrorMessage"] = "취소 사유를 반드시 입력해야 합니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            // 1. 세션 데이터에서 배치 삭제
            var assignmentResults = GetAssignmentResults(gameCode);
            assignmentResults.RemoveAll(r => r.UserId == userId);
            RenumberCourseOrders(assignmentResults);

            // 2. DB에서 코스배치 기록 삭제
            var userAssignments = await _context.GameUserAssignments
                .Where(a => a.GameCode == gameCode && a.UserId == userId)
                .ToListAsync();
            if (userAssignments.Any())
            {
                _context.GameUserAssignments.RemoveRange(userAssignments);
                await _context.SaveChangesAsync();
            }

            // 3. 참가자 상태 취소 처리 (참가취소)
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

            // 4. 세션 업데이트
            SaveAssignmentResults(assignmentResults);
            SaveUnassignedParticipants(GetUnassignedParticipants() ?? new List<ParticipantViewModel>());

            TempData["SuccessMessage"] = "참가자가 취소 처리되었습니다. (코스배치 및 참가 취소가 DB에 즉시 반영)";
            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        public IActionResult OnPostForceAssignParticipant(string userId, string courseName, int holeNumber, string gameCode)
        {
            var unassigned = GetUnassignedParticipants();
            var participant = unassigned.FirstOrDefault(p => p.UserId == userId);
            if (participant == null)
            {
                TempData["ErrorMessage"] = "해당 참가자를 찾을 수 없습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            var assignmentResults = GetAssignmentResults(gameCode);

            // 기존 해당 코스/홀 참가자 순서 보존
            var sameHoleList = assignmentResults
                .Where(r => r.CourseName == courseName && r.HoleNumber == holeNumber.ToString())
                .OrderBy(r => r.CourseOrder)
                .ToList();

            assignmentResults.RemoveAll(r => r.CourseName == courseName && r.HoleNumber == holeNumber.ToString());

            // 기존 참가자 중 가장 큰 CourseOrder 값
            int maxCourseOrder = sameHoleList.Count > 0 ? sameHoleList.Max(x => x.CourseOrder) : 0;

            // 강제배정자를 "마지막 순번"으로 추가
            sameHoleList.Add(new CourseAssignmentResultViewModel
            {
                CourseName = courseName,
                HoleNumber = holeNumber.ToString(),
                CourseOrder = maxCourseOrder + 1, // 바로 여기!
                UserName = participant.Name,
                UserId = participant.UserId,
                GenderText = participant.GenderText,
                AgeGroupText = participant.AgeGroupText ?? "확인필요",
                HandicapValue = participant.HandicapValue,
                AwardCount = participant.AwardCount
            });

            assignmentResults.AddRange(sameHoleList);
            unassigned.Remove(participant);

            // 순번 재정렬(코스/홀별로 1부터)
            RenumberCourseOrders(assignmentResults);

            // 코스명>홀번호>순번대로 정렬
            assignmentResults = assignmentResults
                .OrderBy(r => r.CourseName)
                .ThenBy(r => int.TryParse(r.HoleNumber, out var hn) ? hn : 0)
                .ThenBy(r => r.CourseOrder)
                .ToList();

            // 팀넘버 할당(코스/홀별로 하나, 초과허용)
            AssignTeamNumbersAllowOverflow(assignmentResults);

            SaveAssignmentResults(assignmentResults);
            SaveUnassignedParticipants(unassigned);

            TempData["SuccessMessage"] = $"강제배정자가 {courseName} {holeNumber}홀의 마지막 순번에 배정되었습니다!";
            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        // 팀넘버 할당(코스/홀별로 하나, 초과허용)
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

            // 1. 배치옵션 저장 (게임 설정 저장)
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

            // 2. 기존 DB 배정정보 불러오기
            var prevAssignments = await _context.GameUserAssignments
                .Where(a => a.GameCode == gameCode)
                .ToListAsync();

            // 3. 세션에 남아있는 배정정보와 비교하여 취소자 찾기
            var assignedUserIds = assignmentResults.Select(r => r.UserId).ToHashSet();
            var cancelledUsers = prevAssignments
                .Where(pa => !assignedUserIds.Contains(pa.UserId))
                .Select(pa => pa.UserId)
                .Distinct()
                .ToList();

            // 4. 취소 이력 남기기
            foreach (var userId in cancelledUsers)
            {
                var participant = await _context.GameParticipants
                    .FirstOrDefaultAsync(p => p.GameCode == gameCode && p.UserId == userId);

                if (participant != null)
                {
                    var cancelReason = HttpContext.Session.GetString($"CancelReason_{userId}") ?? "코스 배치 취소";
                    participant.IsCancelled = true;
                    participant.CancelDate = DateTime.Now;
                    participant.CancelReason = !string.IsNullOrWhiteSpace(cancelReason) ? cancelReason : "코스 배치 취소";
                    participant.Approval = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";
                    _context.GameParticipants.Update(participant);
                }
            }
            await _context.SaveChangesAsync();

            // 5. 기존 배정 삭제/새 배정 저장
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

            TempData["SuccessMessage"] = $"코스배치 결과가 정상적으로 저장되었습니다.";
            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        // --- 참가취소, 재참가 등 기존 기능은 그대로 ---
        public async Task<IActionResult> OnPostApproveCancelAsync(string id)
        {
            var participant = await _context.GameParticipants.FirstOrDefaultAsync(p => p.UserId == id);
            if (participant == null)
            {
                TempData["ErrorTitle"] = "오류";
                TempData["ErrorMessage"] = "참가자를 찾을 수 없습니다.";
                return RedirectToPage();
            }
            participant.Approval = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";
            
            _context.GameParticipants.Update(participant);
            await _context.SaveChangesAsync();
            ClearAssignmentSession();

            TempData["SuccessTitle"] = "취소 승인 완료";
            TempData["SuccessMessage"] = "참가자 취소가 승인되었습니다. 코스배치 재실행 필요.";
            return RedirectToPage(new { gameCode = participant.GameCode });
        }

        public async Task<IActionResult> OnPostRejoinAsync(string id)
        {
            var participant = await _context.GameParticipants.FirstOrDefaultAsync(p => p.UserId == id);
            if (participant == null)
            {
                TempData["ErrorTitle"] = "오류";
                TempData["ErrorMessage"] = "참가자를 찾을 수 없습니다.";
                return RedirectToPage();
            }
            participant.IsCancelled = false;
            participant.CancelDate = null;
            participant.CancelReason = null;
            participant.Approval = null;

            _context.GameParticipants.Update(participant);
            await _context.SaveChangesAsync();

            TempData["SuccessTitle"] = "재참가 완료";
            TempData["SuccessMessage"] = "재참가 처리되었습니다.\n반드시 코스배치를 다시 실행해야 참가자가 배정됩니다.";
            return RedirectToPage(new { gameCode = participant.GameCode, tab = "tab-course" });
        }

        // ------------------- 코스배치 옵션/실행 부분 -------------------
        public async Task<IActionResult> OnPostCourseAssignmentAsync(string gameCode)
        {
            int maxPerHole = 4; // default

            // game 먼저 조회
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
            {
                TempData["ErrorTitle"] = "오류";
                TempData["ErrorMessage"] = "대회 정보를 찾을 수 없습니다.";
                return Page();
            }

            // game이 null 아님이 확정된 뒤 설정 읽기
            if (!string.IsNullOrEmpty(game.GameSetting))
            {
                var setting = JsonConvert.DeserializeObject<dynamic>(game.GameSetting);
                maxPerHole = setting?.MaxPerHole ?? 4;
            }

            // User는 항상 존재한다고 전제 → null-억제 연산자 사용
            var participants = await _context.GameParticipants
                .Where(p => p.GameCode == gameCode && !p.IsCancelled)
                .Include(p => p.User!)
                .ThenInclude(u => u.Handicap)
                .ToListAsync();

            var userIds = participants
                .Select(p => p.UserId ?? "")
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            var awardDict = await _context.GameAwardHistories
                .Where(a => !string.IsNullOrEmpty(a.UserId) && userIds.Contains(a.UserId))
                .GroupBy(a => a.UserId)
                .ToDictionaryAsync(g => g.Key ?? "", g => g.Count());

            bool useGenderSort = GenderSort == "true";
            bool useHandicap = Handicapped == "true";

            // 여기서 game.StadiumCode 안전하게 접근 가능
            var courses = await _context.Courses
                .Where(c => c.StadiumCode == game.StadiumCode)
                .ToListAsync();

            var assignmentResults = new List<CourseAssignmentResultViewModel>();
            var unassigned = new List<ParticipantViewModel>();

            // --- 기존 배정 로직 그대로 ---
            if (useGenderSort)
            {
                var maleParticipants = participants.Where(p => p.User?.UserGender == 1).ToList();
                var femaleParticipants = participants.Where(p => p.User?.UserGender == 2).ToList();

                if (useHandicap)
                {
                    maleParticipants = maleParticipants
                        .OrderByDescending(p => p.User?.Handicap?.AgeHandicap ?? 0).ToList();
                    femaleParticipants = femaleParticipants
                        .OrderByDescending(p => p.User?.Handicap?.AgeHandicap ?? 0).ToList();
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
                allTeams.AddRange(maleTeams.Select(t => ("남", t)));
                allTeams.AddRange(femaleTeams.Select(t => ("여", t)));

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
                IEnumerable<GameParticipant> sortedParticipants = participants;
                if (useHandicap)
                {
                    sortedParticipants = sortedParticipants
                        .OrderByDescending(p => p.User?.Handicap?.AgeHandicap ?? 0);
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

                    for (int i = 0; i < t.Count; i++)
                    {
                        var p = t[i];
                        assignmentResults.Add(new CourseAssignmentResultViewModel
                        {
                            CourseName = course.CourseName,
                            HoleNumber = holeIdx.ToString(),
                            CourseOrder = i + 1,
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

            HttpContext.Session.SetString("AssignmentResults", JsonConvert.SerializeObject(assignmentResults));
            HttpContext.Session.SetString("UnassignedParticipants", JsonConvert.SerializeObject(unassigned));
            HttpContext.Session.SetString("GenderSort", GenderSort ?? "false");
            HttpContext.Session.SetString("Handicapped", Handicapped ?? "false");
            HttpContext.Session.SetString("AgeSort", AgeSort ?? "false");
            HttpContext.Session.SetString("AwardSort", AwardSort ?? "false");

            // === 팀넘버 배정 ===
            await AssignTeamNumbers(assignmentResults, gameCode);

            // 다시 저장 (혹은 위에 배정 직전에 해도 됨)
            HttpContext.Session.SetString("AssignmentResults", JsonConvert.SerializeObject(assignmentResults));

            return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
        }

        private async Task AssignTeamNumbers(List<CourseAssignmentResultViewModel> assignmentResults, string gameCode)
        {
            int maxPerHole = 4; // default
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (!string.IsNullOrEmpty(game?.GameSetting))
            {
                var setting = JsonConvert.DeserializeObject<dynamic>(game.GameSetting);
                maxPerHole = setting?.MaxPerHole ?? 4;
            }

            // 코스/홀별 그룹핑 (각 그룹이 하나의 팀)
            var allTeams = assignmentResults
                .GroupBy(r => new { r.CourseName, r.HoleNumber })
                .OrderBy(g => g.Key.CourseName)
                .ThenBy(g => int.TryParse(g.Key.HoleNumber, out var hn) ? hn : 0)
                .ToList();

            int teamSeq = 1;
            foreach (var group in allTeams)
            {
                string currentTeamNumber = $"T{teamSeq:D2}";
                int memberCount = 1;
                foreach (var item in group.OrderBy(r => r.CourseOrder))
                {
                    item.TeamNumber = currentTeamNumber;
                    item.CourseOrder = memberCount;
                    memberCount++;
                }
                teamSeq++;
            }
        }

        // ------------------- 기타 유틸/헬퍼 -------------------
        private string GetAgeGroupText(int age, int gender)
        {
            age = PersonInfoCalculator.CalculateAge(age, gender);
            if (age < 40) return "청년";
            if (age < 60) return "중년";
            return "장년";
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

        public IActionResult OnGetExportPdfAsync(string gameCode)
        {
            // 1. 결과 데이터 준비
            var results = GetAssignmentResults(gameCode);
            if (results == null || results.Count == 0)
            {
                TempData["ErrorMessage"] = "배치 결과 데이터가 없습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            string gameName = results.FirstOrDefault()?.GameName ?? "게임명";
            string gameDate = results.FirstOrDefault()?.GameDate ?? "게임일자";
            string stadiumName = results.FirstOrDefault()?.StadiumName ?? "경기장명";

            // 2. PDF 파일 메모리 스트림 생성
            using var ms = new MemoryStream();
            CourseAssignmentPdfGenerator.GeneratePdf(
                results.Select(r => new CourseAssignmentRow
                {
                    GameName = gameName,
                    GameDate = gameDate,
                    StadiumName = stadiumName,
                    CourseName = r.CourseName ?? "",
                    HoleNumber = r.HoleNumber ?? "",
                    CourseOrder = r.CourseOrder.ToString(),
                    TeamNumber = r.TeamNumber ?? "",
                    ParticipantName = r.UserName ?? "",
                    ParticipantID = r.UserId ?? "",
                    Gender = r.GenderText ?? "",
                    Note = ""
                }).ToList(), ms);

            ms.Position = 0;

            // 3. PDF 파일 직접 다운로드
            var fileName = $"코스배치표_{gameCode}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(ms.ToArray(), "application/pdf", fileName);
        }

        public IActionResult OnGetExportScorecardPdf(string gameCode)
        {
            // 0. 할당 결과 읽기
            var results = GetAssignmentResults(gameCode);
            if (results == null || results.Count == 0)
            {
                TempData["ErrorMessage"] = "게임 할당 결과가 없습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            // 1. 게임/대회 정보 (PDF 상단)
            var game = _context.Games.FirstOrDefault(g => g.GameCode == gameCode);
            string gameName = results.FirstOrDefault()?.GameName ?? (game?.GameName ?? "경기명");
            string gameDate = results.FirstOrDefault()?.GameDate ?? (game?.GameDate.ToString("yyyy-MM-dd") ?? "날짜없음");
            string stadiumName = results.FirstOrDefault()?.StadiumName ?? (game?.StadiumName ?? "경기장");

            // 유틸: 코스명 정규화 (공백제거, 소문자, '코스' 접미사 제거)
            static string NormalizeCourseName(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return "";
                var t = s.Trim();
                t = t.Replace("코스", "", StringComparison.OrdinalIgnoreCase).Trim();
                return t.ToLowerInvariant();
            }

            // DB에서 해당 경기장의 코스들을 가능한 한 읽어온다
            var dbCourses = new List<(string CourseName, int HoleCount)>();
            if (game != null)
            {
                dbCourses = _context.Courses
                    .Where(c => c.StadiumCode == game.StadiumCode)
                    .Select(c => new { c.CourseName, c.HoleCount })
                    .AsEnumerable()
                    .Select(x => ((x.CourseName ?? "").Trim(), x.HoleCount))
                    .ToList();
            }

            // assignment에서 관측된 최대 홀 (fallback 기준)
            int maxHoleFromResults = results
                .Select(r => int.TryParse(r.HoleNumber, out var n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();

            // 결과에 등장하는 코스명 목록 (원본 문자열)
            var resultCourseNames = results
                .Select(r => (r.CourseName ?? "").Trim())
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // courseMap: 정규화된 키 -> (표시명, holeCount)
            var courseMap = new Dictionary<string, (string DisplayName, int HoleCount)>(StringComparer.OrdinalIgnoreCase);

            // 1) DB 코스 우선으로 채우기 (정규화 키 사용)
            foreach (var dc in dbCourses)
            {
                var name = dc.CourseName;
                var key = NormalizeCourseName(name);
                if (!courseMap.ContainsKey(key))
                {
                    courseMap[key] = (name, Math.Max(1, dc.HoleCount));
                }
            }

            // 2) 결과에만 있는 코스는 DB에서 비슷한 이름이 있는지 시도해 보고, 없으면 안전한 기본값 사용
            foreach (var rc in resultCourseNames)
            {
                var key = NormalizeCourseName(rc);
                if (courseMap.ContainsKey(key)) continue;

                // DB에서 정규화된 이름으로 탐색(공백/접미사 차이 보정)
                var matchedDb = dbCourses
                    .FirstOrDefault(dc => NormalizeCourseName(dc.CourseName) == key);

                int holeCount;
                string displayName;
                if (!string.IsNullOrEmpty(matchedDb.CourseName))
                {
                    displayName = matchedDb.CourseName;
                    holeCount = Math.Max(1, matchedDb.HoleCount);
                }
                else
                {
                    // 추가 탐색: DB의 CourseName에 결과 이름이 포함되는 경우(예: "A 코스" vs "A")
                    var containsMatch = dbCourses
                        .FirstOrDefault(dc => (dc.CourseName ?? "").IndexOf(rc, StringComparison.OrdinalIgnoreCase) >= 0);

                    if (!string.IsNullOrEmpty(containsMatch.CourseName))
                    {
                        displayName = containsMatch.CourseName;
                        holeCount = Math.Max(1, containsMatch.HoleCount);
                    }
                    else
                    {
                        // 최종 fallback: 결과에서 관측된 최대 홀 수, 없으면 안전 기본값 9
                        displayName = rc;
                        holeCount = Math.Max(1, Math.Max(maxHoleFromResults, 9));
                    }
                }

                courseMap[key] = (displayName, holeCount);
            }

            // 3) courseMap -> CourseScoreCardData 생성 (정규화된 키로 players 매핑)
            var courses = courseMap.Select(kvp =>
            {
                var normalizedKey = kvp.Key;
                var displayName = kvp.Value.DisplayName;
                var holeCount = Math.Max(1, kvp.Value.HoleCount);

                var holeNumbers = Enumerable.Range(1, holeCount).Select(n => n.ToString()).ToList();

                var players = results
                    .Where(r => !string.IsNullOrEmpty(r.CourseName) &&
                                NormalizeCourseName(r.CourseName) == normalizedKey)
                    .Where(r => !string.IsNullOrEmpty(r.UserId))
                    .GroupBy(r => r.UserId)
                    .Select(g =>
                    {
                        var first = g.First();
                        return new ScoreCardRow
                        {
                            PlayerName = first.UserName ?? "",
                            PlayerID = g.Key ?? "",
                            TeamNumber = first.TeamNumber ?? ""
                        };
                    })
                    .OrderBy(p => p.TeamNumber)
                    .ThenBy(p => p.PlayerName)
                    .ToList();

                return new CourseScoreCardData
                {
                    CourseName = displayName,
                    HoleNumbers = holeNumbers,
                    Players = players
                };
            })
            .OrderBy(c => c.CourseName)
            .ToList();

            // 4) PDF 생성
            using var ms = new MemoryStream();
            ScoreCardPdfGenerator.GeneratePdf(
                gameName,
                gameDate,
                stadiumName,
                courses,
                ms);

            ms.Position = 0;
            var fileName = $"Scorecard_{gameCode}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(ms.ToArray(), "application/pdf", fileName);
        }
    }
}