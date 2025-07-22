using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class StadiumManager : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BTN_ShowStadiumForm_Click(object sender, EventArgs e)
        {
            // 1. 경기장 등록 폼 표시
            Panel_StadiumForm.Visible = true;

            // 2. 코스/홀 입력 폼은 닫기
            Panel_CourseForm.Visible = false;
            Panel_HoleForm.Visible = false;

            // 3. 기존 입력값 초기화
            TB_StadiumCode.Text = "";
            TB_StadiumName.Text = "";
            DDL_StadiumActive.SelectedIndex = 0;
            TB_StadiumNote.Text = "";

            // 필요하다면 등록 결과 안내 메시지 숨김 처리도 추가 가능
        }


        protected void GV_StadiumList_SelectedIndexChanged(object sender, EventArgs e) { }

        protected void BTN_InsertStadium_Click(object sender, EventArgs e)
        {
            // 1. 입력값 수집
            string stadiumCode = TB_StadiumCode.Text.Trim();
            string stadiumName = TB_StadiumName.Text.Trim();
            bool isActive = DDL_StadiumActive.SelectedValue == "True";
            string note = TB_StadiumNote.Text.Trim();

            // 2. 유효성 검사 (간단 예시)
            if (string.IsNullOrEmpty(stadiumCode) || string.IsNullOrEmpty(stadiumName))
            {
                // 안내 메시지 출력 또는 Validation 표시
                return;
            }

            // 3. DB 저장 처리 (예: SYS_StadiumList 테이블 INSERT)
            // StadiumRepository.Insert(stadiumCode, stadiumName, isActive, note);

            // 4. 경기장 목록 새로고침
            // GV_StadiumList.DataSource = StadiumRepository.GetAll();
            // GV_StadiumList.DataBind();

            // 5. 코스 등록 영역 표시
            Panel_CourseForm.Visible = true;

            // 6. 경기장 선택 바인딩 처리 (코스 등록 DropDown에 사용)
            // DDL_StadiumSelect.DataSource = StadiumRepository.GetAll();
            // DDL_StadiumSelect.DataTextField = "StadiumName";
            // DDL_StadiumSelect.DataValueField = "StadiumCode";
            // DDL_StadiumSelect.DataBind();
            // DDL_StadiumSelect.SelectedValue = stadiumCode;
        }


        protected void BTN_InsertCourse_Click(object sender, EventArgs e) { }

        protected void BTN_SaveHoleDetail_Click(object sender, EventArgs e) { }

        protected void GV_HoleDetail_RowDataBound(object sender, GridViewRowEventArgs e) { }

    }
}