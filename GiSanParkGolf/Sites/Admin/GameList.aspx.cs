using static GiSanParkGolf.Global;
using System;
using System.Web.UI;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GameList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.User.Identity.IsAuthenticated)
            {
                if (!Global.uvm.UserClass.Equals(1))
                {
                    Response.Redirect("~/Sites/Login/Admin Alert.aspx");
                    return;
                }
            }
            else
            {
                Response.Redirect("~/Sites/Login/Admin Alert.aspx");
                return;
            }

            if (!Page.IsPostBack)
            {
                Load_GameList();
            }
        }

        protected void Load_GameList()
        {
            GridView1.DataSource = Global.dbManager.GetGameList(0);
            GridView1.DataBind();           
        }
    }
}