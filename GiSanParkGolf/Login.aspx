<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GiSanParkGolf.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%-- 유효성 실패 시 모달 자동 표시 --%>
    <script type="text/javascript">
        window.addEventListener('DOMContentLoaded', function () {
            document.getElementById('validationModal').addEventListener('shown.bs.modal', function () {
                document.getElementById('closeButton').focus();
            });

            document.getElementById('messageModal').addEventListener('shown.bs.modal', function () {
                document.getElementById('closeButton2').focus();
            });
        });

        function showValidate() {
            var modal = new bootstrap.Modal(document.getElementById("validationModal"));
            modal.show();
        }

        function showMessage(message) {
            document.getElementById("messageModalText").innerText = message;
            var modal = new bootstrap.Modal(document.getElementById("messageModal"));
            modal.show();
        }
    </script>

    <div class="container d-flex justify-content-center align-items-center" style="min-height: 70vh;">
        <div class="w-100" style="max-width: 480px;">
             <%--알림 메시지--%> 
            <div class="alert alert-primary shadow-sm rounded text-center mb-4">
                <div class="d-flex align-items-center justify-content-center">
                    <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" fill="currentColor"
                         class="bi bi-exclamation-triangle-fill text-primary me-2" viewBox="0 0 16 16">
                        <path d="M8.982 1.566a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 
                        1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566zM8 
                        5c.535 0 .954.462.9.995l-.35 3.507a.552.552 0 0 1-1.1 
                        0L7.1 5.995A.905.905 0 0 1 8 5zm.002 6a1 1 0 1 1 0 2 
                        1 1 0 0 1 0-2z" />
                    </svg>
                    <h5 class="mb-0 fw-bold">로그인이 필요합니다</h5>
                </div>
                <p class="mt-2 mb-0">아이디와 비밀번호를 입력하신 후 로그인을 진행해주세요.</p>
            </div>

             <%--아이디--%> 
            <div class="d-flex justify-content-center mb-0">
                <div class="input-group" style="width: 360px;">
                    <span class="input-group-text" id="basic-addon-id" style="width:80px;">ID</span>
                    <asp:TextBox ID="txtUserID" runat="server" CssClass="form-control"
                        placeholder="아이디를 입력하세요"
                        aria-describedby="basic-addon-id" />
                </div>
            </div>

             <%--비밀번호--%> 
            <div class="d-flex justify-content-center mb-0">
                <div class="input-group" style="width: 360px;">
                    <span class="input-group-text" id="basic-addon-pw" style="width:80px;">PW</span>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control"
                        placeholder="비밀번호를 입력하세요" TextMode="Password" 
                        aria-describedby="basic-addon-pw" />
                </div>
            </div>

            <%-- 여백 추가 --%>
            <div style="height: 32px;"></div>

            <%-- 버튼 영역 (가로 배치, 폭 고정) --%>
            <div class="d-flex justify-content-center mb-3" style="gap:10px;">
                <asp:Button ID="btnLogin" runat="server" Text="로그인"
                    CssClass="btn btn-primary"
                    OnClick="btnLogin_Click"
                    style="width:160px;" />

                <asp:Button ID="BtnRegister" runat="server" Text="회원가입"
                    CssClass="btn btn-outline-secondary"
                    OnClick="BtnRegister_Click"
                    style="width:160px;" />
            </div>

             <%--유효성 검사--%> 
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                ControlToValidate="txtUserID" ErrorMessage="ID를 입력하여 주십시오."
                Display="None" ValidationGroup="UserLogin" />

            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                ControlToValidate="txtPassword" ErrorMessage="비밀번호를 입력하여 주십시오."
                Display="None" ValidationGroup="UserLogin" />

            <%-- 유효성 검사 모달 --%>
            <div class="modal fade" id="validationModal" tabindex="-1" aria-labelledby="validationModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content border border-danger shadow-lg">
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
                        <div class="modal-body bg-light px-4 py-3">
                            <asp:ValidationSummary ID="ValidationSummary2"
                                runat="server" ShowMessageBox="false" ShowSummary="true"
                                HeaderText="" ValidationGroup="UserLogin" CssClass="text-danger" />
                        </div>
                        <div class="modal-footer bg-light">
                            <button type="button" id="closeButton" class="btn btn-outline-danger px-4" data-bs-dismiss="modal">닫기</button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal fade" id="messageModal" tabindex="-1" aria-labelledby="messageModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content border border-warning shadow-lg">
                        <div class="modal-header bg-warning bg-gradient text-white">
                            <div class="d-flex align-items-center">
                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor"
                                     class="bi bi-exclamation-diamond-fill me-2" viewBox="0 0 16 16">
                                    <path d="M4.94.69a1.5 1.5 0 0 1 2.12 0L15.3 8.94a1.5 1.5 0 0 1 0 2.12l-8.24 8.24a1.5 1.5 0 0 1-2.12 0L.7 11.06a1.5 1.5 0 0 1 0-2.12L8.94.69zM8 5a.5.5 0 0 0-.5.5v3.5a.5.5 
                                    0 0 0 1 0V5.5A.5.5 0 0 0 8 5zm0 6a.75.75 0 1 0 0 1.5A.75.75 0 0 0 8 11z"/>
                                </svg>
                                <h5 class="modal-title fw-bold mb-0" id="messageModalLabel">알림</h5>
                            </div>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="닫기"></button>
                        </div>
                        <div class="modal-body bg-light px-4 py-3">
                            <p id="messageModalText" class="text-dark fw-bold text-center mb-0"></p>
                        </div>
                        <div class="modal-footer bg-light">
                            <button type="button" id="closeButton2" class="btn btn-warning px-4" data-bs-dismiss="modal">확인</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
