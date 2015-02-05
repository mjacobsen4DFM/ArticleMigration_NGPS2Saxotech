
Imports DataModels

Partial Class admin_LoadSectionAnchorMap
    Inherits WebFrontEnd

    Public Function GetPubinfo(ByVal idx As Integer) As String
        Dim data As String = ""
        Select Case idx
            Case 1
                data = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
            Case 2
                data = mytracksession.GetValue(MySession.tracksession.keycodes.publicationName)

        End Select
        Return data
    End Function



    Protected Sub BtnLoad_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnLoad.Click
        Dim myrows As New StringBuilder
        Dim adb As New ArticleDataDbEntities()
        Dim le As New livee_11Entities()
        Dim mysiteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)

        Dim sa As List(Of DataModels.sectionAnchorMap) = BusinessRuleC.BusinessRule.GetSectionAnchorMap(Convert.ToInt32(mysiteid))
        If (sa.Count > 0) Then
            myresponse.InnerHtml = String.Format("Section Anchors already loaded Count: {0} <br/> ", sa.Count)
            Exit Sub
        End If

        Dim res = (From si In le.SECTION_ITEM _
                    Join ci In le.CONTENT_ITEM On si.SECTION_ITEM_UID Equals ci.SECTION_ANCHOR_UID _
                    Where ci.SITE_UID = mysiteid AndAlso ci.CONTENT_TYPE_UID = 102 _
                    Select New With {ci.SECTION_ANCHOR_UID, si.SECTION_NAME}).Distinct().OrderBy(Function(doc) doc.SECTION_NAME)


        For Each v In res

            myrows.Append(String.Format("{0}  {1} <br/> ", v.SECTION_ANCHOR_UID, v.SECTION_NAME))

            Dim SAM As New sectionAnchorMap() With {.sectionAnchor = v.SECTION_ANCHOR_UID, .siteid = mysiteid, .sectionName = v.SECTION_NAME}
            adb.AddTosectionAnchorMaps(SAM)
        Next
        adb.SaveChanges()
        myrows.Append("<br/> Load Complete <br/>")

        myresponse.InnerHtml = myrows.ToString()

    End Sub

    Protected Sub BtnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnUpdate.Click
        Dim dirty As Boolean = False
        Dim myrows As New StringBuilder
        Dim adb As New ArticleDataDbEntities()
        Dim le As New livee_11Entities()
        Dim mysiteid As String = mytracksession.GetValue(MySession.tracksession.keycodes.siteid)
        Dim res = (From si In le.SECTION_ITEM _
            Join ci In le.CONTENT_ITEM On si.SECTION_ITEM_UID Equals ci.SECTION_ANCHOR_UID _
            Where ci.SITE_UID = mysiteid AndAlso ci.CONTENT_TYPE_UID = 102 _
            Select New With {ci.SECTION_ANCHOR_UID, si.SECTION_NAME}).Distinct().OrderBy(Function(doc) doc.SECTION_NAME)


        For Each v In res
            Dim sa As List(Of DataModels.sectionAnchorMap) = BusinessRuleC.BusinessRule.GetSectionAnchorMap(Convert.ToInt32(mysiteid), v.SECTION_ANCHOR_UID)
            If (sa.Count.Equals(0)) Then
                myrows.Append(String.Format("{0}  {1} <br/> ", v.SECTION_ANCHOR_UID, v.SECTION_NAME))
                Dim SAM As New sectionAnchorMap() With {.sectionAnchor = v.SECTION_ANCHOR_UID, .siteid = mysiteid, .sectionName = v.SECTION_NAME}
                adb.AddTosectionAnchorMaps(SAM)
                dirty = True
            End If
        Next
        If (dirty) Then adb.SaveChanges()
        myrows.Append("<br/> Update Complete <br/>")
        myresponse.InnerHtml = myrows.ToString()
    End Sub
End Class
