using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using static GiSanParkGolf.Global;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class Player_Management : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Global.uvm.UserClass.Equals(1))
            {
                Response.Redirect("~/Sites/Login/Admin Alert.aspx");
                return;
            }

            if (!Page.IsPostBack)
            {
                LoadPlayerData();
            }
        }

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

        private bool ReadyOnly
        {
            get => ViewState["ReadyOnly"] != null && (bool)ViewState["ReadyOnly"];
            set => ViewState["ReadyOnly"] = value;
        }

        protected void Search_SearchRequested(object sender, EventArgs e)
        {
            SearchField = search.SelectedField;
            SearchKeyword = search.Keyword;
            ReadyOnly = search.ReadyOnly;

            GridView1.PageIndex = 0;
            LoadPlayerData();
        }

        protected void Search_ResetRequested(object sender, EventArgs e)
        {
            ViewState.Remove("SearchField");
            ViewState.Remove("SearchKeyword");
            ViewState.Remove("ReadyOnly");

            GridView1.PageIndex = 0;
            LoadPlayerData();
        }

        protected void Pager_PageChanged(object sender, int newPage)
        {
            GridView1.PageIndex = newPage;
            LoadPlayerData();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int no = (GridView1.PageIndex * GridView1.PageSize) + e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = no.ToString(); // 첫 번째 컬럼에 No 표시
            }
        }

        private void LoadPlayerData()
        {
            // 검색 조건 준비
            string field = ViewState["SearchField"] as string;
            string keyword = ViewState["SearchKeyword"] as string;
            bool readyOnly = ViewState["ReadyOnly"] != null && (bool)ViewState["ReadyOnly"];

            // 조건 기반 데이터 조회 (SQL 필터링)
            var result = Global.dbManager.GetPlayers(field, keyword, readyOnly);

            // 바인딩
            GridView1.DataSource = result;
            GridView1.DataBind();

            // 총 건수 출력
            lblTotalRecord.Text = result.Count.ToString();

            // 페이징 연동
            pager.CurrentPage = GridView1.PageIndex;
            pager.TotalPages = GridView1.PageCount;
        }
    }
}