<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SimpleTreeView.aspx.cs" Inherits="SimpleTreeView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       <asp:XmlDataSource ID="XmlDataSource1" runat="server" DataFile="~/App_Data/simple.xml">  
        </asp:XmlDataSource>  
        <asp:TreeView ID="TreeView1" runat="server" DataSourceID="XmlDataSource1" >  
            <DataBindings>  
                <asp:TreeNodeBinding DataMember="ToolBox" Text="Asp.Net ToolBox" />  
                <asp:TreeNodeBinding DataMember="Item" TextField="Name" />  
                <asp:TreeNodeBinding DataMember="Option" TextField="Control" />  
            </DataBindings>  
        </asp:TreeView>  
    </div>
    </form>
</body>
</html>
