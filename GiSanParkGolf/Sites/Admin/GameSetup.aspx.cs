using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class GameSetup : System.Web.UI.Page
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
            Helper.RequireAdmin(this);

            if (!IsPostBack)
            {
                SearchField = null;
                SearchKeyword = null;
                GameList.PageIndex = 0;
                LoadGameList();

                gvPlayerList.DataSource = new List<GameJoinUserList>();
                gvPlayerList.DataBind();
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
            string field = SearchField;
            string keyword = SearchKeyword;

            List<GameListModel> result = Global.dbManager.GetGames(field, keyword);

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
            }
        }

        protected void LnkGame_Click(object sender, EventArgs e)
        {
            var btn = (LinkButton)sender;
            LoadGame(btn.ToolTip.ToString());

            gvPlayerList.DataSource = Global.dbManager.GetGameUserList(btn.ToolTip.ToString());
            gvPlayerList.DataBind();
        }

        private void LoadGame(string gameCode)
        {
            var gameinfo = Global.dbManager.GetGameInformation(gameCode);

            if (gameinfo == null)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "key", "launchModal('#MainModal', '오류', '대회 정보를 찾을 수 없습니다.', true);", true);
                return;
            }

            bool isClosed = gameinfo.GameStatus == "대회종료" || gameinfo.GameStatus == "취소";
            if (isClosed)
            {
                string msg = gameinfo.GameStatus == "대회종료" ? "종료된 대회입니다." : "취소된 대회입니다.";
                ClientScript.RegisterStartupScript(this.GetType(), "key", $"launchModal('#MainModal', '확인', '{msg}', true);", true);

                TB_GameName.Text = TB_GameDate.Text = TB_StadiumName.Text = TB_GameHost.Text =
                TB_StartDate.Text = TB_EndDate.Text = TB_HoleMaximum.Text = TB_Note.Text =
                TB_User.Text = TB_GameStatus.Text = TB_GameCode.Text = string.Empty;

                return;
            }

            TB_GameName.Text = gameinfo.GameName;
            TB_GameDate.Text = Helper.ConvertDate(gameinfo.GameDate);
            TB_StadiumName.Text = gameinfo.StadiumName;
            TB_GameHost.Text = gameinfo.GameHost;
            TB_StartDate.Text = Helper.ConvertDate(gameinfo.StartRecruiting);
            TB_EndDate.Text = Helper.ConvertDate(gameinfo.EndRecruiting);
            TB_HoleMaximum.Text = gameinfo.HoleMaximum.ToString();
            TB_Note.Text = gameinfo.GameNote;
            TB_User.Text = gameinfo.ParticipantNumber.ToString();
            TB_GameStatus.Text = gameinfo.GameStatus;
            TB_GameCode.Text = gameinfo.GameCode;
        }

        protected void BTN_SettingYes_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("코스배치 로직 준비 중");
            // 추후 코스배치 구현
        }

        protected void BTN_ToExcel_Click(object sender, EventArgs e)
        {
            //Create a dummy GridView.
            GridView gvCustomers = new GridView();
            gvCustomers.AutoGenerateColumns = false;

            // 컬럼 수동 지정
            gvCustomers.Columns.Add(new BoundField
            {
                DataField = "RowNumber",
                HeaderText = "No",
                ItemStyle = { HorizontalAlign = HorizontalAlign.Center },
                HeaderStyle = { HorizontalAlign = HorizontalAlign.Center }
            });
            gvCustomers.Columns.Add(new BoundField
            {
                DataField = "UserId",
                HeaderText = "ID",
                ItemStyle = { HorizontalAlign = HorizontalAlign.Center },
                HeaderStyle = { HorizontalAlign = HorizontalAlign.Center }
            });
            gvCustomers.Columns.Add(new BoundField
            {
                DataField = "UserName",
                HeaderText = "성명",
                ItemStyle = { HorizontalAlign = HorizontalAlign.Center },
                HeaderStyle = { HorizontalAlign = HorizontalAlign.Center }
            });
            gvCustomers.Columns.Add(new BoundField
            {
                DataField = "UserNumber",
                HeaderText = "생년월일",
                DataFormatString = "{0:yyyy-MM-dd}",
                ItemStyle = { HorizontalAlign = HorizontalAlign.Center },
                HeaderStyle = { HorizontalAlign = HorizontalAlign.Center }
            });

            gvCustomers.DataSource = Global.dbManager.GetGameUserList(TB_GameCode.Text);
            gvCustomers.DataBind();

            string fileName = HttpUtility.UrlEncode("PlayerList.xls", new UTF8Encoding());

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = Encoding.UTF8;
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
                    //string style = @"<style> .textmode { mso-number-format:\@; } </style>";
                    string style = @"<meta http-equiv='Content-Type' content='text/html; charset=utf-8' /> 
                                    <style> .textmode { mso-number-format:\@; } </style>";
                    Response.Write(style);
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

        protected void BTN_RefreshResult_Click(object sender, EventArgs e)
        {

        }
    }
}
