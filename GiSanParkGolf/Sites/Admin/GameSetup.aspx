<%@ Page Title="인원 코스 및 배치" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameSetup.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameSetup" %>

<%@ Register Src="~/Controls/NewSearchControl.ascx" TagPrefix="uc" TagName="NewSearchControl" %>
<%@ Register Src="~/Controls/NewPagingControl.ascx" TagPrefix="uc" TagName="NewPagingControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        let modalConfig = {
            name: "",          // 모달 ID
            title: "",         // 모달 제목
            body: "",          // 모달 본문 메시지
            yesButtonType: 0,  // 예 버튼 종류 (0: 확인형, 1: 실행형)
            launch: false      // 실행 여부
        };

        function launchModal(modalId, title, body, yesButtonType = 0) {
            modalConfig.name = modalId;
            modalConfig.title = title;
            modalConfig.body = body;
            modalConfig.yesButtonType = yesButtonType;
            modalConfig.launch = true;

            renderModal();
        }

        function renderModal() {
            if (!modalConfig.launch || !modalConfig.name) return;

            const showmodal = $(modalConfig.name);

            // 콘텐츠 삽입
            showmodal.find(".modal-title").text(modalConfig.title);
            showmodal.find(".modal-body").text(modalConfig.body);

            // 모든 버튼 초기화
            showmodal.find("#BTN_No, #BTN_Close, #MainContent_BTN_SettingYes, #MainContent_BTN_SaveAssignment_Final").hide().off("click");

            // 예 버튼 설정 - switch 문으로 변경
            switch (modalConfig.yesButtonType) {
                case 0: // 확인 모드
                    showmodal.find("#BTN_Close").show();
                    break;

                case 1: // 실행 모드 (예 / 아니오)
                    showmodal.find("#BTN_No").show();
                    showmodal.find("#MainContent_BTN_SettingYes").show();
                    break;

                case 2:
                    showmodal.find("#BTN_No").show();
                    showmodal.find("#MainContent_BTN_SaveAssignment_Final").show();
                default:
                    break;
            }

            // 모달 실행
            const modalElement = document.querySelector(modalConfig.name);
            const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement);
            modalInstance.show();
        }

        function switchPanels() {
            const leftPanel = document.getElementById('leftPanel');
            const rightPanel = document.getElementById('rightPanel');

            leftPanel.classList.add('fade-out');
            leftPanel.style.display = 'none'; // 바로 공간 제거

            rightPanel.classList.remove('hidden');
            rightPanel.classList.add('fade-in');
        }

        function reversePanels() {
            const leftPanel = document.getElementById('leftPanel');
            const rightPanel = document.getElementById('rightPanel');

            // 우측 패널 페이드 아웃
            rightPanel.classList.remove('fade-in');
            rightPanel.classList.add('fade-out');

            // 바로 우측 패널 숨기고 좌측 패널 보여주기
            rightPanel.style.display = 'none';
            leftPanel.style.display = 'block';
            leftPanel.classList.remove('fade-out');
            leftPanel.classList.add('fade-in');
        }
    </script>

    <style type="text/css" media="print">
        body * {
            visibility: hidden;
        }

        #printArea, #resultPrint {
            visibility: visible;
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
        }

        #printArea *, #resultPrint * {
            visibility: visible;
        }

        .panel-left {
          opacity: 1;
          transition: opacity 0.5s ease;
        }

        .panel-left.fade-out {
          opacity: 0;
          pointer-events: none;
        }

        .panel-right {
          opacity: 0;
          transition: opacity 0.5s ease;
          width: 100%;
          position: absolute;
          top: 0;
          left: 0;
          z-index: 10;
        }

        .panel-right.fade-in {
          opacity: 1;
        }

        .hidden {
          display: none;
        }
    </style>

    <!-- 상단 카드: 페이지 설명 영역 -->
    <div class="mb-3 text-center">
        <h4 class="fw-bold mb-2" id="MainTitle" runat="server">인원 코스 및 배치</h4>
        <p class="text-muted" style="font-size: 0.95rem;">
            해당 대회의 선수 배치를 할 수 있습니다.
        </p>
    </div>

    <div class="container mt-4">
        <div class="row position-relative">
            <!-- 좌측: 대회 목록 -->
            <div id="leftPanel" class="panel-left">
                <div class="custom-card">
                    <h4 class="card-title">대회 설정</h4>
                    <p class="mb-2 text-muted">대회명을 클릭하여 세부정보를 확인하세요.</p>

                    <!-- 검색 컨트롤 -->
                    <uc:NewSearchControl ID="search" runat="server"
                        OnSearchRequested="Search_SearchRequested"
                        OnResetRequested="Search_ResetRequested" />

                    <!-- 그리드 뷰 -->
                    <asp:GridView ID="GameList" runat="server"
                        CssClass="table table-bordered table-hover table-condensed table-striped table-responsive"
                        AutoGenerateColumns="False"
                        DataKeyNames="GameCode" 
                        OnRowCommand="GameList_RowCommand"
                        OnRowDataBound="GameList_RowDataBound"
                        ShowHeaderWhenEmpty="true" >
                        <Columns>
                            <%-- No 컬럼 --%>
                            <asp:TemplateField HeaderText="No">
                                <ItemTemplate />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                                <HeaderStyle Width="50px" />
                            </asp:TemplateField>

                            <%-- 대회명 링크버튼 --%>
                            <asp:TemplateField HeaderText="대회명">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LnkGame" runat="server"
                                        CssClass="HyperLink"
                                        CommandName="SelectRow"
                                        CommandArgument="<%# Container.DataItemIndex %>"
                                        Text='<%# Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 25) %>'>
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <HeaderStyle Width="200px" />
                                <ItemStyle Width="200px" />
                            </asp:TemplateField>

                            <%-- 대회일자 --%>
                            <asp:TemplateField HeaderText="대회일자">
                                <ItemTemplate>
                                    <%# Eval("GameDate", "{0:yyyy-MM-dd}") %>
                                </ItemTemplate>
                                <HeaderStyle Width="90px" />
                                <ItemStyle Width="90px" />
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>데이터가 없습니다.</EmptyDataTemplate>
                    </asp:GridView>

                    <!-- 페이징 컨트롤 -->
                    <%--<div class="d-flex justify-content-between align-items-center mt-2">--%>
                        <small class="text-muted">총 건수: <asp:Literal ID="lblTotalRecord" runat="server" /></small>
                        <uc:NewPagingControl ID="pager" runat="server"
                            OnPageChanged="Pager_PageChanged" />
                    <%--</div>--%>
                </div>
            </div>

            <!-- 우측: 상세정보 탭 카드 -->
            <div id="rightPanel" class="panel-right hidden">
                <div class="custom-card">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h4 class="card-title mb-0">대회 상세정보</h4>
                        <button class="btn btn-outline-success btn-sm" onclick="reversePanels()">목록으로 이동</button>
                    </div>
                    
                    <!-- 탭 메뉴 -->
                    <ul class="nav nav-tabs mb-3">
                        <li class="nav-item"><a class="nav-link active" data-bs-toggle="tab" href="#tab-info">기본정보</a></li>
                        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-player">참가자확인</a></li>
                        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-course">코스배치</a></li>
                        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-result">코스배치 결과 확인</a></li>
                    </ul>
                    
                    <!-- 탭 콘텐츠 -->
                    <div class="tab-content">
                        <!-- 기본정보 -->
                        <div class="tab-pane fade show active" id="tab-info">
                            <div class="input-group mb-2">
                                <span class="input-group-text">대회코드</span>
                                <asp:TextBox ID="TB_GameCode" runat="server" CssClass="form-control" Enabled="false" />
                                <span class="input-group-text">현재상태</span>
                                <asp:TextBox ID="TB_GameStatus" runat="server" CssClass="form-control" Enabled="false" />
                            </div>
                            <div class="input-group mb-2">
                                <span class="input-group-text">대회명</span>
                                <asp:TextBox ID="TB_GameName" runat="server" CssClass="form-control" Enabled="false" />
                                <span class="input-group-text">대회일자</span>
                                <asp:TextBox ID="TB_GameDate" runat="server" CssClass="form-control" Enabled="false" TextMode="Date" />
                            </div>
                            <div class="input-group mb-2">
                                <span class="input-group-text">장소</span>
                                <asp:TextBox ID="TB_StadiumName" runat="server" CssClass="form-control" Enabled="false" />
                                <span class="input-group-text">주최</span>
                                <asp:TextBox ID="TB_GameHost" runat="server" CssClass="form-control" Enabled="false" />
                            </div>
                            <div class="input-group mb-2">
                                <span class="input-group-text">홀당 최대</span>
                                <asp:TextBox ID="TB_HoleMaximum" runat="server" CssClass="form-control" TextMode="Number" Enabled="false" />
                                <span class="input-group-text">참가인원</span>
                                <asp:TextBox ID="TB_User" runat="server" CssClass="form-control" TextMode="Number" Enabled="false" />
                            </div>
                            <div class="input-group mb-2">
                                <span class="input-group-text">모집시작</span>
                                <asp:TextBox ID="TB_StartDate" runat="server" CssClass="form-control" TextMode="Date" Enabled="false" />
                                <span class="input-group-text">모집종료</span>
                                <asp:TextBox ID="TB_EndDate" runat="server" CssClass="form-control" TextMode="Date" Enabled="false" />
                            </div>
                            <div class="input-group mb-2">
                                <span class="input-group-text">비고</span>
                                <asp:TextBox ID="TB_Note" runat="server" CssClass="form-control note" TextMode="MultiLine" Enabled="false" />
                            </div>
                        </div>

                        <!-- 참가자확인 탭 -->
                        <div class="tab-pane fade" id="tab-player">
                            <div class="mb-2 d-flex gap-2 justify-content-end">
                                <asp:Button ID="BTN_ToExcel" runat="server" Text="Excel 저장" OnClick="BTN_ToExcel_Click" CssClass="btn btn-primary btn-sm" />
                                <asp:Button ID="BTN_Print" runat="server" Text="프린트 하기" CssClass="btn btn-outline-dark btn-sm"
                                    OnClientClick="window.print(); return false;" />
                            </div>

                            <div id="printArea">
                                <asp:GridView ID="gvPlayerList" runat="server"
                                    AutoGenerateColumns="False"
                                    DataKeyNames="UserId"
                                    CssClass="table table-bordered table-hover table-condensed table-striped table-responsive"
                                    ShowHeaderWhenEmpty="true" OnRowDataBound="gvPlayerList_RowDataBound">
                                    <HeaderStyle HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="1px" />
                                    <RowStyle HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="1px" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="No.">
                                            <ItemTemplate>
                                                <%-- 번호는 서버 코드에서 삽입하므로 비워둠 --%>
                                            </ItemTemplate>
                                            <ItemStyle Width="8%" />
                                            <HeaderStyle Width="8%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ID">
                                            <ItemTemplate><%# Eval("UserId") %></ItemTemplate>
                                            <ItemStyle Width="15%" />
                                            <HeaderStyle Width="15%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="성명">
                                            <ItemTemplate><%# Eval("UserName") %></ItemTemplate>
                                            <ItemStyle Width="15%" />
                                            <HeaderStyle Width="15%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="생년월일">
                                            <ItemTemplate><%# Eval("FormattedBirthDate") %></ItemTemplate>
                                            <ItemStyle Width="12%" />
                                            <HeaderStyle Width="12%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="성별">
                                            <ItemTemplate><%# Eval("GenderText") %></ItemTemplate>
                                            <ItemStyle Width="10%" />
                                            <HeaderStyle Width="10%" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>데이터가 없습니다.</EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </div>

                        <!-- 코스배치 탭 -->
                        <div class="tab-pane fade" id="tab-course">
                            <!-- 버튼 영역 -->
                            <div class="d-flex justify-content-between align-items-center mb-3">
                                <h5 class="mb-0 text-muted">코스배치를 시작하려면 아래 설정을 확인하세요</h5>
                                <asp:Button ID="BTN_Setting" runat="server"
                                    CssClass="btn btn-outline-success btn-sm"
                                    Text="코스 배치 시작"
                                    OnClientClick="launchModal('#MainModal', '코스배치', '선수 코스배치를 하시겠습니까?', 1); return false;" />
                            </div>

                            <!-- 설정 옵션 카드 -->
                            <div class="border rounded p-3 bg-light">
                                <div class="input-group mb-2">
                                    <span class="input-group-text">핸디캡 적용</span>
                                    <asp:DropDownList ID="DDL_HandicapUse" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="사용하지 않음" Value="False" />
                                        <asp:ListItem Text="사용함" Value="True" />
                                    </asp:DropDownList>
                                </div>

                                <small class="text-muted">
                                    핸디캡 사용 시, 참가자의 개별 핸디 정보를 기준으로 코스를 자동 배치합니다.<br />
                                    미사용 시에는 무작위 또는 순번 기준으로 배치됩니다.
                                </small>
                            </div>
                        </div>

                        <!-- 코스배치 결과 탭 -->
                        <div class="tab-pane fade" id="tab-result">
                            <h5 class="mb-0 text-muted">코스배치 결과 확인</h5>

                            <div class="mb-3 d-flex gap-2 justify-content-end">
                                <asp:Button ID="BTN_RefreshResult" runat="server"
                                    CssClass="btn btn-info btn-sm"
                                    Text="새로고침"
                                    OnClick="BTN_RefreshResult_Click" />

                                <asp:Button ID="BTN_SaveAssignment" runat="server"
                                    CssClass="btn btn-success btn-sm"
                                    Text="배정 결과 저장"
                                    OnClick="BTN_SaveAssignment_Click" />

                                <div style="width: 40px;"></div>

                                <asp:Button ID="BTN_ResultToExcel" runat="server"
                                    CssClass="btn btn-warning btn-sm text-white"
                                    Text="Excel 저장"
                                    OnClick="BTN_ResultToExcel_Click" />

                                <asp:Button ID="Button2" runat="server"
                                    CssClass="btn btn-secondary btn-sm"
                                    Text="프린트 하기"
                                    OnClientClick="window.print(); return false;" />
                            </div>

                            <div id="resultPrint">
                                <asp:GridView ID="gvCourseResult" runat="server"
                                    AutoGenerateColumns="False"
                                    OnRowDataBound="gvCourseResult_RowDataBound"
                                    CssClass="table table-bordered table-hover table-condensed table-striped table-responsive"
                                    EmptyDataText="배치된 코스가 없습니다. 먼저 코스배치를 실행하세요.">
                                    <Columns>
                                        <asp:BoundField DataField="UserId" HeaderText="ID" />
                                        <asp:BoundField DataField="UserName" HeaderText="성명" />
                                        <asp:BoundField DataField="GenderText" HeaderText="성별" />
                                        <asp:BoundField DataField="AgeHandicap" HeaderText="핸디캡" />
                                        <asp:BoundField DataField="HoleNumber" HeaderText="배정홀" />
                                        <asp:BoundField DataField="TeamNumber" HeaderText="팀번호" />
                                    </Columns>
                                </asp:GridView>
                            </div>

                            <small class="text-muted mt-3 d-block">
                                ※ 핸디캡 기준으로 코스가 배정된 경우 우선순위, 구간, 홀 번호 등 조건에 맞게 배정됩니다.<br />
                                ※ 배치 결과는 출력 또는 저장 가능합니다.
                            </small>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- 공통 확인 모달 -->
    <div class="modal fade" id="MainModal" tabindex="-1" aria-labelledby="MainModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">

                <!-- 모달 제목 영역 -->
                <div class="modal-header">
                    <h5 class="modal-title" id="MainModalLabel">확인</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <!-- 모달 메시지 영역 -->
                <div class="modal-body">
                    모달 본문 메시지가 여기에 출력됩니다.
                </div>

                <!-- 모달 버튼 영역 -->
                <div class="modal-footer">
                    <button id="BTN_No" type="button" class="btn btn-secondary" data-bs-dismiss="modal">아니오</button>

                    <!-- 기능별 예 버튼 (launchModal에서 조건에 따라 보임 처리) -->
                    <button id="BTN_Close" type="button" class="btn btn-secondary" data-bs-dismiss="modal">확인</button>
                    <asp:Button ID="BTN_SettingYes" runat="server" Text="예" CssClass="btn btn-primary"
                        OnClick="BTN_SettingYes_Click" />
                    <asp:Button ID="BTN_SaveAssignment_Final" runat="server"
                                CssClass="btn btn-primary" 
                                Text="예"
                                OnClick="BTN_SaveAssignment_Final_Click" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>
