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
using System.Xml.Linq;

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

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ActivateRightPanel",
                        @"document.getElementById('leftPanel')?.classList.add('hidden');
                         document.getElementById('rightPanel')?.classList.remove('d-none');", true);
                }
                HiddenPanelState.Value = "right";
            }
        }

        private bool LoadGame(string gameCode)
        {

            ResetRightPanelData();

            var gameinfo = Global.dbManager.GetGameInformation(gameCode);

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

                return false;
            }

            // 기준이 '-'로 연결된 문자열이면 분리
            var settingParts = gameinfo.GameSetting?.Split('-') ?? new string[0];

            DDL_HandicapUse.SelectedValue = settingParts.Contains("HC") ? "True" : "False";
            DDL_GenderUse.SelectedValue = settingParts.Contains("GD") ? "True" : "False";
            DDL_AgeGroupUse.SelectedValue = settingParts.Contains("AGE") ? "True" : "False";
            DDL_AwardsUse.SelectedValue = settingParts.Contains("AW") ? "True" : "False";

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
            tblPlayMode.Text = gameinfo.PlayModeToText;

            return true;
        }

        protected void gvPlayerList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int no = (gvPlayerList.PageIndex * gvPlayerList.PageSize) + e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = no.ToString();

                int userNumber = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "UserNumber"));
                int userGender = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "UserGender"));

                int age = Helper.CalculateAge(userNumber, userGender);
                e.Row.Cells[5].Text = age + "세";  // 셀 인덱스는 나이 컬럼 위치에 맞게 조절

            }
        }

        private List<AssignedPlayer> cachedAssignment;

        protected void gvCourseResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var player = (AssignedPlayer)e.Row.DataItem;

                // 순번 표시 (GridView의 RowIndex는 0부터 시작)
                e.Row.Cells[0].Text = (e.Row.RowIndex + 1).ToString(); // No 컬럼

                // ViewState에 없을 경우 초기화
                if (cachedAssignment == null)
                {
                    var dataSource = gvCourseResult.DataSource as List<AssignedPlayer>;
                    cachedAssignment = dataSource ?? new List<AssignedPlayer>();
                }

                var teamPlayers = cachedAssignment
                    .Where(p => p.TeamNumber == player.TeamNumber)
                    .ToList();

                // 단독팀 강조 처리
                if (teamPlayers.Count == 1)
                {
                    e.Row.BackColor = System.Drawing.Color.MistyRose;
                    e.Row.Font.Bold = true;
                    e.Row.Cells[8].Text += " (단독)"; // 팀번호 옆 표시
                }
            }
        }

        protected void BTN_SettingYes_Click(object sender, EventArgs e)
        {
            string gameCode = TB_GameCode.Text.Trim();
            var gameInfo = Global.dbManager.GetGameInformation(gameCode);
            int maxPerHole = gameInfo.HoleMaximum;

            var players = Global.dbManager.GetGameUserList(gameCode);
            if (players == null || players.Count == 0) return;

            var sortedPlayers = GetSortedPlayers(players);
            var teamList = BuildTeams(sortedPlayers, maxPerHole);
            var holeSlots = GenerateHoleSlots(gameInfo.StadiumCode);
            var (assignedPlayers, unassignedPlayers) = AssignTeamsToSlots(teamList, holeSlots, maxPerHole, gameCode);
            BindGridViews(assignedPlayers, unassignedPlayers);

            ShowModal("배정 완료", "코스 배치가 성공적으로 완료되었습니다.", true);
        }

        private bool DidUseSorting()
        {
            return DDL_HandicapUse.SelectedValue == "True"
                || DDL_AgeGroupUse.SelectedValue == "True";

            // 성별정렬은 셔플과 상관없어보여 스킵
            //return DDL_HandicapUse.SelectedValue == "True"
            //    || DDL_GenderUse.SelectedValue == "True"
            //    || DDL_AgeGroupUse.SelectedValue == "True";
        }

        private IEnumerable<GameJoinUserList> GetSortedPlayers(IEnumerable<GameJoinUserList> players)
        {
            bool useHandicap = DDL_HandicapUse.SelectedValue == "True";
            bool useGender = DDL_GenderUse.SelectedValue == "True";
            bool useAgeGroup = DDL_AgeGroupUse.SelectedValue == "True";

            var query = players.AsEnumerable();

            if (useAgeGroup)
                query = query.OrderByDescending(p => int.TryParse(p.AgeText?.Replace("세", ""), out int age) ? age : -1);

            if (useHandicap)
                query = query.OrderByDescending(p => p.AgeHandicap);

            //성별정렬은 어차피 다음메서드에서 셔플 뒤, 남자 여자 구분하니까
            //if (useGender)
            //    query = query.OrderBy(p => p.UserGender);

            return query;
        }

        private List<List<GameJoinUserList>> BuildTeams(IEnumerable<GameJoinUserList> sortedPlayers, int maxPerHole)
        {
            bool useGender = DDL_GenderUse.SelectedValue == "True";
            bool skipShuffle = DidUseSorting(); // 정렬 사용 여부에 따라 셔플 생략
            var teamList = new List<List<GameJoinUserList>>();
            var playersList = sortedPlayers.ToList();

            if (!skipShuffle)
                Shuffle(playersList); // 셔플은 정렬 없을 때만!

            if (useGender)
            {
                var maleList = playersList.Where(p => p.UserGender == 1 || p.UserGender == 3).ToList();
                var femaleList = playersList.Where(p => p.UserGender == 2 || p.UserGender == 4).ToList();

                var genderTeams = new List<List<GameJoinUserList>>();

                for (int i = 0; i < maleList.Count; i += maxPerHole)
                    genderTeams.Add(maleList.Skip(i).Take(maxPerHole).ToList());

                for (int i = 0; i < femaleList.Count; i += maxPerHole)
                    genderTeams.Add(femaleList.Skip(i).Take(maxPerHole).ToList());

                // 랜덤 섞기
                genderTeams = genderTeams.OrderBy(t => Guid.NewGuid()).ToList();

                // 팀 인원 수 기준 정렬
                teamList = genderTeams
                    .OrderByDescending(t => t.Count) 
                    .ToList();
            }
            else
            {
                for (int i = 0; i < playersList.Count; i += maxPerHole)
                    teamList.Add(playersList.Skip(i).Take(maxPerHole).ToList());

                teamList = teamList
                    .OrderByDescending(t => t.Count == maxPerHole)
                    .ToList();
            }

            return teamList;
        }

        private void Shuffle<T>(IList<T> list)
        {
            var rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        private List<(string CourseName, int HoleNo)> GenerateHoleSlots(string stadiumCode)
        {
            var courseList = Global.dbManager.GetCourseListByStadium(stadiumCode);
            var holeSlots = new List<(string, int)>();

            foreach (var course in courseList)
            {
                for (int h = 1; h <= course.HoleCount; h++)
                    holeSlots.Add((course.CourseName, h));
            }

            return holeSlots.OrderBy(x => Guid.NewGuid()).ToList();
        }

        private (List<AssignedPlayer>, List<GameJoinUserList>) AssignTeamsToSlots(
            List<List<GameJoinUserList>> teamList,
            List<(string CourseName, int HoleNo)> shuffledSlots,
            int maxPerHole,
            string gameCode)
        {
            var slotCapacity = new Dictionary<string, int>();
            var slotAssignedTeam = new Dictionary<string, string>();
            var assignedPlayers = new List<AssignedPlayer>();
            var unassignedPlayers = new List<GameJoinUserList>();
            int teamNumber = 1;

            foreach (var team in teamList)
            {
                bool assigned = false;

                foreach (var slot in shuffledSlots)
                {
                    string key = $"{slot.CourseName}-{slot.HoleNo}";
                    if (!slotCapacity.ContainsKey(key)) slotCapacity[key] = 0;

                    if (slotCapacity[key] >= maxPerHole) continue;
                    if (slotAssignedTeam.ContainsKey(key)) continue;

                    if (slotCapacity[key] + team.Count <= maxPerHole)
                    {
                        int order = 1;
                        foreach (var player in team)
                        {
                            assignedPlayers.Add(new AssignedPlayer
                            {
                                UserId = player.UserId,
                                UserName = player.UserName,
                                AgeHandicap = player.AgeHandicap,
                                GameCode = gameCode,
                                CourseName = slot.CourseName,
                                CourseOrder = order++,
                                GroupNumber = slot.HoleNo,
                                HoleNumber = key,
                                TeamNumber = $"{teamNumber:D2}",
                                UserNumber = player.UserNumber,
                                UserGender = player.UserGender,
                                AgeText = player.AgeText,
                                GenderText = player.GenderText
                            });
                        }

                        slotCapacity[key] += team.Count;
                        slotAssignedTeam[key] = $"{teamNumber:D2}";
                        teamNumber++;
                        assigned = true;
                        break;
                    }
                }

                if (!assigned)
                    unassignedPlayers.AddRange(team);
            }

            return (assignedPlayers, unassignedPlayers);
        }

        private void BindGridViews(List<AssignedPlayer> assignedPlayers, List<GameJoinUserList> unassignedPlayers)
        {
            //gvCourseResult.DataSource = assignedPlayers.Select((p, index) => new
            //{
            //    RowNumber = index + 1,
            //    p.UserId,
            //    p.UserName,
            //    GenderTextPrint = p.GenderTextPrint,
            //    AgeTextPrint = p.AgeTextPrint,
            //    p.AgeHandicap,
            //    p.HoleNumber,
            //    p.CourseOrder,
            //    p.TeamNumber
            //}).ToList();
            gvCourseResult.DataSource = assignedPlayers;
            gvCourseResult.DataBind();
            ViewState["AssignmentResult"] = assignedPlayers;

            //gvUnassignedPlayers.DataSource = unassignedPlayers.Select(p => new
            //{
            //    p.UserId,
            //    p.UserName,
            //    GenderText = p.GenderText,
            //    AgeText = p.AgeText,
            //    p.AgeHandicap
            //}).ToList();
            gvUnassignedPlayers.DataSource = unassignedPlayers;
            gvUnassignedPlayers.DataBind();
            ViewState["UnassignedPlayers"] = unassignedPlayers;

            hiddenBox.Visible = unassignedPlayers.Any();
        }

        protected void BTN_ToExcel_Click(object sender, EventArgs e)
        {
            GridView gvExport = new GridView();
            gvExport.AutoGenerateColumns = false;

            var headers = new Dictionary<string, string>
            {
                { "RowNumber", "No" },
                { "UserId", "ID" },
                { "UserName", "성명" },
                { "FormattedBirthDate", "생년월일" },
                { "GenderText", "성별" },
                { "AgeText", "연령" },
                { "AwardsSummary", "수상경력" }
            };

            foreach (var col in Helper.GetExportColumns(headers))
            {
                gvExport.Columns.Add(col);
            }

            gvExport.DataSource = Global.dbManager.GetGameUserList(TB_GameCode.Text);
            gvExport.DataBind();

            Helper.ExportGridViewToExcel(gvExport, "PlayerList.xls", Response);
        }

        protected void BTN_ResultToExcel_Click(object sender, EventArgs e)
        {
            GridView gvExport = new GridView();
            gvExport.AutoGenerateColumns = false;

            var headers = new Dictionary<string, string>
            {
                { "RowNumber", "No" },               // 순번
                { "UserId", "아이디" },
                { "UserName", "성명" },
                { "GenderText", "성별" },
                { "AgeText", "연령" },
                { "AgeHandicap", "핸디캡" },
                { "HoleNumber", "배정홀" },
                { "CourseOrder", "코스순번" },
                { "TeamNumber", "팀번호" }
            };

            foreach (var col in Helper.GetExportColumns(headers))
            {
                gvExport.Columns.Add(col);
            }

            gvExport.DataSource = Global.dbManager.GetAssignmentResult(TB_GameCode.Text);
            gvExport.DataBind();

            Helper.ExportGridViewToExcel(gvExport, "PlayerList.xls", Response);
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
            if (gvUnassignedPlayers.Rows.Count != 0)
            {
                ShowModal("경고", "아직 배정되지 않은 플레이어가 있습니다.", false);
                return;
            }

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
            bool useGender = Convert.ToBoolean(DDL_GenderUse.SelectedValue);
            bool useAgeGroup = Convert.ToBoolean(DDL_AgeGroupUse.SelectedValue);
            bool useAwards = Convert.ToBoolean(DDL_AwardsUse.SelectedValue);

            List<string> settingParts = new List<string>();

            if (useHandicap) settingParts.Add("HC");
            if (useGender) settingParts.Add("GD");
            if (useAgeGroup) settingParts.Add("AGE");
            if (useAwards) settingParts.Add("AW");

            string settingCode = settingParts.Count > 0 ? string.Join("-", settingParts) : "DEFAULT";

            bool success = Global.dbManager.ProcessGameTransaction(gameCode, settingCode, assignmentData);

            string title = success ? "저장 완료" : "실패";
            string msg = success ? "배정 결과가 성공적으로 저장되었습니다." : "저장 중 오류가 발생했습니다.";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "SaveResult",
                $"launchModal('#MainModal', '{title}', '{msg}', 0);", true);
        }

        protected void BTN_BackToList_Click(object sender, EventArgs e)
        {
            ResetRightPanelData();

            HiddenPanelState.Value = "left";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "RestoreLeftPanel",
                @"document.getElementById('leftPanel')?.classList.remove('hidden');
                document.getElementById('rightPanel')?.classList.remove('active');", true);
        }

        private void ResetRightPanelData()
        {
            // 1. TextBox 초기화
            TB_GameName.Text = TB_GameDate.Text = TB_StadiumName.Text = TB_GameHost.Text =
            TB_StartDate.Text = TB_EndDate.Text = TB_HoleMaximum.Text = TB_Note.Text =
            TB_User.Text = TB_GameStatus.Text = TB_GameCode.Text = tblPlayMode.Text = string.Empty;

            // 2. 각 그리드뷰 초기화
            gvPlayerList.DataSource = null;
            gvPlayerList.DataBind();

            gvCourseResult.DataSource = null;
            gvCourseResult.DataBind();

            gvUnassignedPlayers.DataSource = null;
            gvUnassignedPlayers.DataBind();

            hiddenBox.Visible = false;
        }

        private void ShowModal(string title, string message, bool scrollToResult = false)
        {
            string safeTitle = HttpUtility.JavaScriptStringEncode(title);
            string safeMessage = HttpUtility.JavaScriptStringEncode(message);

            string script = $"launchModal('#MainModal', '{safeTitle}', '{safeMessage}', 0);";
            if (scrollToResult)
            {
                script += "localStorage.setItem('lastActiveTabId', '#tab-result');";
                script += "setTimeout(showTabWhenReady('#tab-result'), 400);";
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "LaunchModal", script, true);
        }

        protected void gvUnassignedPlayers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var drv = e.Row.DataItem as GameJoinUserList;
                if (drv != null)
                {
                    var (recommendCourse, reason) = RecommendCourseForPlayerWithReason(drv);
                    var lbl = e.Row.FindControl("lblRecommendCourse") as Label;
                    var btn = e.Row.FindControl("BTN_AssignCourse") as Button;
                    if (lbl != null)
                    {
                        lbl.Text = recommendCourse;
                        lbl.Attributes["title"] = reason; // 마우스 오버시 추천사유 툴팁
                    }
                    if (btn != null)
                    {
                        btn.Attributes["title"] = reason; // 마우스 오버시 추천사유 툴팁
                    }
                }
            }
        }

        private List<CourseList> CachedCourseList
        {
            get => ViewState["CachedCourseList"] as List<CourseList>;
            set => ViewState["CachedCourseList"] = value;
        }

        private List<HoleList> CachedHoleList
        {
            get => ViewState["CachedHoleList"] as List<HoleList>;
            set => ViewState["CachedHoleList"] = value;
        }

        private List<AssignedPlayer> CachedAssignmentResult
        {
            get => ViewState["CachedAssignmentResult"] as List<AssignedPlayer>;
            set => ViewState["CachedAssignmentResult"] = value;
        }

        private void CacheGameData(string gameCode)
        {
            var gameInfo = Global.dbManager.GetGameInformation(gameCode);
            if (gameInfo == null) return;

            if (CachedCourseList == null)
                CachedCourseList = Global.dbManager.GetCourseListByStadium(gameInfo.StadiumCode);

            if (CachedHoleList == null && CachedCourseList != null)
            {
                var holes = new List<HoleList>();
                foreach (var course in CachedCourseList)
                {
                    holes.AddRange(Global.dbManager.GetHoleListByCourse(course.CourseCode));
                }
                CachedHoleList = holes;
            }

            if (CachedAssignmentResult == null)
                CachedAssignmentResult = Global.dbManager.GetAssignmentResult(gameCode);
        }

        // 추천 코스+홀 계산 개선 메서드 (캐싱 활용)
        // 추천사유를 함께 반환하도록 변경
        private (string RecommendText, string Reason) RecommendCourseForPlayerWithReason(GameJoinUserList drv)
        {
            string gameCode = TB_GameCode.Text;
            CacheGameData(gameCode);

            var courseList = CachedCourseList ?? new List<CourseList>();
            var holeList = CachedHoleList ?? new List<HoleList>();
            var assignment = CachedAssignmentResult ?? new List<AssignedPlayer>();

            string gender = drv.GenderText;
            int.TryParse(drv.AgeText?.Replace("세", ""), out int age);
            int handicap = drv.AgeHandicap;
            string awards = drv.AwardsSummary ?? string.Empty;

            var holeInfos = new List<(string CourseName, string HoleName, int HoleNo, int Distance, int AssignedCount)>();
            foreach (var course in courseList)
            {
                var holes = holeList.Where(h => h.CourseCode == course.CourseCode);
                foreach (var hole in holes)
                {
                    int assignedCount = assignment.Count(a => a.CourseName == course.CourseName && a.GroupNumber == hole.HoleNo);
                    holeInfos.Add((course.CourseName, hole.HoleName, hole.HoleNo, hole.Distance, assignedCount));
                }
            }

            string bestCourse = null;
            int? bestHoleNo = null;
            string reason = "";

            if (age >= 65)
            {
                var minHole = holeInfos.OrderBy(h => h.Distance).ThenBy(h => h.AssignedCount).FirstOrDefault();
                if (minHole.CourseName != null)
                {
                    bestCourse = minHole.CourseName; bestHoleNo = minHole.HoleNo;
                    reason = "고령자(65세 이상) - 가장 짧은 거리 우선";
                }
            }
            else if (gender == "여" || age < 20)
            {
                var minHole = holeInfos.OrderBy(h => h.AssignedCount).ThenBy(h => h.Distance).FirstOrDefault();
                if (minHole.CourseName != null)
                {
                    bestCourse = minHole.CourseName; bestHoleNo = minHole.HoleNo;
                    reason = "여성 또는 20세 미만 - 덜 배정된 홀 우선";
                }
            }
            else if (awards.Contains("최우수상"))
            {
                var maxHole = holeInfos.OrderByDescending(h => h.AssignedCount).ThenBy(h => h.Distance).FirstOrDefault();
                if (maxHole.CourseName != null)
                {
                    bestCourse = maxHole.CourseName; bestHoleNo = maxHole.HoleNo;
                    reason = "최우수상 수상자 - 많이 배정된 홀 우선";
                }
            }
            else if (handicap >= 5)
            {
                var minHole = holeInfos.OrderBy(h => h.AssignedCount).ThenBy(h => h.Distance).FirstOrDefault();
                if (minHole.CourseName != null)
                {
                    bestCourse = minHole.CourseName; bestHoleNo = minHole.HoleNo;
                    reason = "핸디캡 5 이상 - 덜 배정된 홀 우선";
                }
            }
            else if (gender == "여" && age >= 50 && handicap <= 1)
            {
                var minHole = holeInfos.Where(h => h.Distance == holeInfos.Min(x => x.Distance)).OrderBy(h => h.AssignedCount).FirstOrDefault();
                if (minHole.CourseName != null)
                {
                    bestCourse = minHole.CourseName; bestHoleNo = minHole.HoleNo;
                    reason = "여성, 50세 이상, 핸디캡 1 이하 - 최단거리 홀 우선";
                }
            }

            string recommendText = (string.IsNullOrEmpty(bestCourse) || !bestHoleNo.HasValue)
                ? "추천없음"
                : $"{bestCourse}-{bestHoleNo}";

            if (string.IsNullOrEmpty(reason))
                reason = "기본 추천 기준에 해당 없음";

            return (recommendText, reason);
        }

        protected void BTN_AssignCourse_Click(object sender, EventArgs e)
        {
            HiddenPanelState.Value = "right"; // 패널 상태 유지

            var btn = sender as Button;
            if (btn == null) return;

            string userId = btn.CommandArgument;

            GridViewRow row = btn.NamingContainer as GridViewRow;
            if (row == null) return;

            var lbl = row.FindControl("lblRecommendCourse") as Label;
            string recommendedHole = lbl?.Text;

            if (string.IsNullOrEmpty(recommendedHole) || recommendedHole == "추천없음")
            {
                ShowModal("배정 실패", "추천된 홀이 없습니다.", false);
                return;
            }

            var assignedPlayers = ViewState["AssignmentResult"] as List<AssignedPlayer> ?? new List<AssignedPlayer>();
            var unassignedPlayers = ViewState["UnassignedPlayers"] as List<GameJoinUserList> ?? new List<GameJoinUserList>();

            var player = unassignedPlayers.FirstOrDefault(p => p.UserId == userId);
            if (player == null)
            {
                ShowModal("배정 실패", "플레이어 정보를 찾을 수 없습니다.", false);
                return;
            }

            // 추천홀 파싱 (예: "A-1" -> 코스명:A, 홀번호:1)
            var parts = recommendedHole.Split('-');
            if (parts.Length != 2)
            {
                ShowModal("배정 실패", "추천홀 정보가 올바르지 않습니다.", false);
                return;
            }
            string courseName = parts[0];
            int holeNo;
            if (!int.TryParse(parts[1], out holeNo) || holeNo < 1)
            {
                ShowModal("배정 실패", "홀 번호가 올바르지 않습니다.", false);
                return;
            }
            string holeNumber = $"{courseName}-{holeNo}";

            int maxOrder = assignedPlayers
                .Where(a => a.CourseName == courseName && a.HoleNumber == holeNumber)
                .Select(a => a.CourseOrder)
                .DefaultIfEmpty(0)
                .Max();

            var currentTeamNumber = assignedPlayers
                     .Where(a => a.CourseName == courseName && a.HoleNumber == holeNumber)
                     .Select(a => a.TeamNumber)
                     .FirstOrDefault() ?? ""; // 해당 홀에 이미 배정된 팀넘버가 있으면 사용, 없으면 빈 문자열

            var newAssigned = new AssignedPlayer
            {
                UserId = player.UserId,
                UserName = player.UserName,
                AgeHandicap = player.AgeHandicap,
                GameCode = TB_GameCode.Text,
                CourseName = courseName,
                CourseOrder = maxOrder + 1,
                GroupNumber = holeNo, // 실제 홀 번호로 할당
                HoleNumber = holeNumber,
                TeamNumber = currentTeamNumber, // 필요시 할당
                UserNumber = player.UserNumber,
                UserGender = player.UserGender,
                AgeText = player.AgeText,
                GenderText = player.GenderText
            };

            assignedPlayers.Add(newAssigned);
            unassignedPlayers.Remove(player);

            // 배정 결과 정렬: 코스명, 홀번호, 팀번호, 코스순번 기준
            var sortedAssignedPlayers = assignedPlayers
                .OrderBy(a => a.TeamNumber)
                .ThenBy(a => a.GroupNumber)
                .ThenBy(a => a.CourseName)
                .ThenBy(a => a.CourseOrder)
                .ToList();

            ViewState["AssignmentResult"] = sortedAssignedPlayers;
            ViewState["UnassignedPlayers"] = unassignedPlayers;

            gvCourseResult.DataSource = sortedAssignedPlayers;
            gvCourseResult.DataBind();

            gvUnassignedPlayers.DataSource = unassignedPlayers;
            gvUnassignedPlayers.DataBind();

            hiddenBox.Visible = unassignedPlayers.Any();

            ShowModal("배정 완료", $"{player.UserName}님을 {holeNumber}에 배정했습니다.", false);
        }

        protected void gvUnassignedPlayers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AssignManual")
            {
                string userId = e.CommandArgument.ToString();
                // userId에 해당하는 플레이어를 수동으로 배치하는 코드 작성
                // 예시: 원하는 로직 호출/수동 배치 UI 띄우기 등
            }
        }
    }
}
