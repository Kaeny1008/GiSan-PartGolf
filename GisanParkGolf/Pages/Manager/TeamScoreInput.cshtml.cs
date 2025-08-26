using GisanParkGolf.Pages.Manager.Services;
using GisanParkGolf.Pages.Manager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GiSanParkGolf.Pages.Manager
{
    [Authorize(Policy = "ManagerOnly")]
    public class TeamScoreInputModel : PageModel
    {
        private readonly ITeamScoreInputService _teamScoreInpuService;

        public TeamScoreInputModel(ITeamScoreInputService teamScoreinputService)
        {
            _teamScoreInpuService = teamScoreinputService;
        }

        [BindProperty] public List<TeamScoreCourseViewModel> TeamScoreCourses { get; set; } = new();
        [BindProperty] public string ScannedCode { get; set; } = "";

        public string? ScanMessage { get; set; }
        public bool ScanFound { get; set; } = false;
        [BindProperty(SupportsGet = true)] public string? TeamNumber { get; set; }
        [BindProperty(SupportsGet = true)] public string? GameCode { get; set; }
        [BindProperty(SupportsGet = true)] public string? GameName { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPostScan()
        {
            if (string.IsNullOrWhiteSpace(ScannedCode) || !ScannedCode.Contains('-'))
            {
                TempData["ErrorMessage"] = "바코드 형식이 올바르지 않습니다.";
                ScanFound = false;
                return Page();
            }

            var parts = ScannedCode.Split('-', 2); // "gamecode-teamnumber"
            if (parts.Length < 2)
            {
                TempData["ErrorMessage"] = "바코드 형식이 올바르지 않습니다.";
                ScanFound = false;
                return Page();
            }

            var gameCode = parts[0];
            var teamNumber = parts[1];

            // 서비스에서 데이터 조회
            var teamScoreCourses = _teamScoreInpuService.GetTeamScoreCourses(gameCode, teamNumber);

            if (teamScoreCourses == null || teamScoreCourses.Count == 0)
            {
                TempData["ErrorMessage"] = "해당 정보를 찾을 수 없습니다.";
                ScanFound = false;
                TeamScoreCourses = new();
            }
            else
            {
                //TempData["SuccessMessage"] = $"게임코드: {gameCode}, 팀번호: {teamNumber} 데이터 불러옴";
                GameCode = gameCode;
                GameName = teamScoreCourses.First().GameInformations?.GameName;
                TeamNumber = teamNumber;
                ScanFound = true;
                TeamScoreCourses = teamScoreCourses;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (GameCode == null || TeamNumber == null)
            {
                TempData["ErrorMessage"] = "저장할 데이터가 없습니다. 먼저 스캔해 주세요.";
                return RedirectToPage();
            }

            var form = Request.Form;
            var inputBy = User.Identity?.Name ?? "admin";

            // 점수 데이터 Dictionary 생성: key = "courseCode_userId_holeId", value = 점수
            var scores = new Dictionary<string, int>();

            foreach (var key in form.Keys)
            {
                if (key.StartsWith("Score_"))
                {
                    var parts = key.Split('_');
                    if (parts.Length == 4)
                    {
                        // parts[1] = courseCode, parts[2] = userId, parts[3] = holeId
                        var courseCode = parts[1];
                        var userId = parts[2];
                        var holeId = parts[3];
                        var scoreStr = form[key];
                        if (int.TryParse(scoreStr, out int score))
                        {
                            scores[$"{courseCode}_{userId}_{holeId}"] = score;
                        }
                    }
                }
            }

            // 서비스 DI 받아서 저장
            await _teamScoreInpuService.SaveScoresAsync(GameCode, inputBy, scores);

            TempData["SuccessMessage"] = $@"<span><strong>✅ 저장 성공!</strong><br>
                    <strong>대회명 :</strong> <span style='color:#176d37;'>{GameName}</span>,
                    <strong>팀번호 :</strong> <span style='color:#0a5c1a;'>{TeamNumber}</span><br>
                    데이터를 <strong>저장했습니다.</strong></span>";

            // 저장 후 원래 페이지로 리디렉션
            return RedirectToPage();
        }
    }
}