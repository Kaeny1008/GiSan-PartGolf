using BBS.Models;
using GiSanParkGolf.Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using T_Engine;

namespace GiSanParkGolf.BBS.Controls
{
    public partial class BoardCommentControl : System.Web.UI.UserControl
    {
        // 리포지터리 개체 생성
        private NoteCommentRepository _repository;

        public BoardCommentControl()
        {
            _repository = new NoteCommentRepository();
        }

        private string userId = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // 데이터 출력(현재 게시글의 번호(Id)에 해당하는 댓글 리스트)
                ctlCommentList.DataSource =
                    _repository.GetNoteComments(Convert.ToInt32(Request["Id"]));
                ctlCommentList.DataBind();
            }
        }

        protected void btnWriteComment_Click(object sender, EventArgs e)
        {
            NoteComment comment = new NoteComment();
            comment.BoardId = Convert.ToInt32(Request["Id"]); // 부모글
            comment.Opinion = txtOpinion.Text; // 댓글

            if (Page.User.Identity.IsAuthenticated)
            {
                comment.Name = Helper.CurrentUser?.UserName; // 이름
                comment.Password = Helper.CurrentUser?.UserPassword; // 암호
                comment.UserId = Helper.CurrentUser?.UserId;
            } 
            else
            {
                comment.Name = txtName.Text; // 이름
                Cryptography newCrypt = new Cryptography();
                String cryptPassword = newCrypt.GetEncoding("ParkGolf", txtPassword.Text);
                comment.Password = cryptPassword; // 암호
                comment.UserId = string.Empty;
            }


            // 데이터 입력
            _repository.AddNoteComment(comment);

            Response.Redirect(
                $"{Request.ServerVariables["SCRIPT_NAME"]}?Id={Request["Id"]}&bbsId={Request["bbsId"]}");
        }
    }
}