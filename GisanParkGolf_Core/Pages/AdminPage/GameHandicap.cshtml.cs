using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.Services.AdminPage;
using GisanParkGolf_Core.ViewModels.AdminPage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Pages.AdminPage
{
    [Authorize(Policy = "AdminOnly")]
    public class GameHandicapModel : PageModel
    {
        private readonly IHandicapService _handicapService;

        public GameHandicapModel(IHandicapService handicapService)
        {
            _handicapService = handicapService;
        }

        // �������� ǥ�õ� ������ (PaginatedList ���)
        public PaginatedList<HandicapViewModel> Handicaps { get; set; } = null!;

        // �˻� �ʵ� (�̸� �Ǵ� ID)
        [BindProperty(SupportsGet = true)]
        public string SearchField { get; set; } = "UserName"; // �⺻ �˻� ������ '�̸�'

        // �˻���
        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        // �������� ������ �� ��
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10; // �⺻ 10��

        // ���� ������ ��ȣ
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1; // �⺻ 1������

        // ����/���� �޽����� �����ϱ� ���� TempData
        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        // �������� GET ������� ��û�� �� ����Ǵ� �޼���
        public async Task OnGetAsync()
        {
            // ���� ������ ���� ����¡ �� �˻� ���ǿ� �´� �����͸� ������
            Handicaps = await _handicapService.GetPlayerHandicapsAsync(SearchField, SearchQuery, PageIndex, PageSize);
        }

        // ���� �ڵ�ĸ�� ������Ʈ�� �� ����Ǵ� �޼���
        public async Task<IActionResult> OnPostUpdateAsync(string userId, int age, int newHandicap, string newSource)
        {
            var adminName = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";

            try
            {
                var result = await _handicapService.UpdateHandicapAsync(userId, age, newHandicap, newSource, adminName);
                if (result)
                {
                    SuccessMessage = $"({userId}) ���� �ڵ�ĸ�� ���������� ������Ʈ�Ǿ����ϴ�.";
                }
                else
                {
                    SuccessMessage = "����� ������ ���� ������Ʈ���� �ʾҽ��ϴ�.";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"������Ʈ �� ������ �߻��߽��ϴ�: {ex.Message}";
            }

            // ���� ������ �� �˻� ���¸� �����ϸ� �������� ���ΰ�ħ
            return RedirectToPage(new { SearchField, SearchQuery, PageIndex, PageSize });
        }

        // ��� �ڵ�ĸ�� �ϰ� ������ �� ����Ǵ� �޼���
        public async Task<IActionResult> OnPostRecalculateAllAsync()
        {
            var adminName = User.FindFirstValue(ClaimTypes.Name) ?? "UnknownAdmin";
            try
            {
                var updatedCount = await _handicapService.RecalculateAllHandicapsAsync(adminName);
                SuccessMessage = $"�� {updatedCount}���� �ڵ�ĸ�� �ڵ����� ����Ǿ����ϴ�.";
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"��ü ���� �� ������ �߻��߽��ϴ�: {ex.Message}";
            }

            // ���� ������ �� �˻� ���¸� �����ϸ� �������� ���ΰ�ħ
            return RedirectToPage(new { SearchField, SearchQuery, PageIndex, PageSize });
        }

        // .cshtml ���Ͽ��� '�ڵ�' �ڵ�ĸ ���� �̸� �����ֱ� ���� ���� �޼���
        public int CalculateHandicapByAge(int age)
        {
            return HandicapCalculator.CalculateByAge(age);
        }
    }
}