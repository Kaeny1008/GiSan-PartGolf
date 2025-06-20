using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites
{
    public partial class Player_Register : System.Web.UI.Page
    {
        private OleDbConnection con;
        protected void Page_Load(object sender, EventArgs e)
        {
            TextBox1.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        protected void BTN_Register_Click(object sender, EventArgs e)
        {
            if (txtID.Text.Trim().Equals(string.Empty))
            {
                label6.Text = "아이디는 필수로 입력하여야 합니다.";
                return;
            }

            if (txtPassword.Text.Trim().Equals(string.Empty))
            {
                label6.Text = "암호는 필수로 입력하여야 합니다.";
                return;
            }

            if (txtName.Text.Trim().Equals(string.Empty))
            {
                label6.Text = "성명은 필수로 입력하여야 합니다.";
                return;
            }

            if (DropDownList1.Text.Trim().Equals("선택"))
            {
                label6.Text = "성별은 필수로 선택하여야 합니다.";
                return;
            }
            if (TextBox1.Text.Trim().Equals(string.Empty))
            {
                label6.Text = "생년월일은 필수로 입력하여야 합니다.";
                return;
            }

            if (txtAddress.Text.Trim().Equals(string.Empty))
            {
                label6.Text = "주소는 필수로 입력하여야 합니다.";
                return;
            }
            // 데이터 저장
            try
            {
                con = new OleDbConnection();
                con.ConnectionString = WebConfigurationManager.ConnectionStrings["MDB_ConnectionString"].ConnectionString;
                con.Open();

                string strSQL = "INSERT INTO USER_INFORMATION(UserName, UserGender, UserBirthOfDate, UserAddress, UserAddress2" +
                    ", UserRegistrationDate, UserNote, UserId, UserPassword" +
                    ") VALUES(" +
                    "'" + txtName.Text + "'" +
                    ",'" + DropDownList1.Text + "'" +
                    ",'" + String.Format("{0:yyyy-MM-dd}", DateTime.Parse(TextBox1.Text)) + "'" +
                    ",'" + txtAddress.Text + "'" +
                    ",'" + txtAddress2.Text + "'" +
                    ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
                    ",'" + txtMemo.Text + "'" +
                    ",'" + txtID.Text + "'" +
                    ",'" + txtPassword.Text + "'" +
                    ");";

                OleDbCommand cmd = new OleDbCommand
                {
                    Connection = con,
                    CommandText = strSQL,
                    CommandType = System.Data.CommandType.Text
                };
                cmd.ExecuteNonQuery();

                con.Close();
            }
            catch (OleDbException ex)
            {
                Console.WriteLine(ex.ToString());
                string strAlarm = @"<script language='JavaScript'>window.alert('";
                strAlarm += ex.ToString();
                strAlarm += "');</script>";
                Response.Write(strAlarm);
                return;
            }

            string strJs = "<script>alert('저장 완료.'); location.href='/Sites/Player Management.aspx';</script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "goDefault", strJs);
        }
    }
}