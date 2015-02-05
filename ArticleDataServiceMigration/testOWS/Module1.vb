Imports System.Data
Imports System.IO
Imports System.Configuration
Imports BusinessRule.BusinessRule

Module Module1

    Sub Main()
        Dim _message As String = ""
        Dim rc As New OWC.rest
        Dim siteid As Integer = 577
        Dim destination_siteid As String = "zz"
        '        Dim sqlstatement As String = String.Format("select article_uid, storyurl from saxo_article where siteid = '{0}'", siteid)
        Dim sqlstatement As String = String.Format("select article_uid, storyurl from saxo_article where article_uid in ( '23925837', '23925844','23925970','23926007','23926022','23926031','23926078','23926087')")
        Dim ds As DataSet = DataAuthentication.DataAuthentication.GetDataSet(sqlstatement)
        Dim total As Integer = ds.Tables(0).Rows.Count
        Dim idx As Integer = 0
        For Each dr As DataRow In ds.Tables(0).Rows
            Dim article_uid As String = dr(0).ToString()
            Dim UpdateStoryUrl As String = dr(1).ToString()
            Dim uri As New Uri(UpdateStoryUrl)
            Dim StoryId As String = Path.GetFileName(uri.LocalPath)
            Dim saxohost As String = "http://zzedit.mn1.dc.publicus.com/apps/ows.dll/sites/" ' BusinessRule.BusinessRule.GetSaxotechHost(Convert.ToInt32(siteid))
            Dim StoryAddress As String = String.Format("{0}{1}/stories", saxohost, destination_siteid)
            StoryAddress = String.Format("{0}/{1}", StoryAddress, StoryId)
            Dim username As String = ConfigurationManager.AppSettings("saxo_username").ToString()
            Dim pwd As String = ConfigurationManager.AppSettings("saxo_pwd").ToString()
            rc.executeDelete(StoryAddress, username, pwd, _message)
            DeleteFromSaxoArticle(article_uid)
            idx += 1
            Console.WriteLine(String.Format("idx: {0}/{1} Article_uid: {2}", idx, total, article_uid))
        Next

    End Sub

End Module
