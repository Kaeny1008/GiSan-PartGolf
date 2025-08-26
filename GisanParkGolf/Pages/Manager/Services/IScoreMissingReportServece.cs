using GisanParkGolf.Pages.Manager.ViewModels;

namespace GisanParkGolf.Pages.Manager.Services
{
    public interface IScoreMissingReportServece
    {
        /// <summary>
        /// 진행중이거나 종료대기중인 대회 리스트만 반환
        /// </summary>
        List<GameInfoViewModel> GetRunningOrPendingGames();

        /// <summary>
        /// 선택한 대회(또는 전체)의 점수 입력 누락 명단 조회
        /// </summary>
        List<MissingScoreInfoViewModel> GetMissingScoreList(string? gameCode);
    }
}
