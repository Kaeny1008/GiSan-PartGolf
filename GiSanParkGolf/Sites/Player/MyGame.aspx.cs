using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Player
{
    public partial class MyGame : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 참가취소로 온경우
            if (!string.IsNullOrEmpty(Request.QueryString["GameCancel"]))
            {

            }

            if (!Page.IsPostBack)
            {
                GameList.DataSource = Global.dbManager.GetMyGameList(Global.uvm.UserID);
                GameList.DataBind();
            }
        }
    }
}