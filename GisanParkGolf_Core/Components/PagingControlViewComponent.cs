using GisanParkGolf_Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace GisanParkGolf_Core.Components
{
    public class PagingControlViewComponent : ViewComponent
    {
        // 뷰 컴포넌트를 호출할 때 실행되는 메서드
        public IViewComponentResult Invoke(int totalItems, int itemsPerPage, int currentPage, int pageGroupSize = 10)
        {
            if (totalItems <= 0)
            {
                return Content(string.Empty); // 표시할 항목이 없으면 아무것도 렌더링하지 않음
            }

            var totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
            var buttons = new List<PagingButtonViewModel>();

            // 현재 페이지 그룹의 시작/끝 페이지 계산
            int startPage = ((currentPage - 1) / pageGroupSize) * pageGroupSize + 1;
            int endPage = Math.Min(startPage + pageGroupSize - 1, totalPages);

            // "이전" 버튼
            buttons.Add(new PagingButtonViewModel
            {
                Text = "이전",
                PageIndex = startPage - 1,
                IsEnabled = startPage > 1
            });

            // 페이지 번호 버튼
            for (int i = startPage; i <= endPage; i++)
            {
                buttons.Add(new PagingButtonViewModel
                {
                    Text = i.ToString(),
                    PageIndex = i,
                    IsCurrent = i == currentPage
                });
            }

            // "다음" 버튼
            buttons.Add(new PagingButtonViewModel
            {
                Text = "다음",
                PageIndex = endPage + 1,
                IsEnabled = endPage < totalPages
            });

            // 생성된 버튼 목록을 뷰로 전달
            return View(buttons);
        }
    }
}
