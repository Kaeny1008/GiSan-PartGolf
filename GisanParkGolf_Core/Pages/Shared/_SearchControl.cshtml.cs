using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GisanParkGolf_Core.Pages.Shared
{
    public class SearchFieldOption
    {
        public string Value { get; set; } = "";
        public string Text { get; set; } = "";
    }

    public class SearchControlModel
    {
        public bool ShowCancelCheckbox { get; set; } = false;
        public bool IsCancelRequestedChecked { get; set; } = false;
        public bool ShowReadyUserCheckbox { get; set; } = false;
        public bool ReadyUserChecked { get; set; } = false;
        public int PageSize { get; set; }
        public string? SearchField { get; set; }
        public string? SearchQuery { get; set; }
        public string ResetPageName { get; set; } = "";
        public List<SearchFieldOption> FieldOptions { get; set; } = new();
        public string PageSizeName { get; set; } = "PageSize";
    }
}