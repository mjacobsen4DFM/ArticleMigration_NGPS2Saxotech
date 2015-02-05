Imports BusinessRule.BusinessRule
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Configuration
Imports DataModels

Module Module1

    Sub Main()

        Dim publicationTable As String = "LADN404"
        Dim rc As New OWC.rest

        Dim username As String = ConfigurationManager.AppSettings("saxo_username").ToString()
        Dim pwd As String = ConfigurationManager.AppSettings("saxo_pwd").ToString()
        Dim stepnumber As String = ConfigurationManager.AppSettings("step").ToString()

        Dim idx As Integer = 0, total As Integer = 0, errors As Integer = 0, notparseable As Integer = 0

        Dim regex As Regex = New Regex("ci_\d{8}")

        If (stepnumber.Equals("1")) Then

            Dim sqlstatement As String = String.Format("select f2 from {0} ", publicationTable)

            Dim ds As DataSet = GetDs(sqlstatement)
            For Each dr As DataRow In ds.Tables(0).Rows
                Dim httpAddress As String = dr(0).ToString
                Dim match As Match = regex.Match(httpAddress)
                Dim startdate As String = "N/A", viewuri As String = "N/A", mig_error As String = ""

                If match.Success Then
                    Dim adid As String = match.Value.Replace("ci_", "")
                    Dim _message As String = ""
                    Dim bytestoryxml As Byte() = rc.getFile(New Uri(httpAddress), username, pwd, _message)
                    If _message.Length > 0 Then
                        Dim rowsaffected As Integer = CreateCommand(String.Format("update {0} set failed = 'Y', article_uid = '{1}' where f2 like '%{2}%' ", publicationTable, adid, httpAddress))
                        Console.WriteLine(String.Format("uid: {0} updatedrows {1}  ", adid, rowsaffected))
                        errors += 1
                    End If
                Else
                    notparseable += 1
                End If
                total += 1
                Console.WriteLine(String.Format("Errors: {0}  NotParseable: {1}  Totals: {2}", errors, notparseable, total))
            Next
        End If

        If (stepnumber.Equals("2")) Then
            Dim InArticleTable As Integer = 0
            Dim sqlstatement As String = String.Format("select article_uid from {0} where failed = 'Y' ", publicationTable)
            Dim ds As DataSet = GetDs(sqlstatement)
            For Each dr As DataRow In ds.Tables(0).Rows
                Dim article_uid As String = dr(0).ToString
                sqlstatement = String.Format("select startdate from article where article_uid = '{0}'", article_uid)
                Console.WriteLine(sqlstatement)

                Dim ds2 As DataSet = GetDs(sqlstatement)
                Dim ans As String = "N"
                If (ds2.Tables(0).Rows.Count.Equals(1)) Then ans = "Y"
                sqlstatement = String.Format("update {0} set InArticleTable = '{1}' where article_uid = '{2}' ", publicationTable, ans, article_uid)
                Console.WriteLine(sqlstatement)
                Dim rowsaffected As Integer = CreateCommand(sqlstatement)
                Console.WriteLine("UPDATE COMPLETE ROWSAFFECTED {0}", rowsaffected)

                If (ans.Equals("Y")) Then InArticleTable += 1
                total += 1
            Next
            Console.WriteLine(String.Format("InArticleTable: {0}  out of Total Processed: {1}  ", InArticleTable, total))

        End If
        If (stepnumber.Equals("3")) Then

            Dim sqlstatement As String = String.Format("select article_uid from {0} where failed = 'Y' and InArticleTable = 'N'", publicationTable)
            Dim ds As DataSet = GetDs(sqlstatement)
            For Each dr As DataRow In ds.Tables(0).Rows
                Dim article_uid As String = dr(0).ToString
                Dim database As New livee_11Entities()
                Dim InnerContentItems = From CONTENT_ITEM In database.CONTENT_ITEM
                                        Where CONTENT_ITEM.CONTENT_ITEM_UID.Equals(article_uid)
                                        Select CONTENT_ITEM

                Dim ContentItemRows As List(Of CONTENT_ITEM) = InnerContentItems.ToList()
                For Each ContentItemRow As CONTENT_ITEM In ContentItemRows
                    article_uid = ContentItemRow.CONTENT_ITEM_UID
                    Dim NumberOfImages As Integer = 0
                    Dim articleExists As Boolean = BusinessRule.BusinessRule.DoesArticleExist(article_uid)
                    If Not (articleExists) Then
                        sqlstatement = String.Format("update {0} set siteid = '{1}', startdate = '{2}'  where article_uid = '{3}' ", publicationTable, ContentItemRow.SITE_UID, ContentItemRow.START_DATE, article_uid)
                        Console.WriteLine(sqlstatement)
                        Dim rowsaffected As Integer = CreateCommand(sqlstatement)

                    End If
                Next

            Next


        End If



        'sqlstatement = String.Format("select startdate from article where article_uid = '{0}'", adid)
        'Dim ds2 As DataSet = GetDs(sqlstatement)
        'If (ds2.Tables(0).Rows.Count > 0) Then startdate = ds2.Tables(0).Rows(0).Item(0)
        'sqlstatement = String.Format("select viewuri, storyurl from saxo_article where article_uid = '{0}'", adid)
        'Dim ds3 As DataSet = GetDs(sqlstatement)
        'If (ds3.Tables(0).Rows.Count > 0) Then
        '    viewuri = ds3.Tables(0).Rows(0).Item(0)
        '    Dim storyurl As String = ds3.Tables(0).Rows(0).Item(1)
        '    idx += 1

        '    'Dim ping = New System.Net.NetworkInformation.Ping()
        '    'Dim result = ping.Send(httpAddress)
        '    'If result.Status <> System.Net.NetworkInformation.IPStatus.Success Then
        '    '    sw2.WriteLine(String.Format("{0} ci: {1}  viewuri: {2} GetFile: {3}", httpAddress, adid, viewuri, _message))
        '    'Else
        '    '    sw.WriteLine(String.Format("{0} ci: {1} viewuri: {2} storyurl: {3}", httpAddress, adid, viewuri, storyurl))
        '    'End If
        'Else
        '    Dim rowsaffected As Integer = CreateCommand(String.Format("update {0} set failed = 'Y', article_uid = {1} where httpaddress like '%{2}%' ", publicationTable, adid, httpAddress))
        '    Console.WriteLine(String.Format("uid: {0} updatedrows {1}  ", adid, rowsaffected))

        '    'sqlstatement = String.Format("select count(*) from migrationerror where article_uid = '{0}'", adid)
        '    'Dim ds4 As DataSet = GetDs(sqlstatement)
        '    'If (ds4.Tables(0).Rows.Count > 0) Then mig_error = IIf(ds4.Tables(0).Rows(0).Item(0) = 0, "N", "Y")
        '    'sw2.WriteLine(String.Format("{0} ci: {1} Migration Error: {2}", httpAddress, adid, mig_error))
        'End If
        '        sw.WriteLine("Found: {0}  Not Parseable: {1}  total: {2} ", idx, notparseable, total)
        'sw.Close()
        'sw2.Close()
    End Sub

End Module
