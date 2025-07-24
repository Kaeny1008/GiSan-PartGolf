using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
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
            Page.Validate("UserLogin");
            if (!Page.IsValid)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validationModalScript", "showValidate();", true);
                return;
            }

            string result = Global.dbManager.IsCorrectUser(txtUserID.Text, txtPassword.Text, 0);

            switch (result)
            {
                case "OK":
                    var userInfo = Global.dbManager.GetUserByUserID(txtUserID.Text);
                    if (userInfo == null)
                    {
                        ShowAlert("사용자 정보를 불러오는 데 실패했습니다.");
                        return;
                    }

                                     // Session 또는 Claims 기반으로 저장 가능
                    Session["UserInfo"] = userInfo;

                                    // 인증 쿠키 발급
                                    //여기 주석 GetUserByUserID 여기에서 먼저 쿠키 발급이 일어난다.
                    //FormsAuthentication.SetAuthCookie(txtUserID.Text, false);

                    string returnUrl = Request.QueryString["ReturnUrl"];
                    Response.Redirect(string.IsNullOrEmpty(returnUrl) ? "~/Default.aspx" : returnUrl);
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
            ScriptManager.RegisterStartupScript(this, GetType(), "msgModal", 
                $"showMessage('{message}');", true);
        }

        protected void BtnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Sites/UserManagement/UserRepositories.aspx");
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
                    Global.dbManager.IsCorrectUser(user.UserId, user.UserPassword, 1);

                    return true;
                }
            }
            return false;
        }
    }
}