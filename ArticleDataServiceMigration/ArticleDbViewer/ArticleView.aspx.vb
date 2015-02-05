Imports System.Data
Imports System.IO

Partial Class ArticleView
    Inherits WebFrontEnd

    Protected Sub ArticleView_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not GetAuthenticatedUser() Then Response.Redirect("login.aspx") ' 
        If Not IsPostBack Then
            Dim sessionid As String = mytracksession.GetCookie(MySession.tracksession.cookiecodes.SID)
            Dim query As String = mytracksession.GetValue(MySession.tracksession.keycodes.query)
            Dim pageindex As String = mytracksession.GetValue(MySession.tracksession.keycodes.pageindex)

            Dim ds As DataSet = common.CheckCacheQueryId(sessionid, query)
            gvArticles.DataSource = ds

            If IsNumeric(pageindex) Then gvArticles.PageIndex = Convert.ToInt32(pageindex)
            gvArticles.DataBind()
            Dim totalPages As Integer = gvArticles.PageCount
            For n = 0 To totalPages Step 10
                Dim onebased As Integer = n + IIf(n = 0, 1, 0)
                DropDownPage.Items.Add(onebased.ToString())
            Next

        End If
    End Sub

    Private Sub changePage(ByVal index As Integer)
        Dim sessionid As String = mytracksession.GetCookie(MySession.tracksession.cookiecodes.SID)
        Dim query As String = mytracksession.GetValue(MySession.tracksession.keycodes.query)
        Dim ds As DataSet = common.CheckCacheQueryId(sessionid, query)
        gvArticles.DataSource = ds
        gvArticles.PageIndex = index
        mytracksession.SetValue(MySession.tracksession.keycodes.pageindex, index.ToString)
        gvArticles.DataBind()

    End Sub


    Protected Sub gvArticles_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvArticles.PageIndexChanging
        changePage(e.NewPageIndex)
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


    Private Sub setArticle(ByRef ph As PlaceHolder, ByVal viewuri As String)
        Dim siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
        Dim articlerow As String = common.GetFileData(String.Format("{0}/objects/mark.pbo", Server.MapPath("templates")))
        articlerow = articlerow.Replace("<%targeturl%>", viewuri).Replace("<%icon%>", "images/check.jpg").Replace("<%status%>", "Migrated")
        ph.Controls.Add(New LiteralControl(articlerow))
    End Sub


    Private Sub setArticleToolButtons(ByRef ph As PlaceHolder, ByVal article_uid As String, ByVal viewuri As String, ByVal isEditable As Boolean, ByRef e As System.Web.UI.WebControls.GridViewRowEventArgs)
        Dim articleButton As String = ""
        Dim articleButtons As New StringBuilder
        Dim dsMigrateError As DataSet

        If (viewuri.Length > 0) Then ' exists on Saxotech
            articleButton = common.GetFileData(String.Format("{0}/objects/mark.pbo", Server.MapPath("templates")))
            articleButton = articleButton.Replace("<%targeturl%>", viewuri).Replace("<%icon%>", "images/check.jpg").Replace("<%status%>", "Migrated")
            articleButtons.Append(articleButton)
        Else
            e.Row.BackColor = Drawing.Color.Yellow
            e.Row.ForeColor = Drawing.Color.Black
        End If

        If (isEditable) Then
            If (viewuri.Length = 0) Then
                ' must checkout in order to migrate local content (content url viewuri means it has been sent )
                articleButton = common.GetFileData(String.Format("{0}/objects/migrate.pbo", Server.MapPath("templates")))
                articleButton = articleButton.Replace("<%article_uid%>", article_uid)
                articleButtons.Append(articleButton)
            End If
            ' must checkout in order to edit local content
            articleButton = common.GetFileData(String.Format("{0}/objects/articleEdit.pbo", Server.MapPath("templates")))
            articleButton = articleButton.Replace("<%article_uid%>", article_uid)
            articleButtons.Append(articleButton)
        End If

        'Added error specific images and alt-text to the error.pbo results -MSJ 20130322
        'Changed logic to check whether error rows were returned, rather than have another DB roundtrip
        dsMigrateError = BusinessRule.BusinessRule.GetMigrateArticleErrorInfo(article_uid)
        If (dsMigrateError.Tables(0).Rows.Count <> 0) Then
            articleButton = common.GetFileData(String.Format("{0}/objects/error.pbo", Server.MapPath("templates")))
            articleButton = articleButton.Replace("<%targeturl%>", String.Format("ArticleError.aspx?article_uid={0}", article_uid))
            articleButtons.Append(articleButton)
            e.Row.BackColor = Drawing.Color.White
            e.Row.ForeColor = Drawing.Color.Red
        End If

        ph.Controls.Add(New LiteralControl(articleButtons.ToString()))
    End Sub



    Protected Sub gvArticles_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvArticles.RowDataBound
        If (e.Row.RowType = DataControlRowType.DataRow) Then
            Dim whoami As String = mytracksession.GetValue(MySession.tracksession.keycodes.username)

            Dim index As Integer = GetColumnIndexByHeaderText(gvArticles, "startdate")
            e.Row.Cells(index).Text = Convert.ToDateTime(e.Row.Cells(index).Text).ToShortDateString
            Dim destination_siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.destination_siteid)
            index = GetColumnIndexByHeaderText(gvArticles, "article_uid")
            Dim article_uid As String = e.Row.Cells(index).Text
            Dim viewuri As String = BusinessRule.BusinessRule.GetArticleViewURI(destination_siteid, article_uid)
            Dim owsviewsearch As String = mytracksession.GetValue(MySession.tracksession.keycodes.owsViewsearch).Trim
            Dim owsviewreplace As String = mytracksession.GetValue(MySession.tracksession.keycodes.owsViewReplace).Trim
            Dim ph As PlaceHolder = e.Row.FindControl("PlaceHolder1")
            viewuri = viewuri.Replace(owsviewsearch, owsviewreplace)
            If (viewuri.Length > 0) Then
                viewuri = String.Format("{0}&site=ZZ", viewuri)
            End If
            CType(e.Row.FindControl("pnlCheckIn"), Panel).Visible = False
            CType(e.Row.FindControl("pnlCheckOut"), Panel).Visible = False
            Dim isEditable As Boolean = False, username As String = ""
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
            setArticleToolButtons(ph, article_uid, viewuri, isEditable, e)
        End If
    End Sub

    Protected Sub DropDownPage_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DropDownPage.SelectedIndexChanged
        changePage(Convert.ToInt32(DropDownPage.SelectedValue) - 1)
    End Sub
End Class
