<%@ Page Title="내 대회" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyGame.aspx.cs" Inherits="GiSanParkGolf.Sites.Player.MyGame" %>

<%@ Register Src="~/Controls/NewSearchControl.ascx" TagPrefix="uc" TagName="NewSearchControl" %>
<%@ Register Src="~/Controls/NewPagingControl.ascx" TagPrefix="uc" TagName="NewPagingControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mb-3 text-center">
        <h4 class="fw-bold mb-2" id="H1" runat="server">참여 대회 목록</h4>
        <p class="text-muted" style="font-size: 0.95rem;">
            내 대회의 결과 및 참가여부 수정을 할 수 있습니다.
        </p>
    </div>
    <div class="container mt-4">
        <div class="custom-card">
            <div style="width:40%">
                <uc:NewSearchControl ID="search" runat="server"
                    OnSearchRequested="Search_SearchRequested"
                    OnResetRequested="Search_ResetRequested" />
            </div>
            <asp:GridView ID="GameList" runat="server"
                AutoGenerateColumns="False" DataKeyNames="GameCode"
                CssClass="table table-bordered table-hover table-condensed table-striped table-responsive"
                ShowHeaderWhenEmpty="true"
                OnRowDataBound="GameList_RowDataBound">
                <HeaderStyle HorizontalAlign="center" BorderStyle="Solid" BorderWidth="1px"/>
                <RowStyle HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="1px"/>
                <Columns>
                        <%--No 컬럼은 빈 템플릿 처리--%> 
                    <asp:TemplateField HeaderText="No">
                        <ItemTemplate />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_Name" runat="server" Text="대회명"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkTitle" runat="server" Class="HyperLink" 
                                NavigateUrl=<%# "~/Sites/Player/MyGameDetail.aspx?GameCode=" + Eval("GameCode")%>>
                                <%# Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 25) %>
                            </asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_Writer" runat="server" Text="경기장"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("StadiumName")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_Writer" runat="server" Text="주최자"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("GameHost")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_WriteDate" runat="server" Text="대회일자"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("GameDate", "{0:yyyy-MM-dd}")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_WriteDate" runat="server" Text="모집시작"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("StartRecruiting", "{0:yyyy-MM-dd}")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_WriteDate" runat="server" Text="모집종료"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("EndRecruiting", "{0:yyyy-MM-dd}")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_WriteDate" runat="server" Text="상태"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("GameStatus")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="LB_GameJoin" runat="server" Text="참가여부"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("IsCancelledText")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>참가 신청 또는 참가한 데이터가 없습니다.</EmptyDataTemplate>
            </asp:GridView>
            <div style="text-align: right; font-style: italic; font-size: 8pt;">
                총 건수: <asp:Literal ID="lblTotalRecord" runat="server" />
            </div>
            <uc:NewPagingControl ID="pager" runat="server"
                OnPageChanged="Pager_PageChanged" />
        </div>
    </div>
</asp:Content>
