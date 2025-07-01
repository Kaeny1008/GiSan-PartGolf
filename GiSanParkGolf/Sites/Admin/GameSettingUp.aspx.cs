using GiSanParkGolf.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GameSettingUp : System.Web.UI.Page
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
            ;
            
        }
    }
}