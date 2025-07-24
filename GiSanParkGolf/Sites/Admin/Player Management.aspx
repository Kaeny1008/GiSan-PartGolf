<%@ Page Title="선수정보 관리" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Player Management.aspx.cs" 
    Inherits="GiSanParkGolf.Sites.Admin.Player_Management" EnableEventValidation="false" %>

<%@ Register Src="~/Controls/NewSearchControl.ascx" TagPrefix="uc" TagName="NewSearchControl" %>
<%@ Register Src="~/Controls/NewPagingControl.ascx" TagPrefix="uc" TagName="NewPagingControl" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <h12>(선수이름을 클릭하여 수정)</h12><br />
        <uc:NewSearchControl ID="search" runat="server"
            OnSearchRequested="Search_SearchRequested"
            OnResetRequested="Search_ResetRequested" />
        <hr />
        <div style="font-style: italic; text-align: right; font-size: 8pt;">
            Total Record:
            <asp:Literal ID="lblTotalRecord" runat="server"></asp:Literal>
        </div>

        <asp:GridView ID="GridView1" runat="server" 
            AutoGenerateColumns="False"
            CssClass="table table-bordered table-hover table-condensed table-striped table-responsive"
            AllowPaging="true"
            PageSize="10"
            PagerSettings-Visible="false"
            OnRowDataBound="GridView1_RowDataBound"
            ShowHeaderWhenEmpty="true">
    
            <HeaderStyle HorizontalAlign="center" BorderStyle="Solid" BorderWidth="1px"/>
            <RowStyle HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="1px"/>
            <FooterStyle BackColor="#CCCCCC" />                     

            <Columns>       
                <asp:TemplateField HeaderText="No">
                    <ItemTemplate />
                    <ItemStyle HorizontalAlign="Center" Width="5%" />
                    <HeaderStyle Width="5%" />
                </asp:TemplateField>
        
                <asp:TemplateField>
                    <HeaderTemplate><asp:Label ID="LB_State" runat="server" Text="상태" /></HeaderTemplate>
                    <ItemTemplate><%# Eval("UserWClass") %></ItemTemplate>
                    <HeaderStyle Width="8%" />
                    <ItemStyle Width="8%" />
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate><asp:Label ID="LB_Id" runat="server" Text="ID" /></HeaderTemplate>
                    <ItemTemplate><%# Eval("UserId") %></ItemTemplate>
                    <HeaderStyle Width="10%" />
                    <ItemStyle Width="10%" />
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate><asp:Label ID="LB_Name" runat="server" Text="이름" /></HeaderTemplate>
                    <ItemTemplate>
                        <%# Eval("UserWClass").Equals("승인대기") ? "<span class='badge text-bg-secondary'>New</span>" : "" %>
                        <asp:HyperLink ID="lnkTitle" runat="server" CssClass="HyperLink"
                            NavigateUrl='<%# "~/Sites/Admin/Player Information.aspx?UserId=" + Eval("UserId") %>'>
                            <%# Dul.StringLibrary.CutStringUnicode(Eval("UserName").ToString(), 30) %>
                        </asp:HyperLink>
                    </ItemTemplate>
                    <HeaderStyle Width="15%" />
                    <ItemStyle Width="15%" />
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate><asp:Label ID="LB_Birth" runat="server" Text="생년월일" /></HeaderTemplate>
                    <ItemTemplate><%# Eval("FormattedBirthDate") %></ItemTemplate>
                    <HeaderStyle Width="10%" />
                    <ItemStyle Width="10%" />
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate><asp:Label ID="LB_Gender" runat="server" Text="성별" /></HeaderTemplate>
                    <ItemTemplate><%# Eval("GenderText") %></ItemTemplate>
                    <HeaderStyle Width="7%" />
                    <ItemStyle Width="7%" />
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate><asp:Label ID="LB_Note" runat="server" Text="비고" /></HeaderTemplate>
                    <ItemTemplate><%# Eval("UserNote") %></ItemTemplate>
                    <HeaderStyle Width="45%" />
                    <ItemStyle Width="45%" HorizontalAlign="Left" />
                </asp:TemplateField>
            </Columns>

            <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
            <EmptyDataTemplate>선수가 없습니다.</EmptyDataTemplate>
        </asp:GridView>

        <div class="center_container">
            <div>
                <uc:NewPagingControl ID="pager" runat="server"
                    OnPageChanged="Pager_PageChanged" />
            </div>
        </div>
    </div>
</asp:Content>
