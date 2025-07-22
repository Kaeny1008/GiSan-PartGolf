using GiSanParkGolf.Class; // Global.dbManager 사용 시 참조
using GiSanParkGolf.Models;
using System;
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
                LoadStadiumList();
                SetStepBadge(1);
                HideAllPanels();
            }
        }

        protected void GV_StadiumList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 페이지 유지
            int currentPage = GV_StadiumList.PageIndex;

            string selectedCode = GV_StadiumList.SelectedDataKey.Value.ToString();
            ViewState["StadiumCode"] = selectedCode;

            // 필요한 데이터 로딩
            LoadCourseList(selectedCode);

            // 선택 후 다시 현재 페이지 유지되게 재바인딩
            GV_StadiumList.PageIndex = currentPage;
            LoadStadiumList();

            // UI 변경
            SetStepBadge(3);
            ShowPanel("Course");
        }

        private void LoadCourseList(string stadiumCode)
        {
            var courseList = Global.dbManager.GetCourseListByStadium(stadiumCode);

            GV_CourseList.DataSource = courseList;
            GV_CourseList.DataBind();
        }

        protected void GV_CourseList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedCourseCode = Convert.ToInt32(GV_CourseList.SelectedDataKey.Value);

            // 선택된 코스 코드 저장
            ViewState["CourseCode"] = selectedCourseCode;

            // 선택된 코스 정보를 불러올 수도 있고, 홀 목록을 띄울 수도 있음
            LoadHoleList(selectedCourseCode);

            SetStepBadge(4);           // 단계 표시: 홀 등록/조회
            ShowPanel("Hole");         // 홀 입력 또는 리스트 패널 보여주기
        }

        private void LoadHoleList(int courseCode)
        {
            var holeList = Global.dbManager.GetHoleListByCourse(courseCode);

            if (holeList == null || holeList.Count == 0)
            {
                // 🔹 홀 정보 없음 → 입력 폼 생성
                int holeCount = Global.dbManager.GetHoleCountByCourse(courseCode);  // DB에서 등록된 HoleCount 가져오기
                GenerateHoleGrid(holeCount);

                // 🔸 버튼 상태
                BTN_SaveHoleDetail.Visible = true;
                BTN_UpdateHoleDetail.Visible = false;
            }
            else
            {
                // 🔹 홀 정보 있음 → 기존 내용 바인딩
                GV_HoleDetail.DataSource = holeList;
                GV_HoleDetail.DataBind();

                // 🔸 버튼 상태
                BTN_SaveHoleDetail.Visible = false;
                BTN_UpdateHoleDetail.Visible = true;
            }
        }

        protected void BTN_ShowStadiumForm_Click(object sender, EventArgs e)
        {
            SetStepBadge(2);               // UI 단계 표시: 경기장 등록 단계
            ShowPanel("Stadium");          // 경기장 입력 폼만 보이게 처리
            TB_StadiumCode.Enabled = false; // 경기장 코드 입력 비활성화
        }

        private void SetStepBadge(int step)
        {
            LB_StepGuide1.CssClass = step == 1 ? "badge bg-primary me-2" : "badge bg-secondary me-2";
            LB_StepGuide2.CssClass = step == 2 ? "badge bg-primary me-2" : "badge bg-secondary me-2";
            LB_StepGuide3.CssClass = step == 3 ? "badge bg-primary me-2" : "badge bg-secondary me-2";
            LB_StepGuide4.CssClass = step == 4 ? "badge bg-primary" : "badge bg-secondary";
        }

        private void HideAllPanels()
        {
            Panel_StadiumForm.Visible = false;
            Panel_CourseForm.Visible = false;
            Panel_HoleForm.Visible = false;
        }

        private void LoadStadiumList()
        {
            string field = ViewState["SearchField"] as string ?? "StadiumName";
            string keyword = ViewState["SearchKeyword"] as string ?? "";

            int pageSize = 10;
            int pageIndex = Paging_Stadium.CurrentPage;

            int totalRecords = Global.dbManager.CountStadiums(field, keyword);
            Paging_Stadium.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var stadiumList = Global.dbManager.SearchStadiumsPaged(field, keyword, pageIndex, pageSize);

            GV_StadiumList.DataSource = stadiumList;
            GV_StadiumList.DataBind();
        }

        protected void SearchControl_Stadium_SearchRequested(object sender, EventArgs e)
        {
            // 검색 조건을 ViewState에 저장
            ViewState["SearchField"] = SearchControl_Stadium.SelectedField;
            ViewState["SearchKeyword"] = SearchControl_Stadium.Keyword;

            // 첫 번째 페이지로 초기화
            Paging_Stadium.CurrentPage = 0;

            LoadStadiumList();
        }

        protected void SearchControl_Stadium_ResetRequested(object sender, EventArgs e)
        {
            ViewState.Remove("SearchField");
            ViewState.Remove("SearchKeyword");

            Paging_Stadium.CurrentPage = 0;

            LoadStadiumList();
        }

        protected void Paging_Stadium_PageChanged(object sender, int newPage)
        {
            Paging_Stadium.CurrentPage = newPage;

            LoadStadiumList();
        }

        protected void GV_StadiumList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GV_StadiumList.PageIndex = e.NewPageIndex;

            LoadStadiumList(); // 검색 조건 + 페이지 반영해서 데이터 다시 바인딩
        }


        protected void BTN_InsertStadium_Click(object sender, EventArgs e)
        {
            Page.Validate("StadiumForm");
            if (!Page.IsValid)
            {
                ShowValidationModal();
                return;
            }

            string code = Global.dbManager.GenerateNextCode("SYS_StadiumList", "StadiumCode", "STD", 4);
            string name = TB_StadiumName.Text.Trim();
            bool active = DDL_StadiumActive.SelectedValue == "True";

            bool success = Global.dbManager.InsertStadium(code, name, active);

            if (success)
            {
                LoadStadiumList();
                SetStepBadge(3);
                ShowPanel("Course");
                ViewState["StadiumCode"] = code;
            }
        }

        private void ShowValidationModal()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showValidateModal", "showValidate();", true);
        }

        private void ShowPanel(string target)
        {
            HideAllPanels();  // 모든 패널 숨김 처리

            switch (target)
            {
                case "Stadium":
                    Panel_StadiumForm.Visible = true;
                    break;
                case "Course":
                    Panel_CourseForm.Visible = true;
                    break;
                case "Hole":
                    Panel_HoleForm.Visible = true;
                    break;
            }
        }


        protected void BTN_InsertCourse_Click(object sender, EventArgs e)
        {
            Page.Validate("CourseForm");
            if (!Page.IsValid)
            {
                ShowValidationModal();
                return;
            }

            string stadiumCode = ViewState["StadiumCode"]?.ToString();
            string name = TB_CourseName.Text.Trim();
            int holeCount = int.TryParse(TB_MaxHoleCount.Text.Trim(), out int hc) ? hc : 18;
            bool active = DDL_CourseActive.SelectedValue == "True";

            string code = Global.dbManager.GenerateNextCode("SYS_CourseList", "CourseCode", "CO", 3);

            bool success = Global.dbManager.InsertCourse(stadiumCode, name, holeCount, active);

            if (success)
            {
                SetStepBadge(4);
                ShowPanel("Hole");
                ViewState["CourseCode"] = code;
                GenerateHoleGrid(holeCount); // 자동 홀 목록 생성
            }
        }

        private void GenerateHoleGrid(int holeCount)
        {
            var holeList = new List<HoleDTO>();

            for (int i = 1; i <= holeCount; i++)
            {
                holeList.Add(new HoleDTO
                {
                    HoleName = $"{i}번",
                    Distance = 0,
                    Par = 3
                });
            }

            GV_HoleDetail.DataSource = holeList;
            GV_HoleDetail.DataBind();
        }

        protected void BTN_SaveHoleDetail_Click(object sender, EventArgs e)
        {
            string courseCode = ViewState["CourseCode"]?.ToString();
            var holeList = new List<HoleDTO>();

            foreach (GridViewRow row in GV_HoleDetail.Rows)
            {
                string name = row.Cells[0].Text.Trim();
                int distance = int.TryParse(((TextBox)row.FindControl("TB_Distance")).Text, out int d) ? d : 0;
                int par = int.TryParse(((TextBox)row.FindControl("TB_Par")).Text, out int p) ? p : 0;

                holeList.Add(new HoleDTO
                {
                    HoleName = name,
                    Distance = distance,
                    Par = par
                });
            }

            int courseCode2 = Convert.ToInt32(ViewState["CourseCode"]);
            bool success = Global.dbManager.InsertHoleList(courseCode2, holeList);

            if (success)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showComplete",
                    "launchModal('완료', '홀 정보가 저장되었습니다.', false);", true);

                SetStepBadge(1);
                HideAllPanels();
            }
        }

        protected void BTN_UpdateHoleDetail_Click(object sender, EventArgs e)
        {
            int courseCode = Convert.ToInt32(ViewState["CourseCode"]);
            var holeList = new List<HoleDTO>();

            foreach (GridViewRow row in GV_HoleDetail.Rows)
            {
                var hole = new HoleDTO();

                hole.HoleId = int.TryParse(((Label)row.FindControl("LB_HoleId"))?.Text, out int h) ? h : 0;
                hole.HoleName = ((TextBox)row.FindControl("TB_HoleName"))?.Text?.Trim() ?? "";
                hole.Distance = int.TryParse(((TextBox)row.FindControl("TB_Distance"))?.Text, out int d) ? d : 0;
                hole.Par = int.TryParse(((TextBox)row.FindControl("TB_Par"))?.Text, out int p) ? p : 0;
                hole.CourseCode = courseCode;

                holeList.Add(hole);
            }

            bool success = Global.dbManager.UpdateHoleList(courseCode, holeList);

            if (success)
            {
                LoadHoleList(courseCode);  // 수정 후 목록 재로딩
                ScriptManager.RegisterStartupScript(this, GetType(), "showComplete",
                    "launchModal('완료', '홀 정보가 수정되었습니다.', false);", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showError",
                    "launchModal('오류', '홀 정보 수정에 실패했습니다.', true);", true);
            }
        }
    }
}

