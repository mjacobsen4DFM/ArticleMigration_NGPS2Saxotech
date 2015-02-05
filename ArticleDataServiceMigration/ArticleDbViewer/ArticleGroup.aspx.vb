Imports System.Data
Imports System.Web

Partial Class ArticleGroup
    Inherits WebFrontEnd


    Private Sub loadsitecategory(ByVal siteid As String, ByVal category As String, ByVal sStartdate As String, ByVal sEndDate As String)
        DrpSite.DataSource = BusinessRule.BusinessRule.MySites.Tables(0)
        DrpSite.DataTextField = "pubname"
        DrpSite.DataValueField = "siteid"
        DrpSite.DataBind()
        If (siteid.Length > 0) Then
            DrpSite.SelectedValue = siteid
        Else
            siteid = DrpSite.SelectedValue
        End If
        DrpCategory.DataSource = BusinessRule.BusinessRule.MyCategories(siteid).Tables(0)
        DrpCategory.DataTextField = "category"
        DrpCategory.DataBind()
        If category.Trim.Length > 0 Then DrpCategory.SelectedValue = category
        txtStartDate.Text = sStartdate
        txtEndDate.Text = sEndDate
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim sessionid As String = mytracksession.CreateSessionId()              ' This will create as session if it does not exist 

        lblsession.Text = sessionid

        If Not GetAuthenticatedUser() Then Response.Redirect("login.aspx") ' 
        Dim perm As String = mytracksession.GetValue(MySession.tracksession.keycodes.permission).ToString.ToLower.Trim
        chkLogging.Enabled = False
        If (perm.Equals("superadmin")) Then chkLogging.Enabled = True
        If Not IsPostBack Then
            common.RemoveFromCacheQueryId(sessionid)
            Dim siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
            Dim category As String = mytracksession.GetValue(MySession.tracksession.keycodes.category)
            Dim sStartdate As String = mytracksession.GetValue(MySession.tracksession.keycodes.startdate)
            Dim sEndDate As String = mytracksession.GetValue(MySession.tracksession.keycodes.enddate)
            txtStartDate.Text = mytracksession.GetValue(MySession.tracksession.keycodes.selectbyArticleUID)
            loadsitecategory(siteid, category, sStartdate, sEndDate)
            siteid = DrpSite.SelectedValue
            mytracksession.SetValue(MySession.tracksession.keycodes.siteid, siteid)
            If (mytracksession.GetValue(MySession.tracksession.keycodes.logging).Equals("Y")) Then chkLogging.Checked = True
            Dim grouptype As String = mytracksession.GetValue(MySession.tracksession.keycodes.grouptype)
            If grouptype.Length > 0 Then RadioList.SelectedValue = grouptype
        End If
        buildHoldCheckboxes()
    End Sub

    Private Sub buildHoldCheckboxes()
        'Get hold conditons
        Dim dsHolds As DataSet
        Dim drHold As DataRow
        Dim ph As PlaceHolder = Me.FindControl("phHolds")
        Dim chkBox As System.Web.UI.WebControls.CheckBox
        dsHolds = BusinessRule.BusinessRule.GetMigrationErrorTypes("Hold Rule")
        For Each drHold In dsHolds.Tables(0).Rows
            'holdconditions.Append(String.Format(sHolds, drHold.Item("title").ToString, drHold.Item("description").ToString))
            chkBox = New System.Web.UI.WebControls.CheckBox()

            chkBox.ID = String.Format("chk{0}", drHold.Item("title").ToString())
            chkBox.ToolTip = drHold.Item("description").ToString()
            chkBox.Text = drHold.Item("title").ToString()
            ph.Controls.Add(chkBox)
        Next
    End Sub

    Private Sub creategroup()
        Dim category As String = DrpCategory.SelectedValue.Trim
        Dim startdate As String = txtStartDate.Text
        Dim enddate As String = txtEndDate.Text
        Dim sessionid As String = mytracksession.GetCookie(MySession.tracksession.cookiecodes.SID)
        common.RemoveFromCacheQueryId(sessionid)
        Dim siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
        Dim radio_option As String = RadioList.SelectedValue


        Dim ph As PlaceHolder = Me.FindControl("phHolds")
        Dim holdconditions As New StringBuilder
        Dim dsHolds As DataSet
        Dim drHold As DataRow
        Dim aHolds As New ArrayList

        'Chunked out queries to mix-and-match them
        Dim selectClause As String = "SELECT DISTINCT ISNULL(article_override.selected, 0) AS selected, article.article_uid, article.startdate, article.category, article.heading, article.body AS story, article.imagecount"
        Dim fromClause As String = "FROM article LEFT OUTER JOIN asset ON article.article_uid = asset.article_uid LEFT OUTER JOIN article_override ON article.article_uid = article_override.article_uid LEFT OUTER JOIN migrationerror ON article.article_uid = migrationerror.article_uid"
        Dim whereClause As String = String.Format("WHERE article.siteid = '{0}' ", siteid) 'All articles (a.k.a. radio_option = "A")
        Select Case radio_option
            Case "H"
                whereClause &= String.Format("AND article.article_uid NOT IN (select article_uid from saxo_article where siteid = '{0}')", siteid)
            Case "S"
                whereClause &= String.Format("AND article.article_uid IN(SELECT article_uid FROM saxo_article where (siteid = '{0}'))", siteid)
            Case "G"
                whereClause &= String.Format("AND article.imagecount > 1", siteid)
            Case "I"
                whereClause &= String.Format("AND article.imagecount > 0", siteid)
        End Select


        'Add hold condition criteria
        Dim chkBoxID As String
        Dim bHolds As Boolean = False
        dsHolds = BusinessRule.BusinessRule.GetMigrationErrorTypes("Hold Rule")
        For Each drHold In dsHolds.Tables(0).Rows
            chkBoxID = String.Format("chk{0}", drHold.Item("title").ToString())
            If (Request(chkBoxID) IsNot Nothing) Then
                If (chkBoxID = "chkFreeform") Then
                    whereClause &= " AND asset.asset_type = 111"
                End If
                aHolds.Add(drHold.Item("type").ToString())
                bHolds = True
            End If
        Next


        If (Request("chkErrors") IsNot Nothing) Then
            dsHolds = BusinessRule.BusinessRule.GetMigrationErrorTypes("Unknown Error")
            For Each drHold In dsHolds.Tables(0).Rows
                aHolds.Add(drHold.Item("type").ToString())
                bHolds = True
            Next
            dsHolds = BusinessRule.BusinessRule.GetMigrationErrorTypes("Transmission Error")
            For Each drHold In dsHolds.Tables(0).Rows
                aHolds.Add(drHold.Item("type").ToString())
                bHolds = True
            Next
        End If

        If (bHolds) Then
            whereClause &= String.Format(" AND migrationerror.errortype in({0}) AND article_override.senttimestamp is null ", String.Join(",", aHolds.ToArray()))
        End If

        If (category.Length > 0 AndAlso Not category.Contains(DataAuthentication.DataAuthentication.default_Category)) Then whereClause &= String.Format(" AND article.category = '{0}' ", category)
        If IsDate(txtStartDate.Text) Then
            Dim sStartDate As String = txtStartDate.Text
            whereClause &= String.Format(" AND article.startdate >= '{0}' ", sStartDate)
            mytracksession.SetValue(MySession.tracksession.keycodes.startdate, sStartDate)
        End If
        If IsDate(txtEndDate.Text) Then
            Dim sEndDate As String = txtEndDate.Text
            whereClause &= String.Format(" AND article.startdate <= '{0}' ", sEndDate)
            mytracksession.SetValue(MySession.tracksession.keycodes.enddate, sEndDate)
        End If

        Dim query As String = String.Format("{0} {1} {2} ORDER BY selected, article.startdate, article.article_uid", selectClause, fromClause, whereClause)

        If (txtArticle.Text.Trim.Length > 0) Then
            ' query = String.Format("select article.article_uid,  article.startdate, article.category ,article.heading, article.body as story,article.imagecount from article a  where article.siteid= '{0}' AND article.article_uid = '{1}'", siteid, txtArticle.Text)
            query = String.Format("select article_uid,  startdate, category ,heading, body as story,imagecount from article   where siteid= '{0}' AND  article_uid = '{1}'", siteid, txtArticle.Text)
        End If

        mytracksession.SetValue(MySession.tracksession.keycodes.query, query)
        mytracksession.SetValue(MySession.tracksession.keycodes.grouptype, radio_option)
        Dim ds As DataSet = common.CheckCacheQueryId(sessionid, query)
    End Sub


    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
        Dim bLive As Boolean = True 'Live or Test, for SQL trap
        Dim siteid As String = DrpSite.SelectedValue.ToString()
        mytracksession.SetValue(MySession.tracksession.keycodes.siteid, siteid)
        mytracksession.SetValue(MySession.tracksession.keycodes.publicationName, DrpSite.SelectedItem.Text())
        mytracksession.SetValue(MySession.tracksession.keycodes.category, DrpCategory.SelectedValue)
        mytracksession.SetValue(MySession.tracksession.keycodes.pageindex, "0")
        mytracksession.SetValue(MySession.tracksession.keycodes.destination_siteid, BusinessRule.BusinessRule.GetDestinationSiteid(siteid, DataAuthentication.DataAuthentication.Fields.destination_siteid))
        mytracksession.SetValue(MySession.tracksession.keycodes.owsTarget, BusinessRule.BusinessRule.GetDestinationSiteid(siteid, DataAuthentication.DataAuthentication.Fields.owstarget))
        mytracksession.SetValue(MySession.tracksession.keycodes.owsViewsearch, BusinessRule.BusinessRule.GetDestinationSiteid(siteid, DataAuthentication.DataAuthentication.Fields.owsviewsearch))
        mytracksession.SetValue(MySession.tracksession.keycodes.owsViewReplace, BusinessRule.BusinessRule.GetDestinationSiteid(siteid, DataAuthentication.DataAuthentication.Fields.owsviewreplace))
        mytracksession.SetValue(MySession.tracksession.keycodes.startdate, txtStartDate.Text)
        mytracksession.SetValue(MySession.tracksession.keycodes.enddate, txtEndDate.Text)
        mytracksession.SetValue(MySession.tracksession.keycodes.selectbyArticleUID, txtArticle.Text)


        creategroup()

        Dim grouptype As String = mytracksession.GetValue(MySession.tracksession.keycodes.grouptype)

        mytracksession.SetValue(MySession.tracksession.keycodes.logging, IIf(chkLogging.Checked, "Y", "N"))
        If (bLive OrElse Not String.IsNullOrEmpty(lblTest.Text)) Then
            Select Case grouptype
                Case "A", "H", "S", "I"
                    Response.Redirect("articleview2.aspx")
                Case "G"
                    Response.Redirect("galleryView.aspx")
            End Select
        End If
        lblTest.Text = mytracksession.GetValue(MySession.tracksession.keycodes.query)
    End Sub



    Protected Sub DrpSite_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DrpSite.SelectedIndexChanged
        Dim siteid As String = DrpSite.SelectedValue.ToString()
        mytracksession.SetValue(MySession.tracksession.keycodes.siteid, siteid)
        mytracksession.SetValue(MySession.tracksession.keycodes.publicationName, DrpSite.SelectedItem.Text())
        mytracksession.SetValue(MySession.tracksession.keycodes.category, DataAuthentication.DataAuthentication.default_Category)

        mytracksession.SetValue(MySession.tracksession.keycodes.destination_siteid, BusinessRule.BusinessRule.GetDestinationSiteid(siteid, DataAuthentication.DataAuthentication.Fields.destination_siteid))
        mytracksession.SetValue(MySession.tracksession.keycodes.owsTarget, BusinessRule.BusinessRule.GetDestinationSiteid(siteid, DataAuthentication.DataAuthentication.Fields.owstarget))
        mytracksession.SetValue(MySession.tracksession.keycodes.owsViewsearch, BusinessRule.BusinessRule.GetDestinationSiteid(siteid, DataAuthentication.DataAuthentication.Fields.owsviewsearch))
        mytracksession.SetValue(MySession.tracksession.keycodes.owsViewReplace, BusinessRule.BusinessRule.GetDestinationSiteid(siteid, DataAuthentication.DataAuthentication.Fields.owsviewreplace))
        loadsitecategory(siteid, DataAuthentication.DataAuthentication.default_Category, "", "")
        'buildHoldCheckboxes()
    End Sub
End Class
