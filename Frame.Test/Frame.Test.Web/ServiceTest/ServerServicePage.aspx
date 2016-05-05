<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServerServicePage.aspx.cs" Inherits="Frame.Test.Web.ServiceTest.ServerServicePage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" language="javascript" src="../Scripts/json2.js"></script>
    <script type="text/javascript" language="javascript" src="../Scripts/jquery-1.4.1.min.js"></script>
    <script type="text/javascript" language="javascript">

        // A.执行脚本，脚本不带SQL参数
        function QueryDemoTestNoParams() {
            $.ajax({
                dataType: "json", type: "POST",
                url: "http://localhost:29258/services/DataService/Execute",
                data: { "CommandName": "sqlid:QueryBank", "Params": null },
                success: function (data) {
                    if (data.returnDesc == "OK") {
                        
                    }
                }
            });
        };

        // B.执行脚本，脚本带SQL参数
        function QueryDemoTestWithParams() {
            var params =
            {
                "Name": "工商银行"
            };
            $.ajax({
                dataType: "json", type: "POST",
                url: "http://localhost:29258/services/DataService/Execute",
                data: { "CommandName": "sqlid:QueryBankByName", "Params": JSON.stringify(params) },
                success: function (data) {
                    if (data.returnDesc == "OK") {
                        
                    }
                }
            });
        }

        function QueryDemoTestFromDemoHandler() {
            $.ajax({
                dataType: "json", type: "POST",
                url: "http://localhost:29258/services/DemoHandlerService/GetDemoString",
                data: { "p1": "sqlid:params1", "p2": "params2" },
                success: function (data) {
                    if (data.returnDesc == "OK") {
                        alert(data.returnValue);
                    }
                }
            });
        }

        function QueryDemoTestFromDemoHandlerForJsonParam() {
            var params =
            {
                "Name": "工商银行",
                "Text": "Params"
            };
            $.ajax({
                dataType: "json", type: "POST",
                url: "http://localhost:29258/services/DemoHandlerService/GetDemoStringForJsonParam",
                data: { "json": JSON.stringify(params) },
                success: function (data) {
                    if (data.returnDesc == "OK") {
                        alert(data.returnValue);
                    }
                }
            });
        }

        function QueryDemoTestFromPageDemoHandler() {
            $.ajax({
                dataType: "json", type: "POST",
                url: "http://localhost:29258/pages/PageDemo/GetResult",
                success: function (data) {
                    var d = data;
                },
                error: function (o,info,ex) {
                
                }
            });
        }
    </script>
</head>
<body>
    <input id="btnTest1" type="button" value="Test1" onclick="QueryDemoTestNoParams();" />
    <input id="btnTest2" type="button" value="Test2" onclick="QueryDemoTestWithParams();" />
    <input id="btnTest3" type="button" value="Test3" onclick="QueryDemoTestFromDemoHandler();" />
    <input id="btnTest4" type="button" value="Test4" onclick="QueryDemoTestFromDemoHandlerForJsonParam();" />
    <input id="btnTest5" type="button" value="Test5" onclick="QueryDemoTestFromPageDemoHandler();" />
    <div id="Container">
    </div>
</body>
</html>
