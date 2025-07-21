<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewPagingControl.ascx.cs" Inherits="GiSanParkGolf.Controls.NewPagingControl" %>

<div class="d-flex flex-wrap gap-1 justify-content-center my-2">
    <asp:Repeater ID="rptButtons" runat="server" OnItemCommand="rptButtons_ItemCommand">
        <ItemTemplate>
            <asp:LinkButton runat="server"
                Text='<%# Eval("Text") %>'
                CommandName="Page"
                CommandArgument='<%# Eval("Index") %>'
                CssClass='<%# Eval("CssClass") %>' />
        </ItemTemplate>
    </asp:Repeater>
</div>