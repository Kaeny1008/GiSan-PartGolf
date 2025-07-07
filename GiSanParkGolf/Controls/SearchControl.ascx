<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchControl.ascx.cs" Inherits="GiSanParkGolf.Controls.SearchControl" %>

    <div tyle="text-align:center;">

        <asp:CheckBox ID="CB_ReadyUser" runat="server" Text="승인대기만 보기" AutoPostBack="false" />
        <br />

        <asp:DropDownList ID="DDL_SearchField" runat="server" 
            CssClass="form-select" Width="120px" Style="display: inline-block;"/>

        <asp:TextBox ID="TB_SearchQuery" runat="server" Width="250px" CssClass="form-control" Style="display: inline-block; background-color:aliceblue" placeholder="이름을 입력하여 검색" />
        <asp:Button ID="BTN_Search" runat="server" Width="100px" CssClass="form-control" Style="display: inline-block;" Text="검색" OnClick="BTN_Search_Click" />
    </div>      