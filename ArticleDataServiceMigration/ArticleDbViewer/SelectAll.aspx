<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SelectAll.aspx.vb" Inherits="SelectAll" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Selected Status Update</title>
    <style type="text/css">
      .Mybox 
      {
        background-color:white;
        color:black;
        width:auto;
        height:300px;
        overflow: auto;
      }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input id="myclose" type="button" runat="server" value="Close" onclick="javascript:parent.window.opener.location.reload(true);javascript:window.close();"
            style="width: 108px" />
    </div>
    </form>
</body>
</html>
