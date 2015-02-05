<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="jqueryTest.aspx.vb" Inherits="ArticleDbViewer.jqueryTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <script src="js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <form id="form1" runat="server">
    <div id="smug">
    </div>
     <script type="text/javascript">
        $("#smug").load("php/GetGallery.php?albumid=26940004&albumkey=NZnhnC");
     </script>
    </form>
</body>
</html>
