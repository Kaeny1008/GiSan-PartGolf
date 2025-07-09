using static GiSanParkGolf.Global;
using System;
using System.Web.UI;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GameList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Global.uvm.UserClass.Equals(1))
            {
                Response.Redirect("~/Sites/Login/Admin Alert.aspx");
                return;
            }

            // 검색 모드 결정
            searchProperty.SearchMode =
                    (!string.IsNullOrEmpty(Request.QueryString["SearchField"]) &&
                        !string.IsNullOrEmpty(Request.QueryString["SearchQuery"]));
            if (searchProperty.SearchMode)
            {
                searchProperty.SearchField = Request.QueryString["SearchField"];
                searchProperty.SearchQuery = Request.QueryString["SearchQuery"];
            }

            // 쿼리스트링에 따른 페이지 보여주기
            if (Request["Page"] != null)
            {
                // Page는 보여지는 쪽은 1, 2, 3, ... 코드단에서는 0, 1, 2, ...
                searchProperty.PageIndex = Convert.ToInt32(Request["Page"]) - 1;
            }
            else
            {
                searchProperty.PageIndex = 0; // 1페이지
            }

            // 쿠키를 사용한 리스트 페이지 번호 유지 적용: 
            // 100번째 페이지의 글 보고, 다시 리스트 왔을 때 100번째 페이지 표시
            if (Request.Cookies["GameList"] != null)
            {
                if (!String.IsNullOrEmpty(
                    Request.Cookies["GameList"]["PageNum"]))
                {
                    searchProperty.PageIndex = Convert.ToInt32(
                        Request.Cookies["GameList"]["PageNum"]);
                }
                else
                {
                    searchProperty.PageIndex = 0;
                }
            }

            //레코드 카운트 출력
            if (searchProperty.SearchMode == false)
            {
                // 테이블의 전체 레코드
                searchProperty.RecordCount =
                    Global.dbManager.GetGameCountAll();
            }
            else
            {
                // Notes 테이블 중 searchProperty.SearchField+searchProperty.SearchQuery에 해당하는 레코드 수
                searchProperty.RecordCount =
                    Global.dbManager.GetGameCountBySearch(searchProperty.SearchField, searchProperty.SearchQuery);
            }
            lblTotalRecord.Text = searchProperty.RecordCount.ToString();

            if (!Page.IsPostBack)
            {
                Load_GameList();
            }
        }

        protected void Load_GameList()
        {
            if (searchProperty.SearchMode == false) // 기본 리스트
            {
                GridView1.DataSource = 
                    Global.dbManager.GetGameALL(
                        searchProperty.PageIndex
                        );
            }
            else
            {
                GridView1.DataSource =
                    Global.dbManager.GetGameSeachAll(
                        searchProperty.PageIndex,
                        searchProperty.SearchField,
                        searchProperty.SearchQuery
                        );
            }

                GridView1.DataBind();
        }
    }
}