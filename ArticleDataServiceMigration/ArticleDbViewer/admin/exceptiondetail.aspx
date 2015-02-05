<%@ Page Language="VB" AutoEventWireup="false" CodeFile="exceptiondetail.aspx.vb" Inherits="exceptiondetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:DetailsView ID="DetailsView1" runat="server" Height="50px" Width="125px" 
            AutoGenerateRows="False" BackColor="White" BorderColor="#E7E7FF" 
            BorderStyle="None" BorderWidth="1px" CellPadding="3" 
            DataKeyNames="SessionErrorID" DataSourceID="EntityDataSource1" 
            GridLines="Horizontal">
        <AlternatingRowStyle BackColor="#F7F7F7" />
        <EditRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
        <Fields>
            <asp:BoundField DataField="SessionErrorID" HeaderText="SessionErrorID" 
                ReadOnly="True" SortExpression="SessionErrorID" />
            <asp:BoundField DataField="SID" HeaderText="SID" SortExpression="SID" />
            <asp:BoundField DataField="RequestMethod" HeaderText="RequestMethod" 
                SortExpression="RequestMethod" />
            <asp:BoundField DataField="ServerPort" HeaderText="ServerPort" 
                SortExpression="ServerPort" />
            <asp:BoundField DataField="HTTPS" HeaderText="HTTPS" SortExpression="HTTPS" />
            <asp:BoundField DataField="LocalAddr" HeaderText="LocalAddr" 
                SortExpression="LocalAddr" />
            <asp:BoundField DataField="HostAddress" HeaderText="HostAddress" 
                SortExpression="HostAddress" />
            <asp:BoundField DataField="UserAgent" HeaderText="UserAgent" 
                SortExpression="UserAgent" />
            <asp:BoundField DataField="URL" HeaderText="URL" SortExpression="URL" />
            <asp:BoundField DataField="CustomerRefID" HeaderText="CustomerRefID" 
                SortExpression="CustomerRefID" />
            <asp:BoundField DataField="FormData" HeaderText="FormData" 
                SortExpression="FormData" />
            <asp:BoundField DataField="AllHTTP" HeaderText="AllHTTP" 
                SortExpression="AllHTTP" />
            <asp:BoundField DataField="InsertDate" HeaderText="InsertDate" 
                SortExpression="InsertDate" />
            <asp:CheckBoxField DataField="IsCookieLess" HeaderText="IsCookieLess" 
                SortExpression="IsCookieLess" />
            <asp:CheckBoxField DataField="IsNewSession" HeaderText="IsNewSession" 
                SortExpression="IsNewSession" />
        </Fields>
        <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
        <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
        <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
        <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
    </asp:DetailsView><asp:EntityDataSource ID="EntityDataSource1" runat="server" 
            ConnectionString="name=ArticleDataDbEntities" 
            DefaultContainerName="ArticleDataDbEntities" EnableFlattening="False" 
            EntitySetName="SessionErrors" EntityTypeFilter=""
            Where="it.SessionErrorID = @serrorid " >
            <WhereParameters>
                <asp:QueryStringParameter DbType="Int32" DefaultValue="0" Name="serrorid"   QueryStringField="serrorid" />
            </WhereParameters>
        </asp:EntityDataSource>
        
    </div>
    </form>
</body>
</html>
