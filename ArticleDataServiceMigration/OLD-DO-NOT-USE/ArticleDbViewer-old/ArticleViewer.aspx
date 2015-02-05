<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ArticleViewer.aspx.vb" Inherits="ArticleDbViewer.ArticleViewer" theme="Theme1"%>
<%@ Register TagPrefix="UserControl" TagName="BreadCrumb" Src="~/controls/BreadCrumbTrail.ascx" %> 

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Article Viewer</title>
</head>
<body>
<%--    <UserControl:BreadCrumb ID="BreadCrumb1" runat="server" />
--%>
    <form id="form1" runat="server">
<%--    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
--%>    <div class="nav">
        <asp:Button ID="btnGroup" runat="server" Text="Article Group" width="100px"/>
        <asp:Button ID="btnPrevious" runat="server" Text="Previous" width="100px"/>
        <asp:Button ID="btnNext" runat="server" Text="Next"  Width="100px" />
        <asp:Button ID="btnPush" runat="server" Text="Migrate"  Width="100px" />
        
    </div>
    <div>
<%--        <asp:updatepanel ID="Updatepanel1" runat="server">
        <ContentTemplate>
--%>        <div id="articlePage" runat="server">
            </div>
<%--        </ContentTemplate>
        </asp:updatepanel>
--%>    
    </div>
    </form>
</body>
</html>
