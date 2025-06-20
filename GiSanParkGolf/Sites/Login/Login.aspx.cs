using GiSanParkGolf.Models;
using GiSanParkGolf.Repositories;
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

namespace GiSanParkGolf
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            UserRepository userRepo = new UserRepository();
            if (userRepo.IsCorrectUser(txtUserID.Text, txtPassword.Text))
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
            }
            else
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showMsg", "<script>alert('잘못된 사용자입니다.');</script>");
        }
    }
}