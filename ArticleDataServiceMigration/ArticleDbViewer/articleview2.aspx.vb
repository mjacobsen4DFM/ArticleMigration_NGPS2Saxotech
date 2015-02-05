Imports System.Data
Imports System.IO

Partial Class articleview2
    Inherits WebFrontEnd

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
            If (viewuri.Contains("zzweb")) Then
                viewuri = viewuri.Replace("zzweb", "zzdev")
            End If
            articleButton = articleButton.Replace("<%targeturl%>", viewuri).Replace("<%icon%>", "images/check.jpg").Replace("<%status%>", "Migrated")
            articleButtons.Append(articleButton)
        Else
            e.Row.BackColor = Drawing.Color.Yellow
            e.Row.ForeColor = Drawing.Color.Black
        End If

        If (isEditable) Then
            If (viewuri.Length = 0) Then
                ' must checkout in order to migrate local content (content url viewuri means it has been sent )
                articleButton = common.GetFileData(String.Format("{0}/objects/migratejquery.pbo", Server.MapPath("templates")))
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
            articleButton = articleButton.Replace("<%image%>", BusinessRule.BusinessRule.GetErrorImage(dsMigrateError))
            articleButton = articleButton.Replace("<%message%>", BusinessRule.BusinessRule.GetErrorTypeDescription(dsMigrateError))
            articleButtons.Append(articleButton)
            e.Row.BackColor = Drawing.Color.White
            e.Row.ForeColor = Drawing.Color.Red
        End If

        ph.Controls.Add(New LiteralControl(articleButtons.ToString()))
    End Sub

    Protected Sub gvArticles_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvArticles.RowDataBound
        If (e.Row.RowType = DataControlRowType.DataRow) Then
            Dim whoami As String = mytracksession.GetValue(MySession.tracksession.keycodes.username)

            Dim index As Integer = GetColumnIndexByHeaderText(gvArticles, "Start Date")
            e.Row.Cells(index).Text = Convert.ToDateTime(e.Row.Cells(index).Text).ToShortDateString
            Dim destination_siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.destination_siteid)
            index = GetColumnIndexByHeaderText(gvArticles, "CID")
            Dim article_uid As String = e.Row.Cells(index).Text
            Dim viewuri As String = BusinessRule.BusinessRule.GetArticleViewURI(destination_siteid, article_uid)
            If (String.IsNullOrEmpty(viewuri)) Then
                Dim saxo_username As String = ConfigurationManager.AppSettings("saxo_username").ToString()
                Dim saxo_pwd = ConfigurationManager.AppSettings("saxo_pwd").ToString()
                Dim storyurl As String = BusinessRule.BusinessRule.GetArticleStoryurl(destination_siteid, article_uid)
                viewuri = BusinessRule.BusinessRule.storyurltoViewuri(saxo_username, saxo_pwd, storyurl)
            End If
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

            'Determine if the article has been selected for re-migration; show appropiate image -MSJ 20130325
            Dim bState As Boolean = BusinessRule.BusinessRule.ArticleState(article_uid)
            CType(e.Row.FindControl("pnlChecked"), Panel).Visible = bState
            CType(e.Row.FindControl("pnlOpen"), Panel).Visible = Not bState

            'Determine if the article is checked out; if it is, then ensure it is checked out to the current user
            'If both, then show the article as checked out, and show the edit panel (isEditable)
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not GetAuthenticatedUser() Then Response.Redirect("login.aspx") ' 

        Dim article_uid As String = Request("article_uid")
        Dim responseMessage As String = Request("response")
        'ToDo: add code to handle postback with migrate.aspx response code

        Dim query As String = mytracksession.GetValue(MySession.tracksession.keycodes.query)
        'If (BusinessRule.BusinessRule.AllArticlesSelected(query)) Then
        '    btnSelectAll.ImageUrl = "images/checkbox-checked.png"
        'End If
        Dim ans As String = mytracksession.GetValue(MySession.tracksession.keycodes.myselected)
        SelectAll.Checked = False
        If (ans.Equals("Y")) Then SelectAll.Checked = True
        If Not IsPostBack Then
            BindGrid()
        End If
    End Sub

    Private Sub BindGrid()
        Dim sessionid As String = mytracksession.GetCookie(MySession.tracksession.cookiecodes.SID)
        Dim query As String = mytracksession.GetValue(MySession.tracksession.keycodes.query)
        Dim pageindex As String = mytracksession.GetValue(MySession.tracksession.keycodes.pageindex)

        Dim ds As DataSet = common.CheckCacheQueryId(sessionid, query)
        gvArticles.DataSource = ds

        If IsNumeric(pageindex) Then gvArticles.PageIndex = Convert.ToInt32(pageindex)
        gvArticles.DataBind()
        Dim totalPages As Integer = gvArticles.PageCount
        For n = 1 To totalPages
            DropDownPage.Items.Add(n.ToString())
        Next
    End Sub

    Private Sub ToggleSelectArticles(ByVal bSelected As Boolean)
        Dim sessionid As String = mytracksession.GetCookie(MySession.tracksession.cookiecodes.SID)
        Dim query As String = mytracksession.GetValue(MySession.tracksession.keycodes.query)
        Dim username As String = mytracksession.GetValue(MySession.tracksession.keycodes.username)
        Dim article_uid_index As Integer = GetColumnIndexByHeaderText(gvArticles, "CID")
        Dim article_uid As String = String.Empty
        Dim errmsg As String = String.Empty
        Dim bForced As Boolean = True
        Dim ds As DataSet = common.CheckCacheQueryId(sessionid, query)

        For Each row As DataRow In ds.Tables(0).Rows
            article_uid = row.Item("article_uid").ToString
            errmsg = BusinessRule.BusinessRule.ArticleSelect(article_uid, username, bForced, bSelected)
            If (Not String.IsNullOrEmpty(errmsg)) Then
                errmsg = String.Empty
            End If
        Next
    End Sub

    'Protected Sub btnSelectAll_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSelectAll.Click
    '    Dim bSelected As Boolean
    '    If (btnSelectAll.ImageUrl = "images/checkbox-open.png") Then
    '        bSelected = True
    '        btnSelectAll.ImageUrl = "images/checkbox-checked.png"
    '        lblSelectAll.Text = "Eliminate All Articles, from all pages, from migration"
    '    Else
    '        bSelected = False
    '        btnSelectAll.ImageUrl = "images/checkbox-open.png"
    '        lblSelectAll.Text = "Select All Articles, from all pages, for migration"
    '    End If
    '    ToggleSelectArticles(bSelected)
    '    BindGrid()
    'End Sub
End Class
