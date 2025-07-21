<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewSearchControl.ascx.cs" Inherits="GiSanParkGolf.Controls.NewSearchControl" %>

<asp:Panel ID="CheckControl" CssClass="form-check" runat="server">
    <input type="checkbox" runat="server" id="CB_ReadyUser" class="form-check-input" />
    <label for="<%= CB_ReadyUser.ClientID %>" class="form-check-label">승인대기만 보기</label>
</asp:Panel>

<div class="input-group mb-3">
    <asp:DropDownList ID="DDL_SearchField" runat="server" CssClass="btn btn-success dropdown-toggle"/>
    <asp:TextBox ID="TB_SearchQuery" runat="server" CssClass="form-control"/>
    <asp:Button ID="BTN_Search" runat="server" CssClass="btn btn-outline-secondary" Text="검색" OnClick="BTN_Search_Click" />
    <asp:Button ID="BTN_Reset" runat="server" CssClass="btn btn-primary" Text="초기화" OnClick="BTN_Reset_Click" />
</div>