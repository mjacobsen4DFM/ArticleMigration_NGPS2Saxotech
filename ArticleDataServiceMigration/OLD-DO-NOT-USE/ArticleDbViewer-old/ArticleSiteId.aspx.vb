
Partial Public Class ArticleSiteId
    Inherits System.Web.UI.Page

    Private mytracksession As New MySession.tracksession

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim sessionid As String = mytracksession.CreateSessionId()
            lblsessionid.Text = sessionid
            DrpSite.DataSource = common.MySites.Tables(0)
            DrpSite.DataTextField = "pubname"
            DrpSite.DataValueField = "siteid"
            DrpSite.DataBind()
            Dim siteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
            If (siteid.Length > 0) Then DrpSite.SelectedValue = siteid
        End If
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
        Dim siteid As String = DrpSite.SelectedValue.ToString()
        Dim publication_name As String = DrpSite.SelectedItem.Text()
        Dim destination_siteid As String = common.GetDestinationSiteid(siteid)
        mytracksession.SetValue(MySession.tracksession.keycodes.siteid, siteid)
        mytracksession.SetValue(MySession.tracksession.keycodes.destination_siteid, destination_siteid)
        mytracksession.SetValue(MySession.tracksession.keycodes.publicationName, publication_name)
        Response.Redirect("articlegroup.aspx")
    End Sub

End Class