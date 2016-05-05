<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AjaxServiceBackground.aspx.cs" Inherits="Frame.Test.Web.ServiceTest.AjaxServiceBackground" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" language="javascript" src="../Scripts/json2.js"></script>
    <script type="text/javascript" language="javascript" src="../Scripts/jquery-1.4.1.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button1" runat="server" Text="Button1" 
            onclick="Button1_Click" />
        <asp:Button ID="Button2" runat="server" Text="Button2" 
            onclick="Button2_Click" />
        <asp:Button ID="Button3" runat="server" Text="Button3" 
            onclick="Button3_Click" />

        <asp:Button ID="Button4" runat="server" Text="Button4" 
            onclick="Button4_Click" />
        <asp:Button ID="Button5" runat="server" Text="Button5" 
            onclick="Button5_Click" />
        <asp:Button ID="Button6" runat="server" Text="Button6" 
            onclick="Button6_Click" />
    </div>
    </form>
</body>
</html>
