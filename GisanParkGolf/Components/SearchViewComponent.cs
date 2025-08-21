using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GisanParkGolf.Components
{
    public class SearchViewComponent : ViewComponent
    {
        public class SearchViewModel
        {
            public List<SelectListItem> SearchFields { get; set; } = new();
            public string? SearchField { get; set; }
            public string? SearchQuery { get; set; }
            public bool ReadyUserOnly { get; set; }
        }

        public IViewComponentResult Invoke(
            List<SelectListItem> searchFields,
            string searchField,
            string searchQuery,
            bool readyUserOnly)
        {
            var model = new SearchViewModel
            {
                // 파라미터로 받은 searchFields를 모델에 전달
                SearchFields = searchFields,
                SearchField = searchField,
                SearchQuery = searchQuery,
                ReadyUserOnly = readyUserOnly
            };

            return View(model);
        }
    }
}