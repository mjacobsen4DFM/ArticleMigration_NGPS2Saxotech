<?xml version="1.0" encoding="Windows-1252"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:onl="http://www.saxotech.com/online"
                exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"   cdata-section-elements="onl:caption onl:category" /><xsl:template match="/">
    <xsl:apply-templates select="/xmldocument" />
  </xsl:template>

  
  <!-- Post up images -->
  
  <xsl:template match="xmldocument" >
    <onl:gallery xmlns:onl="http://www.saxotech.com/online" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:schemaLocation="http://www.saxotech.com/online gallery.xsd"   >
      <xsl:attribute name="site">
        <xsl:value-of select="fields/site"/>
      </xsl:attribute>
      <xsl:attribute name="siteuri">
        <xsl:value-of select="fields/siteuri"/>
      </xsl:attribute>
      <onl:metadata shared="0" comments="0"  key="" status="published">
        <xsl:attribute name="date">
          <xsl:value-of select="fields/startdate"/>
        </xsl:attribute>
        <onl:category>
          <xsl:attribute name="id">
            <xsl:value-of select="fields/targetCategory"/>
          </xsl:attribute>
        </onl:category>
        <onl:ranking totalvotes="0"/>
        <onl:multipriorities>
          <onl:multipriority priority="0" mainprofile="true" >
            <xsl:attribute name="profileuri">
              <xsl:value-of select="normalize-space(fields/profileid)"/>
            </xsl:attribute>
          </onl:multipriority>
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
      <onl:images>

        <xsl:for-each select="fields/images/image">
          <onl:image>
            <xsl:attribute name="binaryuri">
              <xsl:value-of select="location"/>
            </xsl:attribute>
            <onl:metadata status="published">
              <xsl:attribute name="itemnumber">
                <xsl:value-of select="itemnumber"/>
              </xsl:attribute>
            </onl:metadata>
            <onl:content>
              <!--<onl:title>
                <![CDATA[]]>
              </onl:title>
              <onl:credits>
                <![CDATA[]]>
              </onl:credits>-->
              <onl:caption>
                <xsl:value-of select="caption"/>
              </onl:caption>
            </onl:content>
          </onl:image>
      </xsl:for-each>
      </onl:images>
    </onl:content>
    </onl:gallery>

  </xsl:template>
</xsl:stylesheet>
