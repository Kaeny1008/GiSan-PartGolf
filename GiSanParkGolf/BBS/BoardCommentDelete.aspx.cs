using BBS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using T_Engine;

namespace GiSanParkGolf.BBS
{
    public partial class BoardCommentDelete : System.Web.UI.Page
    {
        public int BoardId { get; set; } // 게시판 글 번호
        public int Id { get; set; } // 댓글 번호

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["BoardId"] != null && Request.QueryString["Id"] != null)
            {
                BoardId = Convert.ToInt32(Request["BoardId"]);
                Id = Convert.ToInt32(Request["Id"]);
            }
            else
            {
                Response.End();
            }

            if (string.IsNullOrEmpty(Global.uvm.UserClass))
            {
                if (Global.uvm.UserClass.Equals("Administrator"))
                {
                    //관리자 권한으로 온거라면 무조건 삭제

                }
            }
        }

        protected void btnCommentDelete_Click(object sender, EventArgs e)
        {
            if (Request["UId"] != null)
            {
                if (Request["UId"].Equals(Global.uvm.UserID))
                {
                    string commentPass = Global.uvm.Password;
                    Cryptography newCrypt = new Cryptography();
                    commentPass = newCrypt.GetDecoding("ParkGolf", commentPass);
                    CommentDelete(commentPass);
                }
                else
                {
                    CommentDelete(txtPassword.Text);

                }
            }
        }

        protected void CommentDelete(string commentPass)
        {
            var repo = new NoteCommentRepository();
            if (repo.GetCountBy(BoardId, Id, commentPass) > 0)
            {
                repo.DeleteNoteComment(BoardId, Id, commentPass);
                Response.Redirect($"BoardView.aspx?Id={BoardId}");
            }
            else
            {
                lblError.Text = "암호가 틀립니다. 다시 입력해주세요.";
            }
        }
    }
}