<%@ Page Language="VB" AutoEventWireup="false" CodeFile="exception.aspx.vb" Inherits="admin_exception" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 773px">
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" 
            DataKeyNames="ExceptionID" DataSourceID="EntityDataSource1" ForeColor="#333333" 
            GridLines="None">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:HyperLinkField DataNavigateUrlFields="SessionErrorID" DataNavigateUrlFormatString="ExceptionDetail.aspx?serrorid={0}"  Text="Details" />
                <asp:BoundField DataField="ExceptionID" HeaderText="ExceptionID" 
                    ReadOnly="True" SortExpression="ExceptionID" />
                <asp:BoundField DataField="ExceptionLevel" HeaderText="ExceptionLevel" 
                    SortExpression="ExceptionLevel" />
                <asp:BoundField DataField="SessionErrorID" HeaderText="SessionErrorID" 
                    SortExpression="SessionErrorID" />
                <asp:BoundField DataField="Source" HeaderText="Source" 
                    SortExpression="Source" />
                <asp:BoundField DataField="StackTrace" HeaderText="StackTrace" 
                    SortExpression="StackTrace" />
                <asp:BoundField DataField="Message" HeaderText="Message" 
                    SortExpression="Message" />
                <asp:BoundField DataField="Machine" HeaderText="Machine" 
                    SortExpression="Machine" />
                <asp:BoundField DataField="TargetSite" HeaderText="TargetSite" 
                    SortExpression="TargetSite" />
                <asp:BoundField DataField="ts" HeaderText="ts" SortExpression="ts" />
            </Columns>
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />
        </asp:GridView>
    </div>


    <asp:EntityDataSource ID="EntityDataSource1" runat="server" 
        AutoGenerateOrderByClause="false" ConnectionString="name=ArticleDataDbEntities" 
        DefaultContainerName="ArticleDataDbEntities" EnableFlattening="False" 
        EntitySetName="Exceptions"  OrderBy="it.ts DESC">
    </asp:EntityDataSource>


    </form>
</body>
</html>
