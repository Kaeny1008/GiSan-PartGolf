using GiSanParkGolf.Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            // [!] 인증 여부 확인 : 로그인했으면 참, 그렇지 않으면 거짓을 반환
            if (!Page.User.Identity.IsAuthenticated)
            {
                if (LoadCookie())
                {
                    //쿠키를 불러왔다.
                    Debug.WriteLine("인증이 되어 있지 않으므로 쿠키를 확인한다.");
                }
                else
                {
                    // 로그인 페이지로 이동
                    Debug.WriteLine("쿠키도 없으므로 로긴페이지로 이동한다.");
                    Response.Redirect("~/Sites/Login/Login.aspx?ReturnUrl=%2fDefault.aspx");
                }  
            } else
            {
                // [!] 인증 이름 출력
                //lblName.Text = Page.User.Identity.Name;
                Debug.WriteLine("인증이 되어 있으므로 쿠키를 불러온다.");
                LoadCookie();
            }
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

                Debug.WriteLine("로그인 ID: " + userName);
                Debug.WriteLine("Cookie Data: " + userData);
                Debug.WriteLine("만든날짜: " + issueDate);
                Debug.WriteLine("파기날짜: " + expiredDate);
                string[] abcd = userData.Split(':');

                if (DateTime.Now > expiredDate)
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
                }
                return true;
            }
            return false;
        }
    }
}