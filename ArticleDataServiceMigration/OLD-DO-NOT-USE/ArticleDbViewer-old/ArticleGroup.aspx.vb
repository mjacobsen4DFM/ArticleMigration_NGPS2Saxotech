Imports System.Data
Partial Public Class ArticleGroup
    Inherits System.Web.UI.Page

    Private mytracksession As New MySession.tracksession

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim sessionid As String = mytracksession.GetCookie(MySession.tracksession.cookiecodes.SID)
            lblsessionid.Text = sessionid
            common.RemoveFromCacheQueryId(sessionid)
            lblpub.Text = mytracksession.GetValue(MySession.tracksession.keycodes.publicationName)
            Dim siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
            DrpCategory.DataSource = common.MyCategories(siteid).Tables(0)
            DrpCategory.DataTextField = "category"
            DrpCategory.DataBind()
            Dim category As String = mytracksession.GetValue(MySession.tracksession.keycodes.category)
            If (category.Length > 0) Then DrpCategory.SelectedValue = category
        End If
    End Sub

    Private Sub creategroup(ByVal bgalleries As Boolean)

        Dim category As String = DrpCategory.SelectedValue
        Dim startdate As String = txtStartDate.Text
        Dim enddate As String = txtEndDate.Text
        Dim sessionid As String = mytracksession.GetCookie(MySession.tracksession.cookiecodes.SID)
        common.RemoveFromCacheQueryId(sessionid)
        Dim siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
        Dim query As String = String.Format("select article_uid,startdate, category ,heading, body as story, imagecount from article  where  siteid= '{0}' ", siteid)
        If (bgalleries) Then query &= "and imagecount > 1 "

        If (category.Trim.Length > 0) Then query &= String.Format(" and category = '{0}' ", category)
        If IsDate(txtStartDate.Text) Then
            Dim sStartDate As String = txtStartDate.Text
            query &= String.Format(" and startdate >= '{0}' ", sStartDate)
        End If
        If IsDate(txtEndDate.Text) Then
            Dim sEndDate As String = txtEndDate.Text
            query &= String.Format(" and startdate <= '{0}' ", sEndDate)
        End If
        query &= " order by startdate"
        mytracksession.SetValue(MySession.tracksession.keycodes.query, query)
        Dim ds As DataSet = common.CheckCacheQueryId(sessionid, query)
    End Sub


    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
        creategroup(False)
        mytracksession.SetValue(MySession.tracksession.keycodes.category, DrpCategory.SelectedValue)
        Response.Redirect("articleviewer.aspx")

    End Sub

    Protected Sub btnGalleries_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGalleries.Click
        creategroup(True)
        Response.Redirect("galleryviewer.aspx")
    End Sub

    Protected Sub btnChangeSite_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnChangeSite.Click
        Response.Redirect("ArticleSiteId.aspx")
    End Sub
End Class