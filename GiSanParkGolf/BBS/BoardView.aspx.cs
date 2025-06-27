using BBS.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.BBS
{
    public partial class BoardView : System.Web.UI.Page
    {
        private string _Id; // 앞(리스트)에서 넘어 온 번호 저장
        private string bbsID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["bbsId"]))
            {
                bbsID = Request.QueryString["bbsId"].ToString();
                if (bbsID.Equals("notice"))
                {
                    LBMainTitle.Text = "공지사항";

                }
            }

            lnkReply.NavigateUrl = string.Format("BoardReply.aspx?Id={0}&bbsId={1}", Request["Id"], bbsID);
            lnkReply2.NavigateUrl = string.Format("BoardReply.aspx?Id={0}&bbsId={1}", Request["Id"], bbsID);
            lnkReply3.NavigateUrl = string.Format("BoardReply.aspx?Id={0}&bbsId={1}", Request["Id"], bbsID);

            lnkDelete.NavigateUrl = string.Format("BoardDelete.aspx?Id={0}&bbsId={1}", Request["Id"], bbsID);
            lnkDelete2.NavigateUrl = string.Format("BoardDelete.aspx?Id={0}&bbsId={1}", Request["Id"], bbsID);
            lnkDelete3.NavigateUrl = string.Format("BoardDelete.aspx?Id={0}&bbsId={1}", Request["Id"], bbsID);

            lnkModify.NavigateUrl = string.Format("BoardModify.aspx?Id={0}&bbsId={1}", Request["Id"], bbsID);
            lnkModify2.NavigateUrl = string.Format("BoardModify.aspx?Id={0}&bbsId={1}", Request["Id"], bbsID);
            lnkModify3.NavigateUrl = string.Format("BoardModify.aspx?Id={0}&bbsId={1}", Request["Id"], bbsID);

            lnkList.NavigateUrl = string.Format("BoardList.aspx?bbsId={0}", bbsID);

            _Id = Request.QueryString["Id"];
            if (_Id == null)
            {
                Response.Redirect("./BoardList.aspx?bbsId=" + bbsID);
            }

            if (!Page.IsPostBack)
            {
                // 넘어 온 번호에 해당하는 글만 읽어서 각 레이블에 출력
                DisplayData();
            }
        }

        private void DisplayData()
        {
            // 넘어 온 Id 값에 해당하는 레코드를 하나 읽어서 Note 클래스에 바인딩
            var note = (new NoteRepository()).GetNoteById(Convert.ToInt32(_Id));

            lblNum.Text = _Id; // 번호
            lblName.Text = note.Name; // 이름
            lblEmail.Text =
                String.Format("<a href=\"mailto:{0}\">{0}</a>", note.Email);
            lblTitle.Text = note.Title;
            string content = note.Content;

            // 인코딩 방식에 따른 데이터 출력
            string strEncoding = note.Encoding;
            if (strEncoding == "Text") // Text : 소스 그대로 표현
            {
                lblContent.Text =
                    Dul.HtmlUtility.EncodeWithTabAndSpace(content);
            }
            else if (strEncoding == "Mixed") // Mixed : 엔터 처리만
            {
                lblContent.Text = content.Replace("\r\n", "<br />");
            }
            else // HTML : HTML 형식으로 출력
            {
                lblContent.Text = content; // 변환없음
            }

            lblReadCount.Text = note.ReadCount.ToString();
            lblHomepage.Text = String.Format(
                "<a href=\"{0}\" target=\"_blank\">{0}</a>", note.Homepage);
            lblPostDate.Text = note.PostDate.ToString();
            lblPostIP.Text = note.PostIp;
            if (note.FileName.Length > 1)
            {
                lblFile.Text = String.Format(
                    "<a href='./BoardDown.aspx?Id={0}'>"
                    + "{1}{2} / 전송수: {3}</a>",
                    note.Id,
                    "<img src=\"/images/ext/ext_zip.gif\" border=\"0\">",
                    note.FileName, note.DownCount);
                if (Dul.BoardLibrary.IsPhoto(note.FileName))
                {
                    ltrImage.Text = "<img src=\'ImageDown.aspx?FileName="
                        + $"{Server.UrlEncode(note.FileName)}\'>";
                }
            }
            else
            {
                lblFile.Text = "(업로드된 파일이 없습니다.)";
            }
        }
    }
}