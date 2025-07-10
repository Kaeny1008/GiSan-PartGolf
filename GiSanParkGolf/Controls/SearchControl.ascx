<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchControl.ascx.cs" Inherits="GiSanParkGolf.Controls.SearchControl" %>

<%--<div>
    <asp:CheckBox ID="CB_ReadyUser1" runat="server" Text="승인대기만 보기1" AutoPostBack="false" />
    <asp:CheckBox ID="CB_ReadyGame1" runat="server" Text="준비대회만 보기1" AutoPostBack="false" />
</div>--%>

<div class="form-check" id="CheckControl" runat="server">
    <input class="form-check-input" type="checkbox" value="" id="CB_ReadyUser" runat="server">
    <label class="form-check-label" for="CB_ReadyUser">승인대기만 보기</label>
</div>

<%--<style>
    .input-group{
        min-width:100%;
        max-width:100%;
    }
    .btn{
        min-width:20%;
        max-width:20%;
    }
    .form-control{
        min-width:40%;
        max-width:40%;
}
</style>--%>

<div class="input-group mb-3">
    <asp:DropDownList ID="DDL_SearchField" runat="server" CssClass="btn btn-success dropdown-toggle"/>
    <asp:TextBox ID="TB_SearchQuery" runat="server" CssClass="form-control"/>
    <asp:Button ID="BTN_Search" runat="server" CssClass="btn btn-outline-secondary" Text="검색" OnClick="BTN_Search_Click" />
    <asp:Button ID="BTN_Reset" runat="server" CssClass="btn btn-primary" Text="초기화" OnClick="BTN_Reset_Click" />
</div>