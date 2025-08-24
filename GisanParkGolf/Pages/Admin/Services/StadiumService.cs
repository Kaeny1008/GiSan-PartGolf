using GisanParkGolf.Data;
using GisanParkGolf.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GisanParkGolf.Pages.Admin.Admin
{
    public class StadiumService : IStadiumService
    {
        private readonly MyDbContext _dbContext;

        public StadiumService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedList<Stadium>> GetStadiumsAsync(string? searchField, string? searchQuery, int pageIndex, int pageSize)
        {
            var query = _dbContext.Stadiums
                                  .Include(s => s.Courses)
                                  .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrEmpty(searchField))
            {
                switch (searchField)
                {
                    case "StadiumName":
                        query = query.Where(u => u.StadiumName != null && u.StadiumName.Contains(searchQuery));
                        break;
                    case "StadiumCode":
                        query = query.Where(u => u.StadiumCode != null && u.StadiumCode.Contains(searchQuery));
                        break;
                    case "RegionName":
                        query = query.Where(u => u.RegionName != null && u.RegionName.Contains(searchQuery));
                        break;
                    case "CityName":
                        query = query.Where(u => u.CityName != null && u.CityName.Contains(searchQuery));
                        break;
                }
            }
            var orderedQuery = query.OrderByDescending(u => u.CreatedAt);
            return await PaginatedList<Stadium>.CreateAsync(orderedQuery.AsNoTracking(), pageIndex, pageSize);
        }

        public async Task<Stadium?> GetStadiumByIdAsync(string stadiumCode)
        {
            return await _dbContext.Stadiums
                .Include(s => s.Courses.OrderBy(c => c.CourseName))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StadiumCode == stadiumCode);
        }

        public async Task<Course?> GetCourseByIdAsync(int courseCode)
        {
            return await _dbContext.Courses
                .Include(c => c.Holes.OrderBy(h => h.HoleId))
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseCode == courseCode);
        }

        public async Task CreateStadiumAsync(Stadium stadium)
        {
            stadium.StadiumCode = await GenerateNewStadiumCodeAsync();
            stadium.CreatedAt = DateTime.Now;
            _dbContext.Stadiums.Add(stadium);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateCourseAsync(Course course)
        {
            course.CreatedAt = DateTime.Now;
            for (int i = 1; i <= course.HoleCount; i++)
            {
                course.Holes.Add(new Hole { HoleName = $"{i}번 홀", Par = 3, Distance = 0 });
            }
            _dbContext.Courses.Add(course);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveHolesAsync(int courseCode, List<Hole> holes)
        {
            var course = await _dbContext.Courses.Include(c => c.Holes).FirstOrDefaultAsync(c => c.CourseCode == courseCode);
            if (course == null) return;

            var existingHoles = course.Holes.ToDictionary(h => h.HoleId);
            var submittedHoleIds = holes.Select(h => h.HoleId).ToHashSet();

            // 삭제할 홀
            var holesToDelete = existingHoles.Values.Where(h => !submittedHoleIds.Contains(h.HoleId)).ToList();
            _dbContext.Holes.RemoveRange(holesToDelete);

            // 수정 및 추가
            foreach (var submittedHole in holes)
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
                    _dbContext.Holes.Add(submittedHole);
                }
            }
            course.HoleCount = holes.Count;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteStadiumAsync(string stadiumCode)
        {
            var stadium = await _dbContext.Stadiums.Include(s => s.Courses).ThenInclude(c => c.Holes).FirstOrDefaultAsync(s => s.StadiumCode == stadiumCode);
            if (stadium != null)
            {
                _dbContext.Stadiums.Remove(stadium);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteCourseAsync(int courseCode)
        {
            var course = await _dbContext.Courses.Include(c => c.Holes).FirstOrDefaultAsync(c => c.CourseCode == courseCode);
            if (course != null)
            {
                _dbContext.Courses.Remove(course);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<string> GenerateNewStadiumCodeAsync()
        {
            var lastCode = await _dbContext.Stadiums.OrderByDescending(s => s.StadiumCode).Select(s => s.StadiumCode).FirstOrDefaultAsync();
            int nextNum = 1;
            if (lastCode != null && lastCode.StartsWith("STD") && int.TryParse(lastCode.AsSpan(3), out int num))
            {
                nextNum = num + 1;
            }
            return $"STD{nextNum:D4}";
        }
    }
}