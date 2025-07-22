<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BoardCommentControl.ascx.cs" Inherits="GiSanParkGolf.BBS.Controls.BoardCommentControl" %>

<style>
    .form-control { 
        min-width: 100%;
    }
</style>

<script languade="javascript">
    var BoardId, Id, bbsId, immediately;

    function goDelete() {
        var gp = "BoardCommentDelete.aspx?BoardId=" + BoardId + "&Id=" + Id + "&bbsId=" + bbsId + "&immediately=" + immediately;
        console.log(gp);
        location.href = gp;
        
        return false;
    }

    function ShowModal(BoardId1, Id1, bbsId1, immediately1) {
        BoardId = BoardId1;
        Id = Id1;
        bbsId = bbsId1;
        immediately = immediately1;
        $("#SaveModal").modal("show");
    }
</script>

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
                <%# Dul.BoardLibrary.FuncShowTime(Eval("PostDate")) %>
            </td>
            <td style="width: 10px; text-align: center;">
                <%
                    //관리자일때는 댓글삭제가 무조건 가능
                    //해당아이디의 글만 가능
                    //로그인 하지 않았다면 비밀번호 입력 후 가능

                    if (Page.User.Identity.IsAuthenticated)
                    {
                        System.Diagnostics.Debug.WriteLine("로그인 되어 있다.");
                        if (global_asax.uvm.UserClass.Equals(1))
                        {
                            System.Diagnostics.Debug.WriteLine("관리자가 로그인이다.");
                %>
                            <%--<a href='BoardCommentDelete.aspx?BoardId=<%= Request["Id"]%>&Id=<%# Eval("Id") %>&bbsId=<%= Request["bbsId"]%>&immediately=true' title="삭제">삭제</a>--%>

                            <button type="button" class="btn btn-outline-secondary btn-sm" 
                                onclick="ShowModal('<%= Request["Id"]%>','<%# Eval("Id")%>','<%= Request["bbsId"]%>','true')">삭제</button>
                <%
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("관리자 아닌사람이 로그인이다.");
                %>
                            <%# Eval("UserId").ToString().Equals(global_asax.uvm.UserId) ? 
                                    "<button type=\"button\" class=\"btn btn-outline-secondary btn-sm\" onclick=\"ShowModal('" + Request["Id"] + "','" + Eval("Id") + "','" + Request["bbsId"] + "','true')\">삭제</button>" 
                                    : 
                                    "" 
                            %>
                <%
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("로그인 되어 있지 않다.");
                %>
                        <button type="button" class="btn btn-outline-secondary btn-sm" 
                            onclick="ShowModal('<%= Request["Id"]%>','<%# Eval("Id")%>','<%= Request["bbsId"]%>','false')">삭제</button>
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

<!-- Modal -->
<div class="modal fade" id="SaveModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered"> <%--modal-dialog-centered 를 옆에 넣으면 화면 중앙에 나타난다.--%>
        <div class="modal-content">
            <div class="modal-header">
                <h1 class="modal-title fs-5" id="exampleModalLabel">삭제확인</h1>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                해당 댓글을 삭제 하시겠습니까?
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">아니오</button>
                <asp:Button ID="Button2" 
                    runat="server" 
                    OnClientClick="return goDelete();" 
                    class="btn btn-primary" 
                    Text="예" />
            </div>
        </div>
    </div>
</div>
 
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