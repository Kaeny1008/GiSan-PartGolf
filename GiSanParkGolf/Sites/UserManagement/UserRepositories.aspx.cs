using GiSanParkGolf.Class;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.UserManagement
{
    public partial class UserRepositories : System.Web.UI.Page
    {
        private OleDbConnection con;
        protected void Page_Load(object sender, EventArgs e)
        {
            TextBox1.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }
        protected void BTN_Cancel_Click(object sender, EventArgs e)
        {
            txtID.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtReCheck.Text = string.Empty;
            txtName.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtAddress2.Text = string.Empty;
            txtID.Enabled = true;
        }
        protected void BTN_Register_Click(object sender, EventArgs e)
        {
            //if (txtID.Text.Trim().Equals(string.Empty))
            //{
            //    label6.Text = "아이디는 필수로 입력하여야 합니다.";
            //    return;
            //}

            //if (txtPassword.Text.Trim().Equals(string.Empty))
            //{
            //    label6.Text = "암호는 필수로 입력하여야 합니다.";
            //    return;
            //}

            //if (txtName.Text.Trim().Equals(string.Empty))
            //{
            //    label6.Text = "성명은 필수로 입력하여야 합니다.";
            //    return;
            //}

            //if (DropDownList1.Text.Trim().Equals("선택"))
            //{
            //    label6.Text = "성별은 필수로 선택하여야 합니다.";
            //    return;
            //}
            //if (TextBox1.Text.Trim().Equals(string.Empty))
            //{
            //    label6.Text = "생년월일은 필수로 입력하여야 합니다.";
            //    return;
            //}

            //if (txtAddress.Text.Trim().Equals(string.Empty))
            //{
            //    label6.Text = "주소는 필수로 입력하여야 합니다.";
            //    return;
            //}

            if (Page.IsValid)
            {
                // 데이터 저장
                string strSQL = "INSERT INTO USER_INFORMATION(UserName, UserGender, UserBirthOfDate, UserAddress, UserAddress2" +
                    ", UserRegistrationDate, UserNote, UserId, UserPassword, UserWClass" +
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
                    ",'승인대기'" +
                    ");";

                DB_Management dbWrite = new DB_Management();
                string writeResult = dbWrite.DB_Write(strSQL);

                if (writeResult.Equals("Success"))
                {
                    string strJs = "<script>alert('가입 승인대기 되었습니다.'); location.href='/Default.aspx';</script>";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "goDefault", strJs);
                }
                else
                {
                    string strAlarm = @"<script language='JavaScript'>window.alert('";
                    strAlarm += writeResult;
                    strAlarm += "');</script>";
                    Response.Write(strAlarm);
                }
            } else
            {

            }
        }

        protected void Btn_IDCheck_Click(object sender, EventArgs e)
        {
            if (txtID.Text.Equals(string.Empty))
            {
                return;
            }

            DB_Management IDCheck = new DB_Management();
            Boolean checkResult = IDCheck.ID_DuplicateCheck(txtID.Text);

            if (checkResult == false)
            {
                Label13.Text = "ID를 사용 할 수 없습니다.";
                Label13.ForeColor = Color.Red;
                TextBox2.Text = "Ready";
            }
            else
            {
                Label13.Text = "ID를 사용 할 수 있습니다.";
                Label13.ForeColor = Color.Black;
                TextBox2.Text = "OK";
                txtID.Enabled = false;
            }
        }
    }
}