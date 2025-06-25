<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BoardEditorFormControl.ascx.cs" Inherits="GiSanParkGolf.DotNetNote.Controls.BoardEditorFormControl" %>
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
</style>
<h2 style="text-align:center;">게시판</h2>
<asp:Label ID="lblTitleDescription" runat="server" ForeColor="#ff0000">
</asp:Label>
<hr />
<table style="width:100%; border-collapse: collapse; padding: 5px; margin-left:auto; margin-right:auto;">
    <%--게시글이 수정 상태일때 표시되는 부분--%>
    <% if (!String.IsNullOrEmpty(Request.QueryString["Id"]) && 
            FormType == DotNetNote.Models.BoardWriteFormType.Modify) { %>    
    <tr>
        <td class="auto-style1">
            <span style="color: #ff0000;">*</span>번호
        </td>
        <td class="auto-style2">
            <%= Request.QueryString["Id"] %>
        </td>
    </tr>
    <% } %>
    <%--게시글이 수정 상태일때 표시되는 부분--%>

    <tr>
        <td class="BoardWriteFormTableLeftStyle">
            <span style="color: #ff0000;">*</span>이름</td>
        <td>
            <asp:TextBox ID="txtName" runat="server" MaxLength="10" 
                Width="480px" CssClass="form-control"></asp:TextBox>
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
                MaxLength="80" Width="480px" CssClass="form-control" 
                style="display:inline-block;"></asp:TextBox>
            <span style="color:#aaaaaa;font-style:italic">(Optional)</span>
            <asp:RegularExpressionValidator ID="valEmail" runat="server" 
                ErrorMessage="* 메일 형식이 올바르지 않습니다." 
                ControlToValidate="txtEmail" Display="None" 
                ValidationExpression=
                    "\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                SetFocusOnError="True"></asp:RegularExpressionValidator>
        </td>
    </tr>
    <tr>
        <td style="text-align:center;">
            <span style="color: #ff0000;">*</span>제 목
        </td>
        <td>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" 
                Width="480px"></asp:TextBox>
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
                Height="150px" Width="480px"  CssClass="form-control" 
                style="display:inline-block;"></asp:TextBox>
            <asp:RequiredFieldValidator ID="valContent" runat="server" 
                ErrorMessage="* 내용을 기입해 주세요." 
                ControlToValidate="txtContent" Display="None" 
                SetFocusOnError="True"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td style="text-align:center;">파일첨부</td>
        <td>
            <asp:CheckBox ID="chkUpload" runat="server" CssClass="check-inline" 
                Text="이 체크박스를 선택하면 업로드 화면이 나타납니다." 
                AutoPostBack="True" 
                OnCheckedChanged="chkUpload_CheckedChanged"></asp:CheckBox>
            <span style="color:#aaaaaa;font-style:italic">(Optional)</span>
            <br />
            <asp:Panel ID="pnlFile" runat="server" Width="480px" 
                Visible="false" Height="25px">
                <input id="txtFileName" style="width: 290px; 
                    height: 19px" type="file" name="txtFileName" runat="server">
                <asp:label ID="lblFileNamePrevious" text="" runat="server" 
                    Visible="false" />
            </asp:Panel>
        </td>
    </tr>
    <tr>
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
    <tr>
        <td style="text-align:center;">
            <span style="color: #ff0000;">*</span>비밀번호
        </td>
        <td>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" 
                style="display:inline-block;" MaxLength="20" Width="150px" 
                TextMode="Password" EnableViewState="False"></asp:TextBox>
            <span style=" color: #aaaaaa;">(수정/삭제 시 필요)</span> 
            <asp:RequiredFieldValidator ID="valPassword" runat="server" 
                ErrorMessage="* 비밀번호를 기입해 주세요." 
                ControlToValidate="txtPassword" Display="None" 
                SetFocusOnError="True"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <% 
        if (!Page.User.Identity.IsAuthenticated)
        {
    %>
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
                ImageUrl="~/DotNetNote/ImageText.aspx" />
            <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <%
        }
    %>
    <tr>
        <td colspan="2" style="text-align:center;">
            <asp:Button ID="btnWrite" runat="server" Text="저장" 
                CssClass="btn btn-primary" OnClick="btnWrite_Click"></asp:Button> 
            <a href="BoardList.aspx" class="btn btn-default">리스트</a>
            <br />
            <asp:ValidationSummary ID="valSummary" runat="server" 
                ShowSummary="False" 
                ShowMessageBox="True" 
                DisplayMode="List"></asp:ValidationSummary>
            <br />
        </td>
    </tr>
</table>