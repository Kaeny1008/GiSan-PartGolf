using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GisanParkGolf_Core.Helpers
{
    // T는 어떤 타입의 데이터든 담을 수 있다는 의미 (예: HandicapViewModel)
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        // ★★★★★ 에러 수정: 총 아이템 개수를 저장할 속성 추가! ★★★★★
        public int TotalCount { get; private set; }

        public int PageSize { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count; // 생성자에서 총 개수 값을 할당
            PageSize = pageSize;
            this.AddRange(items);
        }

        // 이전 페이지가 있는지 확인
        public bool HasPreviousPage => PageIndex > 1;

        // 다음 페이지가 있는지 확인
        public bool HasNextPage => PageIndex < TotalPages;

        // IQueryable<T>를 받아서 특정 페이지의 데이터만 잘라내는 핵심 메서드
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}