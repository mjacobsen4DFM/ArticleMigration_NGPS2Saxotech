Imports System.Data
Imports System.Text
Partial Class _Default
    Inherits System.Web.UI.Page

    Dim db As New DbCommon.dbcommon


    Private Function getprofileTree(ByVal id As String) As String
        Dim level As Integer = 1
        Dim h As New Hashtable
        Dim idx As Integer = 0
        While (level > 0)
            idx += 1
            If (idx > 10) Then
                Exit While
            End If
            Dim ds As DataSet = db.getDs(String.Format("select treelevel,parentid,fieldname from profile where id= {0}", id))
            With ds.Tables(0)
                If (.Rows.Count.Equals(1)) Then
                    level = CInt(.Rows(0).Item(0))
                    id = .Rows(0).Item(1)
                    Dim fieldname As String = .Rows(0).Item(2)
                    h.Add(level, fieldname)
                    If (level.Equals(0)) Then Exit While
                Else
                    Exit While
                End If
            End With
        End While
        idx = h.Count()
        Dim profilepath As New StringBuilder
        For n = 0 To idx - 1
            profilepath.Append(IIf(n.Equals(idx - 1), h(n).ToString(), String.Format("{0}/", h(n).ToString())))
        Next
        Return profilepath.ToString()
    End Function


    Private Sub report1()
        Dim ds As DataSet = db.getDs(String.Format("select categoryid, COUNT(*) as cnt from item where pubdate >= '{0}' and pubdate <= '{1}' and pubcode = '{2}' group by categoryId order by cnt desc", txtDate.Value, txtDate2.Value, DrpPub.SelectedValue))
        Dim mytable As New StringBuilder
        mytable.Append(String.Format("<table border=""1"">{0}", vbCrLf))
        mytable.Append(String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", "Pofileid", "# stories", "Profile tree"))

        For Each dr In ds.Tables(0).Rows
            Dim ptree As String = getprofileTree(dr(0))
            mytable.Append(String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", dr(0), dr(1), ptree))
        Next
        mytable.Append(String.Format("</table>{0}", vbCrLf))
        viewarea.InnerHtml = mytable.ToString()

    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If (IsDate(txtDate.Value) And IsDate(txtDate2.Value)) Then
            report1()
            lblmsg.Text = ""
        Else
            lblmsg.Text = "Invalid Start or End Date"
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            Dim ds As DataSet = db.getDs("select pubname,pubcode from publication")
            DrpPub.DataTextField = "pubname"
            DrpPub.DataValueField = "pubcode"
            DrpPub.DataSource = ds.Tables(0)
            DrpPub.DataBind()
        End If
    End Sub
End Class
