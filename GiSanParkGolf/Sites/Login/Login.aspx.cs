using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            string result = Global.dbManager.IsCorrectUser(txtUserID.Text, txtPassword.Text);

            switch (result)
            {
                case "OK":
                    // 로그인 인증 되었으면 유저 정보를 불러온다.
                    // 쿠기도 생성
                    Global.uvm = Global.dbManager.GetUserByUserID(txtUserID.Text);

                    if (!String.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                    {
                        // 인증 쿠키값 부여(돌아가는 곳이 있는 경우)
                        //FormsAuthentication.RedirectFromLoginPage(txtUserID.Text, false);
                        Response.Redirect(Request.QueryString["ReturnUrl"]);
                    }
                    else
                    {
                        // 인증 쿠키값 부여(돌아가는 곳이 없을 경우)
                        //FormsAuthentication.SetAuthCookie(txtUserID.Text, false);
                        Response.Redirect("~/Default.aspx");
                    }
                    break;
                case "Logged in":
                    ShowAlert("이미 로그인된 사용자입니다.");
                    break;
                case "Ready":
                    ShowAlert("승인 대기중입니다.");
                    break;
                default:
                    ShowAlert("아이디 또는 비밀번호가 틀렸습니다.");
                    break;
            }
        }

        protected void ShowAlert(string message)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showMsg", "<script>alert('" + message + "');</script>");
        }

        protected void BtnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Sites/UserManagement/UserRepositories.aspx");
        }
    }
}