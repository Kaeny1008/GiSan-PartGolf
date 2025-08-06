using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Components
{
    // 클래스 이름은 "ViewComponent"로 끝나야 합니다.
    public class SearchViewComponent : ViewComponent
    {
        // View Component가 사용할 모델
        public class SearchViewModel
        {
            public bool ReadyUserOnly { get; set; }
            public string? SearchQuery { get; set; }
            public string? SearchField { get; set; }
            public List<SelectListItem> SearchFields { get; set; } = new();
        }

        // View Component의 핵심 메서드. InvokeAsync 또는 Invoke를 사용합니다.
        public IViewComponentResult Invoke(
            string searchField,
            string searchQuery,
            bool readyUserOnly)
        {
            // 검색 필드 드롭다운에 들어갈 목록을 정의합니다.
            var searchFields = new List<SelectListItem>
            {
                new SelectListItem { Text = "이름", Value = "UserName" },
                new SelectListItem { Text = "아이디", Value = "UserId" },
                new SelectListItem { Text = "상태", Value = "UserWClass" }
            };

            // 부모 페이지에서 전달받은 값으로 뷰 모델을 설정합니다.
            var model = new SearchViewModel
            {
                SearchFields = searchFields,
                SearchField = searchField,
                SearchQuery = searchQuery,
                ReadyUserOnly = readyUserOnly
            };

            // 뷰를 렌더링하고 모델을 전달합니다.
            return View(model);
        }
    }
}