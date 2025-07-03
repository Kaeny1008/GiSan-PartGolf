<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewGameSetting.aspx.cs" Inherits="GiSanParkGolf.Sites.Admin.NewGameSetting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>신규대회 개최 설정</title>

    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/Content/bootstrap.min.css") %>    
        <%: Scripts.Render("~/Scripts/bootstrap.bundle.min.js") %>
    </asp:PlaceHolder> 
    <style>
        /*.form-floating mb-3{
            width:80%;
        }*/
    </style>
</head>
<body>
    <form id="form1" runat="server" class="row g-3">
        <div class="input-group mb-3">
            <span class="input-group-text" style="width: 150px">대회명</span>
            <div class="form-floating">
              <input type="text" runat="server" class="form-control" id="GameName" />
              <label for="floatingInputGroup1">* 대회명을 입력하여 주십시오.</label>
            </div>
        </div>
        <div class="input-group mb-3">
            <span class="input-group-text" style="width: 150px">대회일자</span>
            <div class="form-floating">
              <input type="date" runat="server" class="form-control" id="GameDate" />
              <label for="floatingInputGroup2">* 대회일자를 입력하여 주십시오.</label>
            </div>
        </div>        
    </form>
</body>
</html>
