using GisanParkGolf_Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public IList<Stadium> StadiumList { get; set; } = new List<Stadium>();
        public Stadium? SelectedStadium { get; set; }
        public Course? SelectedCourse { get; set; }

        [BindProperty]
        public Stadium NewStadium { get; set; } = new();
        [BindProperty]
        public Course NewCourse { get; set; } = new();
        [BindProperty]
        public List<Hole> HoleDetails { get; set; } = new();

        public async Task OnGetAsync(string? stadiumCode, int? courseCode)
        {
            StadiumList = await _context.Stadiums.AsNoTracking().OrderBy(s => s.StadiumName).ToListAsync();

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
        }

        public async Task<IActionResult> OnPostCreateStadiumAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ShowNewStadiumForm"] = "True";
                ViewData["ValidationFailed Title"] = "경기장 생성 실패";
                ViewData["ValidationFailed"] = "경기장 이름은 필 수 입력입니다.";  

                // 유효성 검사 실패 시, 현재 입력된 값을 유지하며 페이지를 다시 표시
                await OnGetAsync(null, null); // 경기장 목록은 다시 불러와야 함
                return Page();
            }

            NewStadium.StadiumCode = await GenerateNewStadiumCodeAsync();
            NewStadium.CreatedAt = DateTime.Now;

            _context.Stadiums.Add(NewStadium);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { stadiumCode = NewStadium.StadiumCode });
        }

        public async Task<IActionResult> OnPostCreateCourseAsync(string stadiumCode)
        {
            if (string.IsNullOrWhiteSpace(NewCourse.CourseName) || NewCourse.HoleCount <= 0)
            {
                ViewData["ValidationFailed Title"] = "코스 생성 실패";
                ViewData["ValidationFailed"] = "코스명과 홀 수를 정확히 입력하세요.";
                //return RedirectToPage(new { stadiumCode });
                await OnGetAsync(stadiumCode, null);
                return Page();
            }

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

        public async Task<IActionResult> OnPostSaveHolesAsync(int courseCode, string stadiumCode)
        {
            var course = await _context.Courses
                .Include(c => c.Holes)
                .FirstOrDefaultAsync(c => c.CourseCode == courseCode);

            if (course == null) return NotFound();

            var existingHoles = course.Holes.ToDictionary(h => h.HoleId);

            foreach (var submittedHole in HoleDetails)
            {
                if (existingHoles.TryGetValue(submittedHole.HoleId, out var existingHole))
                {
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

            return RedirectToPage(new { stadiumCode, courseCode });
        }

        public async Task<IActionResult> OnPostDeleteStadiumAsync(string stadiumCode)
        {
            // FindAsync 대신, Include를 사용해서 관련된 모든 Course와 Hole을 함께 불러온다.
            var stadium = await _context.Stadiums
                .Include(s => s.Courses)
                .ThenInclude(c => c.Holes)
                .FirstOrDefaultAsync(s => s.StadiumCode == stadiumCode);

            if (stadium != null)
            {
                _context.Stadiums.Remove(stadium);
                // 이제 EF Core는 stadium과 함께 로드된 courses, holes를 모두 알고 있으므로
                // Cascade 설정에 따라 연관된 모든 데이터를 함께 삭제한다.
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteCourseAsync(int courseCode, string stadiumCode)
        {
            // FindAsync 대신, Include를 사용해서 관련된 모든 Hole을 함께 불러온다.
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