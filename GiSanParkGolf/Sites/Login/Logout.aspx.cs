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
            userRepo.LogoutUser(Helper.CurrentUser?.UserId);

            // 세션 제거
            Session.Remove("UserInfo");      // 특정 키만 제거
            Session.Clear();                 // 전체 세션 제거
            Session.Abandon();               // 세션 무효화

            // 인증 쿠키 제거
            FormsAuthentication.SignOut();

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName)
            {
                Expires = DateTime.Now.AddDays(-1)
            };
            HttpContext.Current.Response.Cookies.Add(cookie);

            Response.Redirect("~/Login.aspx");
        }

    }
}