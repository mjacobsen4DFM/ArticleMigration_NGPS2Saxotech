
Partial Class calendar
    Inherits System.Web.UI.Page


    Public Sub Calendar1_DayRender(ByVal sender As Object, ByVal e As DayRenderEventArgs)
        Dim ctlid As String = HttpContext.Current.Request.QueryString("ctlid")
        Dim myscript As String = String.Format("javascript:passDateValue('{0}','{1}')", ctlid, e.Day.Date.ToString("yyyy-MM-dd"))
        e.Cell.Text = String.Format("<a href=""{0}"" >{1}</a>", myscript, e.Day.Date.Day.ToString())
    End Sub

End Class
