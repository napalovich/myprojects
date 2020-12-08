<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" />
  <xsl:template match="/">
    <html>

      <head>
        <link rel="stylesheet" href="styles.css"/>
      </head>
      <body>
        <table id="car">
          <tr>

            <th>Марка</th>
            <th>Модель</th>
            <th>Рік випуску</th>
            <th>Об'єм двигуна</th>
            <th>Колір</th>
            <th>Ціна</th>
          </tr>
          <xsl:for-each select="cars/brand">
            <xsl:variable name ="nam" select="@Brand" />
            <xsl:for-each select="car">
              <tr>
                <td>
                  <xsl:value-of select="$nam"/>
                </td>
                <td>
                  <xsl:value-of select="@model"/>
                </td>
                <td>
                  <xsl:value-of select="@year"/>
                </td>
                <td>
                  <xsl:value-of select="@volume"/>
                </td>
                <td>
                  <xsl:value-of select="@color"/>
                </td>
                <td>
                  <xsl:value-of select="@price"/>
                </td>
              </tr>
            </xsl:for-each>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
