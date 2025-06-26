using GiSanParkGolf.Models;
using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class Player_Management : Page
    {
        private SqlConnection con;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!Page.User.Identity.IsAuthenticated)
                {
                    Response.Redirect("~/Sites/Login/Login.aspx");
                    return;
                }

                PlayerList(string.Empty, false);
            }

            //알림창 띄우는거임
            //string strJs = @"<script language='JavaScript'>window.alert('안녕');</script>";
            //Response.Write(strJs);
        }

        private void PlayerList(string userName, Boolean readyUser)
        {
            string strSQL = "SELECT UserWClass, UserId, UserName, UserNumber, UserNote FROM SYS_Users";
            strSQL += " where UserName like '%' + '" + userName + "' + '%'";
            if (readyUser)
            {
                strSQL += " and UserWClass = '승인대기'";
            }
            strSQL += ";";
            SqlCommand sqlCmd = new SqlCommand();
            con = new SqlConnection();
            con.ConnectionString = WebConfigurationManager.ConnectionStrings["ParkGolfDB"].ConnectionString;

            sqlCmd.Connection = con;
            sqlCmd.CommandText = strSQL;
            sqlCmd.CommandType = System.Data.CommandType.Text;

            con.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(sqlCmd);

            da.Fill(ds);
            GridView1.DataSourceID = "";         //그리드 뷰 데이터 초기화
            GridView1.DataSource = ds;
            GridView1.DataKeyNames = new string[] { "UserId" };
            GridView1.DataBind();
            con.Close();
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
                PlayerList(TB_Search.Text, true);
            } else
            {
                PlayerList(TB_Search.Text, false);
            }
                
        }

        protected void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox1.Checked)
            {
                PlayerList(TB_Search.Text, true);
            }
            else
            {
                PlayerList(TB_Search.Text, false);
            }
        }
    }
}