Imports DataModels

Partial Class admin_SectionAnchorRuleEntity
    Inherits System.Web.UI.Page


    Sub BindGrid()
        Dim context As ArticleDataDbEntities = New ArticleDataDbEntities()
        If (context.sectionAnchorMaps.Count() > 0) Then
            gridSectionAnchor.DataSource = context.sectionAnchorMaps
            gridSectionAnchor.DataBind()
        Else
            Dim sec As List(Of sectionAnchorMap) = New List(Of sectionAnchorMap)
            sec.Add(New sectionAnchorMap())
            gridSectionAnchor.DataSource = sec
            gridSectionAnchor.DataBind()
            Dim columnsCount As Integer = gridSectionAnchor.Columns.Count
            gridSectionAnchor.Rows(0).Cells.Clear()
            gridSectionAnchor.Rows(0).Cells.Add(New TableCell())
            With gridSectionAnchor.Rows(0).Cells(0)
                .ColumnSpan = columnsCount
                .HorizontalAlign = HorizontalAlign.Center
                .ForeColor = System.Drawing.Color.Red
                .Font.Bold = True
                .Text = "No Results found"
            End With
        End If
        ' context.Connection.Close()
    End Sub


    Protected Sub admin_SectionAnchorRuleEntity_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindGrid()
        End If
        lblMessage.Text = ""

    End Sub

    Protected Sub gridSectionAnchor_RowCancelingEdit(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gridSectionAnchor.RowCancelingEdit
        gridSectionAnchor.EditIndex = -1
        BindGrid()
    End Sub



    Protected Sub gridSectionAnchor_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gridSectionAnchor.RowCommand
        If (e.CommandName.Equals("InsertNew")) Then

            Dim row As GridViewRow = gridSectionAnchor.FooterRow
            Dim sectionAnchor As String = CType(row.FindControl("txtSectionanchorNew"), TextBox).Text
            Dim sectionName As String = CType(row.FindControl("txtSectionNameNew"), TextBox).Text
            Dim taxonomy As String = CType(row.FindControl("txttaxonomyNew"), TextBox).Text
            If Not (String.IsNullOrEmpty(sectionAnchor) And String.IsNullOrEmpty(sectionName) And String.IsNullOrEmpty(taxonomy)) Then
                Dim context As ArticleDataDbEntities = New ArticleDataDbEntities()
                Dim sam As New sectionAnchorMap
                sam.sectionAnchor = sectionAnchor
                sam.sectionName = sectionName
                context.sectionAnchorMaps.AddObject(sam)
                context.SaveChanges()
                lblMessage.Text = "Added Successfully"
                BindGrid()
                '  context.Connection.Close()
            End If
        End If
    End Sub

    Protected Sub gridSectionAnchor_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gridSectionAnchor.RowDataBound

    End Sub

    Protected Sub gridSectionAnchor_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles gridSectionAnchor.RowDeleting
        Dim row As GridViewRow = gridSectionAnchor.Rows(e.RowIndex)
        Dim id As Integer = Convert.ToInt32(gridSectionAnchor.DataKeys(e.RowIndex).Value)
        Dim context As ArticleDataDbEntities = New ArticleDataDbEntities()
        Dim sam As sectionAnchorMap = context.sectionAnchorMaps.First(Function(c) c.id = id)
        context.sectionAnchorMaps.DeleteObject(sam)
        context.SaveChanges()
        BindGrid()
        lblMessage.Text = "Deleted successfully"
        ' context.Connection.Close()
    End Sub


    Protected Sub gridSectionAnchor_RowEditing(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gridSectionAnchor.RowEditing
        gridSectionAnchor.EditIndex = e.NewEditIndex
        BindGrid()

    End Sub



    Protected Sub gridSectionAnchor_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gridSectionAnchor.RowUpdating

        Dim row As GridViewRow = gridSectionAnchor.Rows(e.RowIndex)
        Dim sectionAnchor As String = CType(row.FindControl("txtSectionanchor"), TextBox).Text
        Dim sectionName As String = CType(row.FindControl("txtSectionName"), TextBox).Text
        Dim taxonomy As String = CType(row.FindControl("txttaxonomy"), TextBox).Text
        If Not (String.IsNullOrEmpty(sectionAnchor) And String.IsNullOrEmpty(sectionName) And String.IsNullOrEmpty(taxonomy)) Then
            Dim context As ArticleDataDbEntities = New ArticleDataDbEntities()
            Dim sam As sectionAnchorMap = context.sectionAnchorMaps.First(Function(c) c.sectionAnchor = sectionAnchor)
            sam.sectionAnchor = sectionAnchor
            sam.sectionName = sectionName
            context.SaveChanges()
            lblMessage.Text = "Saved Successfully"
            gridSectionAnchor.EditIndex = -1
            BindGrid()
            '   context.Connection.Close()
        End If
    End Sub




End Class
