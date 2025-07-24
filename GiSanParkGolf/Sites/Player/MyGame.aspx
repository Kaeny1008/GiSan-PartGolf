<%@ Page Title="내 대회" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyGame.aspx.cs" Inherits="GiSanParkGolf.Sites.Player.MyGame" %>

<%@ Register Src="~/Controls/NewSearchControl.ascx" TagPrefix="uc" TagName="NewSearchControl" %>
<%@ Register Src="~/Controls/NewPagingControl.ascx" TagPrefix="uc" TagName="NewPagingControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript">
        var gamecode;

        function ShowModal(gamecode2) {
            gamecode = gamecode2;
            $("#SaveModal").modal("show");
        }
        function GoGameCancel() {
            var gp = "MyGame.aspx?GameCancel=true&GameCode=" + gamecode;
            console.log(gp);
            location.href = gp;

            return false;
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
        .form-control{
            min-width: 70%;
            max-width: 70%;
            text-align: left;
        }
    </style>



    <div id="MainContent" runat="server">
        <!-- 상단 카드: 페이지 설명 영역 -->
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
                            <HeaderStyle Width="50px" />
                            <ItemStyle Width="50px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_Name" runat="server" Text="대회명"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkTitle" runat="server" Class="HyperLink" 
                                    NavigateUrl=<%# "~/Sites/Player/MyGame.aspx?GameCode=" + Eval("GameCode")%>>
                                    <%# Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 25) %>
                                </asp:HyperLink>
                            </ItemTemplate>
                            <HeaderStyle Width="200px" />
                            <ItemStyle Width="200px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_Writer" runat="server" Text="경기장"></asp:Label>
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
                                <asp:Label ID="LB_WriteDate" runat="server" Text="상태"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate />
                            <HeaderStyle Width="60px" />
                            <ItemStyle Width="60px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:Label ID="LB_WriteDate" runat="server" Text="여부"></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate />
                            <HeaderStyle Width="40px" />
                            <ItemStyle Width="40px" />
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
                    참가취소 하시겠습니까?
                    <br />
                    다시 참가하려면 경기등록을 다시 신청하여야 합니다.
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">아니오</button>
                    <asp:Button ID="Button2" 
                        runat="server" 
                        OnClientClick="return GoGameCancel();"
                        class="btn btn-primary" 
                        Text="예" />
                </div>
            </div>
        </div>
    </div>

    <div id="GameContent" runat="server">
        <!-- 상단 카드: 페이지 설명 영역 -->
        <div class="mb-3 text-center">
            <h4 class="fw-bold mb-2" id="H2" runat="server">참여 대회 목록</h4>
            <p class="text-muted" style="font-size: 0.95rem;">
                내 대회의 결과 및 정보를 확인 할 수 있습니다.
            </p>
        </div>
        <div class="row">
            <div class="col-md-4" style="background-color:aliceblue; border-top-left-radius:1rem; border-bottom-left-radius:1rem;">
                <div class="center_container">
                    <div style="width:100%">
                        <div style="text-align:left;">
                            <h4 class="mb-3" style="color:cornflowerblue">선택된 대회정보입니다.</h4>
                            <p></p>
                        </div>
                        <div class="input-group mb-3">
                            <span class="input-group-text">대회명</span>
                            <asp:TextBox ID="TB_GameName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="input-group mb-3">
                            <span class="input-group-text">대회일자</span>
                            <asp:TextBox ID="TB_GameDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="input-group mb-3">
                            <span class="input-group-text">대회장소</span>
                            <asp:TextBox ID="TB_StadiumName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="input-group mb-3">
                            <span class="input-group-text">주최</span>
                            <asp:TextBox ID="TB_GameHost" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="input-group mb-3">
                            <span class="input-group-text">홀당 최대인원</span>
                            <asp:TextBox ID="TB_HoleMaximum" runat="server" CssClass="form-control" TextMode="Number" Text="4" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="input-group mb-3">
                            <span class="input-group-text">모집시작</span>
                            <asp:TextBox ID="TB_StartDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="input-group mb-3">
                            <span class="input-group-text">모집종료</span>
                            <asp:TextBox ID="TB_EndDate" runat="server" CssClass="form-control" TextMode="date" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="input-group mb-3">
                            <span class="input-group-text">비고</span>
                            <asp:TextBox ID="TB_Note" runat="server" CssClass="form-control" Height="300px" TextMode="MultiLine" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="input-group mb-3">
                            <span class="input-group-text">참가인원</span>
                            <asp:TextBox ID="TB_User" runat="server" CssClass="form-control" TextMode="Number" Text="4" Enabled="false"></asp:TextBox>
                        </div>
                    </div>
                </div> 
            </div>
            <div class="col" style="background-color:lightskyblue; border-top-right-radius:1rem; border-bottom-right-radius:1rem">
                <div style="text-align:left;">
                    <h4 class="mb-3" style="color:cornflowerblue">대회 정보표시(예상)</h4>
                    <p></p>
                </div>
            </div>
        </div>
        <br />
        <div class="center_container">
            <input type="button" runat="server" 
                class="btn btn-outline-success btn-lg" 
                style="width:300px; height:50px" 
                value="닫기" 
                OnClick="history.go(-1);" />
        </div>
    </div>
</asp:Content>
