using GisanParkGolf.Helpers;
using GisanParkGolf.Pages.Player.ViewModels;

public interface IGameResutService
{
    GameResultViewModel? GetMyResult(string gameCode, string userId);
    List<GameResultViewModel> GetAllResults(string gameCode);
    List<HoleResultViewModel> GetHoleResults(string gameCode, string userId);
    List<CourseViewModel> GetCourses(string gameCode);
    Task<PaginatedList<GameResultViewModel>> GetFilteredResults(string gameCode, string? searchField, string? searchQuery, int pageIndex, int pageSize); // 반환 타입 변경
    int GetTotalPages(string gameCode, string? searchField, string? searchQuery, int pageSize);
}