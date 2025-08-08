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
                .Include(s => s.Courses) // 코스 개수 표시를 위해 Include
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
            // showNewForm 쿼리 파라미터를 가지고 페이지를 새로고침
            return RedirectToPage(new { showNewForm = true });
        }

        public async Task<IActionResult> OnPostCreateStadiumAsync()
        {
            if (!ModelState.IsValid)
            {
                // 유효성 검사 실패 시, 입력값 유지를 위해 showNewForm을 true로 설정
                TempData["ModalType"] = "error";
                TempData["ModalTitle"] = "저장 실패";
                TempData["ModalBody"] = "입력 정보를 확인해주세요.";
                await OnGetAsync(null, null, showNewForm: true);
                return Page();
            }

            NewStadium.StadiumCode = await GenerateNewStadiumCodeAsync();
            NewStadium.CreatedAt = DateTime.Now;

            _context.Stadiums.Add(NewStadium);
            await _context.SaveChangesAsync();

            // 등록 성공 후, 새로 등록된 경기장을 선택한 상태로 리다이렉트
            return RedirectToPage(new { stadiumCode = NewStadium.StadiumCode });
        }

        public async Task<IActionResult> OnPostCreateCourseAsync(string stadiumCode)
        {
            if (string.IsNullOrWhiteSpace(NewCourse.CourseName) || NewCourse.HoleCount <= 0)
            {
                TempData["ModalType"] = "error";
                TempData["ModalTitle"] = "저장 실패";
                TempData["ModalBody"] = "입력 정보를 확인해주세요.";
                await OnGetAsync(stadiumCode, null);
                return Page();
            }
            NewCourse.CourseName = NewCourse.CourseName + " 코스";
            NewCourse.StadiumCode = stadiumCode;
            NewCourse.CreatedAt = DateTime.Now;

            for (int i = 1; i <= NewCourse.HoleCount; i++)
            {
                NewCourse.Holes.Add(new Hole { HoleName = $"{i}번 홀", Par = 3, Distance = 0 });
            }

            _context.Courses.Add(NewCourse);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { stadiumCode, courseCode = NewCourse.CourseCode });
        }

        // (이하 다른 핸들러들은 모두 동일)
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
            TempData["ModalTitle"] = "홀 정보 저장 완료";
            TempData["ModalBody"] = $"총 {HoleDetails.Count}개의 홀이 저장 되었습니다.<br />{newCount}개 신규 / {updatedCount}개 수정 / {holesToDelete.Count}개 삭제";

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