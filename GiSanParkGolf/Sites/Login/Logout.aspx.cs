using GiSanParkGolf.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // [!] 로그아웃

            DB_Management userRepo = new DB_Management();
            userRepo.LogoutUser(Global.uvm.UserId);

            FormsAuthentication.SignOut();
            Global.uvm.UserId = null;
            Global.uvm.UserPassword = null;
            Global.uvm.UserName = null;
            Global.uvm.UserWClass = null;
            Global.uvm.UserClass = 0;

            //쿠키값 삭제
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName)
            {
                Expires = DateTime.Now.AddDays(-1)
            };
            HttpContext.Current.Response.Cookies.Add(cookie);

            Response.Redirect("~/Login.aspx");
        }
    }
}