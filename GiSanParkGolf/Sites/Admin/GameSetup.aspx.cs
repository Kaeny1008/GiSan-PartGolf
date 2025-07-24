using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
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
                ScriptManager.RegisterStartupScript(this, this.GetType(), "LaunchModalError",
                    "launchModal('#MainModal', '오류', '대회 정보를 찾을 수 없습니다.', 0);", true);

                return;
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
                groupedPlayers = HandicapBasedDistribution(players, courseList, maxPerHole);
            }
            else
            {
                // 무작위 배정
                groupedPlayers = RandomDistribution(players, courseList, maxPerHole);
            }

            // 결과 표시
            gvCourseResult.DataSource = groupedPlayers;
            gvCourseResult.DataBind();

            // 완료 안내 모달
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "LaunchModalComplete",
            //    "launchModal('#MainModal', '배정 완료', '코스 배치가 성공적으로 완료되었습니다.', 0);", true);

            ScriptManager.RegisterStartupScript(this, this.GetType(), "LaunchModalComplete",
                @"launchModal('#MainModal', '배정 완료', '코스 배치가 성공적으로 완료되었습니다.', 0);
                  setTimeout(function() {
                      var tabTrigger = document.querySelector('a[href=""#tab-result""]');
                      if (tabTrigger) new bootstrap.Tab(tabTrigger).show();
                  }, 600);", true);

        }

        public List<AssignedPlayer> RandomDistribution(List<GameJoinUserList> players, List<CourseList> courses, int maxPerCourse)
        {
            var result = new List<AssignedPlayer>();
            var rnd = new Random();

            // 참가자 무작위 섞기
            var shuffledPlayers = players.OrderBy(p => rnd.Next()).ToList();

            // 코스별 버킷 생성
            var courseBuckets = new Dictionary<string, List<AssignedPlayer>>();
            foreach (var course in courses)
                courseBuckets[course.CourseName] = new List<AssignedPlayer>();

            int currentCourseIndex = 0;
            int totalCourses = courses.Count;

            foreach (var player in shuffledPlayers)
            {
                bool assigned = false;

                // 최대 totalCourses번까지 시도해서 배정
                for (int attempt = 0; attempt < totalCourses; attempt++)
                {
                    var course = courses[currentCourseIndex % totalCourses];
                    var bucket = courseBuckets[course.CourseName];

                    if (bucket.Count < maxPerCourse)
                    {
                        int groupNo = bucket.Count / 3 + 1;
                        int courseOrder = bucket.Count + 1;

                        bucket.Add(new AssignedPlayer
                        {
                            UserId = player.UserId,
                            UserName = player.UserName,
                            AgeHandicap = player.AgeHandicap,
                            GameCode = player.GameCode,
                            CourseName = course.CourseName,
                            CourseOrder = courseOrder,
                            GenderText = player.GenderText,
                            HoleNumber = $"{course.CourseName}-{groupNo}조",
                            TeamNumber = $"T{groupNo:D2}",
                            GroupNumber = groupNo
                        });

                        assigned = true;
                        break;
                    }

                    currentCourseIndex++;
                }

                // 배정 실패 처리 (선택)
                if (!assigned)
                {
                    Console.WriteLine($"[RandomDistribution] 배정 실패: {player.UserName} / {player.UserId}");
                }
            }

            // 결과 통합
            foreach (var bucket in courseBuckets.Values)
                result.AddRange(bucket);

            return result;
        }

        public List<AssignedPlayer> HandicapBasedDistribution(List<GameJoinUserList> players, List<CourseList> courses, int maxPerCourse)
        {
            var result = new List<AssignedPlayer>();

            // 정렬된 참가자 목록 (핸디 높은 순 → 낮은 순)
            var sortedPlayers = players.OrderByDescending(p => p.AgeHandicap).ToList();

            // 코스별 그릇 만들기
            var courseBuckets = new Dictionary<string, List<AssignedPlayer>>();
            foreach (var course in courses)
                courseBuckets[course.CourseName] = new List<AssignedPlayer>();

            int courseIndex = 0;
            int totalCourses = courses.Count;

            foreach (var player in sortedPlayers)
            {
                // 순서대로 돌아가면서 균형 배정
                for (int i = 0; i < totalCourses; i++)
                {
                    var courseName = courses[courseIndex % totalCourses].CourseName;
                    var bucket = courseBuckets[courseName];

                    if (bucket.Count < maxPerCourse)
                    {
                        bucket.Add(new AssignedPlayer
                        {
                            UserId = player.UserId,
                            UserName = player.UserName,
                            AgeHandicap = player.AgeHandicap,
                            GameCode = player.GameCode,
                            CourseName = courseName,
                            CourseOrder = bucket.Count + 1,
                            GenderText = player.GenderText
                        });
                        break;
                    }

                    courseIndex++;
                }
            }

            // 결과 통합
            foreach (var course in courseBuckets.Values)
                result.AddRange(course);

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

        }
    }
}
