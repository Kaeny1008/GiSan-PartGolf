using GisanParkGolf.Pages.Player.ViewModels;

public interface IGameResutService
{
    GameResultViewModel? GetMyResult(string gameCode, string userId);
    List<GameResultViewModel> GetAllResults(string gameCode);
}