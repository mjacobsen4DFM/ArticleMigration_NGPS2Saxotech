
Imports System.Data.SqlClient
Imports System.Data

Partial Class ArticleFlagRules
    Inherits WebFrontEnd



    Protected Sub ArticleFlagRules_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not GetAuthenticatedUser() Then Response.Redirect("login.aspx") ' 
        Dim perm As String = mytracksession.GetValue(MySession.tracksession.keycodes.permission).ToLower.Trim
        If Not perm.Equals("superadmin") Then
            Response.Redirect("denied.aspx")
        End If

        If Not IsPostBack Then
            Dim ds As New System.Data.DataSet
            Dim dt As New System.Data.DataTable
            ds.Tables.Add(dt)
            ds.Tables(0).Rows.Add(dt.NewRow)
            GridAdd.DataSource = ds
            GridAdd.DataBind()
            GridEntires.DataBind()
        End If


    End Sub

    Protected Sub GridAdd_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridAdd.RowCommand
        Try
            If (e.CommandName = "ADD") Then

                Dim row As GridViewRow = CType(CType(e.CommandSource, Control).Parent.Parent, GridViewRow)
                Dim value As String = CType(row.FindControl("htmltag"), TextBox).Text()
                Dim Conn As System.Data.SqlClient.SqlConnection = Nothing
                Conn = New SqlConnection(SqlDataSource.ConnectionString)
                Conn.Open()
                Dim sqlstatment As String = String.Format("Insert into htmlholdrules (htmltag) values ('{0}')", value)
                Dim cmd As New SqlCommand(sqlstatment, Conn)
                cmd.ExecuteNonQuery()
                Conn.Close()
                GridEntires.DataBind()
            End If
        Catch ex As Exception

        End Try

    End Sub
End Class
