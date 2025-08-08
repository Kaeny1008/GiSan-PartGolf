using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GiSanParkGolf.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class StadiumManagerModel : PageModel
    {        
        private readonly MyDbContext _context;

        public StadiumManagerModel(MyDbContext context)
        {
            _context = context;
        }

        public IList<Stadium> StadiumList { get; set; } = [];
        public Stadium? SelectedStadium { get; set; }
        public Course? SelectedCourse { get; set; }

        [BindProperty]
        public Stadium NewStadium { get; set; } = new();
        [BindProperty]
        public Course NewCourse { get; set; } = new();
        [BindProperty]
        public List<Hole> HoleDetails { get; set; } = [];

        public async Task OnGetAsync(string? stadiumCode, int? courseCode, bool showNewForm = false)
        {
            StadiumList = await _context.Stadiums
                .Include(s => s.Courses) // �ڽ� ���� ǥ�ø� ���� Include
                .AsNoTracking()
                .OrderBy(s => s.StadiumName)
                .ToListAsync();

            if (!string.IsNullOrEmpty(stadiumCode))
            {
                SelectedStadium = await _context.Stadiums
                    .Include(s => s.Courses.OrderBy(c => c.CourseName))
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.StadiumCode == stadiumCode);
            }

            if (courseCode.HasValue)
            {
                SelectedCourse = await _context.Courses
                    .Include(c => c.Holes.OrderBy(h => h.HoleId))
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CourseCode == courseCode);

                if (SelectedCourse != null)
                {
                    HoleDetails = SelectedCourse.Holes.ToList();
                }
            }

            if (showNewForm)
            {
                ViewData["ShowNewStadiumForm"] = "True";
                SelectedStadium = null;
                SelectedCourse = null;
            }
        }

        public IActionResult OnGetNewStadium()
        {
            // showNewForm ���� �Ķ���͸� ������ �������� ���ΰ�ħ
            return RedirectToPage(new { showNewForm = true });
        }

        public async Task<IActionResult> OnPostCreateStadiumAsync()
        {
            if (!ModelState.IsValid)
            {
                // ��ȿ�� �˻� ���� ��, �Է°� ������ ���� showNewForm�� true�� ����
                TempData["ModalType"] = "error";
                TempData["ModalTitle"] = "���� ����";
                TempData["ModalBody"] = "�Է� ������ Ȯ�����ּ���.";
                await OnGetAsync(null, null, showNewForm: true);
                return Page();
            }

            NewStadium.StadiumCode = await GenerateNewStadiumCodeAsync();
            NewStadium.CreatedAt = DateTime.Now;

            _context.Stadiums.Add(NewStadium);
            await _context.SaveChangesAsync();

            // ��� ���� ��, ���� ��ϵ� ������� ������ ���·� �����̷�Ʈ
            return RedirectToPage(new { stadiumCode = NewStadium.StadiumCode });
        }

        public async Task<IActionResult> OnPostCreateCourseAsync(string stadiumCode)
        {
            if (string.IsNullOrWhiteSpace(NewCourse.CourseName) || NewCourse.HoleCount <= 0)
            {
                TempData["ModalType"] = "error";
                TempData["ModalTitle"] = "���� ����";
                TempData["ModalBody"] = "�Է� ������ Ȯ�����ּ���.";
                await OnGetAsync(stadiumCode, null);
                return Page();
            }
            NewCourse.CourseName = NewCourse.CourseName + " �ڽ�";
            NewCourse.StadiumCode = stadiumCode;
            NewCourse.CreatedAt = DateTime.Now;

            for (int i = 1; i <= NewCourse.HoleCount; i++)
            {
                NewCourse.Holes.Add(new Hole { HoleName = $"{i}�� Ȧ", Par = 3, Distance = 0 });
            }

            _context.Courses.Add(NewCourse);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { stadiumCode, courseCode = NewCourse.CourseCode });
        }

        // (���� �ٸ� �ڵ鷯���� ��� ����)
        public async Task<IActionResult> OnPostSaveHolesAsync(int courseCode, string stadiumCode)
        {
            var course = await _context.Courses
                .Include(c => c.Holes)
                .FirstOrDefaultAsync(c => c.CourseCode == courseCode);

            if (course == null) return NotFound();

            var existingHoles = course.Holes.ToDictionary(h => h.HoleId);

            int newCount = HoleDetails.Count(h => h.HoleId == 0);
            int updatedCount = 0;
            foreach (var submittedHole in HoleDetails)
            {
                if (existingHoles.TryGetValue(submittedHole.HoleId, out var existingHole))
                {
                    if (existingHole.HoleName != submittedHole.HoleName ||
                        existingHole.Distance != submittedHole.Distance ||
                        existingHole.Par != submittedHole.Par)
                    {
                        updatedCount++;
                    }

                    existingHole.HoleName = submittedHole.HoleName;
                    existingHole.Distance = submittedHole.Distance;
                    existingHole.Par = submittedHole.Par;
                }
                else
                {
                    submittedHole.CourseCode = courseCode;
                    _context.Holes.Add(submittedHole);
                }
            }

            var submittedHoleIds = HoleDetails.Select(h => h.HoleId).ToHashSet();
            var holesToDelete = existingHoles.Values.Where(h => !submittedHoleIds.Contains(h.HoleId)).ToList();
            if (holesToDelete.Any())
            {
                _context.Holes.RemoveRange(holesToDelete);
            }

            course.HoleCount = HoleDetails.Count;
            await _context.SaveChangesAsync();

            TempData["ModalType"] = "success";
            TempData["ModalTitle"] = "Ȧ ���� ���� �Ϸ�";
            TempData["ModalBody"] = $"�� {HoleDetails.Count}���� Ȧ�� ���� �Ǿ����ϴ�.<br />{newCount}�� �ű� / {updatedCount}�� ���� / {holesToDelete.Count}�� ����";

            return RedirectToPage(new { stadiumCode, courseCode });
        }

        public async Task<IActionResult> OnPostDeleteStadiumAsync(string stadiumCode)
        {
            var stadium = await _context.Stadiums
                .Include(s => s.Courses)
                .ThenInclude(c => c.Holes)
                .FirstOrDefaultAsync(s => s.StadiumCode == stadiumCode);

            if (stadium != null)
            {
                _context.Stadiums.Remove(stadium);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteCourseAsync(int courseCode, string stadiumCode)
        {
            var course = await _context.Courses
                .Include(c => c.Holes)
                .FirstOrDefaultAsync(c => c.CourseCode == courseCode);

            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { stadiumCode });
        }

        private async Task<string> GenerateNewStadiumCodeAsync()
        {
            var lastCode = await _context.Stadiums
                                .OrderByDescending(s => s.StadiumCode)
                                .Select(s => s.StadiumCode)
                                .FirstOrDefaultAsync();

            int nextNum = 1;
            if (lastCode != null && lastCode.StartsWith("STD") && int.TryParse(lastCode.AsSpan(3), out int num))
            {
                nextNum = num + 1;
            }
            return $"STD{nextNum:D4}";
        }
    }
}