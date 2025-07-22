<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Admin Alert.aspx.cs" Inherits="GiSanParkGolf.Sites.Login.Admin_Alert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-5 d-flex justify-content-center align-items-center" style="min-height: 80vh;">
        <div class="text-center">
            <div class="alert alert-danger shadow-lg rounded px-4 py-4 d-inline-block" role="alert">
                <div class="d-flex align-items-center mb-3 justify-content-center">
                    <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" fill="currentColor"
                         class="bi bi-exclamation-triangle-fill text-danger me-3" viewBox="0 0 16 16">
                        <path d="M8.982 1.566a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 
                        1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566zM8 5c.535 0 .954.462.9.995l-.35 
                        3.507a.552.552 0 0 1-1.1 0L7.1 5.995A.905.905 0 0 1 8 
                        5zm.002 6a1 1 0 1 1 0 2 1 1 0 0 1 0-2z" />
                    </svg>
                    <h3 class="mb-0 fw-bold">관리자 전용 페이지</h3>
                </div>
                <p class="mb-0">해당 페이지는 관리자만 접근 가능합니다.<br />
                로그인 후 다시 시도해주세요.</p>
                <p class="mt-2 text-muted" style="font-size: 0.95rem;">
                    아래 버튼을 클릭하면 <strong>자동으로 로그아웃 처리</strong>된 후 로그인 페이지로 이동합니다.
                </p>
                <a href="/Sites/Login/Logout.aspx" class="btn btn-outline-danger btn-sm mt-2">
                    🔐 로그인 페이지로 이동
                </a>
            </div>
        </div>
    </div>
</asp:Content>