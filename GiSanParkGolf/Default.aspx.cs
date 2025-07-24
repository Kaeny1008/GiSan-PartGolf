using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.User.Identity.IsAuthenticated)
            {
                if (!LoadCookie())
                {
                    Debug.WriteLine("인증되어 있지 않고 쿠키도 확인되지 않아 Login으로 이동한다.");
                    Console.WriteLine("인증되어 있지 않고 쿠키도 확인되지 않아 Login으로 이동한다.");
                    Response.Redirect("~/Login.aspx");
                }
                else
                {
                    Debug.WriteLine("인증되어 있지 않지만 쿠키기록이 있다.");
                    Console.WriteLine("인증되어 있지 않지만 쿠키기록이 있다.");
                }
            }
            else
            {
                if (!LoadCookie())
                {
                    Debug.WriteLine("인증은 되어 있지만 쿠키가 확인되지 않아 Login으로 이동한다.");
                    Console.WriteLine("인증은 되어 있지만 쿠키가 확인되지 않아 Login으로 이동한다.");
                    Response.Redirect("~/Login.aspx");
                }
                else
                {
                    Debug.WriteLine("인증되어 있고 쿠키도 확인 되었다.");
                    Console.WriteLine("인증되어 있고 쿠키도 확인 되었다.");
                }
            }

            NoticeList.DataSource = Global.dbManager.GetNoticeRecentPosts("notice");
            NoticeList.DataBind();
            GameList.DataSource = Global.dbManager.GetGameList(5);
            GameList.DataBind();
        }

        protected bool LoadCookie()
        {
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = Context.Request.Cookies[cookieName];

            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                string userData = authTicket.UserData;
                bool expired = authTicket.Expired;

                string[] abcd = userData.Split(':');
                if (expired)
                {
                    Debug.WriteLine("파기일자가 도래하여 인증거부.");
                    return false;
                }
                else
                {
                    Debug.WriteLine("파기일자가 미도래하여 인증완료.");

                    var user = new UserViewModel
                    {
                        UserId = abcd[0],
                        UserPassword = abcd[1],
                        UserName = abcd[2],
                        UserWClass = abcd[3],
                        UserClass = int.Parse(abcd[4])
                    };

                    Session["UserInfo"] = user;  // ← 여기로 전역 대신 저장!

                    // 쿠키 갱신
                    DB_Management userRepo = new DB_Management();
                    userRepo.SetCookie(user.UserId, user.UserPassword, user.UserName, user.UserWClass, user.UserClass, 2);

                    // 로그인 기록 남기기
                    //Global.dbManager.IsCorrectUser(user.UserId, user.UserPassword, 1);

                    return true;
                }
            }
            return false;
        }
    }
}