<%@ Page Title="라이선스 안내" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="license.aspx.cs" Inherits="GiSanParkGolf.license" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-5">
        <div class="card shadow-sm mx-auto" style="max-width:700px;">
            <div class="card-body">
                <h1 class="card-title mb-4 text-center">
                    <i class="bi bi-shield-check text-primary"></i>
                    라이선스 및 저작권 안내
                </h1>
                <hr />
                <section class="mb-4">
                    <h4>오픈소스 라이브러리</h4>
                    <ul class="list-group list-group-flush">
                        <li class="list-group-item">
                            <strong>Bootstrap 5</strong><br />
                            <span class="text-muted">MIT License</span><br />
                            <a href="https://getbootstrap.com/" target="_blank">공식 사이트</a> /
                            <a href="/license/bootstrap.txt" target="_blank">라이선스 전문</a>
                        </li>
                        <li class="list-group-item">
                            <strong>Dapper</strong><br />
                            <span class="text-muted">Apache License 2.0</span><br />
                            <a href="https://github.com/DapperLib/Dapper" target="_blank">공식 사이트</a> /
                            <a href="/license/dapper.txt" target="_blank">라이선스 전문</a>
                        </li>
                    </ul>
                </section>
                <section class="mb-4">
                    <h4>GitHub Copilot 안내</h4>
                    <p>
                        본 사이트의 일부 코드는 <strong>GitHub Copilot</strong>의 도움으로 작성되었습니다.<br />
                        Copilot이 생성한 코드는 참고용이며, 사용자는 반드시 검토 후 적용해야 합니다.<br />
                        Copilot의 사용은 
                        <a href="https://github.com/features/copilot" target="_blank">GitHub Copilot 서비스 안내</a> 및
                        <a href="https://docs.github.com/en/site-policy/github-terms/github-terms-of-service" target="_blank">GitHub 서비스 약관</a>,
                        <a href="https://www.microsoft.com/en-us/legal/licenses" target="_blank">Microsoft 사용권 계약</a>을 따릅니다.<br />
                        사용자는 제3자 저작권, 오픈소스 라이선스, 보안 정책 등을 준수해야 합니다.
                    </p>
                </section>
                <section>
                    <h4>저작권 정보</h4>
                    <p>
                        &copy; 2025 기산파크골프 시스템.<br />
                        모든 권리 보유.
                    </p>
                </section>
            </div>
        </div>
        <div class="text-center mt-3 text-muted small">
            본 라이선스 안내는 투명한 서비스 운영을 위한 공개 고지입니다.
        </div>
    </div>
    <!-- Bootstrap Icons CDN (헤더에 이미 포함되어 있지 않다면 추가) -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css" />
</asp:Content>