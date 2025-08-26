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
                TempData["ErrorMessage"] = "������ �����Ͱ� �����ϴ�. ���� ��ĵ�� �ּ���.";
                return RedirectToPage();
            }

            var form = Request.Form;
            var inputBy = User.Identity?.Name ?? "admin";

            // ���� ������ Dictionary ����: key = "courseCode_userId_holeId", value = ����
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

            // ���� DI �޾Ƽ� ����
            await _teamScoreInpuService.SaveScoresAsync(GameCode, inputBy, scores);

            TempData["SuccessMessage"] = $"�����ڵ�: {GameCode}, ����ȣ: {TeamNumber} �����͸� �����߽��ϴ�.";
            // ���� �� ���� �������� ���𷺼�
            return RedirectToPage();
        }
    }
}