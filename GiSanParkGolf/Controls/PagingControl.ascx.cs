using System;
using System.Diagnostics;
using static GiSanParkGolf.Global;

namespace GiSanParkGolf.Controls
{
    public partial class PagingControl : System.Web.UI.UserControl
    {
        // 페이지 로드할 때 페이저 구현하기
        protected void Page_Load(object sender, EventArgs e)
        {
            // 검색 모드 결정
            searchProperty.SearchMode =
                    (!string.IsNullOrEmpty(Request.QueryString["SearchField"]) &&
                        !string.IsNullOrEmpty(Request.QueryString["SearchQuery"]));
            if (searchProperty.SearchMode)
            {
                searchProperty.SearchField = Request.QueryString["SearchField"];
                searchProperty.SearchQuery = Request.QueryString["SearchQuery"];
            }

            ++searchProperty.PageIndex; // 코드: 0, 1, 2 인덱스로 사용, UI: 1, 2, 3 페이지로 사용
            int i = 0;

            // <!--이전 10개, 다음 10개 페이징 처리 시작-->
            string strPage = "<ul class='pagination pagination-sm'>";
            if (searchProperty.PageIndex > 10) // 이전 10개 링크가 있다면, ...
            {
                // 검색 모드이면 추가적으로 SearchField와 SearchQuery를 전송함
                if (searchProperty.SearchMode)
                {
                    strPage += "<li><a href=\""
                        + Request.ServerVariables["SCRIPT_NAME"]
                        //+ "?BoardName=" + Request["BoardName"] // 멀티 게시판
                        + "?Page="
                        + Convert.ToString(((searchProperty.PageIndex - 1) / (int)10) * 10)
                        + "&SearchField=" + searchProperty.SearchField
                        + "&SearchQuery=" + searchProperty.SearchQuery + "\">◀</a></li>";
                }
                else
                {
                    strPage += "<li><a href=\""
                        + Request.ServerVariables["SCRIPT_NAME"]
                        //+ "?BoardName=" + Request["BoardName"]
                        + "?Page="
                        + Convert.ToString(((searchProperty.PageIndex - 1) / (int)10) * 10)
                        + "\">◀</a></li>";
                }
            }
            else
            {
                strPage += "<li class=\"disabled\"><a>◁</a></li>";
            }
            strPage += "&nbsp;";
            //Debug.WriteLine("총 페이지 수: " + searchProperty.PageCount);
            // 가운데, 숫자 형식의 페이저 표시
            for (
                i = (((searchProperty.PageIndex - 1) / (int)10) * 10 + 1);
                i <= ((((searchProperty.PageIndex - 1) / (int)10) + 1) * 10);
                i++)
            {
                if (i > searchProperty.PageCount)
                {
                    break; // 있는 페이지까지만 페이저 링크 출력
                }
                // 현재 보고 있는 페이지면, 활성화(active)
                if (i == searchProperty.PageIndex)
                {
                    strPage += " <li class='active'><a href='#'>"
                        + i.ToString() + "</a></li>";
                }
                else
                {
                    if (searchProperty.SearchMode)
                    {
                        strPage += "<li><a href=\""
                            + Request.ServerVariables["SCRIPT_NAME"]
                            //+ "?BoardName=" + Request["BoardName"]
                            + "?Page=" + i.ToString()
                            + "&SearchField=" + searchProperty.SearchField
                            + "&SearchQuery=" + searchProperty.SearchQuery + "\">"
                            + i.ToString() + "</a></li>";
                    }
                    else
                    {
                        strPage += "<li><a href=\""
                            + Request.ServerVariables["SCRIPT_NAME"]
                            //+ "?BoardName=" + Request["BoardName"]
                            + "?Page=" + i.ToString()
                            + "\">"
                            + i.ToString() + "</a></li>";
                    }
                    //Debug.WriteLine("이게뭐지? " + Request.ServerVariables["SCRIPT_NAME"]);
                }
                strPage += "&nbsp;";
            }

            // 다음 10개 링크
            if (i < searchProperty.PageCount) // 다음 10개 링크가 있다면, ...
            {
                if (searchProperty.SearchMode)
                {
                    strPage += "<li><a href=\""
                        + Request.ServerVariables["SCRIPT_NAME"]
                        //+ "?BoardName=" + Request["BoardName"]
                        + "?Page="
                        + Convert.ToString(((searchProperty.PageIndex - 1) / (int)10) * 10 + 11)
                        + "&SearchField=" + searchProperty.SearchField
                        + "&SearchQuery=" + searchProperty.SearchQuery + "\">▶</a></li>";
                }
                else
                {
                    strPage += "<li><a href=\""
                        + Request.ServerVariables["SCRIPT_NAME"]
                        //+ "?BoardName=" + Request["BoardName"]
                        + "?Page="
                        + Convert.ToString(((searchProperty.PageIndex - 1) / (int)10) * 10 + 11)
                        + "\">▶</a></li>";
                }
            }
            else
            {
                strPage += "<li class=\"disabled\"><a>▷</a></li>";
            }

            // <!--이전 10개, 다음 10개 페이징 처리 종료-->
            strPage += "</ul>";

            ctlPagingControl.Text = strPage;
        }
    }
}