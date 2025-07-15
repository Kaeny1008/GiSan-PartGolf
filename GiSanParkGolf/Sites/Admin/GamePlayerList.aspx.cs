using GiSanParkGolf.Class;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GamePlayerList : System.Web.UI.Page
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

        protected void BTN_ToExcel_Click(object sender, EventArgs e)
        {
            //Create a dummy GridView.
            GridView gvCustomers = new GridView();
            gvCustomers.DataSource = Global.dbManager.GetGameUserList(Request.QueryString["GameCode"]);
            gvCustomers.DataBind();

            string fileName = HttpUtility.UrlEncode("PlayerList.xls", new UTF8Encoding());

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    foreach (GridViewRow row in gvCustomers.Rows)
                    {
                        //Apply text style to each Row.
                        row.Attributes.Add("class", "textmode");
                    }
                    gvCustomers.RenderControl(hw);

                    //Style to format numbers to string.
                    string style = @"<style> .textmode { mso-number-format:\@; } </style>";
                    Response.Write(style);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // GridView 등 서버 컨트롤을 직접 렌더링할 때 예외 방지
        }
    }
}