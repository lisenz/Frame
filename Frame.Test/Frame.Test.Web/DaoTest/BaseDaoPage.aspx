<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BaseDaoPage.aspx.cs" Inherits="Frame.Test.Web.DaoTest.BaseDaoPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnProc1" runat="server" Text="RunProc1" 
            onclick="btnProc1_Click" /><br />
        <asp:Button ID="btnQueryDataSet" runat="server" Text="QueryDataSet" 
            onclick="btnQueryDataSet_Click" /><br />
        <asp:Button ID="btnQueryDataSet1" runat="server" Text="QueryDataSet1" 
            onclick="btnQueryDataSet1_Click" /><br />
        <asp:Button ID="btnQueryDataSet2" runat="server" Text="QueryDataSet2" 
            onclick="btnQueryDataSet2_Click" /><br />
        <asp:Button ID="btnQueryDataSet3" runat="server" Text="QueryDataSet3" 
            onclick="btnQueryDataSet3_Click" /><br />
        <asp:Button ID="btnQueryDataSet4" runat="server" Text="QueryDataSet4" 
            onclick="btnQueryDataSet4_Click" /><br />
        <asp:Button ID="Button6" runat="server" Text="TestStatement1" /><br />
        <asp:Button ID="Button7" runat="server" Text="TestStatement1" /><br />
        <asp:Button ID="Button8" runat="server" Text="TestStatement1" /><br />
    </div>
    </form>
</body>
</html>
