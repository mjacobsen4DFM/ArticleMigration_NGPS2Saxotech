<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GalleryViewer.aspx.vb" Inherits="ArticleDbViewer.ListGalleries" EnableSessionState="True" %>
<%@ Register TagPrefix="UserControl" TagName="BreadCrumb" Src="~/controls/BreadCrumbTrail.ascx" %> 

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <UserControl:BreadCrumb ID="BreadCrumb1" runat="server" />
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div>
        <asp:Button ID="btnPrevious" runat="server" Text="Previous" />
        <asp:Button ID="btnNext" runat="server" Text="Next" />
    </div>
    <div>
        <asp:updatepanel ID="Updatepanel1" runat="server">
        <ContentTemplate>
            <div id="articlePage" runat="server">
            </div>
        </ContentTemplate>
        </asp:updatepanel>
    
    </div>
    </form>
</body>
</html>
