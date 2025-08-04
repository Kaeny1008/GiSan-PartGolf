<%@ Page Title="대회참가" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="JoinGame.aspx.cs" Inherits="GiSanParkGolf.Sites.Player.JoinGame" %>

<%@ Register Src="~/Controls/NewSearchControl.ascx" TagPrefix="uc" TagName="NewSearchControl" %>
<%@ Register Src="~/Controls/NewPagingControl.ascx" TagPrefix="uc" TagName="NewPagingControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript">
        function ShowModal() {
            $("#SaveModal").modal("show");
        }

        let showModalName, showTitle, showBody, onlyYes, showYesButton;
        let launch = false;

        function launchModal(modalname, title, body, yes, yesbutton) {
            showModalName = modalname;
            showTitle = title;
            showBody = body;
            onlyYes = yes;
            showYesButton = yesbutton;
            launch = true;
            pageLoad(); // 바로 실행
        }

        function pageLoad() {
            if (!launch) return;
            const modal = $(showModalName);

            const bodyWithBr = showBody.replace(/\r\n|\n/g, "<br>");

            modal.find('.modal-title').text(showTitle);
            modal.find('.modal-body').html(bodyWithBr);

            const $buttons = $('#SaveModalButtons');
            $buttons.html(''); // 초기화

            if (onlyYes) {
                // 성공 메시지일 경우 확인 누르면 이동
                if (showTitle === "참가신청 성공") {
                    $buttons.append(`
                        <button type="button" class="btn btn-primary" onclick="location.href='/Sites/Player/JoinGame.aspx';">
                            확인
                        </button>`
                    );
                } else {
                    $buttons.append(`
                        <button type="button" class="btn btn-primary" onclick="history.back();">
                            확인
                        </button>`
                    );
                }
            } else {
                $buttons.append(`<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">아니오</button>`);
                switch (showYesButton) {
                    case 0:
                        $buttons.append(`<button type="button" class="btn btn-primary" onclick="SubmitJoin();">예</button>`);
                        break;
                }
            }

            modal.modal("show");
            launch = false;
        }

        function SubmitJoin() {
            document.getElementById('<%= BTN_JoinGame.ClientID %>').click();
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
        <!-- 상단 카드: 페이지 설명 영역 -->
        <div class="mb-3 text-center">
            <h4 class="fw-bold mb-2" id="MainTitle" runat="server">참여가능 대회 목록</h4>
            <p class="text-muted" style="font-size: 0.95rem;">
                참가하려는 대회명을 선택하여 참가신청을 하십시오.
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
        <!-- 상단 카드: 페이지 설명 영역 -->
        <div class="mb-3 text-center">
            <h4 class="fw-bold mb-2" id="H1" runat="server">선택된 대회정보입니다.</h4>
            <p class="text-muted" style="font-size: 0.95rem;">
                확인 후 '참가신청' 버튼을 눌러주십시오.
            </p>
        </div>
        <div class="container mt-4">
            <div class="custom-card" style="width: 40%">
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
                <%-- 참가신청 버튼: 모달로 확인 처리 --%>
                <asp:Button ID="BTN_Save" runat="server"
                    CssClass="btn btn-outline-success btn-lg"
                    Style="width:300px; height:50px"
                    Text="참가신청"
                    ValidationGroup="NewGame"
                    OnClientClick="launchModal('#SaveModal', '확인', '참가신청 하시겠습니까?', false, 0); return false;" />
            </div>
        </div>
    </div>

    <%-- 상황별 안내를 위한 단일 모달 --%>
    <div class="modal fade" id="SaveModal" tabindex="-1" aria-labelledby="SaveModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">

                <%-- 모달 헤더 --%>
                <div class="modal-header">
                    <h5 class="modal-title" id="SaveModalLabel">알림</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <%-- 모달 본문 메시지 --%>
                <div class="modal-body" id="SaveModalMessage">
                    메시지 내용이 여기에 들어옵니다.
                </div>

                <%-- 모달 푸터 버튼 컨테이너 --%>
                <div class="modal-footer" id="SaveModalButtons">
                    <%-- 버튼들은 JS에서 동적 생성됨 --%>
                </div>

            </div>
        </div>
    </div>
    <%-- 참가신청 처리용 서버 버튼 --%>
    <asp:Button ID="BTN_JoinGame" runat="server"
        Text="참가신청 처리"
        OnClick="JoinGame_Click"
        Style="display:none" />
</asp:Content>
