using GiSanParkGolf.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GameHandicap : System.Web.UI.Page
    {
        private readonly DB_Management db = new DB_Management();

        private string SelectedGameCode => ddlGame.SelectedValue;
        private string SearchTerm
        {
            get => ViewState["SearchTerm"] as string ?? string.Empty;
            set => ViewState["SearchTerm"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGames();
                SearchTerm = string.Empty;
                BindGrid();
            }
        }

        private void LoadGames()
        {
            // 게임 목록 바인딩 (GameCode, GameName)
            var games = db.GetGames().ToList();

            ddlGame.DataSource = games;
            ddlGame.DataTextField = "GameName";
            ddlGame.DataValueField = "GameCode";
            ddlGame.DataBind();
        }

        private void BindGrid()
        {
            var list = db.GetHandicaps(SelectedGameCode, SearchTerm).ToList();
            gvHandicap.DataSource = list;
            gvHandicap.DataBind();
        }

        protected void ddlGame_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvHandicap.PageIndex = 0;
            BindGrid();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchTerm = txtSearch.Text.Trim();
            gvHandicap.PageIndex = 0;
            BindGrid();
        }

        protected void gvHandicap_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvHandicap.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvHandicap_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvHandicap.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvHandicap_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvHandicap.EditIndex = -1;
            BindGrid();
        }

        protected void gvHandicap_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var row = gvHandicap.Rows[e.RowIndex];
            string userId = gvHandicap.DataKeys[e.RowIndex].Value.ToString();
            int newHc = int.Parse(((TextBox)row.FindControl("txtHc")).Text);
            string newSrc = ((DropDownList)row.FindControl("ddlSource")).SelectedValue;

            db.UpdateHandicap(userId, SelectedGameCode, newHc, newSrc);

            gvHandicap.EditIndex = -1;
            lblMsg.CssClass = "text-success";
            lblMsg.Text = "핸디캡 정보가 수정되었습니다.";
            BindGrid();
        }

        protected void btnRecalc_Click(object sender, EventArgs e)
        {
            db.RecalculateAll(SelectedGameCode);

            lblMsg.CssClass = "text-success";
            lblMsg.Text = "전체 참가자 핸디캡을 자동 계산했습니다.";
            BindGrid();
        }
    }
}

