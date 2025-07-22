using GiSanParkGolf.Class;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace GiSanParkGolf
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // [!] 인증 여부 확인 : 로그인했으면 참, 그렇지 않으면 거짓을 반환
            if (!Page.User.Identity.IsAuthenticated)
            {
                Debug.WriteLine("인증이 되어 있지 않으므로 쿠키를 확인한다.");
                if (LoadCookie())
                {
                    //쿠키를 불러왔다.
                    Debug.WriteLine("쿠키가 확인되어 페이지를 이동한다.");
                    Response.Redirect("~/Default.aspx");
                }
                else
                {
                    // 로그인 페이지로 이동
                    Debug.WriteLine("쿠키도 없으므로 로그인을 기다린다.");
                }
            }
            else
            {
                // [!] 인증 이름 출력
                //lblName.Text = Page.User.Identity.Name;
                Debug.WriteLine("인증이 되어 있으므로 쿠키를 불러온다.");
                if (LoadCookie())
                {
                    Debug.WriteLine("쿠키가 확인되어 페이지를 이동한다.");
                    Response.Redirect("~/Default.aspx");
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Page.Validate("UserLogin");  // 수동 그룹 유효성 검사
            if (!Page.IsValid)
            {
                //ScriptManager.RegisterStartupScript(this, GetType(), "validationModalScript", "ShowValidationModal();", true);
                //ClientScript.RegisterStartupScript(this.GetType(), "key", "launchModal();", true);
                ScriptManager.RegisterStartupScript(this, GetType(), "validationModalScript", "showValidat();", true);

                return;
            }

            string result = Global.dbManager.IsCorrectUser(txtUserID.Text, txtPassword.Text, 0);

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
                    ShowAlert("승인 대기중입니다. 관리자 승인을 기다려주세요.");
                    break;
                default:
                    ShowAlert("아이디 또는 비밀번호가 틀렸습니다.");
                    break;
            }
        }

        protected void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "msgModal", @"
                showMessage('" + message + "');", true);
        }

        protected void BtnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Sites/UserManagement/UserRepositories.aspx");
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
                    Global.uvm.UserId = abcd[0];
                    Global.uvm.UserPassword = abcd[1];
                    Global.uvm.UserName = abcd[2];
                    Global.uvm.UserWClass = abcd[3];
                    Global.uvm.UserClass = int.Parse(abcd[4]);

                    //파기 시간을 2일 연장한다.
                    DB_Management userRepo = new DB_Management();
                    userRepo.SetCookie(abcd[0], abcd[1], abcd[2], abcd[3], int.Parse(abcd[4]), 2);

                    //DB에 로그인 기록을 남긴다.
                    //Global.dbManager.IsCorrectUser(abcd[0], abcd[1], 1);
                }
                return true;
            }
            return false;
        }
    }
}