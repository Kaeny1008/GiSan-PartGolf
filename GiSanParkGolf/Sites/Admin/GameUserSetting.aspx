<%@ Page Title="인원 코스 및 배치" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameUserSetting.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameUserSetting" %>

<%@ Register Src="~/Controls/PagingControl.ascx" TagPrefix="uc1" TagName="PagingControl" %>
<%@ Register Src="~/Controls/SearchControl.ascx" TagPrefix="uc1" TagName="SearchControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-5" style="background-color:aliceblue; border-top-left-radius:1rem; border-bottom-left-radius:1rem;">
            <div class="row">
                <div style="text-align:left;">
                    <h4 style="color:cornflowerblue">대회인원 코스 및 배치</h4>
                    <p>대회명을 클릭하여 설정하여 주십시오.</p>
                </div>
            </div>
            <div class="row mb-1">
                <div>
                <uc1:SearchControl runat="server" ID="SearchControl" />
                </div>
            </div>
            <div class="row">
                <asp:GridView ID="GameList"
                    runat="server" AutoGenerateColumns="False" DataKeyNames="GameCode"
                    CssClass="table table-bordered table-hover table-condensed table-striped table-responsive"
                    ShowHeaderWhenEmpty="true" >
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
                                <asp:LinkButton ID="LnkGame" runat="server" CssClass="HyperLink" OnClick="LnkGame_Click" CommandName="select" 
                                    ToolTip='<%#Eval("GameCode")%>'>
                                    <%#Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 25)%>
                                </asp:LinkButton>
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

        <style>
            .input-group{
                text-align: center;
                width:100%;
            }
            .input-group-text{
                min-width: 20%;
                max-width: 20%;
                text-align: center;
            }
            .form-control{
                min-width: 30%;
                max-width: 30%;
                text-align: left;
            }
        </style>

        <div class="col" style="background-color:lightskyblue; border-top-right-radius:1rem; border-bottom-right-radius:1rem">
            <div style="text-align:left;">
                <h4 style="color:white">선택된 대회정보입니다.</h4>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">대회명</span>
                <asp:TextBox ID="TB_GameName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                <span class="input-group-text">대회일자</span>
                <asp:TextBox ID="TB_GameDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">대회장소</span>
                <asp:TextBox ID="TB_StadiumName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                <span class="input-group-text">주최</span>
                <asp:TextBox ID="TB_GameHost" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">홀당 최대인원</span>
                <asp:TextBox ID="TB_HoleMaximum" runat="server" CssClass="form-control" TextMode="Number" Text="" Enabled="false"></asp:TextBox>
                <span class="input-group-text" style="background-color:blueviolet;color:white">참가인원</span>
                <asp:TextBox ID="TB_User" runat="server" CssClass="form-control" TextMode="Number" Text="" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">모집시작</span>
                <asp:TextBox ID="TB_StartDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
                <span class="input-group-text">모집종료</span>
                <asp:TextBox ID="TB_EndDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <span class="input-group-text">비고</span>
                <asp:TextBox ID="TB_Note" runat="server" CssClass="form-control bc-white" Height="300px" Width="80%" TextMode="MultiLine" Enabled="false"></asp:TextBox>
            </div>
            <div class="btn-group" role="group" aria-label="Basic mixed styles example">
                <button runat="server" id="BTN_1" type="button" class="btn btn-danger" disabled>참가마감</button>
                <button runat="server" id="BTN_2" type="button" class="btn btn-warning" disabled>참가자 확인</button>
                <button runat="server" id="BTN_3" type="button" class="btn btn-success" disabled>선수 코스배치</button>
            </div>
        </div>
    </div>

</asp:Content>
