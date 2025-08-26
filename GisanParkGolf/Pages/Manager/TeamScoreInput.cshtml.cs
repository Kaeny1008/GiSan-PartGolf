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
                ScanFound = true;
                TeamScoreCourses = teamScoreCourses;
            }

            return Page();
        }
    }
}