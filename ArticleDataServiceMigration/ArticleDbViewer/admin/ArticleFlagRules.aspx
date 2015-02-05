<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ArticleFlagRules.aspx.vb" Inherits="ArticleFlagRules" %>

<%@ Register TagPrefix="mycontrol" TagName="info" Src="~/controls/InfoControl.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <center>
            <mycontrol:info ID="Info1" runat="server" />

            <asp:GridView ID="GridAdd" runat="server" AutoGenerateColumns="False" CellPadding="4"
                BackColor="White" BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px">
                <Columns>
                    <asp:TemplateField HeaderText="htmltag">
                        <ItemTemplate>
                            <asp:TextBox ID="htmltag" runat="server"></asp:TextBox>
                        </ItemTemplate>
                        <ControlStyle Width="100px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="">
                        <ItemTemplate>
                            <asp:Button ID="BtnAdd" CommandName="ADD" runat="server" Text="Add" />
                        </ItemTemplate>
                        <ControlStyle Width="40px" />
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#99CCCC" ForeColor="#003399" />
                <RowStyle BackColor="White" ForeColor="#003399" />
                <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                <PagerStyle BackColor="#99CCCC" ForeColor="#003399" HorizontalAlign="Left" />
                <HeaderStyle BackColor="#003399" Font-Bold="True" ForeColor="#CCCCFF" />
            </asp:GridView>
            <asp:GridView ID="GridEntires" runat="server" AutoGenerateColumns="False" BackColor="White"
                BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" CellPadding="3" CellSpacing="1"
                DataKeyNames="id" GridLines="None" DataSourceID="SqlDataSource">
                <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                <RowStyle BackColor="White" ForeColor="#003399" />
                <PagerStyle BackColor="#99CCCC" ForeColor="#003399" HorizontalAlign="Left" />
                <Columns>
                    <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
                    <asp:BoundField DataField="id" HeaderText="id"  ReadOnly="True" SortExpression="id" InsertVisible="False" />
                    <asp:BoundField DataField="htmltag" HeaderText="htmltag" SortExpression="htmltag" />
                </Columns>
                <RowStyle BackColor="#DEDFDE" ForeColor="Black" />
                <SelectedRowStyle BackColor="#9471DE" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Right" />
                <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#E7E7FF" />
            </asp:GridView>
            </center>
           <asp:SqlDataSource ID="SqlDataSource" runat="server" 
            ConnectionString="<%$ ConnectionStrings:connstr %>" 
            DeleteCommand="DELETE FROM [htmlHoldRules] WHERE [id] = @id" 
            InsertCommand="INSERT INTO [htmlHoldRules] ([htmltag]) VALUES (@htmltag)" 
            SelectCommand="SELECT * FROM [htmlHoldRules]" 
            UpdateCommand="UPDATE [htmlHoldRules] SET [htmltag] = @htmltag WHERE [id] = @id">
               <DeleteParameters>
                   <asp:Parameter Name="id" Type="Int32" />
               </DeleteParameters>
               <InsertParameters>
                   <asp:Parameter Name="htmltag" Type="String" />
               </InsertParameters>
               <UpdateParameters>
                   <asp:Parameter Name="htmltag" Type="String" />
                   <asp:Parameter Name="id" Type="Int32" />
               </UpdateParameters>
           </asp:SqlDataSource>

    </div>
    </form>
</body>
</html>
