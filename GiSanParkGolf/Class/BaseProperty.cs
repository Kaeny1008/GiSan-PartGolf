using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace GiSanParkGolf.Class
{
    /// <summary>
    /// BaseProperty 클래스: 공통으로 사용되는 속성모음
    /// </summary>
    public class Search_Property
    {
        /// <summary>
        /// 페이지 검색모드 일반모드, 검색모드
        /// </summary>
        public bool SearchMode { get; set; } = false;

        /// <summary>
        /// 페이지 검색필드
        /// </summary>
        public string SearchField { get; set; }

        /// <summary>
        /// 페이지 검색내용
        /// </summary>
        public string SearchQuery { get; set; }

        //public int PageIndex { get; set; } // 현재 보여줄 페이지 번호

        //public int RecordCount { get; set; } // 총 레코드 개수(글번호 순서 정렬용)

        /// <summary>
        /// 승인대기중인 유저(string 'True', 'False')
        /// </summary>
        public string ReadyUser { get; set; }

        /// <summary>
        /// 몇 번째 페이지를 보여줄 건지 : 웹 폼에서 속성으로 전달됨
        /// </summary>
        [Category("페이징처리")] // Category 특성은 모두 생략 가능(속성에 표시됨)
        public int PageIndex { get; set; }


        /// <summary>
        /// 총 몇 개의 페이지가 만들어지는지 : 총 레코드 수 / 10(한 페이지에서 보여줄)
        /// </summary>
        [Category("페이징처리")]
        public int PageCount { get; set; }


        /// <summary>
        /// 페이지 사이즈 : 한 페이지에 몇 개의 레코드를 보여줄 건지 결정
        /// </summary>
        [Category("페이징처리")]
        [Description("한 페이지에 몇 개의 레코드를 보여줄 건지 결정")]
        public int PageSize { get; set; } = 10; // 페이지 사이즈는 기본값이 10


        /// <summary>
        /// 레코드 카운트 : 현재 테이블에 몇 개의 레코드가 있는지 지정
        /// </summary>
        private int _RecordCount;
        [Category("페이징처리")]
        [Description("현재 테이블에 몇 개의 레코드가 있는지 지정")]
        public int RecordCount
        {
            get { return _RecordCount; }
            set
            {
                _RecordCount = value;
                // 총 페이지 수 계산
                PageCount = ((_RecordCount - 1) / PageSize) + 1; // 계산식
                //Debug.WriteLine("총 페이지 수: " + PageCount);
            }
        }
    }
}