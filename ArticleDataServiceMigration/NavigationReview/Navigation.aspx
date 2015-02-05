<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Navigation.aspx.cs" Inherits="Navigation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
  <asp:TreeView id="LinksTreeView"
        Font-Names= "Arial"
        ForeColor="Blue"
        EnableClientScript="true"
        PopulateNodesFromClient="true"  
        OnTreeNodePopulate="PopulateNode"
        runat="server">
        <Nodes>

          <asp:TreeNode Text="Root" 
            SelectAction="Expand"  
            PopulateOnDemand="true"/>

        </Nodes>
      </asp:TreeView>
    </div>
    </form>
</body>
</html>
