<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BoardSearchFormSingleControl.ascx.cs" Inherits="GiSanParkGolf.BBS.Controls.BoardSearchFormSingleControl" %>

<%--<div style="text-align:center;">
    <asp:DropDownList ID="SearchField" runat="server" 
        CssClass="form-select" Width="80px" Style="display: inline-block;">
        <asp:ListItem Value="Title">제목</asp:ListItem>
        <asp:ListItem Value="Content">내용</asp:ListItem>
        <asp:ListItem Value="Name">이름</asp:ListItem>
    </asp:DropDownList>
    <asp:TextBox ID="SearchQuery" runat="server" Width="200px" 
        CssClass="form-control" Style="display: inline-block;" ValidationGroup="Group1"></asp:TextBox>
    <asp:RequiredFieldValidator ID="valSearchQuery" runat="server" 
        ControlToValidate="SearchQuery" Display="None" 
        ErrorMessage="검색할 단어를 입력하세요." ValidationGroup="Group1"></asp:RequiredFieldValidator>
    <asp:ValidationSummary ID="valSummary" runat="server" 
        ShowSummary="False" ShowMessageBox="True" ValidationGroup="Group1"></asp:ValidationSummary>
    <asp:Button ID="btnSearch" runat="server" Text="검 색" 
        CssClass="form-control" Width="100px" 
        Style="display: inline-block;" OnClick="btnSearch_Click" ValidationGroup="Group1"></asp:Button>
</div>--%>
<div class="center_container">
    <div>
    <div class="input-group mb-3">
        <asp:DropDownList ID="SearchField" runat="server" CssClass="btn btn-success dropdown-toggle">
            <asp:ListItem Value="Title">제목</asp:ListItem>
            <asp:ListItem Value="Content">내용</asp:ListItem>
            <asp:ListItem Value="Name">이름</asp:ListItem>
        </asp:DropDownList>
        <asp:TextBox ID="SearchQuery" runat="server" CssClass="form-control"/>
        <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-outline-secondary" Text="검색" OnClick="btnSearch_Click" />
        <asp:Button ID="btnReset" runat="server" CssClass="btn btn-primary" Text="초기화" />
    </div>
    </div>
</div>
<br />

<%--<% if (!string.IsNullOrEmpty(Request.QueryString["SearchField"]) 
        && !String.IsNullOrEmpty(Request.QueryString["SearchQuery"])) { %>
<div style="text-align:center;">
    <a href="<%#"/BBS/BoardList.aspx?bbsId=" + Request.QueryString["bbsId"]%>" class="btn btn-success">검색 완료</a>
</div>
<% } %>--%>