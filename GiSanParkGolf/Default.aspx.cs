using GiSanParkGolf.Class;
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
                    Response.Redirect("~/Login.aspx");
                }
                else
                {
                    Debug.WriteLine("인증되어 있지 않지만 쿠키기록이 있다.");
                }
            }
            else
            {
                if (!LoadCookie())
                {
                    Debug.WriteLine("인증은 되어 있지만 쿠키가 확인되지 않아 Login으로 이동한다.");
                    Response.Redirect("~/Login.aspx");
                }
                else
                {
                    Debug.WriteLine("인증되어 있고 쿠키도 확인 되었다.");
                }
            }

            NoticeList.DataSource = Global.dbManager.GetNoticeRecentPosts("notice");
            NoticeList.DataBind();
            GameList.DataSource = Global.dbManager.GetGameList(5);
            GameList.DataBind();
        }

        protected Boolean LoadCookie()
        {
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = Context.Request.Cookies[cookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = null;
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                string userName = authTicket.Name; //userName
                string userData = authTicket.UserData;
                DateTime issueDate = authTicket.IssueDate; //cookie created date
                DateTime expiredDate = authTicket.Expiration; //cookie expired date
                bool expired = authTicket.Expired;

                Debug.WriteLine("로그인 ID: " + userName);
                Debug.WriteLine("Cookie Data: " + userData);
                Debug.WriteLine("만든날짜: " + issueDate);
                Debug.WriteLine("파기날짜: " + expiredDate);
                string[] abcd = userData.Split(':');

                if (expired)
                {
                    Debug.WriteLine("파기일자가 도래하여 인증거부.");
                    return false;
                }
                else
                {
                    Debug.WriteLine("파기일자가 미도래하여 인증완료.");
                    Global.uvm.UserID = abcd[0];
                    Global.uvm.Password = abcd[1];
                    Global.uvm.UserName = abcd[2];
                    Global.uvm.UserWClass = abcd[3];
                    Global.uvm.UserClass = int.Parse(abcd[4]);

                    //파기 시간을 2일 연장한다.
                    DB_Management userRepo = new DB_Management();
                    userRepo.SetCookie(abcd[0], abcd[1], abcd[2], abcd[3], int.Parse(abcd[4]), 2);

                    //DB에 로그인 기록을 남긴다.
                    Global.dbManager.IsCorrectUser(abcd[0], abcd[1], 1);
                }
                return true;
            }
            return false;
        }
    }
}