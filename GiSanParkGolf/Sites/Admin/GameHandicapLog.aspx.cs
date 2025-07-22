using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GameHandicapLog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadHandicapLogs();
        }

        private void LoadHandicapLogs(string field = null, string keyword = null)
        {
            try
            {
                var logs = Global.dbManager.GetHandicapChangeLogs(field, keyword);

                gvLog.DataSource = logs;
                gvLog.DataBind();

                pager.CurrentPage = gvLog.PageIndex;
                pager.TotalPages = gvLog.PageCount;

                if (!logs.Any())
                {
                    lblModalMessage.Text = "핸디캡 변경 로그가 없습니다.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showModalScript", "showModal();", true);
                }
            }
            catch (Exception ex)
            {
                lblModalMessage.Text = $"조회 중 오류 발생: {ex.Message}";
                ScriptManager.RegisterStartupScript(this, GetType(), "showModalScript", "showModal();", true);
            }
        }

        protected void Search_SearchRequested(object sender, EventArgs e)
        {
            ViewState["SearchField"] = search.SelectedField;
            ViewState["SearchKeyword"] = search.Keyword;

            LoadHandicapLogs(search.SelectedField, search.Keyword);
        }

        protected void Search_ResetRequested(object sender, EventArgs e)
        {
            ViewState.Remove("SearchField");
            ViewState.Remove("SearchKeyword");

            LoadHandicapLogs(); // 전체 조회
        }

        protected void gvLog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLog.PageIndex = e.NewPageIndex;
            LoadHandicapLogs();
        }

        protected void gvLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int rowIndex = e.Row.RowIndex + (gvLog.PageSize * gvLog.PageIndex);
                e.Row.Cells[0].Text = (rowIndex + 1).ToString();
            }
        }

        protected void Pager_PageChanged(object sender, int newPage)
        {
            gvLog.PageIndex = newPage;

            string field = ViewState["SearchField"] as string;
            string keyword = ViewState["SearchKeyword"] as string;

            LoadHandicapLogs(field, keyword);
        }
    }
}