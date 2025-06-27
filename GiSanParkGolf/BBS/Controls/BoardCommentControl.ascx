<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BoardCommentControl.ascx.cs" Inherits="GiSanParkGolf.BBS.Controls.BoardCommentControl" %>

<style>
    .form-control { 
        min-width: 100%;
    }
</style>

<%--<h3>댓글 리스트</h3>--%>
<asp:Repeater ID="ctlCommentList" runat="server">
    <HeaderTemplate>
        <table style=
            "padding: 10px; margin-left: 20px; margin-right: 20px; width: 95%;">
    </HeaderTemplate>
    <ItemTemplate>
        <tr style="border-bottom: 1px dotted silver;">
            <td style="width: 80px;">
                <%# Eval("Name") %>
            </td>
            <td style="width: 350px;">
                <%# Dul.HtmlUtility.Encode(Eval("Opinion").ToString()) %>
            </td>
            <td style="width: 180px;">
                <%# Eval("PostDate") %>
            </td>
            <td style="width: 10px; text-align: center;">
                <%
                    //관리자일때는 댓글삭제가 무조건 가능 아니면 해당아이디의 글만 가능
                    if (string.IsNullOrEmpty(global_asax.uvm.UserClass))
                    {
                        if (global_asax.uvm.UserClass.Equals("Administrator"))
                        {
                %>
                            <a href='BoardCommentDelete.aspx?BoardId=<%= Request["Id"]%>&Id=<%# Eval("Id") %>&bbsId=<%= Request["bbsId"]%>&UId=<%# Eval("UserId") %>' title="삭제">삭제</a>
                <%
                        }
                        else
                        {
                %>
                            <%# Eval("UserId").ToString().Equals(global_asax.uvm.UserID) ? 
                                    "<a href='BoardCommentDelete.aspx?BoardId=" + Request["Id"] + "&id=" + Eval("Id") + "&bbsId=" + Request["bbsId"] + "&UId=" + Eval("UserId") + "' title=\"삭제\">삭제</a>" 
                                    : 
                                    ""
                            %>
                <%
                        }
                    }
                    else
                    {
                %>
                        <a href='BoardCommentDelete.aspx?BoardId=<%= Request["Id"]%>&Id=<%# Eval("Id") %>&bbsId=<%= Request["bbsId"]%>&UId=<%# Eval("UserId") %>' title="삭제">삭제</a>
                <%
                    }    
                %>
                <%--<img src="/images/dnn/icon_delete_red.gif" border="0">--%>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
 
<%--<h3>댓글 입력</h3>--%>
<table style="width: 500px; margin-left: auto; margin-right: auto;">
    <tr><td></td></tr>
    <tr>
        <td style="width: 64px; text-align: right;">댓글 
        </td>
        <td colspan="4" style="width: 448px;">
            <asp:TextBox ID="txtOpinion" runat="server" 
                TextMode="MultiLine" Height="75px"
                Width="100%" CssClass="form-control" 
                Style="display: inline-block;"></asp:TextBox>
        </td>
    </tr>
    
    <%--로그인 중일때는 표시하지 않는 부분--%>
    <% 
        if (!Page.User.Identity.IsAuthenticated)
        {
    %>
        <tr>
            <td style="width: 64px; text-align: right;">이 름 : 
            </td>
            <td style="width: 128px;">
                <asp:TextBox ID="txtName" runat="server" Width="128px" 
                    CssClass="form-control"
                    Style="display: inline-block;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 64px; text-align: right;">암 호 : 
            </td>
            <td style="width: 128px;">
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" 
                    style="display:inline-block;" MaxLength="20" Width="150px" 
                    TextMode="Password" EnableViewState="False"></asp:TextBox>
            </td>
        </tr>
    <%
        }
    %>
    <tr>
        <td></td>
        <td style="text-align:left;">
            <asp:Button ID="btnWriteComment" runat="server" Text="등록" CssClass="btn btn-primary" Width="96px" Style="display: inline-block;" OnClick="btnWriteComment_Click" />
        </td>
    </tr>

    
</table>
 
<hr />