
Partial Class _Default
    Inherits System.Web.UI.Page



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            DrpSite.DataSource = BusinessRule.BusinessRule.MySites.Tables(0)
            DrpSite.DataTextField = "pubname"
            DrpSite.DataValueField = "siteid"
            DrpSite.DataBind()
        Else
            If Request("process") IsNot Nothing Then
                Response.Redirect("process.aspx")
            End If
        End If
    End Sub




End Class
