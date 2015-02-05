Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Data
Imports System
Imports System.Web


Public Class common

    Public Shared Function GetFileData(ByVal inputFilePath As String) As String
        If Not File.Exists(inputFilePath) Then Return ""
        Dim sr As New StreamReader(inputFilePath)
        Dim buffer As String = sr.ReadToEnd()
        sr.Close()
        Return buffer
    End Function

    Public Shared Function CheckCacheQueryId(ByVal id As String, ByVal query As String) As System.Data.DataSet
        Dim ts As TimeSpan   '  put in cache 8 hour expiration
        Dim key As String = String.Format("{0}Query", id)
        Dim ds As DataSet = CType(HttpContext.Current.Cache(key), System.Data.DataSet)
        If ds Is Nothing Then
            ds = BusinessRule.BusinessRule.BusGetDataset(query)
            ts = New TimeSpan(8, 0, 0)
            HttpContext.Current.Cache.Insert(key, ds, Nothing, DateTime.MaxValue, ts)
        End If
        Return ds
    End Function

    Public Shared Sub RemoveFromCacheQueryId(ByVal id As String)
        Dim key As String = String.Format("{0}Query", id)
        HttpContext.Current.Cache.Remove(key)
    End Sub


End Class
