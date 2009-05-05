﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html"/>

	<xsl:template match="/">
		<html>
			<head>
				<title>Album Art Downloader XUI Scripts</title>
			</head>
			<body>
				<h1>
					Scripts for <a href="{Updates/Application/@URI}">
						<xsl:value-of select="Updates/Application/@Name"/>
					</a>
				</h1>
				<ul>
					<xsl:for-each select="Updates/Script">
						<li>
							<a href="{/Updates/@BaseURI}{@URI}">
								<xsl:value-of select="@Name"/>
							</a>
							<em>
								<xsl:text> v</xsl:text>
								<xsl:value-of select="@Version"/>
							</em>
						</li>
					</xsl:for-each>
				</ul>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
