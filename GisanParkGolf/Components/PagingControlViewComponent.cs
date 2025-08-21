using GisanParkGolf.Data; // PagingButtonViewModel이 있는 네임스페이스
using GisanParkGolf.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace GisanParkGolf.Components
{
    public class PagingControlViewComponent : ViewComponent
    {
        // 뷰 컴포넌트를 호출할 때 실행되는 메서드
        public IViewComponentResult Invoke(int totalItems, int itemsPerPage, int currentPage, int pageGroupSize = 10)
        {
            if (totalItems <= 0 || itemsPerPage <= 0)
            {
                return Content(string.Empty); // 표시할 항목이 없으면 아무것도 렌더링하지 않음
            }

            var totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

            // 현재 페이지가 전체 페이지 수를 넘지 않도록 보정
            currentPage = Math.Max(1, Math.Min(currentPage, totalPages));

            var buttons = new List<PagingButtonViewModel>();

            // --- "이전" 그룹으로 이동하는 버튼 로직 (선택적, 더 나은 UX를 위해) ---
            // int startPageInGroup = ((currentPage - 1) / pageGroupSize) * pageGroupSize + 1;
            // int endPageInGroup = Math.Min(startPageInGroup + pageGroupSize - 1, totalPages);

            // "처음" 버튼
            buttons.Add(new PagingButtonViewModel
            {
                Text = "처음",
                PageIndex = 1,
                IsEnabled = currentPage > 1
            });

            // "이전" 버튼 (한 페이지씩 이동)
            buttons.Add(new PagingButtonViewModel
            {
                Text = "이전",
                PageIndex = currentPage - 1,
                IsEnabled = currentPage > 1 // ★★★ 핵심 수정 #1: 현재 페이지가 1보다 크면 무조건 활성화
            });

            // 페이지 번호 버튼 (현재 페이지 주변 5개만 보여주는 방식으로 개선)
            int startPage = Math.Max(1, currentPage - 2);
            int endPage = Math.Min(totalPages, currentPage + 2);

            // 페이지 번호가 5개 미만일 경우 조정
            if (endPage - startPage + 1 < 5)
            {
                if (startPage == 1)
                {
                    endPage = Math.Min(totalPages, startPage + 4);
                }
                else if (endPage == totalPages)
                {
                    startPage = Math.Max(1, endPage - 4);
                }
            }

            for (int i = startPage; i <= endPage; i++)
            {
                buttons.Add(new PagingButtonViewModel
                {
                    Text = i.ToString(),
                    PageIndex = i,
                    IsCurrent = i == currentPage
                });
            }

            // "다음" 버튼 (한 페이지씩 이동)
            buttons.Add(new PagingButtonViewModel
            {
                Text = "다음",
                PageIndex = currentPage + 1,
                IsEnabled = currentPage < totalPages // ★★★ 핵심 수정 #2: 현재 페이지가 마지막 페이지보다 작으면 무조건 활성화
            });

            // "마지막" 버튼
            buttons.Add(new PagingButtonViewModel
            {
                Text = "마지막",
                PageIndex = totalPages,
                IsEnabled = currentPage < totalPages
            });

            // 생성된 버튼 목록을 뷰로 전달
            return View(buttons);
        }
    }
}