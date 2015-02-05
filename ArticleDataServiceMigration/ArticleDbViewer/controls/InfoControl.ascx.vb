
Partial Class InfoControl
    Inherits System.Web.UI.UserControl

    Public Sub New()
    End Sub

    Protected Sub InfoControl_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim username As String = DirectCast(Me.Page, WebFrontEnd).mytracksession.GetValue(MySession.tracksession.keycodes.username)
        Dim perm As String = DirectCast(Me.Page, WebFrontEnd).mytracksession.GetValue(MySession.tracksession.keycodes.permission).ToLower.Trim
        Dim windowsid As String = DirectCast(Me.Page, WebFrontEnd).mytracksession.GetValue(MySession.tracksession.keycodes.windowsid)
        Dim PubName As String = DirectCast(Me.Page, WebFrontEnd).mytracksession.GetValue(MySession.tracksession.keycodes.publicationName)
        Dim Category As String = DirectCast(Me.Page, WebFrontEnd).mytracksession.GetValue(MySession.tracksession.keycodes.category)
        Dim articlegroup As Boolean = False
        If HttpContext.Current.Request.Url.AbsoluteUri IsNot Nothing Then
            Dim uri As String = HttpContext.Current.Request.Url.AbsoluteUri.ToLower.Trim
            If uri.Contains("articlegroup.aspx") Then articlegroup = True
        End If

        pnlAdmin.Visible = False

        PnlPub.Visible = False
        PnlCat.Visible = False
        Pnlgroup.visible = False
        PnlLogout.Visible = True
        lblPublication.Text = PubName
        lblCategory.Text = Category
        If windowsid.Equals("Y") Then PnlLogout.Visible = False
        If PubName.Length > 0 And Not articlegroup Then PnlPub.Visible = True
        If Category.Length > 0 And Not articlegroup Then PnlCat.Visible = True
        If Not articlegroup Then
            Pnlgroup.visible = True
        End If
        If perm.Equals("superadmin") Then pnlAdmin.Visible = True
        lbluser.Text = username
    End Sub
End Class
