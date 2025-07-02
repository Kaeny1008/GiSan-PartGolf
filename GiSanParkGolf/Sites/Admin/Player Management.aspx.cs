using GiSanParkGolf.Models;
using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.EnterpriseServices;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class Player_Management : Page
    {
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

            TB_Search.Attributes["onkeypress"] =
               "if (event.keyCode==13){" +
               ClientScript.GetPostBackEventReference(BTN_Search, "") + "; return false;}";

            // 쿼리스트링에 따른 페이지 보여주기
            Debug.WriteLine("요청 Page No. : " + Request["Page"]);
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
            if (Request.Cookies["BBS"] != null)
            {
                if (!String.IsNullOrEmpty(
                    Request.Cookies["BBS"]["PageNum"]))
                {
                    pageIndex = Convert.ToInt32(
                        Request.Cookies["BBS"]["PageNum"]);
                }
                else
                {
                    pageIndex = 0;
                }
            }

            // 레코드 카운트 출력
            //if (SearchMode == false)
            //{
            //    // Notes 테이블의 전체 레코드
            //    RecordCount =
            //        _repository.GetCountAll(bbsID);
            //}
            //else
            //{
            //    // Notes 테이블 중 SearchField+SearchQuery에 해당하는 레코드 수
            //    RecordCount =
            //        _repository.GetCountBySearch(SearchField, SearchQuery, bbsID);
            //}
            //lblTotalRecord.Text = RecordCount.ToString();
            lblTotalRecord.Text = "0";

            if (!Page.IsPostBack)
            {
                PlayerList(string.Empty, 0);
            }
        }

        private void PlayerList(string userName, int readyUser)
        {
            GridView1.DataSource = Global.dbManager.GetUserList(userName, readyUser, pageIndex);
            GridView1.DataBind();
        }

        protected void grdList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            mf_List();
        }
        protected void mf_List()
        {
            //SqlParameter[] param = {
            //                            new SqlParameter("@UserName", SqlDbType.VarChar, 20)
            //                        };

            //param[0].Value = UserName.Trim();

            //DataSet ds = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["..."].ToString(), CommandType.StoredProcedure, "프로시저명", param);

            //grd_List.DataSource = ds1.Tables[0];
            //grd_List.DataBind();
        }
        protected void MyButtonClick(object sender, System.EventArgs e)
        {
            Button btn = (Button)sender;

            GridViewRow gvr = (GridViewRow)btn.NamingContainer;
            int x = gvr.RowIndex;
            GridView1.Rows[x].BackColor = Color.AliceBlue;

            string userid = GridView1.Rows[x].Cells[1].Text;
            
            Response.Redirect("~/Sites/Admin/Player Information.aspx?UserId=" + userid);
        }

        protected void BTN_Search_Click(object sender, EventArgs e)
        {
            if (CheckBox1.Checked)
            {
                PlayerList(TB_Search.Text, 1);
            } else
            {
                PlayerList(TB_Search.Text, 0);
            }
                
        }

        protected void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox1.Checked)
            {
                PlayerList(TB_Search.Text, 1);
            }
            else
            {
                PlayerList(TB_Search.Text, 0);
            }
        }
    }
}