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

        private void LoadHandicapLogs()
        {
            try
            {
                var logs = Global.dbManager.GetHandicapChangeLogs()
                    .OrderByDescending(log => log.ChangedAt)
                    .ToList();

                gvLog.DataSource = logs;
                gvLog.DataBind();
            }
            catch (Exception ex)
            {
                lblModalMessage.Text = $"⚠️ 조회 중 오류 발생: {ex.Message}";
                ScriptManager.RegisterStartupScript(this, GetType(), "showModalScript", "showModal();", true);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtUserId.Text?.Trim();

            try
            {
                var logs = Global.dbManager.GetHandicapChangeLogs();

                var filtered = string.IsNullOrEmpty(keyword)
                    ? logs
                    : logs.Where(log =>
                           (log.UserId?.Contains(keyword) ?? false) ||
                           (log.UserName?.Contains(keyword) ?? false))
                           .ToList();

                gvLog.DataSource = filtered;
                gvLog.DataBind();
            }
            catch (Exception ex)
            {
                lblModalMessage.Text = $"⚠️ 오류 발생: {ex.Message}";
                ScriptManager.RegisterStartupScript(this, GetType(), "showModalScript", "showModal();", true);
            }
        }

        protected void gvLog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLog.PageIndex = e.NewPageIndex;
            LoadHandicapLogs(); // 기존 데이터 로딩 함수
        }
        protected void gvLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int rowIndex = e.Row.RowIndex + 1 + (gvLog.PageSize * gvLog.PageIndex);
                e.Row.Cells[0].Text = rowIndex.ToString(); // 첫 번째 컬럼에 번호 출력
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtUserId.Text = string.Empty;               // 검색어 초기화
            LoadHandicapLogs();                          // 전체 데이터 다시 불러오기
        }

    }
}