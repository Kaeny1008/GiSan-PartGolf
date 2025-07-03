using System;
using System.Diagnostics;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using static GiSanParkGolf.Global;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class Player_Management : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.User.Identity.IsAuthenticated)
            {
                if (!Global.uvm.UserClass.Equals(1))
                {
                    Response.Redirect("~/Sites/Login/Admin Alert.aspx");
                    return;
                }
            } 
            else
            {
                Response.Redirect("~/Sites/Login/Admin Alert.aspx");
                return;
            }
            
            if (!string.IsNullOrEmpty(Request.QueryString["ReadyUser"]))
            {
                searchProperty.ReadyUser = Request.QueryString["ReadyUser"];
            }
            else
            {
                searchProperty.ReadyUser = "False";
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
            if (Request.Cookies["PlayerManagement"] != null)
            {
                if (!String.IsNullOrEmpty(
                    Request.Cookies["PlayerManagement"]["PageNum"]))
                {
                    searchProperty.PageIndex = Convert.ToInt32(
                        Request.Cookies["PlayerManagement"]["PageNum"]);
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
                    Global.dbManager.GetUserCountAll(searchProperty.ReadyUser);
            }
            else
            {
                // Notes 테이블 중 searchProperty.SearchField+searchProperty.SearchQuery에 해당하는 레코드 수
                searchProperty.RecordCount = 
                    Global.dbManager.GetUserCountBySearch(searchProperty.SearchField, searchProperty.SearchQuery, searchProperty.ReadyUser);
            }
            lblTotalRecord.Text = searchProperty.RecordCount.ToString();

            // 페이징 처리... 나중에 좀 쌓였을때 확인 해봐야할듯..
            //pageProperty.PageIndex = searchProperty.PageIndex;
            //pageProperty.RecordCount = searchProperty.RecordCount;

            if (!Page.IsPostBack)
            {
                PlayerList();
            }
        }

        private void PlayerList()
        {
            if (searchProperty.SearchMode == false) // 기본 리스트
            {
                GridView1.DataSource = 
                    Global.dbManager.GetUserAll(
                        searchProperty.PageIndex, 
                        searchProperty.ReadyUser)
                    ;
            }
            else // 검색 결과 리스트
            {
                GridView1.DataSource = 
                    Global.dbManager.GetUserSeachAll(
                        searchProperty.PageIndex, 
                        searchProperty.ReadyUser, 
                        searchProperty.SearchField, 
                        searchProperty.SearchQuery)
                    ;
            }
            
            GridView1.DataBind();
        }

        //사용하진 않지만 행의 번호를 확인하는 방법이라 지우지 않음.
        protected void MyButtonClick(object sender, System.EventArgs e)
        {
            Button btn = (Button)sender;

            GridViewRow gvr = (GridViewRow)btn.NamingContainer;
            int x = gvr.RowIndex;
            GridView1.Rows[x].BackColor = Color.AliceBlue;

            string userid = GridView1.Rows[x].Cells[1].Text;
            
            Response.Redirect("~/Sites/Admin/Player Information.aspx?UserId=" + userid);
        }
    }
}