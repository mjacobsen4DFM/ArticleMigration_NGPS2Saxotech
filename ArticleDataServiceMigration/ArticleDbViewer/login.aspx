﻿
<%@ Page Language="VB" AutoEventWireup="false" CodeFile="login.aspx.vb" Inherits="login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <center>
    <div style="background-color:#4A3C8C;color:White">
        Article Data Services
    </div>
    <div>
        <asp:Label ID="lblmsg" runat="server" Text=""></asp:Label>
    </div>
    <div>
        <asp:login ID="Login1" runat="server" BackColor="#EFF3FB" BorderColor="#B5C7DE" 
            BorderPadding="4" BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" 
            Font-Size="0.8em" ForeColor="#333333">
            <InstructionTextStyle Font-Italic="True" ForeColor="Black" />
            <LoginButtonStyle BackColor="White" BorderColor="#507CD1" BorderStyle="Solid" 
                BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284E98" />
            <TextBoxStyle Font-Size="0.8em" />
            <TitleTextStyle BackColor="#507CD1" Font-Bold="True" Font-Size="0.9em" 
                ForeColor="White" />
        </asp:login>
    </div>
    </center>
    </form>
</body>
</html>