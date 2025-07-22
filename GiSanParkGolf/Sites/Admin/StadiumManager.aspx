<%@ Page Title="경기장 및 코스 관리" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StadiumManager.aspx.cs"
    Inherits="GiSanParkGolf.Sites.Admin.StadiumManager" %>

<%@ Register TagPrefix="uc" TagName="NewSearchControl" Src="~/Controls/NewSearchControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="NewPagingControl" Src="~/Controls/NewPagingControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <%--CSS 애니메이션--%> 
    <style>
        .fade-in {
            animation: fadeIn 0.5s ease-in-out;
        }
        @keyframes fadeIn {
            from { opacity: 0; }
            to   { opacity: 1; }
        }
    </style>
    <script type="text/javascript">
        function launchModal(title, message, showNo = false) {
            document.getElementById("MainModalLabel").innerText = title;
            document.getElementById("MainModalMessage").innerText = message;

            // 버튼 표시 설정
            document.getElementById("BTN_No").style.display = showNo ? "inline-block" : "none";
            document.getElementById("BTN_ClientYes").style.display = "inline-block";

            const modal = new bootstrap.Modal(document.getElementById("MainModal"));
            modal.show();
        }

        function handleModalConfirm() {
            // 필요한 작업 정의 (예: 팝업 호출, UI 전환 등)
            console.log("예 버튼 클릭됨");

            // 필요 시 모달 닫기
            bootstrap.Modal.getInstance(document.getElementById("MainModal")).hide();
        }
        function showValidate() {
            var modal = new bootstrap.Modal(document.getElementById("validationModal"));
            modal.show();
        }
    </script>

    <div class="container mt-4">

        <%-- ▶ 단계 안내 뱃지 --%>
        <div class="step-indicator text-center mb-3">
            <asp:Label ID="LB_StepGuide1" runat="server" CssClass="badge bg-primary me-2" Text="① 경기장 목록" />
            <asp:Label ID="LB_StepGuide2" runat="server" CssClass="badge bg-secondary me-2" Text="② 경기장 등록" />
            <asp:Label ID="LB_StepGuide3" runat="server" CssClass="badge bg-secondary me-2" Text="③ 코스 등록" />
            <asp:Label ID="LB_StepGuide4" runat="server" CssClass="badge bg-secondary" Text="④ 홀 정보 입력" />
        </div>

        <!-- 경기장 목록 -->
        <div class="custom-card mb-4">
            <div class="d-flex justify-content-between align-items-center">
                <h5 class="card-title">경기장 목록</h5>
                <asp:Button ID="BTN_ShowStadiumForm" runat="server" Text="신규 등록"
                            CssClass="btn btn-outline-primary btn-sm"
                            OnClick="BTN_ShowStadiumForm_Click" />
            </div>
            <uc:NewSearchControl ID="SearchControl_Stadium" runat="server"
                OnSearchRequested="SearchControl_Stadium_SearchRequested"
                OnResetRequested="SearchControl_Stadium_ResetRequested" />
            <asp:GridView ID="GV_StadiumList" runat="server"
                          AutoGenerateColumns="False"
                          CssClass="table table-bordered table-hover table-striped"
                          AllowPaging="true"
                          PageSize="10"
                          DataKeyNames="StadiumCode"
                          OnSelectedIndexChanged ="GV_StadiumList_SelectedIndexChanged"
                          OnPageIndexChanging="GV_StadiumList_PageIndexChanging"
                          ShowHeaderWhenEmpty="true">
                <Columns>
                    <asp:BoundField DataField="StadiumCode" HeaderText="코드" />
                    <asp:BoundField DataField="StadiumName" HeaderText="경기장 이름" />
                    <asp:CommandField ShowSelectButton="True" SelectText="선택" ButtonType="Button" />
                </Columns>

                <EmptyDataTemplate>
                    <div class="text-center text-muted p-4 fs-5">
                        ⚠️ 현재 등록된 경기장 정보가 없습니다.
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
            <uc:NewPagingControl ID="Paging_Stadium" runat="server"
                OnPageChanged="Paging_Stadium_PageChanged" />
        </div>

        <!-- 경기장 등록 폼 -->
        <asp:Panel ID="Panel_StadiumForm" runat="server" Visible="false" CssClass="fade-in">
            <div class="custom-card mb-4">
                <h5 class="card-title">신규 경기장 등록</h5>
                <div class="border rounded p-3 bg-light">
                    <div class="input-group mb-2">
                        <span class="input-group-text">경기장 코드</span>
                        <asp:TextBox ID="TB_StadiumCode" runat="server" CssClass="form-control" />
                    </div>
                    <div class="input-group mb-2">
                        <span class="input-group-text">경기장 이름</span>
                        <asp:TextBox ID="TB_StadiumName" runat="server" CssClass="form-control" />
                    </div>
                    <div class="input-group mb-3">
                        <span class="input-group-text">사용 여부</span>
                        <asp:DropDownList ID="DDL_StadiumActive" runat="server" CssClass="form-select">
                            <asp:ListItem Text="사용함" Value="True" />
                            <asp:ListItem Text="사용 안 함" Value="False" />
                        </asp:DropDownList>
                    </div>
                    <div class="input-group mb-3">
                        <span class="input-group-text">비고</span>
                        <asp:TextBox ID="TB_StadiumNote" runat="server" CssClass="form-control"
                                     TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="text-end mt-2">
                        <asp:Button 
                            ID="BTN_InsertStadium" 
                            runat="server" 
                            Text="등록"
                            OnClick="BTN_InsertStadium_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <%-- 코스 등록 폼 --%>
        <asp:Panel ID="Panel_CourseForm" runat="server" Visible="false" CssClass="fade-in">
            <div class="custom-card mb-2">
                <h5 class="card-title">코스 등록</h5>
                <div class="border rounded p-3 bg-light">
            
                    <%-- 코스명 입력 --%>
                    <div class="input-group mb-2">
                        <span class="input-group-text">코스명</span>
                        <asp:TextBox ID="TB_CourseName" runat="server" CssClass="form-control" placeholder="예: A코스" />
                    </div>
                    <asp:RequiredFieldValidator 
                        ID="RFV_CourseName"
                        runat="server"
                        ControlToValidate="TB_CourseName"
                        ErrorMessage="코스명을 입력하세요."
                        ValidationGroup="CourseForm"
                        Display="None" />

                    <%-- 최대 홀 수 입력 --%>
                    <div class="input-group mb-2">
                        <span class="input-group-text">최대 홀 수</span>
                        <asp:TextBox ID="TB_MaxHoleCount" runat="server" CssClass="form-control" TextMode="Number" />
                    </div>
                    <asp:RequiredFieldValidator 
                        ID="RFV_MaxHoleCount"
                        runat="server"
                        ControlToValidate="TB_MaxHoleCount"
                        ErrorMessage="최대 홀 수를 입력하세요."
                        ValidationGroup="CourseForm"
                        Display="None" />

                    <%-- 사용 여부 선택 --%>
                    <div class="input-group mb-3">
                        <span class="input-group-text">사용 여부</span>
                        <asp:DropDownList ID="DDL_CourseActive" runat="server" CssClass="form-select">
                            <asp:ListItem Text="사용함" Value="True" />
                            <asp:ListItem Text="사용 안 함" Value="False" />
                        </asp:DropDownList>
                    </div>

                    <%-- 등록 버튼 --%>
                    <div class="text-end mt-2">
                        <asp:Button ID="BTN_InsertCourse" runat="server" 
                            Text="코스 등록" CssClass="btn btn-success btn-sm"
                            OnClick="BTN_InsertCourse_Click"
                            ValidationGroup="CourseForm" />
                    </div>
                </div>
            </div>

            <div class="custom-card">
                <h5 class="card-title">등록된 코스 목록</h5>
                <asp:GridView ID="GV_CourseList" runat="server"
                              OnSelectedIndexChanged="GV_CourseList_SelectedIndexChanged"
                              AutoGenerateColumns="False"
                              CssClass="table table-bordered table-hover table-striped mt-2"
                              DataKeyNames="CourseCode">
                    <Columns>
                        <asp:BoundField DataField="CourseCode" HeaderText="코드" />
                        <asp:BoundField DataField="CourseName" HeaderText="코스 이름" />
                        <asp:BoundField DataField="HoleCount" HeaderText="홀 수" />
                        <asp:BoundField DataField="UseStatus" HeaderText="사용중 여부" />
                        <asp:CommandField ShowSelectButton="True" SelectText="선택" ButtonType="Button" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <!-- 홀 상세 정보 입력 -->
        <asp:Panel ID="Panel_HoleForm" runat="server" Visible="false" CssClass="fade-in">
            <div class="custom-card mb-4">
                <h5 class="card-title">홀 정보 입력</h5>
                <p class="text-muted">자동 생성된 홀에 대해 거리와 Par 정보를 입력하세요.</p>
                <asp:GridView ID="GV_HoleDetail" runat="server"
                              CssClass="table table-bordered table-hover table-striped"
                              AutoGenerateColumns="False">
                    <Columns>
                        <asp:TemplateField HeaderText="홀 번호">
                            <ItemTemplate>
                                <asp:Label ID="LB_HoleId" runat="server" Text='<%# Eval("HoleId") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="HoleName" HeaderText="홀명" />
                        <asp:TemplateField HeaderText="거리(m)">
                            <ItemTemplate>
                                <asp:TextBox ID="TB_Distance" runat="server"
                                             Text='<%# Eval("Distance") %>' CssClass="form-control form-control-sm"
                                             TextMode="Number" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Par">
                            <ItemTemplate>
                                <asp:TextBox ID="TB_Par" runat="server"
                                             Text='<%# Eval("Par") %>' CssClass="form-control form-control-sm"
                                             TextMode="Number" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <div class="text-end mt-2">
                    <asp:Button ID="BTN_SaveHoleDetail" runat="server" 
                        Text="홀 정보 저장"
                        CssClass="btn btn-warning btn-sm"
                        OnClick="BTN_SaveHoleDetail_Click" />
                    <asp:Button ID="BTN_UpdateHoleDetail" runat="server" 
                                Text="홀 정보 수정"
                                CssClass="btn btn-success btn-sm"
                                OnClick="BTN_UpdateHoleDetail_Click" />
                </div>
            </div>
        </asp:Panel>
    </div>

    <asp:RequiredFieldValidator 
        ID="RFV_StadiumName" 
        runat="server"
        ControlToValidate="TB_StadiumName" 
        ErrorMessage="경기장 이름을 입력하세요."
        ValidationGroup="StadiumForm" 
        Display="None" />
    <asp:ValidationSummary 
        ID="ValidationSummaryCourse"
        runat="server"
        ShowSummary="true"
        ShowMessageBox="false"
        ValidationGroup="CourseForm" />

    <%-- 공통 알림 모달 --%>
    <div class="modal fade" id="MainModal" tabindex="-1" aria-labelledby="MainModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
            
                <%-- 제목 영역 --%>
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="MainModalLabel">알림</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="닫기"></button>
                </div>

                <%-- 메시지 영역 --%>
                <div class="modal-body">
                    <p id="MainModalMessage" class="mb-0">메시지를 여기에 표시합니다.</p>
                </div>

                <%-- 버튼 영역 --%>
                <div class="modal-footer">
                    <button type="button" id="BTN_No" class="btn btn-secondary px-4" data-bs-dismiss="modal">아니오</button>
                    <button type="button" id="BTN_ClientYes" class="btn btn-primary px-4" onclick="handleModalConfirm()">예</button>
                </div>
            </div>
        </div>
    </div>

    <%-- 유효성 검사 모달 --%>
    <div class="modal fade" id="validationModal" tabindex="-1" aria-labelledby="validationModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border border-danger shadow-lg">
            
                <%-- 제목 영역 --%>
                <div class="modal-header bg-danger bg-gradient text-white">
                    <div class="d-flex align-items-center">
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor"
                             class="bi bi-exclamation-circle-fill me-2" viewBox="0 0 16 16">
                            <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM8 5a.5.5 0 0 0-.5.5v3a.5.5 
                            0 0 0 1 0v-3A.5.5 0 0 0 8 5zm.002 6a.5.5 0 1 0 0 1 
                            .5.5 0 0 0 0-1z"/>
                        </svg>
                        <h5 class="modal-title fw-bold mb-0" id="validationModalLabel">입력 오류 발생</h5>
                    </div>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="닫기"></button>
                </div>

                <%-- 오류 메시지 영역 --%>
                <div class="modal-body bg-light px-4 py-3">
                    <asp:ValidationSummary ID="ValidationSummary2"
                        runat="server"
                        ShowMessageBox="false"
                        ShowSummary="true"
                        HeaderText=""
                        ValidationGroup="StadiumForm"
                        CssClass="text-danger" />
                </div>

                <%-- 버튼 영역 --%>
                <div class="modal-footer bg-light">
                    <button type="button" id="closeButton" class="btn btn-outline-danger px-4" data-bs-dismiss="modal">닫기</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
