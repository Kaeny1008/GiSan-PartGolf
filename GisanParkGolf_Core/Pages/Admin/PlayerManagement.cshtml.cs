using ClosedXML.Excel;
using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;
using GisanParkGolf_Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class PlayerManagementModel : PageModel
    {
        private readonly IPlayerService _playerService;

        public PlayerManagementModel(IPlayerService playerService)
        {
            _playerService = playerService;
            SearchFields = new List<SelectListItem>
            {
                new SelectListItem { Text = "�̸�", Value = "UserName" },
                new SelectListItem { Text = "���̵�", Value = "UserId" }
            };
        }

        // --- �Ӽ��� ---
        public List<SelectListItem> SearchFields { get; }

        // �ڡڡڡڡ� ������: �⺻ �˻� �ʵ带 'UserName'���� ���� �ڡڡڡڡ�
        [BindProperty(SupportsGet = true)]
        public string SearchField { get; set; } = "UserName";

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool ReadyUserOnly { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public PaginatedList<SYS_Users> Users { get; set; } = null!;

        // --- �޼���� ---
        public async Task OnGetAsync()
        {
            Users = await _playerService.GetPlayersAsync(SearchField, SearchQuery, ReadyUserOnly, PageIndex, PageSize);
        }

        public async Task<IActionResult> OnGetExcelAsync()
        {
            var users = await _playerService.GetPlayersForExcelAsync(SearchField, SearchQuery, ReadyUserOnly);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("���� ���");
                worksheet.Cell(1, 1).Value = "���̵�";
                worksheet.Cell(1, 2).Value = "�̸�";
                worksheet.Cell(1, 3).Value = "���� ����";
                worksheet.Cell(1, 4).Value = "�����";

                int currentRow = 2;
                foreach (var user in users)
                {
                    worksheet.Cell(currentRow, 1).Value = user.UserId;
                    worksheet.Cell(currentRow, 2).Value = user.UserName;
                    worksheet.Cell(currentRow, 3).Value = user.UserWClass;
                    worksheet.Cell(currentRow, 4).Value = user.UserRegistrationDate;
                    currentRow++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string fileName = $"GisanParkGolf_Players_{System.DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
    }
}