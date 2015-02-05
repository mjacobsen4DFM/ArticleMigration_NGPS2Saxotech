<%@ Page Language="VB" AutoEventWireup="false" CodeFile="LoadSectionAnchorMap.aspx.vb"
    Inherits="admin_LoadSectionAnchorMap" %>

<%@ Register TagPrefix="mycontrol" TagName="info" Src="~/controls/InfoControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <center>
        <div>
            <mycontrol:info ID="Info1" runat="server" />
            <div>
                Siteid
                <% = GetPubinfo(1)%></div>
            <div>
                Publication
                <% = GetPubinfo(2)%></div>
            <div>
                <asp:Button ID="BtnLoad" runat="server" Text="Load" Width="100px"  /> &nbsp;  &nbsp;
                <asp:Button ID="BtnUpdate" runat="server" Text="Update"  Width="100px" />

            </div>
            <div id="myresponse" style="background-color: Black; color: Yellow; width: 500px;
                height: 500px; overflow: auto" runat="server">
            </div>
        </div>
    </center>
    </form>
</body>
</html>
