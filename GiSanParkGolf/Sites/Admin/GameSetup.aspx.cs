using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            } else
            {
                pager.CurrentPage = GameList.PageIndex;
                pager.TotalPages = GameList.PageCount;
            }
        }

        protected void Search_SearchRequested(object sender, EventArgs e)
        {
            SearchField = search.SelectedField;
            SearchKeyword = search.Keyword;
            ViewState["GameList"] = null;

            GameList.SelectedIndex = -1;
            GameList.PageIndex = 0;
            LoadGameList();
        }

        protected void Search_ResetRequested(object sender, EventArgs e)
        {
            ViewState.Remove("SearchField");
            ViewState.Remove("SearchKeyword");
            ViewState["GameList"] = null;
            
            GameList.SelectedIndex = -1;
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

            List<GameListModel> result;

            if (ViewState["GameList"] == null)
            {
                result = Global.dbManager.GetGames(field, keyword);
                ViewState["GameList"] = result;
            }
            else
            {
                result = (List<GameListModel>)ViewState["GameList"];
            }

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

        protected void GameList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRow")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                string gameCode = GameList.DataKeys[index].Value.ToString();

                bool loadResult = LoadGame(gameCode);

                if (loadResult)
                {
                    gvPlayerList.DataSource = Global.dbManager.GetGameUserList(gameCode);
                    gvPlayerList.DataBind();

                    gvCourseResult.DataSource = Global.dbManager.GetAssignmentResult(gameCode);
                    gvCourseResult.DataBind();

                    GameList.DataSource = ViewState["GameList"];
                    GameList.DataBind();

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ActivatePanels", "switchPanels();", true);
                }

                //pager.CurrentPage = GameList.PageIndex;
                //pager.TotalPages = GameList.PageCount;
            }
        }

        private bool LoadGame(string gameCode)
        {
            var gameinfo = Global.dbManager.GetGameInformation(gameCode);

            if (gameinfo.GameSetting == "HC")
                DDL_HandicapUse.SelectedValue = "True";
            else if (gameinfo.GameSetting == "NO")
                DDL_HandicapUse.SelectedValue = "False";
            else
                DDL_HandicapUse.ClearSelection();  // 초기값이면 선택 안함

            if (gameinfo == null)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "LaunchModalError",
                    "launchModal('#MainModal', '오류', '대회 정보를 찾을 수 없습니다.', 0);", true);

                return false;
            }

            bool isClosed = gameinfo.GameStatus == "대회종료" || gameinfo.GameStatus == "취소";
            if (isClosed)
            {
                string msg = gameinfo.GameStatus == "대회종료" ? "종료된 대회입니다." : "취소된 대회입니다.";
                string safeMessage = HttpUtility.JavaScriptStringEncode(msg);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "LaunchModalError",
                    $"launchModal('#MainModal', '확인', '{safeMessage}', 0);", true);

                TB_GameName.Text = TB_GameDate.Text = TB_StadiumName.Text = TB_GameHost.Text =
                TB_StartDate.Text = TB_EndDate.Text = TB_HoleMaximum.Text = TB_Note.Text =
                TB_User.Text = TB_GameStatus.Text = TB_GameCode.Text = string.Empty;

                return false;
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

            return true;
        }

        protected void gvPlayerList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int no = (gvPlayerList.PageIndex * gvPlayerList.PageSize) + e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = no.ToString();
            }
        }

        private List<AssignedPlayer> cachedAssignment;

        protected void gvCourseResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var player = (AssignedPlayer)e.Row.DataItem;

                // ViewState에 없을 경우 초기화
                if (cachedAssignment == null)
                {
                    var dataSource = gvCourseResult.DataSource as List<AssignedPlayer>;
                    cachedAssignment = dataSource ?? new List<AssignedPlayer>();
                }

                var teamPlayers = cachedAssignment
                    .Where(p => p.TeamNumber == player.TeamNumber)
                    .ToList();

                if (teamPlayers.Count == 1)
                {
                    e.Row.BackColor = System.Drawing.Color.MistyRose;
                    e.Row.Font.Bold = true;
                    e.Row.Cells[5].Text += " (단독)";
                }
            }
        }


        protected void BTN_SettingYes_Click(object sender, EventArgs e)
        {
            string gameCode = TB_GameCode.Text.Trim();
            var gameInfo = Global.dbManager.GetGameInformation(gameCode);
            if (gameInfo == null)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "LaunchModalError",
                    "launchModal('#MainModal', '오류', '대회 정보를 찾을 수 없습니다.', 0);", true);
                return;
            }

            int maxPerHole = gameInfo.HoleMaximum;
            string stadiumCode = gameInfo.StadiumCode;
            bool useHandicap = Convert.ToBoolean(DDL_HandicapUse.SelectedValue);
            string settingCode = useHandicap ? "HC" : "NO";  // 핸디캡 적용 여부

            // 참가자 + 핸디캡 포함 데이터 한 번에 가져오기
            var players = Global.dbManager.GetGameUserList(gameCode);  // AgeHandicap 포함됨

            // 코스 리스트 가져오기
            var courseList = Global.dbManager.GetCourseListByStadium(stadiumCode);
            if (courseList == null || courseList.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "LaunchModalError",
                    "launchModal('#MainModal', '오류', '해당 경기장의 코스 정보를 찾을 수 없습니다.', 0);", true);
                return;
            }

            // 배정 결과 객체
            List<AssignedPlayer> groupedPlayers;

            if (useHandicap)
            {
                // 핸디캡 기반 배정
                groupedPlayers = HandicapBasedDistribution(players, courseList, maxPerHole, gameCode);
            }
            else
            {
                // 무작위 배정
                groupedPlayers = RandomDistribution(players, courseList, maxPerHole, gameCode);
            }

            // ViewState 저장
            ViewState["AssignmentResult"] = groupedPlayers;

            // 팀번호 기준으로 정렬
            var sortedPlayers = groupedPlayers
                .OrderByDescending(p => p.GenderText == "남자") // 남자 true면 먼저 나옴
                .ThenBy(p => p.TeamNumber)
                .ThenBy(p => p.CourseOrder)
                .ToList();

            // 바인딩
            gvCourseResult.DataSource = sortedPlayers;
            gvCourseResult.DataBind();


            // 완료 안내 모달
            ScriptManager.RegisterStartupScript(this, this.GetType(), "LaunchModalComplete",
                @"launchModal('#MainModal', '배정 완료', '코스 배치가 성공적으로 완료되었습니다.', 0);
                  setTimeout(function() {
                      var tabTrigger = document.querySelector('a[href=""#tab-result""]');
                      if (tabTrigger) new bootstrap.Tab(tabTrigger).show();
                  }, 600);", true);
        }

        Func<List<GameJoinUserList>, List<GameJoinUserList>> RandomSort = list => 
        list.OrderBy(p => Guid.NewGuid()).ToList();

        Func<List<GameJoinUserList>, List<GameJoinUserList>> HandicapSort = list => 
        list.OrderByDescending(p => p.AgeHandicap).ThenBy(p => Guid.NewGuid()).ToList();

        public List<AssignedPlayer> DistributePlayers(
            List<GameJoinUserList> players,
            List<CourseList> courses,
            int maxPerHole,
            string genderPrefix,
            string gameCode,
            Func<List<GameJoinUserList>, List<GameJoinUserList>> sortStrategy)
        {
            var result = new List<AssignedPlayer>();
            var sortedPlayers = sortStrategy(players);

            var courseBuckets = courses.ToDictionary(c => c.CourseName, c => new List<AssignedPlayer>());
            var courseHoleLookup = courses.ToDictionary(c => c.CourseName, c => 1);
            var teamNumberLookup = new Dictionary<string, string>();

            int globalTeamCounter = 1;
            int playerIndex = 0;
            int courseIndex = 0;

            while (playerIndex < sortedPlayers.Count)
            {
                var course = courses[courseIndex];
                var courseName = course.CourseName;
                var bucket = courseBuckets[courseName];
                var holeNo = courseHoleLookup[courseName];

                // 현재 홀에 몇 명이 있는지 계산
                int currentHoleSize = bucket.Count(p => p.GroupNumber == holeNo);

                // 현재 홀이 다 찼으면 다음 홀로 이동
                if (currentHoleSize >= maxPerHole)
                {
                    courseHoleLookup[courseName]++;
                    continue;
                }

                // 팀 넘버 생성
                string teamKey = $"{genderPrefix}-{courseName}-G{holeNo}";
                if (!teamNumberLookup.ContainsKey(teamKey))
                    teamNumberLookup[teamKey] = $"{genderPrefix}{globalTeamCounter++:D2}";

                string teamNumber = teamNumberLookup[teamKey];
                string holeLabel = $"{genderPrefix}-{courseName}-{holeNo}";

                // 플레이어 할당
                var player = sortedPlayers[playerIndex];

                var assigned = new AssignedPlayer
                {
                    UserId = player.UserId,
                    UserName = player.UserName,
                    AgeHandicap = player.AgeHandicap,
                    GameCode = gameCode,
                    CourseName = courseName,
                    CourseOrder = currentHoleSize + 1,
                    GenderText = player.GenderText,
                    GroupNumber = holeNo,
                    HoleNumber = holeLabel,
                    TeamNumber = teamNumber
                };

                bucket.Add(assigned);
                result.Add(assigned);
                playerIndex++;
            }

            return result;
        }

        public List<AssignedPlayer> RandomDistribution(
            List<GameJoinUserList> players,
            List<CourseList> courses,
            int maxPerHole,
            string gameCode)
        {
            var maleGroup = new[] { 1, 3 };
            var femaleGroup = new[] { 2, 4 };

            var malePlayers = players.Where(p => maleGroup.Contains(p.UserGender)).ToList();
            var femalePlayers = players.Where(p => femaleGroup.Contains(p.UserGender)).ToList();

            var result = new List<AssignedPlayer>();
            result.AddRange(DistributePlayers(malePlayers, courses, maxPerHole, "M", gameCode, RandomSort));
            result.AddRange(DistributePlayers(femalePlayers, courses, maxPerHole, "F", gameCode, RandomSort));
            return result;
        }

        public List<AssignedPlayer> HandicapBasedDistribution(
            List<GameJoinUserList> players,
            List<CourseList> courses,
            int maxPerHole,
            string gameCode)
        {
            var maleGroup = new[] { 1, 3 };
            var femaleGroup = new[] { 2, 4 };

            var malePlayers = players.Where(p => maleGroup.Contains(p.UserGender)).ToList();
            var femalePlayers = players.Where(p => femaleGroup.Contains(p.UserGender)).ToList();

            var result = new List<AssignedPlayer>();
            result.AddRange(DistributePlayers(malePlayers, courses, maxPerHole, "M", gameCode, HandicapSort));
            result.AddRange(DistributePlayers(femalePlayers, courses, maxPerHole, "F", gameCode, HandicapSort));
            return result;
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
                DataField = "FormattedBirthDate",
                HeaderText = "생년월일",
                ItemStyle = { HorizontalAlign = HorizontalAlign.Center },
                HeaderStyle = { HorizontalAlign = HorizontalAlign.Center }
            });
            gvCustomers.Columns.Add(new BoundField
            {
                DataField = "GenderText",
                HeaderText = "성별",
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
            string gameCode = TB_GameCode.Text.Trim();

            if (string.IsNullOrEmpty(gameCode))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "RefreshFail",
                    "launchModal('#MainModal', '실패', '게임코드가 비어 있습니다.', 0);", true);
                return;
            }

            //DB에서 저장된 배정 결과 다시 가져오기
            var assignmentData = Global.dbManager.GetAssignmentResult(gameCode);
            gvCourseResult.DataSource = assignmentData;
            gvCourseResult.DataBind();

            //필요하면 ViewState에도 다시 저장
            ViewState["AssignmentResult"] = assignmentData;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "LaunchModalComplete",
                @"launchModal('#MainModal', '새로고침 완료', '저장된 배정 결과를 다시 불러왔습니다.', 0);
                  setTimeout(function() {
                      var tabTrigger = document.querySelector('a[href=""#tab-result""]');
                      if (tabTrigger) new bootstrap.Tab(tabTrigger).show();
                  }, 600);", true);
        }


        protected void BTN_SaveAssignment_Click(object sender, EventArgs e)
        {
            // 모달 띄우기만 하고 저장은 아직 안 함!
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ConfirmSave",
                    @"launchModal('#MainModal', '확인', '이전에 저장된 배정 결과가 있을 수 있습니다.\r\n덮어씌우시겠습니까?', 2);", true);
        }

        protected void BTN_SaveAssignment_Final_Click(object sender, EventArgs e)
        {
            string gameCode = TB_GameCode.Text.Trim();
            var assignmentData = ViewState["AssignmentResult"] as List<AssignedPlayer>;

            if (assignmentData == null || assignmentData.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SaveError",
                    "launchModal('#MainModal', '오류', '배정 결과가 없어서 저장할 수 없습니다.', 0);", true);
                return;
            }

            bool useHandicap = Convert.ToBoolean(DDL_HandicapUse.SelectedValue);
            string settingCode = useHandicap ? "HC" : "NO";

            Global.dbManager.UpdateGameSetting(gameCode, settingCode);
            bool success = Global.dbManager.SaveAssignmentResult(assignmentData);

            string title = success ? "저장 완료" : "실패";
            string msg = success ? "배정 결과가 성공적으로 저장되었습니다." : "저장 중 오류가 발생했습니다.";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "SaveResult",
                $"launchModal('#MainModal', '{title}', '{msg}', 0);", true);
        }
    }
}
