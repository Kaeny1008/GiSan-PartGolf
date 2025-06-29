<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BoardView.aspx.cs" Inherits="GiSanParkGolf.BBS.BoardView" %>

<%@ Register Src="~/BBS/Controls/BoardCommentControl.ascx"
    TagPrefix="uc1" TagName="BoardCommentControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .MainLabel {
            text-align: center;
            font-size: 30px;
        }
    </style>

    <script languade="javascript">
        var Id, bbsId, Ignorepass;

        function GoNoteDelete() {
            var gp = "BoardDelete.aspx?Id=" + Id + "&bbsId=" + bbsId + "&ignorepass=" + Ignorepass;
            console.log(gp);
            location.href = gp;
        
            return false;
        }

        function SelectModalShoe(modalname, id, bbsid, ignorepass) {
            Id = id;
            bbsId = bbsid;
            Ignorepass = ignorepass;
            $(modalname).modal("show");
        }
    </script>

    <div style="text-align: center;">
        <asp:Label ID="LBMainTitle" runat="server" Class="MainLabel" Text="게시판" />
    </div>
    <span style="color: #ff0000">
        글 보기 - 현재 글에 대해서 수정 및 삭제를 할 수 있습니다. </span>
    <hr />
    <table style="width: 100%; margin-left: auto; margin-right: auto;">
        <tbody>
            <tr style="color: white; background-color: #46698c;">
                <td style="width: 100px; text-align: right; height: 35px;">
                    <b style="font-size: 18px">제 목</b> :
                </td>
                <td colspan="3">
                    <asp:Label ID="lblTitle" Font-Bold="True" Font-Size="18px" 
                        Width="100%" runat="server"></asp:Label>
                </td>
            </tr>
            <tr style="background-color: #efefef;">
                <td class="text-right">작성자 ID :
                </td>
                <td>
                    <asp:Label ID="lbUserId" Width="100%" runat="server">
                    </asp:Label>
                </td>
                <td class="text-right">
                </td>
                <td>
                </td>
            </tr>
            <tr style="background-color: #efefef;">
                <td class="text-right">번 호 :
                </td>
                <td>
                    <asp:Label ID="lblNum" Width="84" runat="server">
                    </asp:Label>
                </td>
                <td class="text-right">E-mail :
                </td>
                <td>
                    <asp:Label ID="lblEmail" Width="100%" runat="server">
                    </asp:Label>
                </td>
            </tr>
            <tr style="background-color: #efefef;">
                <td class="text-right">이 름 :
                </td>
                <td>
                    <asp:Label ID="lblName" Width="100%" runat="server">
                    </asp:Label>
                </td>
                <td class="text-right">Homepage :
                </td>
                <td>
                    <asp:Label ID="lblHomepage" Width="100%" runat="server">
                    </asp:Label>
                </td>
            </tr>
            <tr style="background-color: #efefef;">
                <td class="text-right">작성일 :
                </td>
                <td>
                    <asp:Label ID="lblPostDate" Width="100%" runat="server">
                    </asp:Label></td>
                <td class="text-right">IP 주소 :
                </td>
                <td>
                    <asp:Label ID="lblPostIP" Width="100%" runat="server">
                    </asp:Label>
                </td>
            </tr>
            <tr style="background-color: #efefef;">
                <td class="text-right">조회수 :
                </td>
                <td>
                    <asp:Label ID="lblReadCount" Width="100%" runat="server">
                    </asp:Label>
                </td>
                <td class="text-right">파일 :
                </td>
                <td>
                    <asp:Label ID="lblFile" Width="100%" runat="server">
                    </asp:Label>
                </td>
            </tr>

            <tr>
                <td colspan="4" style="padding: 10px;">
                    <asp:Literal ID="ltrImage" runat="server"></asp:Literal>
                    <asp:Label ID="lblContent" runat="server" 
                        Width="100%" Height="115px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <hr />
                </td>
            </tr>
            <tr>
                <td colspan="4">
 
                    <uc1:BoardCommentControl runat="server" 
                        ID="BoardCommentControl" />
 
                </td>
            </tr>
        </tbody>
    </table>
 
    <div style="text-align: center;">

        <% 
            //공지사항일때는 Administrator만 답쓰기 가능
            if (Request.QueryString["bbsId"].Equals("notice"))
            {
                if (!String.IsNullOrEmpty(global_asax.uvm.UserClass.ToString()))
                {
                    if (global_asax.uvm.UserClass.Equals(1))
                    {
        %>
                        <asp:HyperLink ID="lnkReply" runat="server" 
                            CssClass="btn btn-outline-primary">답글</asp:HyperLink>
                        <button type="button" class="btn btn-outline-danger" 
                            onclick="SelectModalShoe('#ReplyModal','<%= Request["Id"]%>','<%= Request["bbsId"]%>','true')">삭제</button>
                        <asp:HyperLink ID="lnkModify" runat="server" 
                            CssClass="btn btn-outline-warning">수정</asp:HyperLink>
                        <%--<asp:HyperLink ID="lnkReply" runat="server" 
                            CssClass="btn btn-default">답변</asp:HyperLink>
                        <asp:HyperLink ID="lnkDelete" runat="server" 
                            CssClass="btn btn-default">삭제</asp:HyperLink>
                        <asp:HyperLink ID="lnkModify" runat="server" 
                            CssClass="btn btn-default">수정</asp:HyperLink>--%>
        <%
                    }
                }
            }
            else
            {
                //여기 아래부분은 차차 만들어가야함
                //로그인 여부에 따라 삭제 수정가능하고
                //그에따른 링크(비번없이 바로 삭제 가능하도록) 수정하여야함.

                //로그인 되어 있으면
                if (Page.User.Identity.IsAuthenticated)
                {
                    if (lbUserId.Text.Equals(global_asax.uvm.UserID))
                    {
                        //내가 작성한 글이면
        %>
                        <%--<asp:HyperLink ID="lnkDelete2" runat="server" 
                            CssClass="btn btn-default">삭제</asp:HyperLink>
                        <asp:HyperLink ID="lnkModify2" runat="server" 
                            CssClass="btn btn-default">수정</asp:HyperLink>--%>
        <%
                    }
                }
                //로그인 되어 있지 않다면
                else
                {
        %>

        <%
                }
            }
        %>


        <%--이건 그냥 있어야지.. 보드랑 권한이랑 상관없이--%>
        <%--<asp:HyperLink ID="lnkList" runat="server" 
            CssClass="btn btn-default">리스트</asp:HyperLink>--%>
        <button type="button" class="btn btn-outline-dark" onclick="history.go(-1)">리스트</button>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="ReplyModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="exampleModalLabel">확인</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    삭제 하시겠습니까?
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">아니오</button>
                    <asp:Button ID="Button2" 
                        runat="server" 
                        OnClientClick="return GoNoteDelete();" 
                        class="btn btn-primary" 
                        Text="예" />
                </div>
            </div>
        </div>
    </div>
 
    <asp:Label ID="lblError" runat="server" 
        ForeColor="Red" EnableViewState="False"></asp:Label>
    <br />
 
</asp:Content>
