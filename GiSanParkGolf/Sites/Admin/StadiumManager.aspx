<%@ Page Title="경기장 및 코스 관리" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StadiumManager.aspx.cs"
    Inherits="GiSanParkGolf.Sites.Admin.StadiumManager" %>

<%@ Register TagPrefix="uc" TagName="NewSearchControl" Src="~/Controls/NewSearchControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="NewPagingControl" Src="~/Controls/NewPagingControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function promptHoleDelete(holeId) {
            // 숨은 필드에 삭제 대상 HoleId 저장
            document.getElementById("HF_TargetHoleId").value = holeId;
            // 공통 모달 띄우기
            launchModal("홀 삭제", "정말 삭제하시겠습니까?\r\n(복구할 수 없습니다.)", {
                showDeleteAll: false,
                showCancel: true,
                showOk: false,
                showDelete: true,
                showCourseDelete: false,
                showStadiumDelete: false
            });

            return false; // 서버 postback 방지
        }

        function handleModalConfirm() {
            __doPostBack('<%= BTN_ServerHoleDelete.UniqueID %>', '');
        }

        function promptDeleteAllHoles() {
            launchModal("전체 삭제 확인", "정말 이 코스의 모든 홀을 삭제하시겠습니까?\r\n(복구할 수 없습니다.)", {
                showDeleteAll: true,
                showCancel: true,
                showOk: false,
                showDelete: false,
                showCourseDelete: false,
                showStadiumDelete: false
            });
        }

        function handleDeleteAllHoles() {
            __doPostBack('<%= BTN_ServerDeleteAllHoles.UniqueID %>', '');
        }

        function promptCourseDelete(courseCode) {
            console.log("삭제할 코스코드 : " + courseCode);
            document.getElementById("HF_TargetCourseCode").value = courseCode;

            launchModal("코스 삭제", "정말 이 코스를 삭제하시겠습니까?\r\n(복구할 수 없습니다.)", {
                showDeleteAll: false,
                showCancel: true,
                showOk: false,
                showDelete: false,
                showCourseDelete: true,
                showStadiumDelete: false
            });

            return false;
        }

        function handleCourseConfirm() {
            __doPostBack('<%= BTN_ServerCourseDelete.UniqueID %>', '');
        }

        function promptStadiumDelete(stadiumCode) {
            console.log("삭제할 경기장코드 : " + stadiumCode);
            document.getElementById("HF_TargetStadiumCode").value = stadiumCode;

            launchModal("코스 삭제", "정말 이 경기장을 삭제하시겠습니까?\r\n(복구할 수 없습니다.)", {
                showDeleteAll: false,
                showCancel: true,
                showOk: false,
                showDelete: false,
                showCourseDelete: false,
                showStadiumDelete: true
            });

            return false;
        }

        function handleCourseConfirm() {
            __doPostBack('<%= BTN_ServerCourseDelete.UniqueID %>', '');
        }

        function launchModal(title, message, options = {}) {
            document.getElementById("MainModalLabel").innerText = title;
            document.getElementById("MainModalMessage").innerText = message;

            try {
                const showCancel = options.showCancel ?? false;
                const showDelete = options.showDelete ?? false;
                const showOk = options.showOk ?? true;
                const showDeleteAll = options.showDeleteAll ?? false;
                const showCourseDelete = options.showCourseDelete ?? false;
                const showStadiumDelete = options.showStadiumDelete ?? false;

                document.getElementById("BTN_Cancel").style.display = showCancel ? "inline-block" : "none";
                document.getElementById("BTN_DeleteHolesConfirm").style.display = showDelete ? "inline-block" : "none";
                document.getElementById("BTN_OK").style.display = showOk ? "inline-block" : "none";
                document.getElementById("BTN_DeleteAllHolesConfirm").style.display = showDeleteAll ? "inline-block" : "none";
                document.getElementById("BTN_DeleteCourseConfirm").style.display = showCourseDelete ? "inline-block" : "none";
                document.getElementById("BTN_DeleteStadiumConfirm").style.display = showStadiumDelete ? "inline-block" : "none";

                console.log("모달 구성 완료 → 띄우기");

                const modal = new bootstrap.Modal(document.getElementById("MainModal"));
                modal.show();
            } catch (e) {
                console.error("모달 오류", e);
            }
        }

        function showValidate() {
            var modal = new bootstrap.Modal(document.getElementById("validationModal"));
            modal.show();
        }

        function scrollToCourseForm() {
            const target = document.getElementById("Panel_CourseForm");
            if (target) {
                target.scrollIntoView({ behavior: "smooth", block: "start" });
            }
        }

        function scrollToHoleForm() {
            const target = document.getElementById("Panel_HoleForm");
            if (target) {
                target.scrollIntoView({ behavior: "smooth", block: "start" });
            }
        }
    </script>

    <div class="container mt-4">
        <%--홀 삭제할때 쓰이는 버튼--%>
        <asp:LinkButton ID="BTN_ServerHoleDelete" runat="server" OnClick="BTN_ServerHoleDelete_Click" style="display:none" />
        <asp:LinkButton ID="BTN_ServerDeleteAllHoles" runat="server" OnClick="BTN_ServerDeleteAllHoles_Click" style="display:none" />
        <asp:LinkButton ID="BTN_ServerCourseDelete" runat="server" OnClick="BTN_ServerCourseDelete_Click" style="display:none" />
        <asp:LinkButton ID="BTN_ServerStadiumDelete" runat="server" OnClick="BTN_ServerStadiumDelete_Click" style="display:none" />

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
            <uc:NewSearchControl ID="StadiumSearch" runat="server"
                OnSearchRequested="StadiumSearch_SearchRequested"
                OnResetRequested="StadiumSearch_ResetRequested" />

            <asp:HiddenField ID="HF_TargetStadiumCode" runat="server" ClientIDMode="Static" />
            <asp:GridView ID="GV_StadiumList" runat="server"
                AutoGenerateColumns="False"
                CssClass="table table-bordered table-hover table-striped"
                AllowPaging="true"
                PageSize="10"
                DataKeyNames="StadiumCode"
                OnSelectedIndexChanged="GV_StadiumList_SelectedIndexChanged"
                RowStyle-CssClass="clickable-row"
                ShowHeaderWhenEmpty="true">    
                <Columns>
                    <asp:TemplateField HeaderText="No">
                        <ItemTemplate>
                            <%# (GV_StadiumList.PageSize * GV_StadiumList.PageIndex) + Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>

                    <asp:BoundField DataField="StadiumCode" HeaderText="코드" />

                    <%--경기장 이름을 링크 버튼으로 처리--%> 
                    <asp:TemplateField HeaderText="경기장 이름">
                        <ItemTemplate>
                            <asp:LinkButton 
                                runat="server" 
                                CssClass="link-hover" 
                                CommandName="Select" 
                                Text='<%# Eval("StadiumName") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="비고">
                        <ItemTemplate>
                            <div style="max-width:200px; white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
                                <%# Eval("Note") %>
                            </div>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="경기장 삭제">
                        <ItemTemplate>
                            <button type="button"
                                class="btn btn-sm btn-outline-danger"
                                onclick='<%# "return promptStadiumDelete(\"" + Eval("StadiumCode") + "\");" %>'>
                                삭제
                            </button>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>

                <EmptyDataTemplate>
                    <div class="text-center text-muted p-4 fs-5">
                        ⚠️ 현재 등록된 경기장 정보가 없습니다.
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
            <uc:NewPagingControl ID="StadiumPaging" runat="server"
                OnPageChanged="StadiumPaging_PageChanged" />
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
                        <asp:RequiredFieldValidator 
                            ID="RFV_StadiumName" 
                            runat="server"
                            ControlToValidate="TB_StadiumName" 
                            ErrorMessage="경기장 이름을 입력하세요."
                            ValidationGroup="StadiumForm" 
                            Display="None" />
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
        <asp:Panel ID="Panel_CourseForm" runat="server" ClientIDMode="Static" Visible="false" CssClass="fade-in">
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
                            OnClick="BTN_InsertCourse_Click" />
                    </div>
                </div>
            </div>

            <div class="custom-card">
                <h5 class="card-title">등록된 코스 목록</h5>
                <asp:HiddenField ID="HF_TargetCourseCode" runat="server" ClientIDMode="Static" />
                <asp:GridView ID="GV_CourseList" runat="server"
                    OnSelectedIndexChanged="GV_CourseList_SelectedIndexChanged"
                    AutoGenerateColumns="False"
                    CssClass="table table-bordered table-hover table-striped mt-2"
                    DataKeyNames="CourseCode">
    
                    <Columns>
                        <asp:BoundField DataField="CourseCode" HeaderText="코드" />

                        <%--코스 이름을 클릭형 링크로 처리--%>
                        <asp:TemplateField HeaderText="코스 이름">
                            <ItemTemplate>
                                <asp:LinkButton 
                                    runat="server" 
                                    CommandName="Select" 
                                    Text='<%# Eval("CourseName") %>' 
                                    CssClass="link-hover" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="HoleCount" HeaderText="홀 수" />
                        <asp:BoundField DataField="ActiveStatus" HeaderText="사용중 여부" />

                        <asp:TemplateField HeaderText="코스삭제">
                            <ItemTemplate>
                                <button type="button"
                                    class="btn btn-sm btn-outline-danger"
                                    onclick='<%# "return promptCourseDelete(" + Eval("CourseCode") + ");" %>'>
                                    삭제
                                </button>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <!-- 홀 상세 정보 입력 -->
        <asp:Panel ID="Panel_HoleForm" runat="server" ClientIDMode="Static" Visible="false" CssClass="fade-in">
            <div class="custom-card mb-4">
                <h5 class="card-title">홀 정보 입력</h5>
                <p class="text-muted">자동 생성된 홀에 대해 거리와 Par 정보를 입력하세요.</p>
                <asp:Button ID="BTN_AddHoleRow" 
                    runat="server" 
                    Text="홀 추가" 
                    CssClass="btn btn-outline-primary btn-sm" 
                    OnClick="BTN_AddHoleRow_Click" />
                <%--홀 전체삭제는 만들기는 했지만 쓰진 않을것 같다.--%>
                <%--<asp:Button ID="BTN_DeleteAllHoles"
                    runat="server"
                    Text="홀 전체 삭제"
                    CssClass="btn btn-outline-danger btn-sm ms-2"
                    OnClientClick="promptDeleteAllHoles(); return false;" />--%>

                <asp:HiddenField ID="HF_TargetHoleId" runat="server" ClientIDMode="Static" />

                <asp:GridView ID="GV_HoleDetail" runat="server"
                              CssClass="table table-bordered table-hover table-striped"
                              AutoGenerateColumns="False">
                    <Columns>
                        <asp:TemplateField HeaderText="홀 번호">
                            <ItemTemplate>
                                <asp:Label ID="LB_HoleId" runat="server" Text='<%# Eval("HoleId") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="홀명">
                            <ItemTemplate>
                                <asp:TextBox ID="TB_HoleName" runat="server"
                                             Text='<%# Eval("HoleName") %>' CssClass="form-control form-control-sm"/>
                            </ItemTemplate>
                        </asp:TemplateField>
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
                        <asp:TemplateField HeaderText="삭제">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="BTN_HoleDeletePrompt"
                                    CommandArgument='<%# Eval("HoleId") %>'
                                    OnClientClick='<%# "return promptHoleDelete(" + Eval("HoleId") + ");" %>'
                                    Text="🗑️ 삭제"
                                    CssClass="text-danger text-decoration-none" />
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
                    <button type="button" id="BTN_Cancel" class="btn btn-secondary px-4" data-bs-dismiss="modal">
                        취소
                    </button>
                    <button type="button" id="BTN_DeleteHolesConfirm" class="btn btn-danger px-4" onclick="handleModalConfirm(); return false;">
                        삭제
                    </button>
                    <button type="button" id="BTN_DeleteCourseConfirm" class="btn btn-danger px-4" onclick="handleCourseConfirm(); return false;">
                        삭제
                    </button>
                    <button type="button" id="BTN_DeleteStadiumConfirm" class="btn btn-danger px-4" onclick="handleStadiumConfirm(); return false;">
                        삭제
                    </button>
                    <button type="button" id="BTN_OK" class="btn btn-primary px-4" data-bs-dismiss="modal">
                        확인
                    </button>
                    <button type="button" id="BTN_DeleteAllHolesConfirm" class="btn btn-danger px-4" onclick="handleDeleteAllHoles(); return false;">
                        홀 전체 삭제
                    </button>
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
                    <asp:ValidationSummary 
                        ID="ValidationSummaryCourse"
                        runat="server"
                        ShowSummary="true"
                        ShowMessageBox="false"
                        ValidationGroup="CourseForm"
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
