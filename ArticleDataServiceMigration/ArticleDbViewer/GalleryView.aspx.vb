
Imports System.Data
Imports System.IO

Partial Class GalleryView
    Inherits WebFrontEnd

    Protected Sub GalleryView_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not GetAuthenticatedUser() Then Response.Redirect("login.aspx") ' 
        If Not IsPostBack Then
            Dim sessionid As String = mytracksession.GetCookie(MySession.tracksession.cookiecodes.SID)
            Dim query As String = mytracksession.GetValue(MySession.tracksession.keycodes.query)
            Dim pageindex As String = mytracksession.GetValue(MySession.tracksession.keycodes.pageindex)

            Dim ds As DataSet = common.CheckCacheQueryId(sessionid, query)
            gvGalleries.DataSource = ds

            If IsNumeric(pageindex) Then gvGalleries.PageIndex = Convert.ToInt32(pageindex)
            gvGalleries.DataBind()
        End If
    End Sub



    Protected Sub gvGalleries_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvGalleries.PageIndexChanging
        Dim sessionid As String = mytracksession.GetCookie(MySession.tracksession.cookiecodes.SID)
        Dim query As String = mytracksession.GetValue(MySession.tracksession.keycodes.query)
        Dim ds As DataSet = common.CheckCacheQueryId(sessionid, query)
        gvGalleries.DataSource = ds
        gvGalleries.PageIndex = e.NewPageIndex
        mytracksession.SetValue(MySession.tracksession.keycodes.pageindex, e.NewPageIndex.ToString)
        gvGalleries.DataBind()
    End Sub


    Private Function GetColumnIndexByHeaderText(ByVal gv As GridView, ByVal columnText As String) As Integer
        Dim cell As TableCell
        For n = 0 To gv.HeaderRow.Cells.Count - 1
            cell = gv.HeaderRow.Cells(n)
            If (cell.Text.ToString.Equals(columnText)) Then
                Return n
            End If
        Next
        Return -1
    End Function


    Private Sub setImages(ByRef ph As PlaceHolder, ByVal article_uid As String, ByVal isEditable As Boolean)
        Dim siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
        Dim galleryrow As String = common.GetFileData(String.Format("{0}/objects/galleryrow.pbo", Server.MapPath("templates")))

        Dim dsImages As System.Data.DataSet = BusinessRule.BusinessRule.GetImagePaths(article_uid)
        Dim dr As DataRow
        For Each dr In dsImages.Tables(0).Rows
            Dim mygalleryrow As String = galleryrow
            Dim asset_uid As String = ""
            Dim tagnames() As String = {"asset_uid", "imagepath", "caption"}
            For Each tag In tagnames
                Dim value As String = dr(tag)
                If tag.Equals("asset_uid") Then asset_uid = value
                If tag.Equals("caption") Then value = System.Web.HttpUtility.HtmlEncode(value)
                mygalleryrow = mygalleryrow.Replace(String.Format("<%{0}%>", tag), value)

                Dim checked As String = ""
                If (BusinessRule.BusinessRule.IsSoftDelete(article_uid, asset_uid, siteid)) Then checked = "checked "
                If (Not isEditable) Then checked &= " disabled=""disabled"" "
                mygalleryrow = mygalleryrow.Replace("<%checked%>", checked)
                mygalleryrow = mygalleryrow.Replace("<%article_uid%>", article_uid)
                mygalleryrow = mygalleryrow.Replace("<%width%>", "150")
                mygalleryrow = mygalleryrow.Replace("<%style%>", "")


            Next
            ph.Controls.Add(New LiteralControl(mygalleryrow))
        Next

    End Sub



    Protected Sub gvGalleries_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvGalleries.RowDataBound
        Dim index As Integer, username As String = ""
        Dim whoami As String = mytracksession.GetValue(MySession.tracksession.keycodes.username)
        If (e.Row.RowType = DataControlRowType.DataRow) Then
            index = GetColumnIndexByHeaderText(gvGalleries, "article_uid")
            Dim article_uid As String = e.Row.Cells(index).Text
            Dim ph As PlaceHolder = e.Row.FindControl("PlaceHolder1")
            CType(e.Row.FindControl("pnlCheckIn"), Panel).Visible = False
            CType(e.Row.FindControl("pnlCheckOut"), Panel).Visible = False
            Dim isEditable As Boolean = False
            If (BusinessRule.BusinessRule.IsArticleLocked(article_uid, username)) Then
                If (whoami.Equals(username)) Then
                    CType(e.Row.FindControl("pnlCheckIn"), Panel).Visible = True
                    isEditable = True
                Else
                    CType(e.Row.FindControl("pnlwho"), Panel).Visible = True        ' someone else has row checked out
                    CType(e.Row.FindControl("lblUsername"), Label).Text = username  ' show username
                End If
            Else
                CType(e.Row.FindControl("pnlCheckOut"), Panel).Visible = True       ' this row is available for check out ?
            End If
            setImages(ph, article_uid, isEditable)

        End If

    End Sub

End Class
