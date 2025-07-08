using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace GiSanParkGolf.Sites.Player
{
    public partial class JoinGame : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["GameCode"]))
            {
                MainDIV.Visible = false;
                GameContent.Visible = true;

            }
            else
            {
                MainDIV.Visible = true; 
                GameContent.Visible = false;
            }

            GameList.DataSource = Global.dbManager.GetGameReadyList();
            GameList.DataBind();
        }
    }
}