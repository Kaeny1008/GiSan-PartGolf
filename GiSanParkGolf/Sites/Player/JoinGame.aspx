<%@ Page Title="대회참가" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="JoinGame.aspx.cs" Inherits="GiSanParkGolf.Sites.Player.JoinGame" %>

<%@ Register Src="~/Controls/NewSearchControl.ascx" TagPrefix="uc" TagName="NewSearchControl" %>
<%@ Register Src="~/Controls/NewPagingControl.ascx" TagPrefix="uc" TagName="NewPagingControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript">
        function ShowModal() {
            $("#SaveModal").modal("show");
        }
    </script>
    <style>
        .input-group{
            text-align: center;
            width:100%;
        }
        .input-group-text{
            min-width: 30%;
            max-width: 30%;
            text-align: center;
        }
        .redfont{
            color:red;
        }
        .form-control{
            min-width: 70%;
            max-width: 70%;
            text-align: left;
        }
        .bc-white{
            background-color:white;
        }
    </style>

    <div id="MainContent" runat="server">
        <div class="center_container">
            <div style="width:100%">
                <div style="text-align:left;">
                    <h4 style="color:cornflowerblue">참여가능 대회 목록</h4>
                    <p>참가하려는 대회명을 선택하여 참가신청을 하십시오.</p>
                </div>
                <div style="width:40%">
                    <uc:NewSearchControl ID="search" runat="server"
                        OnSearchRequested="Search_SearchRequested"
                        OnResetRequested="Search_ResetRequested" />
                </div>
                <asp:GridView ID="GameList" runat="server"
                    AutoGenerateColumns="False" CssClass="table table-bordered table-hover table-condensed table-striped table-responsive"
                    ShowHeaderWhenEmpty="true" OnRowDataBound="GameList_RowDataBound">
                    <HeaderStyle HorizontalAlign="center" BorderStyle="Solid" BorderWidth="1px"/>
                    <RowStyle HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="1px"/>
                    <Columns>
                        <%-- No 컬럼: RowDataBound에서 처리 --%>
                        <asp:TemplateField HeaderText="No">
                            <ItemTemplate />
                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                            <HeaderStyle Width="50px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_Name" runat="server" Text="대회명"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkTitle" runat="server" Class="HyperLink" 
                                    NavigateUrl=<%# "~/Sites/Player/JoinGame.aspx?GameCode=" + Eval("GameCode")%>>
                                    <%# Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 25) %>
                                </asp:HyperLink>
                            </ItemTemplate>
                            <HeaderStyle Width="200px" />
                            <ItemStyle Width="200px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_Writer" runat="server" Text="개최지"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#Eval("StadiumName")%>
                            </ItemTemplate>
                            <HeaderStyle Width="120px" />
                            <ItemStyle Width="120px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_Writer" runat="server" Text="주최자"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#Eval("GameHost")%>
                            </ItemTemplate>
                            <HeaderStyle Width="120px" />
                            <ItemStyle Width="120px" />
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
                                <asp:Label ID="LB_WriteDate" runat="server" Text="모집시작"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#Eval("StartRecruiting", "{0:yyyy-MM-dd}")%>
                            </ItemTemplate>
                            <HeaderStyle Width="90px" />
                            <ItemStyle Width="90px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_WriteDate" runat="server" Text="모집종료"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#Eval("EndRecruiting", "{0:yyyy-MM-dd}")%>
                            </ItemTemplate>
                            <HeaderStyle Width="90px" />
                            <ItemStyle Width="90px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_WriteDate" runat="server" Text="참가자"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#Eval("ParticipantNumber")%>
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                            <ItemStyle Width="50px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_WriteDate" runat="server" Text="상태"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%#Eval("GameStatus")%>
                            </ItemTemplate>
                            <HeaderStyle Width="50px" />
                            <ItemStyle Width="50px" />
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>참가 신청 가능한 대회가 없습니다.</EmptyDataTemplate>
                </asp:GridView>
                <div style="text-align: right; font-style: italic; font-size: 8pt;">
                    총 건수: <asp:Literal ID="lblTotalRecord" runat="server" />
                </div>
                <uc:NewPagingControl ID="pager" runat="server"
                    OnPageChanged="Pager_PageChanged" />
            </div>
        </div>
    </div>

    <div id="GameContent" runat="server">
        <div class="center_container">
            <div style="width:40%">
                <div style="text-align:left;">
                    <h4 style="color:cornflowerblue">선택된 대회정보입니다.</h4>
                    <p>확인 후 '참가신청' 버튼을 눌러주십시오.</p>
                </div>
                <div class="input-group mb-3">
                    <span class="input-group-text redfont">대회명</span>
                    <asp:TextBox ID="TB_GameName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                </div>
                <div class="input-group mb-3">
                    <span class="input-group-text redfont">대회일자</span>
                    <asp:TextBox ID="TB_GameDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
                </div>
                <div class="input-group mb-3">
                    <span class="input-group-text redfont">대회장소</span>
                    <asp:TextBox ID="TB_StadiumName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                </div>
                <div class="input-group mb-3">
                    <span class="input-group-text redfont">주최</span>
                    <asp:TextBox ID="TB_GameHost" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                </div>
                <div class="input-group mb-3">
                    <span class="input-group-text redfont">홀당 최대인원</span>
                    <asp:TextBox ID="TB_HoleMaximum" runat="server" CssClass="form-control" TextMode="Number" Text="4" Enabled="false"></asp:TextBox>
                </div>
                <div class="input-group mb-3">
                    <span class="input-group-text redfont">모집시작</span>
                    <asp:TextBox ID="TB_StartDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
                </div>
                <div class="input-group mb-3">
                    <span class="input-group-text redfont">모집종료</span>
                    <asp:TextBox ID="TB_EndDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
                </div>
                <div class="input-group mb-3">
                    <span class="input-group-text">비고</span>
                    <asp:TextBox ID="TB_Note" runat="server" CssClass="form-control bc-white" Height="300px" TextMode="MultiLine" Enabled="false"></asp:TextBox>
                </div>
                <br />
                <asp:button ID="BTN_Save" type="button" runat="server" 
                    class="btn btn-outline-success btn-lg" 
                    style="width:300px; height:50px" 
                    Text="참가신청" 
                    ValidationGroup="NewGame"
                    OnClientClick="ShowModal();return false;" />
            </div>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="SaveModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="exampleModalLabel">확인</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    참가신청 하시겠습니까?
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">아니오</button>
                    <asp:Button ID="Button2" 
                        runat="server" 
                        OnClick="BTN_Save_Click" 
                        class="btn btn-primary" 
                        Text="예" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
