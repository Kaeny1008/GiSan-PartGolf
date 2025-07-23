using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using T_Engine;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class Player_Information : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Helper.RequireAdmin(this); // 관리자 확인

            if (!Page.IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["UserId"]))
                {
                    UserInformationLoad(Request.QueryString["UserId"]);
                }
            }
            
        }
        protected void UserInformationLoad(string userID)
        {
            SelectUserViewModel user = Global.dbManager.GetSelectUserByUserID(userID);

            if (user == null)
                return; // 또는 오류 처리

            txtID.Text = user.UserID;
            txtName.Text = user.UserName;
            txtBirthDay.Text = user.UserNumber.ToString();
            txtGender.Text = user.UserGender.ToString();
            txtAddress.Text = user.UserAddress;
            txtAddress2.Text = user.UserAddress2;
            txtMemo.Text = user.UserNote;

            switchCheckDefault.Checked = user.UserWClass == "승인";
            DropDownList1.Text = user.UserClass.ToString();
        }

        protected void BTN_Register_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //Debug.WriteLine(switchCheckDefault.Checked);
                //Debug.WriteLine(txtName.Text);
                //패스워드가 입력되었다면 암호화 하기.
                string newPassword = txtPassword.Text;

                if (!newPassword.Equals(string.Empty))
                {
                    Cryptography newCrypt = new Cryptography();
                    newPassword = newCrypt.GetEncoding("ParkGolf", txtPassword.Text);
                }

                UserModify(newPassword);
            }
        }

        private void UserModify(string userPassword)
        {
            //String.Format("{0:yyyy-MM-dd}", DateTime.Parse(TextBox1.Text))
            string strSQL = "UPDATE SYS_Users SET";
            strSQL += " UserName = '" + txtName.Text + "'";
            strSQL += ", UserNumber = '" + txtBirthDay.Text + "'";
            strSQL += ", UserGender = '" + txtGender.Text + "'";
            strSQL += ", UserAddress = '" + txtAddress.Text + "'";
            strSQL += ", UserAddress2 = '" + txtAddress2.Text + "'";
            strSQL += ", UserNote = '" + txtMemo.Text + "'";
            strSQL += ", UserClass = '" + DropDownList1.Text + "'";

            if (!userPassword.Equals(string.Empty))
            {
                strSQL += ", UserPassword = '" + txtPassword.Text + "'";
            }
            if (switchCheckDefault.Checked)
            {
                strSQL += ", UserWClass = '승인'";
            } else
            {
                strSQL += ", UserWClass = '승인대기'";
            }
                strSQL += " WHERE UserId = '" + txtID.Text + "'";
            strSQL += ";";

            Debug.WriteLine(strSQL);

            DB_Management dbWrite = new DB_Management();
            string writeResult = dbWrite.DB_Write(strSQL);

            if (writeResult.Equals("Success"))
            {
                //string strJs = "<script>alert('수정 되었습니다.'); location.href='/Sites/Admin/Player Management.aspx';</script>";
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "goDefault", strJs);
                Response.Redirect("/Sites/Admin/Player Management.aspx");
            }
            else
            {
                string strAlarm = @"<script language='JavaScript'>window.alert('";
                strAlarm += writeResult;
                strAlarm += "');</script>";
                Response.Write(strAlarm);
            }
        }
    }
}