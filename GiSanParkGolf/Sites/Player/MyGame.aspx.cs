using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Player
{
    public partial class MyGame : System.Web.UI.Page
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

        private static string gamecode = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadGameList();
            }
        }

        protected void Search_SearchRequested(object sender, EventArgs e)
        {
            SearchField = search.SelectedField;
            SearchKeyword = search.Keyword;
            GameList.PageIndex = 0;
            LoadGameList();
        }

        protected void Search_ResetRequested(object sender, EventArgs e)
        {
            ViewState.Remove("SearchField");
            ViewState.Remove("SearchKeyword");
            GameList.PageIndex = 0;
            LoadGameList();
        }

        protected void Pager_PageChanged(object sender, int newPage)
        {
            GameList.PageIndex = newPage;
            LoadGameList();
        }

        private void LoadGameList()
        {
            var games = Global.dbManager.GetMyGameList(Helper.CurrentUser?.UserId);

            IEnumerable<GameListModel> filtered = games;

            if (!string.IsNullOrEmpty(SearchField) && !string.IsNullOrEmpty(SearchKeyword))
            {
                string kw = SearchKeyword.ToLower();
                switch (SearchField)
                {
                    case "GameName":
                        filtered = filtered.Where(g => g.GameName?.ToLower().Contains(kw) == true);
                        break;
                    case "StadiumName":
                        filtered = filtered.Where(g => g.StadiumName?.ToLower().Contains(kw) == true);
                        break;
                    case "GameHost":
                        filtered = filtered.Where(g => g.GameHost?.ToLower().Contains(kw) == true);
                        break;
                }
            }

            var result = filtered.ToList();

            GameList.DataSource = result;
            GameList.DataBind();

            lblTotalRecord.Text = result.Count.ToString();
            pager.CurrentPage = GameList.PageIndex;
            pager.TotalPages = GameList.PageCount;
        }

        protected void GameList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Helper.SetRowNumber(GameList, e);
        }
    }
}