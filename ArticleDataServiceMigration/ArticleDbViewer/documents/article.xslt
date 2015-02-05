<?xml version="1.0" encoding="Windows-1252"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:onl="http://www.saxotech.com/online"
                exclude-result-prefixes="msxsl">
  <xsl:output   method="xml" indent="yes" cdata-section-elements="onl:keyword onl:heading onl:category onl:summary onl:body onl:byline onl:seolabel onl:key onl:value onl:caption "/>

  <xsl:template match="/">
    <xsl:apply-templates select="/xmldocument" />
  </xsl:template>

  <xsl:template match="xmldocument" >

    <onl:story xmlns:onl="http://www.saxotech.com/online" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:schemaLocation="http://www.saxotech.com/online story.xsd">
      <xsl:attribute name="site">
        <xsl:value-of select="fields/site"/>
      </xsl:attribute>
      <xsl:attribute name="siteuri">
        <xsl:value-of select="fields/siteuri"/>
      </xsl:attribute>
      <xsl:attribute name="date">
        <xsl:value-of select="fields/startdate"/>
      </xsl:attribute>
      
      <onl:metadata  source="none" rating="0" comments="false" paystatus="0" priority="0" storytype="standard" multimedia="list">
        <xsl:attribute name="status">
          <xsl:value-of select="normalize-space(fields/status)"/>
        </xsl:attribute>
        <onl:created>
          <xsl:attribute name="timestamp">
            <xsl:value-of select="normalize-space(fields/timestamp)"/>
          </xsl:attribute>
        </onl:created>

        <onl:modified>
          <xsl:attribute name="timestamp">
            <xsl:value-of select="normalize-space(fields/timestamp)"/>
          </xsl:attribute>
        </onl:modified>

        <onl:design frontpage="" section="" story=""/>
        <onl:theme/>
        <onl:keywords>
          <xsl:for-each select="fields/keywords/keyword">
            <onl:keyword>
              <xsl:value-of select="normalize-space(.)" />
            </onl:keyword>
          </xsl:for-each>
        </onl:keywords>
        <onl:channels web="1"/>
        <onl:category>
          <xsl:attribute name="id">
            <xsl:value-of select="normalize-space(fields/targetCategory)"/>
          </xsl:attribute>
        </onl:category>
        <onl:seolabels>
          <onl:seolabel main="1">
          </onl:seolabel>
        </onl:seolabels>
        <onl:accesscontrol flow="0" singlesale="0"/>
        <onl:multipriorities>
          <onl:multipriority priority="0" mainprofile="true" >
            <xsl:attribute name="profileuri">
              <xsl:value-of select="normalize-space(fields/profileid)"/>
            </xsl:attribute>
          </onl:multipriority>


            <xsl:for-each select="fields/profileids/profileid">
              <onl:multipriority priority="0" mainprofile="false" >
                <xsl:attribute name="profileuri">
                  <xsl:value-of select="normalize-space(.)" />
                </xsl:attribute>
              </onl:multipriority>
            </xsl:for-each>
          <!-- for legacy profileid2 support-->
          <xsl:choose>
            <xsl:when test="string-length(fields/profileid2) != 0">
              <onl:multipriority priority="0" mainprofile="false" >
                <xsl:attribute name="profileuri">
                  <xsl:value-of select="normalize-space(fields/profileid2)"/>
                </xsl:attribute>
              </onl:multipriority>
            </xsl:when>
            <xsl:otherwise>
            </xsl:otherwise>
          </xsl:choose>
        
        
        
        </onl:multipriorities>
      </onl:metadata>
      <onl:content>
        <onl:kicker/>
        <onl:heading>
          <xsl:value-of select="normalize-space(fields/heading)" />
        </onl:heading>
        <onl:subheading/>
        <onl:netheading>
        </onl:netheading>
        <onl:summary>
          <xsl:value-of select="normalize-space(fields/summary)" />
        </onl:summary>

        <!-- Main Image -->
        <xsl:if test="string-length(fields/image)!=0">
          <onl:image>
            <xsl:attribute name="binaryuri">
              <xsl:value-of select="fields/image"/>
            </xsl:attribute>
            <onl:body>
              <xsl:value-of select="fields/imageCaption"/>
            </onl:body>
          </onl:image>
        </xsl:if>
        
        <onl:paragraphs>
          <onl:paragraph>
            <onl:body>
              <xsl:value-of select="normalize-space(fields/body)" />
            </onl:body>
          </onl:paragraph>
        </onl:paragraphs>

        <onl:byline>
          <xsl:value-of select="normalize-space(fields/byline)" />
        </onl:byline>
        <onl:location/>



        <onl:extrafields>
          <onl:extrafield>
            <onl:key>siteid</onl:key>
            <onl:value>
              <xsl:value-of select="normalize-space(fields/siteid)"/>
            </onl:value>
          </onl:extrafield>
          <onl:extrafield>
            <onl:key>origsite</onl:key>
            <onl:value>
              <xsl:value-of select="normalize-space(fields/origsite)"/>
            </onl:value>
          </onl:extrafield>
          <onl:extrafield>
            <onl:key>contentid</onl:key>
            <onl:value>
              <xsl:value-of select="normalize-space(fields/contentid)"/>
            </onl:value>
          </onl:extrafield>
          <onl:extrafield>
            <onl:key>originalcategory</onl:key>
            <onl:value>
              <xsl:value-of select="normalize-space(fields/category)"/>
            </onl:value>
          </onl:extrafield>
          <onl:extrafield>
            <onl:key>anchor</onl:key>
            <onl:value>
              <xsl:value-of select="normalize-space(fields/anchor)"/>
            </onl:value>
          </onl:extrafield>
          <onl:extrafield>
            <onl:key>seodescription</onl:key>
            <onl:value>
              <xsl:value-of select="normalize-space(fields/seodescription)"/>
            </onl:value>
          </onl:extrafield>
          <onl:extrafield>
            <onl:key>relatedarticles</onl:key>
            <onl:value>
              <xsl:value-of select="normalize-space(fields/relatedarticles)"/>
            </onl:value>
          </onl:extrafield>
          <onl:extrafield>
            <onl:key>enddate</onl:key>
            <onl:value>
              <xsl:value-of select="normalize-space(fields/enddate)"/>
            </onl:value>
          </onl:extrafield>
        </onl:extrafields>
      </onl:content>
      <onl:teaser/>
      <onl:parents/>
      
      <xsl:if test="string-length(fields/galleryuid)!=0">
        <onl:children>
        <onl:contentlink type="gallery">
          <xsl:attribute name="key">
            <xsl:value-of select="fields/galleryuid"/>
          </xsl:attribute>
          <xsl:attribute name="contenturi">
            <xsl:value-of select="fields/gallerypathuid"/>
          </xsl:attribute>
        </onl:contentlink>
      </onl:children>
      </xsl:if>


      <onl:taxonomywords>
        <xsl:for-each select="fields/taxonomywords/taxonomyword">
          <onl:taxonomyword>
            <xsl:attribute name="id">
              <xsl:value-of select="normalize-space(.)" />
            </xsl:attribute>
          </onl:taxonomyword>
        </xsl:for-each>
      </onl:taxonomywords>


      <!--<onl:taxonomywords>
        <onl:taxonomyword>
          <xsl:attribute name="id">
            <xsl:value-of select="fields/taxonomyword"/>
          </xsl:attribute>
        </onl:taxonomyword>
      </onl:taxonomywords>-->

      <onl:geopoints/>
      <onl:mediafiles/>
    </onl:story>
  </xsl:template>
</xsl:stylesheet>
