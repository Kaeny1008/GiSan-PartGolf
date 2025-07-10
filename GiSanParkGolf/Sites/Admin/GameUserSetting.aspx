<%@ Page Title="인원 코스 및 배치" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameUserSetting.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameUserSetting" %>

<%@ Register Src="~/Controls/PagingControl.ascx" TagPrefix="uc1" TagName="PagingControl" %>
<%@ Register Src="~/Controls/SearchControl.ascx" TagPrefix="uc1" TagName="SearchControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-5" style="background-color:lightskyblue; border-top-left-radius:1rem; border-bottom-left-radius:1rem;">
            <div class="row">
                <div style="text-align:left;">
                    <h4 style="color:cornflowerblue">대회인원 코스 및 배치</h4>
                    <p>대회명을 클릭하여 설정하여 주십시오.</p>
                </div>
            </div>
            <div class="row mb-1">
                <uc1:SearchControl runat="server" ID="SearchControl" />
            </div>
            <div class="row">
                <asp:GridView ID="GameList"
                    runat="server" AutoGenerateColumns="False" DataKeyNames="GameCode"
                    CssClass="table table-bordered table-hover table-condensed table-striped table-responsive"
                    ShowHeaderWhenEmpty="true">
                    <HeaderStyle HorizontalAlign="center" BorderStyle="Solid" BorderWidth="1px"/>
                    <RowStyle HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="1px"/>
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_No" runat="server" Text="No."></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#Eval("RowNumber")%>
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                            <ItemStyle Width="50px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_Name" runat="server" Text="대회명"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkTitle" runat="server" Class="HyperLink" 
                                    NavigateUrl=<%# "~/Sites/Admin/GameUserSetting.aspx?GameCode=" + Eval("GameCode")%>>
                                    <%# Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 25) %>
                                </asp:HyperLink>
                            </ItemTemplate>
                            <HeaderStyle Width="200px" />
                            <ItemStyle Width="200px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_WriteDate" runat="server" Text="대회일자"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#Eval("GameDate", "{0:yyyy-MM-dd}")%>
                            </ItemTemplate>
                            <HeaderStyle Width="90px" />
                            <ItemStyle Width="90px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_WriteDate" runat="server" Text="상태"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#Eval("GameStatus").ToString().Equals("모집중") ?
                                    "<a style=\"color:blue;\">모집중</a>"
                                    :
                                    "<a>" + Eval("GameStatus") + "</a>"
                                %>
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                            <ItemStyle Width="50px" />
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>데이터가 없습니다.</EmptyDataTemplate>
                </asp:GridView>
            </div>
            <div class="row">
                <div style="font-style: italic; text-align: right; font-size: 8pt;">
                    Total Record:
                    <asp:Literal ID="lblTotalRecord" runat="server"></asp:Literal>
                </div>
                <div class="center_container">
                    <uc1:PagingControl runat="server" ID="PagingControl" />
                </div>
            </div>
        </div>
        <div class="col" style="background-color:aliceblue; border-top-right-radius:1rem; border-bottom-right-radius:1rem">

        </div>
    </div>

</asp:Content>
