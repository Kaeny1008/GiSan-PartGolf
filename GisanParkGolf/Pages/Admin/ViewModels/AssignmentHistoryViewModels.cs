namespace GisanParkGolf.Pages.Admin.ViewModels
{
    public class AssignmentHistoryViewModel
    {
        public int HistoryId { get; set; }
        public string? ChangeType { get; set; }
        public string? ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? RawDetailsJson { get; set; }
        public string? Summary { get; set; }

        // 사람이 읽기 쉽도록 BuildHistoryViewModels에서 채워지는 key/value 딕셔너리
        public Dictionary<string, string> DetailsDict { get; set; } = new Dictionary<string, string>();
    }
}
