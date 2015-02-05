
Public Class common

    Public Shared DataMine As New DbCommon.DataMineDb




    Public Shared Sub RemoveFromCacheQueryId(ByVal id As String)
        Dim DataSetKey As String = String.Format("{0}Query", id)
        HttpContext.Current.Cache.Remove(id)
        HttpContext.Current.Cache.Remove(DataSetKey)
    End Sub


    Public Shared Function CheckCacheQueryId(ByVal id As String, ByVal query As String) As System.Data.DataSet
        Dim ds As System.Data.DataSet
        Dim QueryId As String = String.Format("{0}Query", id)
        ds = CType(HttpContext.Current.Cache(QueryId), System.Data.DataSet)
        If ds Is Nothing Then
            ds = DataMine.GetDs(query)
            Dim ts As TimeSpan   '  put in cache 8 hour expiration
            ts = New TimeSpan(8, 0, 0)
            HttpContext.Current.Cache.Insert(QueryId, ds, Nothing, DateTime.MaxValue, ts)
        End If
        Return ds
    End Function






    Public Shared Function MyCategories(ByVal siteid As String) As System.Data.DataSet
        Dim ds As System.Data.DataSet
        Dim query As String = String.Format("select distinct category from article where siteid = '{0}'", siteid)
        ds = DataMine.GetDs(query)
        Return ds
    End Function


    Public Shared Function MySites() As System.Data.DataSet
        Dim ds As System.Data.DataSet
        Dim query As String = "select siteid, pubname,destination_siteid from saxo_pubMap"
        ds = DataMine.GetDs(query)
        Return ds
    End Function

    Public Shared Function GetDestinationSiteid(ByVal siteid As String) As String
        Dim ds As System.Data.DataSet
        Dim query As String = String.Format("select destination_siteid from saxo_pubMap where siteid = {0}", siteid)
        ds = DataMine.GetDs(query)
        Return ds.Tables(0).Rows(0).Item(0).ToString()
    End Function



    Public Shared Function GetArticleViewURI(ByVal destination_siteid As String, ByVal article_uid As String) As String
        Dim viewuri As String = ""
        Dim ds As System.Data.DataSet
        Dim query As String = String.Format("select viewuri from saxo_article where destination_siteid = '{0}' and article_uid = '{1}' ", destination_siteid, article_uid)
        ds = DataMine.GetDs(query)
        If ds.Tables(0).Rows.Count = 1 Then viewuri = ds.Tables(0).Rows(0).Item(0).ToString()
        Return viewuri
    End Function



    Public Shared Function GetImagePaths(ByVal article_uid As String) As System.Data.DataSet
        Dim ds As System.Data.DataSet
        Dim query As String = String.Format("select imagepath, caption, asset_uid from Image where asset_uid in ( select asset_uid from asset where article_uid = '{0}' and asset_type = 108 )", article_uid)
        ds = DataMine.GetDs(query)

        Return ds
    End Function


    Public Shared Function softDelete(ByVal asset_uid As String, ByVal siteid As String) As Boolean
        Dim ds As System.Data.DataSet
        Dim status As Boolean = False
        Dim query As String = String.Format("select soft_delete from image_tracker where asset_uid = '{0}' and siteid = '{1}' ", asset_uid, siteid)
        ds = DataMine.GetDs(query)
        If (ds.Tables(0).Rows.Count = 1) Then status = True
        Return status
    End Function


    Public Shared Function SmugLookUp(ByVal article_uid As String, ByRef albumid As String, ByRef albumkey As String) As Boolean
        Dim ds As System.Data.DataSet
        Dim query As String = String.Format("select albumid,albumkey from smug_gallery where article_uid = '{0}'", article_uid)
        ds = DataMine.GetDs(query)
        If (ds.Tables(0).Rows.Count = 1) Then
            albumid = ds.Tables(0).Rows(0).Item(0).ToString()
            albumkey = ds.Tables(0).Rows(0).Item(1).ToString()
            Return True
        End If
        Return False
    End Function

    Public Shared Function SmugLookUpTracker(ByVal article_uid As String, ByRef image_uid As String) As Boolean
        Dim ds As System.Data.DataSet
        Dim query As String = String.Format("select image_uid from smug_gallery_tracker where article_uid = '{0}' and image_uid = '{1}'", article_uid, image_uid)
        ds = DataMine.GetDs(query)
        If (ds.Tables(0).Rows.Count = 1) Then
            Return True
        End If
        Return False
    End Function



End Class
