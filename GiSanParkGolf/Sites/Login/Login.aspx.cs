using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using GiSanParkGolf.Class;

namespace GiSanParkGolf
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            DB_Management userRepo = new DB_Management();
            if (userRepo.IsCorrectUser(txtUserID.Text, txtPassword.Text).Equals("OK"))
            {
                // [!] 인증 부여
                if (!String.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                {
                    Debug.WriteLine(Request.QueryString["ReturnUrl"].ToString());
                    // 인증 쿠키값 부여
                    FormsAuthentication.RedirectFromLoginPage(txtUserID.Text, false);
                }
                else
                {
                    // 인증 쿠키값 부여(처음 인증시)
                    FormsAuthentication.SetAuthCookie(txtUserID.Text, false);
                    Response.Redirect("~/Default.aspx");
                }
            } else if (userRepo.IsCorrectUser(txtUserID.Text, txtPassword.Text).Equals("Ready"))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showMsg", "<script>alert('승인 대기중입니다.');</script>");
            } else
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showMsg", "<script>alert('아이디 또는 비밀번호가 틀렸습니다.');</script>");
        }

        protected void BtnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Sites/UserManagement/UserRepositories.aspx");
        }
    }
}