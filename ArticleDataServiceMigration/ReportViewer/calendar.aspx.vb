
Partial Class calendar
    Inherits System.Web.UI.Page
    Public Sub Calendar1_DayRender(ByVal sender As Object, ByVal e As DayRenderEventArgs)
        If e.Day.Date = DateTime.Now.ToString("d") Then
            e.Cell.BackColor = System.Drawing.Color.LightGray
        End If
    End Sub

    Public Sub Calendar1_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim strjscript As String = "<script language=""javascript"">"
        strjscript &= "window.opener." & _
              HttpContext.Current.Request.QueryString("formname") & ".value = '" & _
              Calendar1.SelectedDate.ToString("MM/dd/yyyy") & "';window.close();"
        strjscript = strjscript & "</script" & ">" 'Don't Ask, Tool Bug
        Literal1.Text = strjscript  'Set the literal control's text to the JScript code
    End Sub



End Class
