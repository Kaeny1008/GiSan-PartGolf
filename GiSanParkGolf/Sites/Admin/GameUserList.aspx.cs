using GiSanParkGolf.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GameUserList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Load_UserList();
            }
        }

        protected void Load_UserList()
        {
            GameList.DataSource = Global.dbManager.GetGameUserList(Request.QueryString["GameCode"]);
            GameList.DataBind();
        }
    }
}