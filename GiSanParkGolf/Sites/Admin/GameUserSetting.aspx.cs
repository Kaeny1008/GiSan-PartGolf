using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GameUserSetting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Global.uvm.UserClass.Equals(1))
            {
                Response.Redirect("~/Sites/Login/Admin Alert.aspx");
                return;
            }

            if (!Page.IsPostBack)
            {
                GameList.DataSource = Global.dbManager.GetMyGameList(Global.uvm.UserID);
                GameList.DataBind();
            }
        }
    }
}