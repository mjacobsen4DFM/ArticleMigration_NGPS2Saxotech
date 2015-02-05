Public Partial Class BreadCrumbTrail
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'Dim Referer As String = Request.ServerVariables("SCRIPT_NAME")
            'Select Case Referer
            '    Case "/ArticleDbViewer/ArticleGroup.aspx"
            '        aArticleGroup.Visible = False
            '        aArticleView.Visible = False
            '        aGalleryview.Visible = False
            '    Case "/ArticleDbViewer/articleviewer.aspx"
            '        aArticleView.HRef = ""
            '        aGalleryview.Visible = False

            '    Case "/ArticleDbViewer/galleryviewer.aspx"
            '        aArticleView.Visible = False
            '        aGalleryview.HRef = ""

            'End Select
        Catch ex As Exception

        End Try
    End Sub
End Class