<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Frame.Test.Web.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" language="javascript" src="Scripts/jquery-1.4.1.min.js"></script>
    <script type="text/javascript" language="javascript" src="Scripts/json2.js"></script>
    <script type="text/javascript" language="javascript" src="Scripts/Barrett.js"></script>
    <script type="text/javascript" language="javascript" src="Scripts/BigInt.js"></script>
    <script type="text/javascript" language="javascript" src="Scripts/RSA.js"></script>
    <script type="text/javascript" language="javascript">

        // A.执行脚本，脚本不带SQL参数
        function QueryDemoTestNoParams() {
            $.ajax({
                dataType: "json", type: "POST",
                url: "services/session/GetSession",
                success: function (data) {
                    if (data) {

                    }
                }
            });
        };

        $(document).ready(function () {
            $("#btnRSA").click(function () {
                $.ajax({
                    dataType: "json", type: "POST",
                    url: "services/DemoHandlerService/get_public_xml",
                    success: function (result) {
                        if (result && result.returnDesc == "OK") {
                            setMaxDigits(129);
                            var strExponent = result.returnValue.Exponent;
                            var strModulus = result.returnValue.Modulus;
                            var key = new RSAKeyPair(strExponent, "", strModulus);
                            var pwdRtn = encryptedString(key, "hello,world!");

                            // 解密
                            $.ajax({
                                dataType: "json", type: "POST",
                                url: "services/DemoHandlerService/decrypt",
                                data: { "str": pwdRtn },
                                success: function (result) {
                                    if (result) {
                                    }
                                }
                            });

                        }
                    }
                });
            });
        });

    </script>
</head>
<body>
    <form id="form1" runat="server">    
    <input id="btnTest1" type="button" value="Test1" onclick="QueryDemoTestNoParams();" />
    <div>
    <a href="AppSessionTest.aspx">AppSession</a>
        <br />
        <a href="DataTest.aspx">DataTest</a><br />
        <a href="DaoTest/DaoPage.aspx">Dao Demo</a><br />
        <a href="ServiceTest/ServerServicePage.aspx">Service Demo</a><br />
        <a href="ServiceTest/AjaxServiceBackground.aspx">Ajax Service Demo</a>
        <a id="btnRSA">RSA Demo</a>
    </div>
    </form>
</body>
</html>
