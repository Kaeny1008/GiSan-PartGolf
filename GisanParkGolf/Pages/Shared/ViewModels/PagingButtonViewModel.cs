namespace GisanParkGolf.ViewModels.Shared
{
    public class PagingButtonViewModel
    {
        public string Text { get; set; } = string.Empty;
        public int PageIndex { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool IsCurrent { get; set; } = false;
    }
}
