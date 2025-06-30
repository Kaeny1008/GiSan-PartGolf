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

        private string bbsId = string.Empty;

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

            bbsId = Request.QueryString["bbsId"];

            if (!string.IsNullOrEmpty(Global.uvm.UserClass.ToString()))
            {
                if (Global.uvm.UserClass.Equals("Administrator"))
                {
                    //관리자 권한으로 온거라면 무조건 삭제
                    ImmediatelyDelete();
                }
                else
                {
                    if (Request.QueryString["immediately"].Equals("true"))
                    {
                        ImmediatelyDelete();
                    }
                }
            }
        }

        protected void btnCommentDelete_Click(object sender, EventArgs e)
        {
            CommentDelete(txtPassword.Text);
        }

        protected void CommentDelete(string commentPass)
        {
            var repo = new NoteCommentRepository();
            if (repo.GetCountBy(BoardId, Id, commentPass) > 0)
            {
                repo.DeleteNoteComment(BoardId, Id, commentPass);
                Response.Redirect($"BoardView.aspx?Id={BoardId}&bbsId={bbsId}");
            }
            else
            {
                lblError.Text = "암호가 틀립니다. 다시 입력해주세요.";
            }
        }

        protected void ImmediatelyDelete()
        {
            var repo = new NoteCommentRepository();
            repo.DeleteNoteComment2(BoardId, Id);
            Response.Redirect($"BoardView.aspx?Id={BoardId}&bbsId={bbsId}");
        }
    }
}