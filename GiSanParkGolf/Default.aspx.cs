using GiSanParkGolf.Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.EnterpriseServices;
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
            NoticeList.DataSource = Global.dbManager.GetNoticeRecentPosts("notice");
            NoticeList.DataBind();
            GameList.DataSource = Global.dbManager.GetGameList(5);
            GameList.DataBind();
        }
    }
}