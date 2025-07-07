<%@ Page Title="선수정보 관리" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Player Management.aspx.cs" 
    Inherits="GiSanParkGolf.Sites.Admin.Player_Management" EnableEventValidation="false" %>

<%@ Register Src="~/Controls/PagingControl.ascx" TagPrefix="uc1" TagName="PagingControl" %>
<%@ Register Src="~/Controls/SearchControl.ascx" TagPrefix="uc1" TagName="SearchControl" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <uc1:SearchControl runat="server" ID="SearchControl" />
        <hr />
        <div style="font-style: italic; text-align: right; font-size: 8pt;">
            Total Record:
            <asp:Literal ID="lblTotalRecord" runat="server"></asp:Literal>
        </div>
        <asp:Label ID="LabelResult" runat="server" Text="선택정보" Visible="false"></asp:Label>
        <h12>(선수이름을 클릭하여 수정)</h12><br />

        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
            CssClass="table table-bordered table-hover table-condensed table-striped table-responsive">
        <HeaderStyle HorizontalAlign="center" BorderStyle="Solid" BorderWidth="1px"/>
        <RowStyle HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="1px"/>
        <FooterStyle BackColor="#CCCCCC" />                     
            <Columns>       
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_No" runat="server" Text="No."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%#Eval("RowNumber")%>
                    </ItemTemplate>
                    <HeaderStyle Width="5%" />
                    <ItemStyle Width="5%" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_No" runat="server" Text="상태"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%#Eval("UserWClass")%>
                    </ItemTemplate>
                    <HeaderStyle Width="10%" />
                    <ItemStyle Width="10%" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_No" runat="server" Text="ID"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%#Eval("UserId")%>
                    </ItemTemplate>
                    <HeaderStyle Width="10%" />
                    <ItemStyle Width="10%" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_No" runat="server" Text="이름"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%# Eval("UserWClass").Equals("승인대기") ? 
                                "<span class=\"badge text-bg-secondary\">New</span>" 
                                : 
                                "" 
                        %>
                        <asp:HyperLink ID="lnkTitle" runat="server" Class="HyperLink"
                            NavigateUrl='<%# "~/Sites/Admin/Player Information.aspx?UserId=" + Eval("UserId") %>'>
                            <%# Dul.StringLibrary.CutStringUnicode(Eval("UserName").ToString(), 30) %>
                        </asp:HyperLink>
                    </ItemTemplate>
                    <HeaderStyle Width="15%" />
                    <ItemStyle Width="15%" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_No" runat="server" Text="생년월일"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%#Eval("UserNumber")%>
                    </ItemTemplate>
                    <HeaderStyle Width="10%" />
                    <ItemStyle Width="10%" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="LB_No" runat="server" Text="비고"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%#Eval("UserNote")%>
                    </ItemTemplate>
                    <HeaderStyle Width="50%" />
                    <ItemStyle Width="50%" />
                </asp:TemplateField>
            </Columns>
                <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
        </asp:GridView>

        <div class="center_container">
            <div>
                <uc1:PagingControl runat="server" ID="PagingControl" />
            </div>
        </div>
    </div>
</asp:Content>
