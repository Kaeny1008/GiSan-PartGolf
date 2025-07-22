using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using static GiSanParkGolf.Global;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GameList : System.Web.UI.Page
    {
        private string SearchField
        {
            get => ViewState["SearchField"]?.ToString();
            set => ViewState["SearchField"] = value;
        }

        private string SearchKeyword
        {
            get => ViewState["SearchKeyword"]?.ToString();
            set => ViewState["SearchKeyword"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Helper.RequireAdmin(this); // 관리자 확인

            if (!Page.IsPostBack)
            {
                LoadGameList();
            }
        }

        protected void Search_SearchRequested(object sender, EventArgs e)
        {
            SearchField = search.SelectedField;
            SearchKeyword = search.Keyword;
            GridView1.PageIndex = 0;
            LoadGameList();
        }

        protected void Search_ResetRequested(object sender, EventArgs e)
        {
            ViewState.Remove("SearchField");
            ViewState.Remove("SearchKeyword");
            GridView1.PageIndex = 0;
            LoadGameList();
        }

        protected void Pager_PageChanged(object sender, int newPage)
        {
            GridView1.PageIndex = newPage;
            LoadGameList();
        }

        private void LoadGameList()
        {
            // 검색 조건 준비
            string field = SearchField;
            string keyword = SearchKeyword;

            // SQL 조건 전달 → 필터된 결과만 가져오기
            var result = Global.dbManager.GetGames(field, keyword);

            // 바인딩
            GridView1.DataSource = result;
            GridView1.DataBind();

            lblTotalRecord.Text = result.Count.ToString();
            pager.CurrentPage = GridView1.PageIndex;
            pager.TotalPages = GridView1.PageCount;
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int no = (GridView1.PageIndex * GridView1.PageSize) + e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = no.ToString(); // No. 컬럼 출력
            }
        }
    }
}