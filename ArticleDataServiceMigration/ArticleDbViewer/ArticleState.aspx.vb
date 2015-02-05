
Partial Class ArticleState
    Inherits WebFrontEnd

    Protected Sub ArticleSelect_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not GetAuthenticatedUser() Then
            Exit Sub
        End If

        Dim article_uid As String = Request("article_uid").ToString
        Dim bState As Boolean = BusinessRule.BusinessRule.ArticleState(article_uid)
        'Dim stateLink As String = "<a href=""#"" id='3100076' class="ArticleSelect"><img src="ArticleState.aspx?article_uid=3100076&stateType=selected" alt="Select Article (3100076) for migration" height="25" widht="25" /></a>"
        Dim isImage As String = "<img src=""images/checkbox-checked.png"" height=""18"" border=""0"" />"
        Dim notImage As String = "<img src=""images/checkbox-open.png"" height=""18"" border=""0"" />"
        Dim statusImage As String = IIf(bState, isImage, notImage)
        Response.Write(statusImage)
    End Sub
End Class
