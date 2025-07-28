<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BoardEditorFormControl.ascx.cs" Inherits="GiSanParkGolf.BBS.Controls.BoardEditorFormControl" %>
<style>
    .BoardWriteFormTableLeftStyle {
        width: 100px; 
        text-align:center;
    }
    .auto-style1 {
        width: 100px;
        text-align: right;
        height: 18px;
        text-align:center;
    }
    .auto-style2 {
        width: 500px;
        height: 18px;
    }
    .MainLabel {
        text-align: center;
        font-size: 30px;
    }
    .form-control { 
        min-width: 100%;
    }
</style>
<%--<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        //쓸일이 있겠지 놔둬보자.
    }
</script>--%>
<script languade="javascript">
    function ShowModal() {
        $("#SaveModal").modal("show");
    }
    window.onload = function () {
        var fileInput = document.getElementById('<%= txtFileName.ClientID %>');

            fileInput.addEventListener('change', function () {
                var file = this.files[0];
                if (file && file.size > 4 * 1024 * 1024) { // 4MB 이상
                    alert('파일 크기가 4MB 이상은 업로드 할 수 없습니다.');
                    this.value = ''; // 파일 입력 초기화
                }
            });
        };
</script>



<div style="text-align: center;">
    <asp:Label ID="LBMainTitle" runat="server" Class="MainLabel" Text="게시판" />
</div>
<asp:Label ID="lblTitleDescription" runat="server" ForeColor="#ff0000" />
<hr />
<table style="width: 70%; border-collapse: collapse; padding: 5px; margin-left:auto; margin-right:auto;">
    <%--게시글이 수정 상태일때 표시되는 부분--%>
    <% 
        if (!String.IsNullOrEmpty(Request.QueryString["Id"]) && 
            FormType == BBS.Models.BoardWriteFormType.Modify) 
        { 
    %>    
        <tr>
            <td class="auto-style1">
                <span style="color: #ff0000;">*</span>번호
            </td>
            <td class="auto-style2">
                <%= Request.QueryString["Id"] %>
            </td>
        </tr>
    <% 
        } 
    %>
    <%--게시글이 수정 상태일때 표시되는 부분--%>

    <% 
        //비로그인 중일때는 이름, 이메일 입력
        //로그인시 자동입력되도록 함.
        if (!Page.User.Identity.IsAuthenticated)
        {
    %>
            <tr>
                <td class="BoardWriteFormTableLeftStyle">
                    <span style="color: #ff0000;">*</span>이름</td>
                <td>
                    <asp:TextBox ID="txtName" runat="server" MaxLength="10" 
                        Width="80%" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valName" runat="server" 
                        ErrorMessage="* 이름을 작성해 주세요." 
                        ControlToValidate="txtName" Display="None" 
                        SetFocusOnError="True"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="text-align:center;">E-mail</td>
                <td>
                    <asp:TextBox ID="txtEmail" runat="server" 
                        MaxLength="80" Width="80%" CssClass="form-control" 
                        style="display:inline-block;"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="valEmail" runat="server" 
                        ErrorMessage="* 메일 형식이 올바르지 않습니다." 
                        ControlToValidate="txtEmail" Display="None" 
                        ValidationExpression=
                            "\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                        SetFocusOnError="True"></asp:RegularExpressionValidator>
                </td>
            </tr>
    <%
        }
    %>

    <tr>
        <td style="text-align:center;">
            <span style="color: #ff0000;">*</span>제 목
        </td>
        <td>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" /><br />
            <asp:RequiredFieldValidator ID="valTitle" runat="server" 
                ErrorMessage="* 제목을 기입해 주세요." 
                ControlToValidate="txtTitle" Display="None" 
                SetFocusOnError="True"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td style="text-align:center;">
            <span style="color: #ff0000;">*</span>내 용
        </td>
        <td>
            <asp:TextBox ID="txtContent" TextMode="MultiLine" runat="server" 
                Height="300px" Width="100%" CssClass="form-control" 
                style="display:inline-block;"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="valContent" runat="server" 
                ErrorMessage="* 내용을 기입해 주세요." 
                ControlToValidate="txtContent" Display="None" 
                SetFocusOnError="True"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td style="text-align:center;">파일첨부(최대 4Mb)</td>
        <td>
<%--            <asp:CheckBox ID="chkUpload" runat="server" CssClass="check-inline" 
                Text="이 체크박스를 선택하면 업로드 화면이 나타납니다." 
                AutoPostBack="True" 
                OnCheckedChanged="chkUpload_CheckedChanged"></asp:CheckBox>--%>
            <br />
            <asp:Panel ID="pnlFile" runat="server" Width="100%" 
                Visible="true">
                <input id="txtFileName" style="width: 100%; 
                    height: 100%" type="file" name="txtFileName" runat="server"><br />
                    <% 
                        if (lblFileNamePrevious.Visible)
                        {
                    %>
                        <asp:label ID="lblFileNamePrevious" text="" runat="server" Visible="false" />
                    <%
                        }
                    %>
            </asp:Panel>
        </td>
    </tr>
    <tr style="visibility:hidden">
        <td style="text-align:center;">
            <span style="color: #ff0000;">*</span>인코딩
        </td>
        <td>
            <asp:RadioButtonList ID="rdoEncoding" runat="server" 
                RepeatDirection="Horizontal" RepeatLayout="Flow">
                <asp:ListItem Value="Text" Selected="True">Text</asp:ListItem>
                <asp:ListItem Value="HTML">HTML</asp:ListItem>
                <asp:ListItem Value="Mixed">Mixed</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>

    <% 
        //비로그인 중일때는 보안코드, 비밀번호를 입력
        //로그인시 자동입력되도록 함.
        if (!Page.User.Identity.IsAuthenticated)
        {
    %>
        <tr>
            <td style="text-align:center;">
                <span style="color: #ff0000;">*</span>비밀번호
            </td>
            <td>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" 
                    style="display:inline-block;" MaxLength="20" Width="150px" 
                    TextMode="Password" EnableViewState="False"></asp:TextBox>
                <span style=" color: #aaaaaa;">(수정/삭제 시 필요)</span> 
                <br />
                <asp:RequiredFieldValidator ID="valPassword" runat="server" 
                    ErrorMessage="* 비밀번호를 기입해 주세요." 
                    ControlToValidate="txtPassword" Display="None" 
                    SetFocusOnError="True"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td style="text-align:center;">
                <span style="color: #ff0000;">*</span>보안코드
            </td>
            <td>
                <asp:TextBox ID="txtImageText" runat="server" 
                    CssClass="form-control" 
                    style="display:inline-block;" 
                    EnableViewState="False" MaxLength="20" 
                    Width="150px"></asp:TextBox>
                <span style=" color: #aaaaaa;">
                    (아래에 제시되는 보안코드를 입력하십시오.)</span>
                <br />
                <asp:Image ID="imgSecurityImageText" runat="server" 
                    ImageUrl="~/BBS/ImageText.aspx" />
                <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
            </td>
        </tr>
    <%
        }
    %>
</table>

<div>
    <div colspan="2" style="text-align:center;">
        <asp:Button ID="btnWrite" runat="server" Text="저장" OnClientClick="ShowModal();return false;" 
            CssClass="btn btn-primary" OnClick="btnWrite_Click"></asp:Button> 
        <%--<a href="BoardList.aspx" class="btn btn-default">리스트</a>--%>
        <br />
        <asp:ValidationSummary ID="valSummary" runat="server" 
            ShowSummary="False" 
            ShowMessageBox="True" 
            DisplayMode="List"></asp:ValidationSummary>
        <br />
    </div>
</div>

<!-- Modal -->
<div class="modal fade" id="SaveModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered"> <%--modal-dialog-centered 를 옆에 넣으면 화면 중앙에 나타난다.--%>
        <div class="modal-content">
            <div class="modal-header">
                <h1 class="modal-title fs-5" id="exampleModalLabel">확인</h1>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                저장 하시겠습니까?
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">아니오</button>
                <asp:Button ID="Button2" 
                    runat="server" 
                    OnClick="btnWrite_Click"
                    class="btn btn-primary" 
                    Text="예" />
            </div>
        </div>
    </div>
</div>