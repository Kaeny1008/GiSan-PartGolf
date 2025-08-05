<%@ Page Title="인원 코스 및 배치" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameSetup.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.GameSetup" %>

<%@ Register Src="~/Controls/NewSearchControl.ascx" TagPrefix="uc" TagName="NewSearchControl" %>
<%@ Register Src="~/Controls/NewPagingControl.ascx" TagPrefix="uc" TagName="NewPagingControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        let modalConfig = {
            name: "",
            title: "",
            body: "",
            yesButtonType: 0,
            launch: false
        };

        function launchModalWithUserId(modalId, title, body, yesButtonType, userId) {
            launchModal(modalId, title, body, yesButtonType);
            // HiddenField에 userId 저장 (ASP.NET ClientID)
            document.getElementById('<%= manualAssignUserId.ClientID %>').value = userId;
        }

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

            var htmlMessage = modalConfig.body.replace(/(\r\n|\n)/g, '<br/>');

            showmodal.find(".modal-title").text(modalConfig.title);
            showmodal.find(".modal-body").html(htmlMessage);

            showmodal.find("button, input[type='submit']").hide().off("click");

            switch (modalConfig.yesButtonType) {
                case 0:
                    showmodal.find("#BTN_Close").show();
                    break;
                case 1:
                    showmodal.find("#BTN_No").show();
                    showmodal.find("#MainContent_BTN_SettingYes").show();
                    break;
                case 2:
                    showmodal.find("#BTN_No").show();
                    showmodal.find("#MainContent_BTN_SaveAssignment_Final").show();
                    break;
                case 3:
                    /*showmodal.find("#MainContent_BTN_Cleanup").show();*/
                    break;
                case 4:
                    showmodal.find("#MainContent_BTN_MovePlayer").show();
                    break;
                default:
                    break;
            }

            const modalElement = document.querySelector(modalConfig.name);
            const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement);
            modalInstance.show();
        }

        function openManualAssignModal(userId) {
            var hiddenField = document.getElementById('<%= manualAssignUserId.ClientID %>');
            hiddenField.value = userId;
            $('#ManualAssignModal').modal('show');
        }

        $(document).ready(function () {
            // 탭 클릭 시 bottomPanel로 스크롤 이동
            $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
                var bottomPanel = document.getElementById('bottomPanel');
                if (bottomPanel) {
                    bottomPanel.scrollIntoView({ behavior: 'smooth', block: 'start' });
                }
            });
        });

        function onCourseChanged() {
            var courseSelect = document.getElementById('<%= manualCourseSelect.ClientID %>');
            var holeSelect = document.getElementById("manualHoleInput");
            var selectedCourse = courseSelect.value;

            holeSelect.innerHTML = '<option value="">홀 선택</option>';

            if (selectedCourse && courseHoleCountMap[selectedCourse]) {
                var holeCount = courseHoleCountMap[selectedCourse];
                for (var i = 1; i <= holeCount; i++) {
                    var opt = document.createElement("option");
                    opt.value = i;
                    opt.text = i;
                    holeSelect.appendChild(opt);
                }
            }
        }

        document.addEventListener("DOMContentLoaded", function () {
            // 홀 선택할 때마다 HiddenField에 값 저장 (중복 없이)
            var holeInput = document.getElementById('manualHoleInput');
            var holeHidden = document.getElementById('<%= manualHoleValue.ClientID %>');
            if (holeInput && holeHidden) {
                holeInput.addEventListener('change', function () {
                    holeHidden.value = this.value;
                });
            }
        });

        $(function () {
            $('#ManualAssignModal').on('show.bs.modal', function () {
                document.getElementById('MainContent_manualCourseSelect').selectedIndex = 0;
                document.getElementById('manualHoleInput').selectedIndex = 0;
            });
        });
    </script>

    <style>
        #MainContent_gvCourseResult {
          border-collapse: collapse;
          width: 100%;
          background: #fff;
          font-family: "Apple SD Gothic Neo", "Malgun Gothic", Arial, sans-serif;
          border: 1px solid #e0e0e0;
        }

        #MainContent_gvCourseResult th,
        #MainContent_gvCourseResult td {
          border: 1px solid #e0e0e0;
          padding: 10px 12px;
          text-align: center;
          font-size: 15px;
        }

        #MainContent_gvCourseResult tr:first-child th {
          background: #f7f7f7;
          color: #222;
          font-weight: 600;
        }

        #MainContent_gvCourseResult tr.team-bg-even td {
          background-color: #e3f0fd; /* 밝은 파스텔 블루 */
        }

        #MainContent_gvCourseResult tr.team-bg-odd td {
          background-color: #e6fff7; /* 밝은 민트 */
        }

        #MainContent_gvCourseResult tr.team-bg-even:hover td,
        #MainContent_gvCourseResult tr.team-bg-odd:hover td {
          background-color: #f1f1f1;
        }
    </style>

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
            <div id="topPanel">
                <div class="custom-card">
                    <h4 class="card-title">대회 선택</h4>
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
                                        Text='<%# Dul.StringLibrary.CutStringUnicode(Eval("GameName").ToString(), 50) %>'>
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
            <div id="bottomPanel">
                <div class="custom-card">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <h4 class="card-title mb-0">대회 상세정보</h4>
                        <asp:Button ID="BTN_BackToList" runat="server" Text="목록으로 이동"
                            CssClass="btn btn-outline-success btn-sm"
                            OnClick="BTN_BackToList_Click" />
                    </div>
                    
                    <!-- 탭 메뉴 -->
                    <ul class="nav nav-tabs mb-3">
                        <li class="nav-item"><a class="nav-link active" data-bs-toggle="tab" href="#tab-info">기본정보</a></li>
                        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-player">참가자확인</a></li>
                        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-course">코스배치</a></li>
                        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-result">코스배치 결과 확인</a></li>
                        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-cancelplayer">취소 참가자 목록</a></li>
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
                                <span class="input-group-text">방식</span>
                                <asp:TextBox ID="tblPlayMode" runat="server" CssClass="form-control" Enabled="false" />
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
                                                <%-- 서버 코드에서 삽입하므로 비워둠 --%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ID">
                                            <ItemTemplate><%# Eval("UserId") %></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="성명">
                                            <ItemTemplate><%# Eval("UserName") %></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="생년월일">
                                            <ItemTemplate><%# Eval("FormattedBirthDate") %></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="성별">
                                            <ItemTemplate><%# Eval("GenderText") %></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="연령">
                                            <ItemTemplate>
                                                <%-- 서버 코드에서 삽입하므로 비워둠 --%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="수상경력">
                                            <ItemTemplate>
                                                <%# Eval("AwardsSummary") ?? "없음" %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="상태">
                                            <ItemTemplate>
                                                <%# (Eval("IsCancelled").ToString() == "1") ? "취소" : "참가" %>
                                            </ItemTemplate>
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

                                <div class="input-group mb-2">
                                    <span class="input-group-text">성별 기준</span>
                                    <asp:DropDownList ID="DDL_GenderUse" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="사용하지 않음" Value="False" />
                                        <asp:ListItem Text="사용함" Value="True" />
                                    </asp:DropDownList>
                                </div>

                                <div class="input-group mb-2">
                                    <span class="input-group-text">연령대 기준</span>
                                    <asp:DropDownList ID="DDL_AgeGroupUse" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="사용하지 않음" Value="False" />
                                        <asp:ListItem Text="사용함" Value="True" />
                                    </asp:DropDownList>
                                </div>

                                <div class="input-group mb-2">
                                    <span class="input-group-text">수상경력 기준</span>
                                    <asp:DropDownList ID="DDL_AwardsUse" runat="server" CssClass="form-select" 
                                        Enabled="false" ToolTip="현재는 데이터가 없으므로 사용하지 않습니다.">
                                        <asp:ListItem Text="사용하지 않음" Value="False" />
                                        <asp:ListItem Text="사용함" Value="True" />
                                    </asp:DropDownList>
                                </div>

                                <small class="text-muted">
                                    위 기준들을 선택하면 참가자 정보를 기반으로 더 정밀한 코스 배치를 할 수 있습니다.<br />
                                    선택한 기준은 자동 배정에 반영되며, 사용 여부에 따라 결과가 달라질 수 있습니다.
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

                                <asp:Button ID="BTN_RetryCourseAssignment" runat="server"
                                    CssClass="btn btn-danger btn-sm"
                                    Text="코스 재배치"
                                    OnClientClick="launchModal('#MainModal', '코스 재배치', '코스배치를 다시 하시겠습니까?', 1); return false;" />

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
                                    CssClass=""
                                    EmptyDataText="배치된 코스가 없습니다. 먼저 코스배치를 실행하세요.">
                                    <Columns>
                                        <asp:BoundField DataField="RowNumber" HeaderText="No" />
                                        <asp:BoundField DataField="UserId" HeaderText="ID" />
                                        <asp:BoundField DataField="UserName" HeaderText="성명" />
                                        <asp:BoundField DataField="GenderText" HeaderText="성별" />
                                        <asp:TemplateField HeaderText="연령">
                                            <ItemTemplate>
                                                <%# Eval("AgeText") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="AgeHandicap" HeaderText="핸디캡" />
                                        <asp:BoundField DataField="HoleNumber" HeaderText="배정홀" />
                                        <asp:BoundField DataField="CourseOrder" HeaderText="코스순번" />
                                        <asp:BoundField DataField="TeamNumber" HeaderText="팀번호" />
                                        <asp:TemplateField HeaderText="참가여부">
                                            <ItemTemplate>
                                                <%# 
                                                    Eval("AssignmentStatus")?.ToString() == "Cancelled" ? "참가취소" :
                                                    Eval("AssignmentStatus")?.ToString() == "Assigned" ? "배정" :
                                                    ""
                                                %>
                                                <asp:Button
                                                    ID="btnApproveCancel"
                                                    runat="server"
                                                    Text="취소 승인"
                                                    CommandName="ApproveCancel"
                                                    CommandArgument='<%# Eval("UserId") %>'
                                                    CssClass="btn btn-warning btn-sm"
                                                    Visible='<%# Eval("AssignmentStatus")?.ToString() == "CancelRequested" %>'
                                                    OnClick="btnApproveCancel_Click"
                                                />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="취소">
                                            <ItemTemplate>
                                                <asp:Button ID="BTN_CancelAssignment" runat="server" Text="배정취소" CommandArgument='<%# Eval("UserId") %>' OnClick="BTN_CancelAssignment_Click" CssClass="btn btn-danger btn-sm" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>

                                <div id="hiddenBox" runat="server">
                                    <div id="lblUnassignedNotice" runat="server" class="alert alert-warning" style="margin-bottom:10px;">
                                        💡 미배정된 인원 목록입니다. 제한으로 인해 배정되지 않은 플레이어들이며, 추가 배치를 위해 확인해주세요.
                                    </div>
                                    <asp:GridView ID="gvUnassignedPlayers" runat="server"
                                        AutoGenerateColumns="False"
                                        CssClass="table table-bordered table-hover table-condensed table-striped table-responsive" 
                                        OnRowDataBound="gvUnassignedPlayers_RowDataBound" 
                                        EmptyDataText="미배정된 인원이 없습니다.">
                                        <Columns>
                                            <asp:BoundField DataField="UserId" HeaderText="ID" />
                                            <asp:BoundField DataField="UserName" HeaderText="성명" />
                                            <asp:BoundField DataField="GenderText" HeaderText="성별" />
                                            <asp:BoundField DataField="AgeText" HeaderText="연령" />
                                            <asp:BoundField DataField="AgeHandicap" HeaderText="핸디캡" />
                                            <asp:TemplateField HeaderText="추천 배정">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRecommendCourse" runat="server" />
                                                    <asp:Button ID="BTN_AssignCourse" 
                                                        runat="server" 
                                                        Text="배정" 
                                                        CssClass="btn btn-sm btn-success ms-2"
                                                        CommandName="AssignCourse"
                                                        CommandArgument='<%# Eval("UserId") %>'
                                                        OnClick="BTN_AssignCourse_Click" />
                                                </ItemTemplate>
                                                <ItemStyle Width="15%" HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="수동배정">
                                                <ItemTemplate>
                                                    <asp:Button ID="BTN_AssignManual" 
                                                        runat="server" 
                                                        Text="배정" 
                                                        CommandName="AssignManual" 
                                                        CommandArgument='<%# Eval("UserId") %>' 
                                                        CssClass="btn btn-primary btn-sm"
                                                        OnClientClick='<%# "openManualAssignModal(\"" + Eval("UserId") + "\"); return false;" %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>

                            </div>
                        </div>

                        <!-- 취소처리된 참가자 탭 -->
                        <div class="tab-pane fade" id="tab-cancelplayer">
                            <asp:GridView
                                ID="gvCancelPlayers"
                                runat="server"
                                CssClass="table table-bordered table-striped table-hover"
                                AutoGenerateColumns="False"
                                EmptyDataText="취소된 참가자가 없습니다." 
                                OnRowDataBound="gvCancelPlayers_RowDataBound"
                                DataKeyNames="UserId">
                                <Columns>
                                    <asp:TemplateField HeaderText="No.">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ID">
                                        <ItemTemplate><%# Eval("UserId") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="성명">
                                        <ItemTemplate><%# Eval("UserName") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="생년월일">
                                        <ItemTemplate><%# Eval("FormattedBirthDate") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="성별">
                                        <ItemTemplate><%# Eval("GenderText") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="연령">
                                        <ItemTemplate>
                                            <ItemTemplate><%# Eval("AgeText") %></ItemTemplate>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="수상경력">
                                        <ItemTemplate>
                                            <%# Eval("AwardsSummary") ?? "없음" %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="재참가">
                                        <ItemTemplate>
                                            <asp:Button
                                                ID="btnRestore"
                                                runat="server"
                                                Text="미배정으로 이동"
                                                CommandName="Restore"
                                                CommandArgument='<%# Eval("UserId") %>'
                                                CssClass="btn btn-success btn-sm" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="manualAssignUserId" runat="server" />
    <asp:HiddenField ID="manualHoleValue" runat="server" />

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
                    <%--<asp:Button ID="BTN_Cleanup" runat="server" 
                        class="btn btn-secondary"
                        Text="확인" 
                        OnClientClick="registerModalCleanup(); return false;" />--%>
                    <asp:Button ID="BTN_MovePlayer" runat="server" 
                        class="btn btn-secondary"
                        Text="확인" 
                        OnClick="btnRestore_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- 수동배정 모달 추가 -->
    <div class="modal fade" id="ManualAssignModal" tabindex="-1" aria-labelledby="ManualAssignModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="ManualAssignModalLabel">수동 배정</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p>선수에게 직접 코스를 배정할 수 있습니다.<br />배정할 코스와 홀을 선택하세요.</p>
                    <div class="mb-2">
                        <label class="form-label">코스 선택</label>
                        <asp:DropDownList 
                            ID="manualCourseSelect" 
                            runat="server" 
                            CssClass="form-select" 
                            OnChange="onCourseChanged()">
                            <asp:ListItem Text="코스 선택" Value="" />
                             <%--서버에서 바인딩--%> 
                        </asp:DropDownList>
                    </div>
                    <div class="mb-2">
                        <label for="manualHoleInput" class="form-label">홀 번호</label>
                        <select id="manualHoleInput" class="form-select">
                            <option value="">홀 선택</option>
                        </select>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">취소</button>
                    <asp:Button ID="BTN_AssignManual" runat="server" Text="배정" OnClick="BTN_AssignManual_Click" CssClass="btn btn-primary" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
