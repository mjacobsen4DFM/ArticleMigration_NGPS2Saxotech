﻿<%@ Page Language="VB" AutoEventWireup="false" CodeFile="NotAuthenticated.aspx.vb"
    Inherits="NotAuthenticated" Theme="Theme1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Not Authenticated</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <center>
            <table border="1" width="80%">
                <tr align="left">
                    <td class="TimeOutHead" align="center">
                        It appears that your browser is rejecting cookies or session time out
                    </td>
                </tr>
                <tr align="left">
                    <td class="Normal" align="right">
                        <input type="button" value="Close" onclick="javascript:window.close();" style="width: 108px" />
                    </td>
                </tr>
            </table>
        </center>
    </div>
    </form>
</body>
</html>
