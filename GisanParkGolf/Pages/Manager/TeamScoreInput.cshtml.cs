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
                TempData["ErrorMessage"] = "���ڵ� ������ �ùٸ��� �ʽ��ϴ�.";
                ScanFound = false;
                return Page();
            }

            var parts = ScannedCode.Split('-', 2); // "gamecode-teamnumber"
            if (parts.Length < 2)
            {
                TempData["ErrorMessage"] = "���ڵ� ������ �ùٸ��� �ʽ��ϴ�.";
                ScanFound = false;
                return Page();
            }

            var gameCode = parts[0];
            var teamNumber = parts[1];

            // ���񽺿��� ������ ��ȸ
            var teamScoreCourses = _teamScoreInpuService.GetTeamScoreCourses(gameCode, teamNumber);

            if (teamScoreCourses == null || teamScoreCourses.Count == 0)
            {
                TempData["ErrorMessage"] = "�ش� ������ ã�� �� �����ϴ�.";
                ScanFound = false;
                TeamScoreCourses = new();
            }
            else
            {
                //TempData["SuccessMessage"] = $"�����ڵ�: {gameCode}, ����ȣ: {teamNumber} ������ �ҷ���";
                ScanFound = true;
                TeamScoreCourses = teamScoreCourses;
            }

            return Page();
        }
    }
}