Imports System.Data
Imports System.Web.HttpServerUtility

Partial Class articleXML
    Inherits WebFrontEnd


    Protected Sub articleXML_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim sqlstatement As String = String.Format("select xmldata from saxo_article where article_uid = '{0}' ", Request("article_uid").ToString)
        Dim ds As DataSet = BusinessRule.BusinessRule.BusGetDataset(sqlstatement)
        If ds.Tables(0).Rows.Count = 1 Then
            Dim mydata As String = ds.Tables(0).Rows(0).Item(0)
            Response.Write(mydata)
        End If

    End Sub
End Class
