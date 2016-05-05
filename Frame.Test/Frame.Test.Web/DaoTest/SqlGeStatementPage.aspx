<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SqlGeStatementPage.aspx.cs" Inherits="Frame.Test.Web.DaoTest.SqlGeStatementPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnTestStatement1" runat="server" Text="TestStatement1" onclick="btnTestStatement1_Click" /><br />
        <asp:Button ID="btnTestStatement2" runat="server" Text="TestStatement2" onclick="btnTestStatement2_Click" /><br />
        <asp:Button ID="btnTestStatement3" runat="server" Text="TestStatement3" onclick="btnTestStatement3_Click" /><br />
    </div>
    </form>
</body>
</html>
