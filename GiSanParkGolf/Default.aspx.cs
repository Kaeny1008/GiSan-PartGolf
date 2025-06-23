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
                Response.Redirect("~/Sites/Login/Login.aspx?ReturnUrl=%2fDefault.aspx"); // 로그인 페이지로 이동
            } else
            {
                // [!] 인증 이름 출력
                //lblName.Text = Page.User.Identity.Name;
            }
        }
    }
}