using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.Pages.AdminPage.AdminPage;
using GisanParkGolf_Core.Pages.AdminPage.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "AdminOnly")]
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
        public bool AssignmentLocked { get; set; } = false;

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
                AssignmentLocked = game.AssignmentLocked;

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
            var reject = await RejectIfAssignmentLockedAsync(gameCode);
            if (reject != null) return reject;

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

        public async Task<IActionResult> OnPostForceAssignParticipant(string userId, string courseName, int holeNumber, string gameCode)
        {
            // 바로 Reject 검사(중복 호출 가능하므로 내부 async 메서드에서만 검사하도록 정리하면 중복 제거)
            var reject = await RejectIfAssignmentLockedAsync(gameCode);
            if (reject != null) return reject;

            // 기존 OnPostForceAssignParticipantAsync 로직을 이 메서드로 옮기거나
            // 기존 async 메서드를 호출:
            return await OnPostForceAssignParticipantAsync(userId, courseName, holeNumber, gameCode);
        }

        public async Task<IActionResult> OnPostForceAssignParticipantAsync(string userId, string courseName, int holeNumber, string gameCode)
        {
            var reject = await RejectIfAssignmentLockedAsync(gameCode);
            if (reject != null) return reject;

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
            // 0. 기본 유효성 검사
            var assignmentResults = GetAssignmentResults(gameCode);
            if (assignmentResults == null || assignmentResults.Count == 0)
            {
                TempData["ErrorMessage"] = "저장할 배치 결과가 없습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            // 0.5 추가 검사: userId 없는 레코드가 있는지 확인
            var invalidRows = assignmentResults.Where(r => string.IsNullOrWhiteSpace(r.UserId)).ToList();
            if (invalidRows.Any())
            {
                TempData["ErrorMessage"] = $"저장 실패: UserId가 누락된 배치 항목이 {invalidRows.Count}건 있습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            // 서버측 잠금 검사 (추가 안전장치)
            var gameForCheck = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (gameForCheck == null)
            {
                TempData["ErrorMessage"] = "대회 정보를 찾을 수 없습니다.";
                return RedirectToPage(new { gameCode = gameCode });
            }
            if (gameForCheck.AssignmentLocked)
            {
                TempData["ErrorMessage"] = "이미 확정된 배치입니다. 변경할 수 없습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            // 1. 게임 설정 JSON 준비
            var gameSettingObj = new
            {
                GenderSort = this.GenderSort ?? "false",
                Handicapped = this.Handicapped ?? "false",
                AgeSort = this.AgeSort ?? "false",
                AwardSort = this.AwardSort ?? "false"
            };
            var settingJson = JsonConvert.SerializeObject(gameSettingObj);

            // 2. 트랜잭션 시작 (원자성 보장)
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // 3. 게임 엔티티 업데이트(상태, 세팅, 잠금)
                var game = gameForCheck; // 이미 로드됨
                game.GameSetting = settingJson;
                game.GameStatus = "Assigned";
                game.AssignmentLocked = true; // 자동 확정 정책일 경우
                _context.Games.Update(game);
                await _context.SaveChangesAsync();

                // 4. 이전 배정 로드 (DB)
                var prevAssignments = await _context.GameUserAssignments
                    .Where(a => a.GameCode == gameCode)
                    .ToListAsync();

                // 5. 세션 결과와 비교하여 취소 대상 찾기
                var assignedUserIds = assignmentResults.Select(r => r.UserId).ToHashSet();
                var cancelledUsers = prevAssignments
                    .Where(pa => !assignedUserIds.Contains(pa.UserId))
                    .Select(pa => pa.UserId)
                    .Distinct()
                    .ToList();

                // 6. 참가자(취소) 업데이트
                if (cancelledUsers.Any())
                {
                    var participantsToCancel = await _context.GameParticipants
                        .Where(p => p.GameCode == gameCode && cancelledUsers.Contains(p.UserId))
                        .ToListAsync();

                    foreach (var participant in participantsToCancel)
                    {
                        var cancelReason = HttpContext.Session.GetString($"CancelReason_{participant.UserId}") ?? "코스 배치 취소";
                        participant.IsCancelled = true;
                        participant.CancelDate = DateTime.Now;
                        participant.CancelReason = !string.IsNullOrWhiteSpace(cancelReason) ? cancelReason : "코스 배치 취소";
                        participant.Approval = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";
                        _context.GameParticipants.Update(participant);
                    }
                    await _context.SaveChangesAsync();
                }

                // 7. 기존 배정 삭제
                if (prevAssignments.Any())
                {
                    _context.GameUserAssignments.RemoveRange(prevAssignments);
                    await _context.SaveChangesAsync();
                }

                // 8. 새 배정 레코드 준비 및 대량 삽입
                var newAssignments = assignmentResults.Select(r => new GameUserAssignment
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
                }).ToList();

                if (newAssignments.Any())
                {
                    await _context.GameUserAssignments.AddRangeAsync(newAssignments);
                    await _context.SaveChangesAsync();
                }

                // 9. 히스토리 기록 (요약)
                var summary = new
                {
                    SavedBy = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin",
                    SavedAt = DateTime.Now,
                    TotalAssigned = newAssignments.Count,
                    UnassignedCount = GetUnassignedParticipants()?.Count ?? 0,
                    RemovedUserIds = cancelledUsers
                };
                await AddAssignmentHistoryAsync(gameCode, "SaveAndFinalize", summary);

                // 10. 커밋
                await tx.CommitAsync();

                TempData["SuccessMessage"] = "코스배치 결과가 정상적으로 저장되었습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }
            catch (Exception ex)
            {
                // 롤백 후 로깅/유저 메시지
                await tx.RollbackAsync();
                // TODO: 실제 앱에서는 ILogger로 예외 로깅
                TempData["ErrorMessage"] = "코스배치 저장 중 오류가 발생했습니다. 관리자에게 문의하세요.\n" + ex.Message;
                // 예외를 던지거나 상세 메시지를 로그에 남기도록 처리
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }
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
            // 잠금 검사 — 변경 금지 시 즉시 리다이렉트
            var reject = await RejectIfAssignmentLockedAsync(gameCode);
            if (reject != null) return reject;

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
            // 1. 결과 데이터 준비 (세션/DB)
            var results = GetAssignmentResults(gameCode);
            if (results == null || results.Count == 0)
            {
                TempData["ErrorMessage"] = "배치 결과 데이터가 없습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            // 우선적으로 DB의 Game 엔티티에서 정보를 가져오고, 없으면 results에서 보완
            var game = _context.Games.FirstOrDefault(g => g.GameCode == gameCode);
            string gameName = game?.GameName ?? results.FirstOrDefault()?.GameName ?? "게임명";
            string gameDate = game?.GameDate.ToString("yyyy-MM-dd") ?? results.FirstOrDefault()?.GameDate ?? "게임일자";
            string stadiumName = game?.StadiumName ?? results.FirstOrDefault()?.StadiumName ?? "경기장명";

            // 안전한 방식: AssignedDate가 non-nullable이더라도 레코드 자체가 없을 수 있으니 null 체크
            // 방법 선택: 여기서는 nullable 프로젝션(옵션 B)을 사용
            var latestAssignedDate = _context.GameUserAssignments
                .Where(a => a.GameCode == gameCode)
                .OrderByDescending(a => a.AssignedDate)
                .Select(a => (DateTime?)a.AssignedDate) // nullable로 변환
                .FirstOrDefault();

            string assignmentCompletedAt = latestAssignedDate.HasValue
                ? latestAssignedDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                : "미등록";

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
                }).ToList(), ms, assignmentCompletedAt);

            ms.Position = 0;

            // 3. PDF 파일 직접 다운로드
            var fileName = $"코스배치표_{gameCode}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(ms.ToArray(), "application/pdf", fileName);
        }

        public IActionResult OnGetExportScorecardPdf(string gameCode)
        {
            // 0. 할당 결과 읽기 (세션/DB)
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

            // --- 새로 추가: 코스배치 완료 시간 조회 (GameUserAssignments.AssignedDate 최신값)
            var latestAssignedDate = _context.GameUserAssignments
                .Where(a => a.GameCode == gameCode)
                .OrderByDescending(a => a.AssignedDate)
                .Select(a => (DateTime?)a.AssignedDate) // nullable로 안전하게 프로젝션
                .FirstOrDefault();

            string assignmentCompletedAt = latestAssignedDate.HasValue
                ? latestAssignedDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                : "미등록";

            // DB에서 코스 정의(코스명 + 홀수) 가져오기
            var courseDefs = new List<(string CourseName, int HoleCount)>();
            if (game != null)
            {
                courseDefs = _context.Courses
                    .Where(c => c.StadiumCode == game.StadiumCode)
                    .Select(c => new { c.CourseName, c.HoleCount })
                    .AsEnumerable()
                    .Select(x => ((x.CourseName ?? "").Trim(), x.HoleCount))
                    .ToList();
            }
            else
            {
                // 안전하게 결과에서 발견된 코스명으로 채움(홀수는 관측 또는 기본 9)
                var observedMaxHole = results.Select(r => int.TryParse(r.HoleNumber, out var n) ? n : 0).DefaultIfEmpty(0).Max();
                var distinctCourses = results.Select(r => (r.CourseName ?? "").Trim()).Distinct().ToList();
                foreach (var cn in distinctCourses)
                {
                    courseDefs.Add((cn, observedMaxHole > 0 ? observedMaxHole : 9));
                }
            }

            // 팀별 출력: 각 팀당 한 페이지, 각 페이지에 모든 코스(코스별 표)를 넣음.
            // 1) 팀 목록(TeamNumber 기준 그룹화)
            var teamsGrouped = results
                .Where(r => !string.IsNullOrEmpty(r.TeamNumber))
                .GroupBy(r => r.TeamNumber)
                .OrderBy(g => g.Key)
                .ToList();

            // 2) 각 팀의 플레이어 목록(중복 제거, UserId 기준)
            var teams = new List<TeamScoreCardData>();
            foreach (var g in teamsGrouped)
            {
                var players = g
                    .GroupBy(r => r.UserId)
                    .Select(u =>
                    {
                        var first = u.First();
                        return new ScoreCardRow
                        {
                            PlayerName = first.UserName ?? "",
                            PlayerID = u.Key ?? "",
                            TeamNumber = first.TeamNumber ?? "",
                            CourseOrder = first.CourseOrder > 0 ? first.CourseOrder.ToString() : "" // CourseOrder 채움
                        };
                    })
                    .OrderBy(p =>
                    {
                        // 기본 정렬: 코스순번 있으면 숫자 기준, 없으면 이름
                        if (int.TryParse(p.CourseOrder, out var ord)) return ord;
                        return int.MaxValue;
                    })
                    .ThenBy(p => p.PlayerName)
                    .ToList();

                teams.Add(new TeamScoreCardData { TeamNumber = g.Key ?? "", Players = players });
            }

            // 3) coursesTemplate: 각 코스의 HoleNumbers 정보만 전달 (Players는 팀별로 채워짐)
            var coursesTemplate = courseDefs.Select(cd => new CourseScoreCardData
            {
                CourseName = cd.CourseName,
                HoleNumbers = Enumerable.Range(1, cd.HoleCount).Select(i => i.ToString()).ToList()
            }).ToList();

            using var ms = new MemoryStream();
            // assignmentCompletedAt을 전달
            ScoreCardPdfGenerator.GeneratePdfByTeamLandscape(gameName, gameDate, stadiumName, coursesTemplate, teams, ms, assignmentCompletedAt);
            ms.Position = 0;
            var fileName = $"Scorecard_{gameCode}_ByTeam_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(ms.ToArray(), "application/pdf", fileName);
        }

        private async Task AddAssignmentHistoryAsync(string gameCode, string changeType, object detailsObj)
        {
            var jsonDetails = JsonConvert.SerializeObject(detailsObj ?? new { });
            var history = new GameAssignmentHistory
            {
                GameCode = gameCode,
                ChangedBy = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin",
                ChangeType = changeType,
                Details = jsonDetails,
                ChangedAt = DateTime.Now
            };
            _context.Add(history);
            await _context.SaveChangesAsync();
        }

        private async Task<IActionResult?> RejectIfAssignmentLockedAsync(string gameCode)
        {
            if (string.IsNullOrEmpty(gameCode)) return null;

            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game != null && game.AssignmentLocked)
            {
                TempData["ErrorMessage"] = "코스배치는 확정되어 변경할 수 없습니다. 우선 잠금을 해제하세요.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            return null;
        }

        public async Task<IActionResult> OnPostUnlockAssignmentAsync(string gameCode)
        {
            if (string.IsNullOrEmpty(gameCode))
            {
                TempData["ErrorMessage"] = "잘못된 요청입니다. gameCode가 없습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            // (선택) 정책 기반 접근은 클래스 레벨 [Authorize(Policy = "AdminOnly")]에 의해 이미 적용됨.
            // 추가 안전장치: Claim 기반 체크도 가능
            if (!(User.IsInRole("Admin") || User.HasClaim("IsAdmin", "true")))
            {
                TempData["ErrorMessage"] = "잠금 해제 권한이 없습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameCode == gameCode);
            if (game == null)
            {
                TempData["ErrorMessage"] = "해당 대회를 찾을 수 없습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            if (!game.AssignmentLocked)
            {
                TempData["InfoMessage"] = "이미 잠금이 해제되어 있습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }

            // 명시적으로 속성만 수정 표시
            game.AssignmentLocked = false;
            _context.Entry(game).Property(g => g.AssignmentLocked).IsModified = true;

            try
            {
                var affected = await _context.SaveChangesAsync();
                // 로깅/디버그용: affected는 변경된 row 수
                if (affected <= 0)
                {
                    TempData["ErrorMessage"] = "잠금 해제 시 데이터베이스에 적용되지 않았습니다(영향 행 수 0). 관리자에게 문의하세요.";
                    return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
                }

                var summary = new
                {
                    UnlockedBy = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin",
                    UnlockedAt = DateTime.Now
                };
                await AddAssignmentHistoryAsync(gameCode, "Unlock", summary);

                TempData["SuccessMessage"] = "코스배치 잠금이 해제되었습니다.";
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // 동시성 문제 처리
                TempData["ErrorMessage"] = "잠금 해제 중 동시성 충돌이 발생했습니다. 다시 시도하세요.\n" + ex.Message;
                // TODO: ILogger로 ex 상세 로깅
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }
            catch (Exception ex)
            {
                // TODO: ILogger로 ex 상세 로깅
                TempData["ErrorMessage"] = "잠금 해제 중 오류가 발생했습니다. 관리자에게 문의하세요.\n" + ex.Message;
                return RedirectToPage(new { gameCode = gameCode, tab = "tab-result" });
            }
        }
    }
}