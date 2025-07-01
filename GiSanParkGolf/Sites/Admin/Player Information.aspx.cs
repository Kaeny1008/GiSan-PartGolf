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
            DB_Management userRepo = new DB_Management();
            Global.suvm = userRepo.GetSelectUserByUserID(userID);

            txtID.Text = Global.suvm.UserID;
            txtName.Text = Global.suvm.UserName;
            txtBirthDay.Text = Global.suvm.UserNumber.ToString();
            txtGender.Text = Global.suvm.UserGender.ToString();
            txtAddress.Text = Global.suvm.UserAddress;
            txtAddress2.Text = Global.suvm.UserAddress2;
            txtMemo.Text = Global.suvm.UserNote;
            if (Global.suvm.UserWClass.Equals("승인"))
            {
                switchCheckDefault.Checked = true;

            } else
            {
                switchCheckDefault.Checked = false;
            }
            DropDownList1.Text = Global.suvm.UserClass.ToString();
            //txtID.Text = Global.suvm.UserWClass;
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