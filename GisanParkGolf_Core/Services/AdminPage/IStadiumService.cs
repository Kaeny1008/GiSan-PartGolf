using GisanParkGolf_Core.Data;
using GisanParkGolf_Core.Helpers;

namespace GisanParkGolf_Core.Services.AdminPage
{
    public interface IStadiumService
    {
        // 페이징된 목록 조회
        Task<PaginatedList<Stadium>> GetStadiumsAsync(string? searchField, string? searchQuery, int pageIndex, int pageSize);

        // 특정 경기장 정보 가져오기 (코스, 홀 정보 포함)
        Task<Stadium?> GetStadiumByIdAsync(string stadiumCode);

        // 특정 코스 정보 가져오기 (홀 정보 포함)
        Task<Course?> GetCourseByIdAsync(int courseCode);

        // 경기장 생성
        Task CreateStadiumAsync(Stadium stadium);

        // 코스 생성
        Task CreateCourseAsync(Course course);

        // 홀 정보 저장 (수정/추가/삭제)
        Task SaveHolesAsync(int courseCode, List<Hole> holes);

        // 경기장 삭제
        Task DeleteStadiumAsync(string stadiumCode);

        // 코스 삭제
        Task DeleteCourseAsync(int courseCode);
    }
}
