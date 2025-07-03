using GiSanParkGolf.BBS.Controls;
using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.EnterpriseServices;
using System.Web.Configuration;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class Player_Management : Page
    {
        // 공통 속성: 검색 모드: 검색 모드이면 true, 그렇지 않으면 false.
        // [참고] 이러한 공통 속성들은 Base 클래스에 모아 넣고 상속해줘도 좋음
        public bool SearchMode { get; set; } = false;
        public string SearchField { get; set; }
        public string SearchQuery { get; set; }
        public string ReadyUser { get; set; }

        public int pageIndex = 0; // 현재 보여줄 페이지 번호
        public int recordCount = 0; // 총 레코드 개수

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.User.Identity.IsAuthenticated)
            {
                Debug.WriteLine("선수정보 관리 : 로그인 되어 있다.");
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
                ReadyUser = Request.QueryString["ReadyUser"];
            }
            else
            {
                ReadyUser = "False";
            }

                // 검색 모드 결정
                SearchMode =
                    (!string.IsNullOrEmpty(Request.QueryString["SearchField"]) &&
                        !string.IsNullOrEmpty(Request.QueryString["SearchQuery"]));
            if (SearchMode)
            {
                SearchField = Request.QueryString["SearchField"];
                SearchQuery = Request.QueryString["SearchQuery"];
            }

            // 쿼리스트링에 따른 페이지 보여주기
            if (Request["Page"] != null)
            {
                // Page는 보여지는 쪽은 1, 2, 3, ... 코드단에서는 0, 1, 2, ...
                pageIndex = Convert.ToInt32(Request["Page"]) - 1;
            }
            else
            {
                pageIndex = 0; // 1페이지
            }

            // 쿠키를 사용한 리스트 페이지 번호 유지 적용: 
            // 100번째 페이지의 글 보고, 다시 리스트 왔을 때 100번째 페이지 표시
            if (Request.Cookies["PlayerManagement"] != null)
            {
                if (!String.IsNullOrEmpty(
                    Request.Cookies["PlayerManagement"]["PageNum"]))
                {
                    pageIndex = Convert.ToInt32(
                        Request.Cookies["PlayerManagement"]["PageNum"]);
                }
                else
                {
                    pageIndex = 0;
                }
            }

            //레코드 카운트 출력
            if (SearchMode == false)
            {
                // 테이블의 전체 레코드
                recordCount = Global.dbManager.GetUserCountAll(ReadyUser);
            }
            else
            {
                // Notes 테이블 중 SearchField+SearchQuery에 해당하는 레코드 수
                recordCount = Global.dbManager.GetUserCountBySearch(SearchField, SearchQuery, ReadyUser);
            }
            lblTotalRecord.Text = recordCount.ToString();

            // 페이징 처리
            PagingControl.PageIndex = pageIndex;
            PagingControl.RecordCount = recordCount;

            if (!Page.IsPostBack)
            {
                PlayerList();
            }
        }

        private void PlayerList()
        {
            if (SearchMode == false) // 기본 리스트
            {
                GridView1.DataSource = Global.dbManager.GetUserAll(pageIndex, ReadyUser);
            }
            else // 검색 결과 리스트
            {
                GridView1.DataSource = Global.dbManager.GetUserSeachAll(pageIndex, ReadyUser, SearchField, SearchQuery);
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