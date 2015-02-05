Imports System.Data
Imports System.Web.HttpServerUtility
Imports System.Net
Imports System.IO

Partial Class ArticleEdit
    Inherits WebFrontEnd


    Protected Sub ArticleEdit_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        pageload(String.Empty)
    End Sub

    Private Sub pageload(ByVal internalMessage As String)
        If Not GetAuthenticatedUser() Then Response.Redirect("notauthenticated.aspx") ' 
        Dim article_uid As String = Request("article_uid")
        Dim responseMessage As String = Request("response")
        Dim arrayDataSouce As New ArrayList
        arrayDataSouce.Add(New ListItem(article_uid))
        gvGalleries.DataSource = arrayDataSouce
        gvGalleries.DataBind()

        If (Not String.IsNullOrEmpty(responseMessage)) Then
            lblmsg1.Text = IIf(CBool(responseMessage), String.Format("Sending Article ({0}) Succeeded", article_uid), String.Format("Sending Article ({0}) Failed", article_uid))
            pnlForm.Visible = False
            '            btnSave.Visible = False
            '  btnMigrate.Visible = False
            Exit Sub
        End If

        If Not IsPostBack Then
            Dim ds As DataSet = BusinessRule.BusinessRule.GetArticle(article_uid)
            If (ds.Tables(0).Rows.Count = 1) Then
                With ds.Tables(0).Rows(0)

                    TxtHeadline.Text = .Item("heading")
                    TxtBody.Text = .Item("Body")
                    txtbyline.Text = .Item("byline")
                    TxtSummary.Text = .Item("summary")
                    TxtProfile.Text = BusinessRule.BusinessRule.GetSaxoProfile(.Item("siteid"), article_uid, .Item("anchor"))

                End With
            End If
            ds = BusinessRule.BusinessRule.GetFreeForm(article_uid)
            Dim html As New StringBuilder
            For Each dr As DataRow In ds.Tables(0).Rows
                html.Append(dr(0).ToString())
            Next
            lblFreeForm.Text = Server.HtmlEncode(html.ToString)
            If (html.ToString.Trim.Length > 0) Then
                viewform.InnerHtml = html.ToString
            End If
            If (Not String.IsNullOrEmpty(internalMessage)) Then lblmsg2.Text = "2:" & internalMessage
        End If
    End Sub

    'Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
    '    Dim article_uid As String = Request("article_uid")
    '    Dim _message As String = SaveArticle(article_uid) 'Using common code for saving article
    '    lblmsg1.Text = IIf(_message.Length > 0, _message, String.Format("Article: {0} Saved", article_uid))
    'End Sub


    Public Function GetUserName() As String
        Return mytracksession.GetValue(MySession.tracksession.keycodes.username)
    End Function

    Public Function GetArticleuid() As String
        Return Request("article_uid")
    End Function

    Private Function SaveArticle(ByVal article_uid As String) As String
        'Common code for saving article
        Dim bEdited As Boolean = False
        Dim headline As String = Server.UrlDecode(TxtHeadline.Text)
        Dim summary As String = Server.UrlDecode(TxtSummary.Text)
        Dim body As String = Server.UrlDecode(TxtBody.Text)
        Dim byline As String = Server.UrlDecode(txtbyline.Text)
        Dim username As String = mytracksession.GetValue(MySession.tracksession.keycodes.username)
        Dim _message As String = String.Empty

        'Select the article, if not already selected (for tracking)
        Dim bOverriden As Boolean = DataAuthentication.DataAuthentication.ArticleState(article_uid)
        If (Not bOverriden) Then BusinessRule.BusinessRule.ArticleSelect(article_uid, username)

        'Restore form values
        TxtHeadline.Text = headline
        TxtSummary.Text = summary
        TxtBody.Text = body
        txtbyline.Text = byline

        'Save Article
        _message = BusinessRule.BusinessRule.SaveArticle(article_uid, headline, summary, body, byline)

        'Mark the Article as edited
        bEdited = IIf(String.IsNullOrEmpty(_message), True, False)
        If (bEdited) Then BusinessRule.BusinessRule.ArticleEdit(article_uid, username)

        'Report
        Return _message
    End Function

    Private Sub setImages(ByRef ph As PlaceHolder, ByVal article_uid As String, ByVal isEditable As Boolean)
        Dim imagewidth As String = "200"
        Dim siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
        Dim galleryrow As String = common.GetFileData(String.Format("{0}/objects/galleryrow.pbo", Server.MapPath("templates")))

        Dim dsImages As System.Data.DataSet = BusinessRule.BusinessRule.GetImagePaths(article_uid)
        Dim dr As DataRow
        Dim numimages As Integer = Convert.ToInt32(dsImages.Tables(0).Rows.Count)
        If (numimages = 0) Then
            PlaceHoldImages.Visible = False
            Exit Sub
        ElseIf (numimages = 1) Then
            imagewidth = "500"
            isEditable = False
        Else

        End If

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
                If (Not isEditable) Then
                    checked &= " disabled=""disabled"" "
                    mygalleryrow = mygalleryrow.Replace("<%style%>", "style='visibility:hidden;'")
                End If
                mygalleryrow = mygalleryrow.Replace("<%checked%>", checked)
                mygalleryrow = mygalleryrow.Replace("<%article_uid%>", article_uid)
                mygalleryrow = mygalleryrow.Replace("<%width%>", imagewidth)
                mygalleryrow = mygalleryrow.Replace("<%style%>", "")


            Next
            ph.Controls.Add(New LiteralControl(mygalleryrow))
        Next

    End Sub

    Protected Sub gvGalleries_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvGalleries.RowDataBound
        If (e.Row.RowType = DataControlRowType.DataRow) Then
            Dim article_uid As String = CType(e.Row.DataItem, ListItem).Text
            Dim ph As PlaceHolder = e.Row.FindControl("PlaceHolder1")
            setImages(ph, article_uid, True)
        End If
    End Sub

    'Protected Sub btnMigrate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnMigrate.Click
    '    Dim uri As String = String.Empty
    '    Dim article_uid As String = Request("article_uid")
    '    Dim _message As String = SaveArticle(article_uid) 'Using common code for saving article
    '    Dim bEdited As Boolean = IIf(String.IsNullOrEmpty(_message), True, False)

    '    If (Not bEdited) Then
    '        lblmsg1.Text = _message
    '        Exit Sub
    '    End If

    '    'Using migrate.aspx for common migration code
    '    uri = String.Format("{0}{1}/migrate.aspx?article_uid={2}&edited={3}", Request.Url.GetLeftPart(UriPartial.Authority), Request.ApplicationPath, article_uid, bEdited.ToString)
    '    Response.Redirect(uri)
    'End Sub

    'Protected Overrides Sub OnError(ByVal e As EventArgs) 'Handles Me.Error
    '    Dim bFree As Boolean
    '    Dim sender As New Object
    '    MyBase.OnError(e)
    '    Dim ex = Server.GetLastError().GetBaseException()
    '    ' handle HttpRequestValidationException
    '    If TypeOf ex Is System.Web.HttpRequestValidationException Then
    '        bFree = IIf(String.IsNullOrEmpty(lblFreeForm.Text), False, True)
    '        Response.Clear()
    '        pageload("Freeform cannot contain HTML. Please, remove any markup. - " & bFree & " - " & lblFreeForm.Text)
    '    End If
    'End Sub
End Class
