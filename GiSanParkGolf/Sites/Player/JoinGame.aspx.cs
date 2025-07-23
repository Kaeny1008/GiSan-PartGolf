using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using static GiSanParkGolf.Global;

namespace GiSanParkGolf.Sites.Player
{
    public partial class JoinGame : System.Web.UI.Page
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

        private static string gameCode = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["GameCode"]))
                {
                    MainContent.Visible = false;
                    GameContent.Visible = true;
                    gameCode = Request.QueryString["GameCode"];
                    EarlyJoin();
                }
                else
                {
                    MainContent.Visible = true;
                    GameContent.Visible = false;
                    GameList.PageIndex = 0;
                    LoadGameList();
                }
            }
        }

        protected void LoadGame()
        {
            var gameinfo = (new DB_Management()).GetGameInformation(gameCode);

            TB_GameName.Text = gameinfo.GameName;
            TB_GameDate.Text = Helper.ConvertDate(gameinfo.GameDate);
            TB_StadiumName.Text = gameinfo.StadiumName;
            TB_GameHost.Text = gameinfo.GameHost;
            TB_StartDate.Text = Helper.ConvertDate(gameinfo.StartRecruiting);
            TB_EndDate.Text = Helper.ConvertDate(gameinfo.EndRecruiting);
            TB_HoleMaximum.Text = gameinfo.HoleMaximum.ToString();
            TB_Note.Text = gameinfo.GameNote;
        }

        protected void EarlyJoin()
        {
            // 기존 참가 신청했는지 확인
            string earlyJoinCheck = Global.dbManager.GetEarlyJoin(gameCode, Helper.CurrentUser?.UserId);
            if (!string.IsNullOrEmpty(earlyJoinCheck))
            {
                if (earlyJoinCheck.Equals("Join"))
                {
                    //string strJs = "<script>alert('이미 대회참가 신청을 하셨습니다.'); location.href='javascript:history.go(-1)';</script>";
                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "goDefault", strJs);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alreadyJoin",
                        "launchModal('#SaveModal', '안내', '이미 대회 참가 신청을 하셨습니다.', true, -1);", true);

                    return;
                }
            }

            LoadGame();
        }

        protected void JoinGame_Click(object sender, EventArgs e)
        {
            string ipaddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipaddr))
            {
                ipaddr = Request.ServerVariables["REMOTE_ADDR"];
            }

            GameJoinUserModel gjum = new GameJoinUserModel
            {
                UserId = Helper.CurrentUser?.UserId
                , JoinIP = ipaddr
                , GameCode = gameCode
            };
           
            string dbWrite = Global.dbManager.GameJoin(gjum);
            if (dbWrite.Equals("Success"))
            {
                //Response.Redirect(string.Format("~/Sites/Player/JoinGame.aspx"));
                string js = $"launchModal('#SaveModal', '참가신청 성공', '참가신청이 완료 되었습니다.', true, -1);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "joinSuccess", js, true);
            }
            else
            {
                string js = $"launchModal('#SaveModal', '참가신청 실패', '{dbWrite}', true, -1);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "joinError", js, true);
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
            var result = Global.dbManager.GetGameReadyList(SearchField, SearchKeyword);

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
                if (status == "모집중")
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Text = "<span style='color:blue;'>모집중</span>";
                }
            }
        }
    }
}