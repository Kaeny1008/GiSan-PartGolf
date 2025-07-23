using BBS.Models;
using GiSanParkGolf.Class; // Global.dbManager 사용 시 참조
using GiSanParkGolf.Models;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class StadiumManager : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
            }
        }

        private void InitializePage()
        {
            LoadStadiumData();
            SetStepBadge(1);
            ShowOnlyPanel("None");
        }

        private void BindCourseList(string stadiumCode)
        {
            var list = Global.dbManager.GetCourseListByStadium(stadiumCode);
            GV_CourseList.DataSource = list;
            GV_CourseList.DataBind();
        }

        private void SetStepBadge(int step)
        {
            LB_StepGuide1.CssClass = step == 1 ? "badge bg-primary me-2" : "badge bg-secondary me-2";
            LB_StepGuide2.CssClass = step == 2 ? "badge bg-primary me-2" : "badge bg-secondary me-2";
            LB_StepGuide3.CssClass = step == 3 ? "badge bg-primary me-2" : "badge bg-secondary me-2";
            LB_StepGuide4.CssClass = step == 4 ? "badge bg-primary" : "badge bg-secondary";
        }

        private void ShowOnlyPanel(string target)
        {
            Panel_StadiumForm.Visible = target == "StadiumForm";
            Panel_CourseForm.Visible = target == "CourseForm";
            Panel_HoleForm.Visible = target == "HoleForm";
        }

        private void ShowValidationAlert(string message)
        {
            string script = $"showValidate();";
            ScriptManager.RegisterStartupScript(this, GetType(), "validationModal", script, true);
        }

        private void ShowToast(string title, string message, bool showCancel = false, bool showDelete = false, bool showOk = true, bool showDeleteAll = false)
        {
            var options = $"{{ showCancel: {showCancel.ToString().ToLower()}, showDelete: {showDelete.ToString().ToLower()}, showOk: {showOk.ToString().ToLower()}, showDeleteAll: {showDeleteAll.ToString().ToLower()} }}";
            string script = $"launchModal('{title}', '{message}', {options});";
            ScriptManager.RegisterStartupScript(this, GetType(), "modal", script, true);
        }

        private void ClearStadiumInputs()
        {
            TB_StadiumName.Text = "";
            TB_StadiumNote.Text = "";
            DDL_StadiumActive.SelectedIndex = 0;
        }

        private void ClearCourseInputs()
        {
            TB_CourseName.Text = "";
            TB_MaxHoleCount.Text = "18";
            DDL_CourseActive.SelectedIndex = 0;
        }

        private void GenerateHoleGrid(int holeCount)
        {
            var list = new List<HoleList>();

            for (int i = 1; i <= holeCount; i++)
            {
                list.Add(new HoleList
                {
                    HoleName = $"{i}번",
                    Distance = 0,
                    Par = 3
                });
            }

            GV_HoleDetail.DataSource = list;
            GV_HoleDetail.DataBind();
        }

        protected void BTN_ShowStadiumForm_Click(object sender, EventArgs e)
        {
            SetStepBadge(2);
            ShowOnlyPanel("StadiumForm");
            ClearStadiumInputs();
        }

        protected void BTN_InsertStadium_Click(object sender, EventArgs e)
        {
            Page.Validate("StadiumForm");
            if (!Page.IsValid)
            {
                ShowValidationAlert("경기장 정보를 정확히 입력해주세요.");
                return;
            }

            string name = TB_StadiumName.Text.Trim();
            string note = TB_StadiumNote.Text.Trim();
            bool isActive = DDL_StadiumActive.SelectedValue == "True";
            string newCode = Global.dbManager.CreateCode("SYS_StadiumList", "StadiumCode", "STD", 4);

            var stadium = new StadiumList
            {
                StadiumCode = newCode,
                StadiumName = name,
                IsActive = isActive,
                Note = note
            };

            bool success = Global.dbManager.InsertStadium(stadium);

            if (success)
            {
                LoadStadiumData();
                ViewState["StadiumCode"] = newCode;

                SetStepBadge(3);
                ShowOnlyPanel("CourseForm");
                ClearCourseInputs();

                ShowToast("등록 완료", "경기장이 성공적으로 등록되었습니다.");
            }
            else
            {
                ShowToast("오류", "경기장 등록에 실패했습니다.", true);
            }
        }

        protected void GV_StadiumList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string code = GV_StadiumList.SelectedDataKey["StadiumCode"].ToString();
            ViewState["StadiumCode"] = code;

            BindCourseList(code);
            SetStepBadge(3);
            ShowOnlyPanel("CourseForm");
            ClearCourseInputs();

            // 유지되던 PageIndex값 재저장 (필수 아님, 확인용)
            int currentPage = GV_StadiumList.PageIndex;
            // 선택 이후에도 경기장 목록 다시 보여주고 싶다면:
            GV_StadiumList.PageIndex = currentPage;
            LoadStadiumData();

            ScriptManager.RegisterStartupScript(this, GetType(), "scrollCourseForm", "scrollToCourseForm();", true);
        }

        protected void BTN_InsertCourse_Click(object sender, EventArgs e)
        {
            Page.Validate("CourseForm");
            if (!Page.IsValid)
            {
                ShowValidationAlert("코스 정보를 정확히 입력해주세요.");
                return;
            }

            string stadiumCode = ViewState["StadiumCode"]?.ToString();
            string name = TB_CourseName.Text.Trim();
            bool isActive = DDL_CourseActive.SelectedValue == "True";
            int holeCount = int.TryParse(TB_MaxHoleCount.Text.Trim(), out int hc) ? hc : 18;

            var course = new CourseList
            {
                StadiumCode = stadiumCode,
                CourseName = name,
                HoleCount = holeCount,
                IsActive = isActive
            };

            int newCourseCode = Global.dbManager.InsertCourseAndGetId(course);

            if (newCourseCode > 0)
            {
                BindCourseList(stadiumCode);

                ViewState["CourseCode"] = newCourseCode;

                SetStepBadge(4);
                //ShowOnlyPanel("HoleForm");
                // 기존 ShowOnlyPanel은 사용하지 않고, 개별 Visible 속성 처리
                Panel_CourseForm.Visible = true;  // 코스 입력 영역 계속 표시
                Panel_HoleForm.Visible = true;    // 홀 입력 영역 표시
                Panel_StadiumForm.Visible = false;

                GenerateHoleGrid(holeCount);
                BTN_SaveHoleDetail.Visible = true;
                BTN_UpdateHoleDetail.Visible = false;

                ShowToast("등록 완료", "코스가 등록되었으며 홀을 입력하십시오.");
                ScriptManager.RegisterStartupScript(this, GetType(), "scrollCourseForm", "scrollToHoleForm();", true);
            }
            else
            {
                ShowToast("오류", "코스 등록에 실패했습니다.", true);
            }
        }

        protected void BTN_SaveHoleDetail_Click(object sender, EventArgs e)
        {
            int currentPage = GV_StadiumList.PageIndex; //페이지 보존

            string courseCode = ViewState["CourseCode"]?.ToString();
            var holeList = GetCurrentHoleGrid();

            bool success = Global.dbManager.SaveHoleList(courseCode, holeList);

            if (success)
            {
                // 저장 후에만 홀 카운트 반영
                int newHoleCount = holeList.Count;
                Global.dbManager.UpdateHoleCount(courseCode, newHoleCount);

                ShowToast("저장 완료", "홀 정보가 성공적으로 저장되었습니다.");
                SetStepBadge(1);
                ShowOnlyPanel("None");
                ViewState.Remove("CourseCode");
                ClearCourseInputs();

                GV_StadiumList.PageIndex = currentPage; //페이지 복원
                LoadStadiumData();
            }
            else
            {
                ShowToast("오류", "홀 정보 저장 중 오류가 발생했습니다.", true);
            }
        }


        protected void GV_CourseList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string courseCode = GV_CourseList.SelectedDataKey["CourseCode"].ToString();
            ViewState["CourseCode"] = courseCode;

            var holeList = Global.dbManager.GetHoleListByCourse(courseCode);

            if (holeList == null || holeList.Count == 0)
            {
                var course = Global.dbManager.GetCourseByCode(courseCode);
                int holeCount = course?.HoleCount ?? 18; // 기본값 보완

                GenerateHoleGrid(holeCount);
                BTN_SaveHoleDetail.Visible = true;
                BTN_UpdateHoleDetail.Visible = false;
            }
            else
            {
                GV_HoleDetail.DataSource = holeList;
                GV_HoleDetail.DataBind();
                BTN_SaveHoleDetail.Visible = false;
                BTN_UpdateHoleDetail.Visible = true;
            }

            SetStepBadge(4);
            //ShowOnlyPanel("HoleForm");
            // 기존 ShowOnlyPanel은 사용하지 않고, 개별 Visible 속성 처리
            Panel_CourseForm.Visible = true;  // ✅ 코스 입력 영역 계속 표시
            Panel_HoleForm.Visible = true;    // ✅ 홀 입력 영역 표시
            Panel_StadiumForm.Visible = false;

            // 유지되던 PageIndex값 재저장 (필수 아님, 확인용)
            int currentPage = GV_StadiumList.PageIndex;
            // 선택 이후에도 경기장 목록 다시 보여주고 싶다면:
            GV_StadiumList.PageIndex = currentPage;
            LoadStadiumData();

            ScriptManager.RegisterStartupScript(this, GetType(), "scrollHoleForm", "scrollToHoleForm();", true);
        }

        protected void BTN_UpdateHoleDetail_Click(object sender, EventArgs e)
        {
            int currentPage = GV_StadiumList.PageIndex; // 페이지 보존

            string courseCode = ViewState["CourseCode"]?.ToString();
            var holeList = new List<HoleList>();

            foreach (GridViewRow row in GV_HoleDetail.Rows)
            {
                int holeId = int.TryParse(((Label)row.FindControl("LB_HoleId"))?.Text, out int h) ? h : 0;
                string name = ((TextBox)row.FindControl("TB_HoleName"))?.Text?.Trim() ?? "";
                int distance = int.TryParse(((TextBox)row.FindControl("TB_Distance"))?.Text, out int d) ? d : 0;
                int par = int.TryParse(((TextBox)row.FindControl("TB_Par"))?.Text, out int p) ? p : 0;

                holeList.Add(new HoleList
                {
                    HoleId = holeId,
                    CourseCode = courseCode,
                    HoleName = name,
                    Distance = distance,
                    Par = par
                });
            }

            bool success = Global.dbManager.UpdateHoleList(courseCode, holeList);

            if (success)
            {
                // 수정 완료 후 HoleCount 업데이트
                int updatedHoleCount = holeList.Count;
                Global.dbManager.UpdateHoleCount(courseCode, updatedHoleCount);

                ShowToast("수정 완료", "홀 정보가 성공적으로 수정되었습니다.");
                SetStepBadge(1);
                ShowOnlyPanel("None");
                ViewState.Remove("CourseCode");
                ClearCourseInputs();

                GV_StadiumList.PageIndex = currentPage; //페이지 복원
                LoadStadiumData();
            }
            else
            {
                ShowToast("오류", "홀 정보 수정 중 오류가 발생했습니다.", true);
            }
        }

        protected void StadiumSearch_SearchRequested(object sender, EventArgs e)
        {
            ViewState["SearchField"] = StadiumSearch.SelectedField;
            ViewState["SearchKeyword"] = StadiumSearch.Keyword;
            ViewState["ReadyOnly"] = StadiumSearch.ReadyOnly;

            GV_StadiumList.PageIndex = 0;
            SetStepBadge(1);
            ShowOnlyPanel("None");
            LoadStadiumData();
        }

        protected void StadiumSearch_ResetRequested(object sender, EventArgs e)
        {
            ViewState.Remove("SearchField");
            ViewState.Remove("SearchKeyword");
            ViewState.Remove("ReadyOnly");

            GV_StadiumList.PageIndex = 0;
            SetStepBadge(1);
            ShowOnlyPanel("None");
            LoadStadiumData();
        }

        protected void StadiumPaging_PageChanged(object sender, int newPage)
        {
            GV_StadiumList.PageIndex = newPage;
            LoadStadiumData();
        }

        private void LoadStadiumData()
        {
            string field = ViewState["SearchField"] as string;
            string keyword = ViewState["SearchKeyword"] as string;
            bool readyOnly = ViewState["ReadyOnly"] as bool? ?? false;

            int pageIndex = GV_StadiumList.PageIndex;
            int pageSize = GV_StadiumList.PageSize;

            var list = Global.dbManager.SearchStadiumList(field, keyword, readyOnly, pageIndex, pageSize);

            GV_StadiumList.DataSource = list;
            GV_StadiumList.DataBind();

            StadiumPaging.CurrentPage = pageIndex;
            StadiumPaging.TotalPages = GV_StadiumList.PageCount;

            if (list.Count == 0)
            {
                ShowToast("데이터 없음", "현재 조건에 맞는 경기장 데이터가 없습니다.");
            }
        }

        protected void BTN_AddHoleRow_Click(object sender, EventArgs e)
        {
            var holeList = GetCurrentHoleGrid(); // 현재 GridView 내용 그대로 불러오기
            int nextHoleNum = holeList.Count + 1;

            holeList.Add(new HoleList
            {
                HoleName = $"{nextHoleNum}번",
                Distance = 0,
                Par = 3
            });

            GV_HoleDetail.DataSource = holeList;
            GV_HoleDetail.DataBind();
        }

        protected void BTN_ServerHoleDelete_Click(object sender, EventArgs e)
        {
            int holeId = int.TryParse(HF_TargetHoleId.Value, out int h) ? h : 0;
            string courseCode = ViewState["CourseCode"]?.ToString();

            if (holeId > 0 && !string.IsNullOrEmpty(courseCode))
            {
                bool success = Global.dbManager.DeleteHoleById(holeId);

                if (success)
                {
                    var holeList = Global.dbManager.GetHoleListByCourse(courseCode);
                    GV_HoleDetail.DataSource = holeList;
                    GV_HoleDetail.DataBind();

                    Global.dbManager.UpdateHoleCount(courseCode, holeList.Count);

                    ShowToast("삭제 완료", "홀 정보가 삭제되었습니다.");
                }
                else
                {
                    ShowToast("오류", "홀 정보 삭제 중 오류가 발생했습니다.", true);
                }
            }
        }

        private List<HoleList> GetCurrentHoleGrid()
        {
            var holeList = new List<HoleList>();

            foreach (GridViewRow row in GV_HoleDetail.Rows)
            {
                int holeId = int.TryParse(((Label)row.FindControl("LB_HoleId"))?.Text, out int h) ? h : 0;
                string holeName = ((TextBox)row.FindControl("TB_HoleName"))?.Text?.Trim() ?? $"{row.RowIndex + 1}번";
                int distance = int.TryParse(((TextBox)row.FindControl("TB_Distance"))?.Text, out int d) ? d : 0;
                int par = int.TryParse(((TextBox)row.FindControl("TB_Par"))?.Text, out int p) ? p : 3;

                holeList.Add(new HoleList
                {
                    HoleId = holeId,
                    HoleName = holeName,
                    Distance = distance,
                    Par = par
                });
            }

            return holeList;
        }

        protected void BTN_ServerDeleteAllHoles_Click(object sender, EventArgs e)
        {
            string courseCode = ViewState["CourseCode"]?.ToString();

            if (!string.IsNullOrEmpty(courseCode))
            {
                bool success = Global.dbManager.DeleteAllHolesByCourse(courseCode);

                if (success)
                {
                    // 리스트 재로드
                    var holeList = Global.dbManager.GetHoleListByCourse(courseCode);
                    GV_HoleDetail.DataSource = holeList;
                    GV_HoleDetail.DataBind();

                    ShowToast("삭제 완료", "해당 코스의 모든 홀이 삭제되었습니다.");
                }
                else
                {
                    ShowToast("오류", "홀 전체 삭제 중 오류가 발생했습니다.", showOk:true);
                }
            }
        }

        protected void BTN_ServerCourseDelete_Click(object sender, EventArgs e)
        {
            int courseCode = int.TryParse(HF_TargetCourseCode.Value, out int h) ? h : 0;

            bool success = Global.dbManager.DeleteCourse(courseCode);

            if (success)
            {
                ShowToast("코스 삭제 완료", "해당 코스가 삭제되었습니다.");

                //InitializePage();
                BindCourseList(ViewState["StadiumCode"].ToString());
                SetStepBadge(3);
                ShowOnlyPanel("CourseForm");
                ClearCourseInputs();

                ScriptManager.RegisterStartupScript(this, GetType(), "scrollCourseForm", "scrollToCourseForm();", true);
            }
            else
            { 
                ShowToast("삭제 실패", "삭제 중 오류가 발생했습니다.", true); 
            }
        }

        protected void BTN_ServerStadiumDelete_Click(object sender, EventArgs e)
        {
            string stadiumCode = Request.Form["HF_TargetStadiumCode"];

            bool success = Global.dbManager.DeleteStadium(stadiumCode);

            if (success)
                ShowToast("경기장 삭제 완료", "해당 경기장이 삭제되었습니다.");
            else
                ShowToast("삭제 실패", "삭제 중 오류가 발생했습니다.", true);

            InitializePage(); // 필요 시 경기장 목록 다시 불러오기
        }
    }
}

