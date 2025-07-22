<%@ Page Title="인원 코스 및 배치" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameSetup.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameSetup" %>

<%@ Register Src="~/Controls/NewSearchControl.ascx" TagPrefix="uc" TagName="NewSearchControl" %>
<%@ Register Src="~/Controls/NewPagingControl.ascx" TagPrefix="uc" TagName="NewPagingControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript"> 
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
            modal.find('.modal-title').text(showTitle);
            modal.find('.modal-body').text(showBody);

            modal.find('#MainContent_BTN_PlayerCheckYes, #MainContent_BTN_SettingYes').hide();
            modal.find('#BTN_No').text(onlyYes ? "확인" : "아니오");

            const yesButtons = [
                "#MainContent_BTN_PlayerCheckYes",
                "#MainContent_BTN_SettingYes"
            ];
            if (!onlyYes && showYesButton >= 0 && showYesButton < yesButtons.length) {
                modal.find(yesButtons[showYesButton]).show();
            }

            modal.modal("show");
        }
        function winPopUPCenter(url, winName, width = 800, height = 600, scroll = "no", resize = "no") {
            const left = (screen.width - width) / 2;
            const top = (screen.height - height) / 2;
            const spec = `toolbar=no,status=no,location=yes,width=${width},height=${height},top=${top},left=${left},scrollbars=${scroll},resizable=${resize}`;

            const gameCode = document.getElementById('MainContent_TB_GameCode').value;
            const finalUrl = `${url}?GameCode=${gameCode}`;

            const win = window.open(finalUrl, winName, spec);
            if (win) win.focus();

            $(showModalName).modal("hide"); // 모달 닫기
        }
    </script>

    <style type="text/css" media="print">
        body * {
            visibility: hidden;
        }

        #printArea, #printArea * {
            visibility: visible;
        }

        #printArea {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
        }
    </style>

    <div class="container body-content">
        <div class="row">
            <!-- 좌측: 대회 목록 -->
            <div class="col-md-5">
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
                        OnRowDataBound="GameList_RowDataBound">
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
                                        ToolTip='<%# Eval("GameCode") %>'
                                        CommandName="select"
                                        OnClick="LnkGame_Click">
                                        <%# Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 25) %>
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
                    <div class="d-flex justify-content-between align-items-center mt-2">
                        <small class="text-muted">총 건수: <asp:Literal ID="lblTotalRecord" runat="server" /></small>
                        <uc:NewPagingControl ID="pager" runat="server"
                            OnPageChanged="Pager_PageChanged" />
                    </div>
                </div>
            </div>

            <!-- 우측: 상세정보 탭 카드 -->
            <div class="col-md-7">
                <div class="custom-card">
                    <h4 class="card-title mb-3">대회 상세정보</h4>

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
                                    ShowHeaderWhenEmpty="true" OnRowDataBound="GameList_RowDataBound">
                                    <HeaderStyle HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="1px" />
                                    <RowStyle HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="1px" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="No.">
                                            <ItemTemplate><%# Eval("RowNumber") %></ItemTemplate>
                                            <ItemStyle Width="10%" />
                                            <HeaderStyle Width="10%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ID">
                                            <ItemTemplate><%# Eval("UserId") %></ItemTemplate>
                                            <ItemStyle Width="30%" />
                                            <HeaderStyle Width="30%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="성명">
                                            <ItemTemplate><%# Eval("UserName") %></ItemTemplate>
                                            <ItemStyle Width="30%" />
                                            <HeaderStyle Width="30%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="생년월일">
                                            <ItemTemplate><%# Eval("UserNumber") %></ItemTemplate>
                                            <ItemStyle Width="30%" />
                                            <HeaderStyle Width="30%" />
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
                                    OnClientClick="launchModal('#MainModal', '코스배치', '선수 코스배치를 하시겠습니까?', false, 2); return false;" />
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
                            <div class="d-flex justify-content-between align-items-center mb-3">
                                <h5 class="mb-0 text-muted">코스배치 결과 확인</h5>
                                <asp:Button ID="BTN_RefreshResult" runat="server"
                                    CssClass="btn btn-outline-info btn-sm"
                                    Text="새로고침"
                                    OnClick="BTN_RefreshResult_Click" />
                            </div>

                            <asp:GridView ID="gvCourseResult" runat="server"
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-condensed table-striped table-responsive"
                                EmptyDataText="배치된 코스가 없습니다. 먼저 코스배치를 실행하세요.">
                                <Columns>
                                    <asp:BoundField DataField="UserName" HeaderText="성명" />
                                    <asp:BoundField DataField="UserHandicap" HeaderText="핸디캡" />
                                    <asp:BoundField DataField="HoleNumber" HeaderText="배정홀" />
                                    <asp:BoundField DataField="TeamNumber" HeaderText="팀번호" />
                                </Columns>
                            </asp:GridView>

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
                    <asp:Button ID="BTN_PlayerCheckYes" runat="server" Text="예" CssClass="btn btn-primary"
                        OnClientClick="winPopUPCenter('GamePlayerList.aspx', 'User List'); return false;" />

                    <asp:Button ID="BTN_SettingYes" runat="server" Text="예" CssClass="btn btn-primary"
                        OnClick="BTN_SettingYes_Click" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>
