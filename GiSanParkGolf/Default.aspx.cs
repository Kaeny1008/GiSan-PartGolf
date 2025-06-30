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

                }
                else
                {
                    // 로그인 페이지로 이동
                    Response.Redirect("~/Sites/Login/Login.aspx?ReturnUrl=%2fDefault.aspx");
                }  
            } else
            {
                // [!] 인증 이름 출력
                //lblName.Text = Page.User.Identity.Name;
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
                Debug.WriteLine("로그인 이름: " + userData);
                Debug.WriteLine("만든날짜: " + issueDate);
                Debug.WriteLine("파기날짜: " + expiredDate);
                if (DateTime.Now > expiredDate)
                {
                    return false;
                }
                else 
                {

                    //Global.uvm.UserID = sqlDR.GetString(0);
                    //Global.uvm.Password = sqlDR.GetString(1);
                    //Global.uvm.UserName = sqlDR.GetString(2);
                    //Global.uvm.UserWClass = sqlDR.GetString(3);
                    //Global.uvm.UserClass = sqlDR.GetInt32(4);
                }
                return true;
            }
            return false;
        }
    }
}