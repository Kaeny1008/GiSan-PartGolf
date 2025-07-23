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
                // 참가취소로 온경우
                if (!string.IsNullOrEmpty(Request.QueryString["GameCancel"]))
                {
                    gamecode = Request.QueryString["GameCode"];
                    GameCancel();
                    return;
                }

                if (!string.IsNullOrEmpty(Request.QueryString["GameCode"]))
                {
                    MainContent.Visible = false;
                    GameContent.Visible = true;
                    gamecode = Request.QueryString["GameCode"];
                    LoadGame();
                }
                else
                {
                    MainContent.Visible = true;
                    GameContent.Visible = false;
                    LoadGameList();
                }
            }
        }

        protected void GameCancel()
        {
            string dbWrite = Global.dbManager.MyGameCancel(gamecode, Helper.CurrentUser?.UserId);
            if (dbWrite.Equals("Success"))
            {
                Response.Redirect(string.Format("~/Sites/Player/MyGame.aspx"));
            }
            else
            {
                string strAlarm = @"<script language='JavaScript'>window.alert('";
                strAlarm += dbWrite;
                strAlarm += "');</script>";
                Response.Write(strAlarm);
            }
        }

        protected void LoadGame()
        {
            var gameinfo = (new DB_Management()).GetGameInformation(gamecode);

            TB_GameName.Text = gameinfo.GameName;
            TB_GameDate.Text = ConvertDate(gameinfo.GameDate);
            TB_StadiumName.Text = gameinfo.StadiumName;
            TB_GameHost.Text = gameinfo.GameHost;
            TB_StartDate.Text = ConvertDate(gameinfo.StartRecruiting);
            TB_EndDate.Text = ConvertDate(gameinfo.EndRecruiting);
            TB_HoleMaximum.Text = gameinfo.HoleMaximum.ToString();
            TB_Note.Text = gameinfo.GameNote;
            TB_User.Text = gameinfo.ParticipantNumber.ToString();
        }

        protected string ConvertDate(DateTime datetime)
        {
            DateTime now = datetime;
            string formattedDate = now.ToString("yyyy-MM-dd");

            return formattedDate;
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
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int no = (GameList.PageIndex * GameList.PageSize) + e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = no.ToString();

                var status = DataBinder.Eval(e.Row.DataItem, "GameStatus")?.ToString();
                var cell = e.Row.Cells[e.Row.Cells.Count - 2]; // 상태 컬럼

                if (status == "준비중")
                {
                    cell.Text = "<span style='color:blue;'>준비중</span>";
                }
                else
                {
                    cell.Text = $"<span>{status}</span>";
                }

                // 취소/완료 버튼 컬럼
                var btnCell = e.Row.Cells[e.Row.Cells.Count - 1];
                var gameCode = DataBinder.Eval(e.Row.DataItem, "GameCode")?.ToString();

                if (status == "준비중")
                {
                    btnCell.Text = $"<button type='button' class='btn btn-danger' style='--bs-btn-padding-y: .25rem; --bs-btn-padding-x: .5rem; --bs-btn-font-size: .75rem;' onclick='ShowModal(\"{gameCode}\")'>취소</button>";
                }
                else
                {
                    btnCell.Text = "<span>완료</span>";
                }
            }
        }
    }
}