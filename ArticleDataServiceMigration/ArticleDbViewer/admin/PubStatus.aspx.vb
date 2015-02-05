Imports BusinessRule.BusinessRule
Imports System.Data

Partial Class admin_PubStatus
    Inherits WebFrontEnd


    Dim myrows As New StringBuilder
    Dim myrows2 As New StringBuilder

    Protected Sub admin_PubStatus_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            DrpPub.Items.Clear()
            Dim ds As DataSet = BusinessRule.BusinessRule.distinctPubs()
            For Each dr In ds.Tables(0).Rows
                Dim siteid As String = dr.Item(0).ToString
                Dim publicationName As String = dr.Item(1).ToString
                DrpPub.Items.Add(New ListItem(publicationName, siteid))
            Next
            DrpPub.Items.Add(New ListItem("All", "All"))
        End If
    End Sub



    Private Sub saxoStatus(ByVal siteid As String, ByVal publicationName As String)
        Dim saxostart As String = PubEarliestStartSaxo(siteid)
        '            Dim migstart As String = PubEarliestStart(siteid)
        Dim saxoend As String = PubLatestStartSaxo(siteid)
        Dim count As String = BusinessRule.BusinessRule.PubStatusSaxo(siteid.ToString)
        myrows.Append(String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td>", siteid, publicationName, saxostart, saxoend, count))
    End Sub

    Private Sub migrationStatus(ByVal siteid As String, ByVal publicationName As String)
        Dim migstart As String = PubEarliestStart(siteid)
        Dim migend As String = PubLatestStart(siteid)
        Dim count As String = BusinessRule.BusinessRule.PubStatus(siteid.ToString)
        myrows2.Append(String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", siteid, publicationName, migstart, migend, count))
    End Sub



    Protected Sub btnsubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnsubmit.Click
        Dim siteid As String = DrpPub.SelectedValue
        Dim publicationName As String = DrpPub.SelectedItem.Text
        If siteid.Equals("All") Then
            Dim ds As DataSet = BusinessRule.BusinessRule.distinctPubs()
            For Each dr In ds.Tables(0).Rows
                siteid = dr.Item(0).ToString
                publicationName = dr.Item(1).ToString
                migrationStatus(siteid, publicationName)
                saxoStatus(siteid, publicationName)
            Next
        Else
            migrationStatus(siteid, publicationName)
            saxoStatus(siteid, publicationName)
        End If
        Dim mytable1 As String = String.Format("<table border='1'><tr><td colspan='5'>Migrated From QA0 </td></tr><tr><td>siteid</td><td>pub</td><td>startdate</td><td>endDate</td></tr>{0}</table>", myrows2.ToString())
        Dim mytable2 As String = String.Format("<table border='1'><tr><td colspan='5'>Migrated to Saxotech</td></tr> <tr><td>siteid</td><td>pub</td><td>startdate</td><td>endDate</td><td>totals</td></tr>{0}</table>", myrows.ToString())
        mydata.InnerHtml = String.Format("{0} <br/> {1}", mytable1, mytable2)
    End Sub
End Class
