import System
import AlbumArtDownloader.Scripts
import util

/**
 * Searches for Cover using LastFM WS api.
 * Album and Cover both must be spelled correctly, otherwise no result
 * will be found.
 **/
class LastFmCover(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "LastFM Cover"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "daju"
	
	# The sizes an image node of the result document may have as size attribute.
	Sizes:
		get: return ("small","medium","large","extralarge")

	# The sizes in pixel an image with the coresponding size attribute has.
	# Thes values are not documented and hance can change / be wrong.
	SizesInPx:
		get: return (34,64,174,300)	
	
		
	
	
	def Search(artist as string, album as string, results as IScriptResults):
		if(artist!= null and album!=null):
			encodedArtist = EncodeUrl(artist)
			encodedAlbum = EncodeUrl(album)
			baseUrl = "http://ws.audioscrobbler.com/2.0/"
			apiKey = "2410a53db5c7490d0f50c100a020f359"
			url = "${baseUrl}?method=album.getinfo&api_key=${apiKey}&artist=${encodedArtist}&album=${encodedAlbum}"
			x = System.Xml.XmlDocument()
			try:
				#if album is unknown x.Load(url) will throw
				x.Load(url)
				stateNode=x.SelectSingleNode("lfm");
				if stateNode.Attributes.GetNamedItem("status").InnerText == "ok":
					resultArtist = stateNode.SelectSingleNode("album/artist").InnerText
					resultAlbum = stateNode.SelectSingleNode("album/name").InnerText
					resultInfoUrl = stateNode.SelectSingleNode("album/url").InnerText
					# Medium image used as thumbnail
					mediumImageUrl = null
					mediumImageNodes = x.SelectNodes("lfm/album/image[@size='medium']")
					if mediumImageNodes.Count == 1:
						mediumImageUrl = mediumImageNodes[0].InnerText;
					name = resultArtist+" - "+resultAlbum
					found = false
					counter = Sizes.Length-1
					# Because all images look the same, only the picture with the highest
					# size attribute will be returned
					while not found and counter>=0:
						size = Sizes[counter]
						px = SizesInPx[counter]
						counter--
						resultNodes=x.SelectNodes("lfm/album/image[@size='${size}']")
						if(resultNodes.Count>0):
							found = true
							results.EstimatedCount += resultNodes.Count
							for node in resultNodes:
								picUrl = node.InnerText
								thumbnailUrl = null
								if mediumImageUrl != null:
									thumbnailUrl = mediumImageUrl
								else:
									thumbnailUrl = picUrl
								
								results.Add(thumbnailUrl, name, resultInfoUrl, px, px, picUrl);
							
						
				else:
					#Status was not 'ok'
					results.EstimatedCount = 0
			except e:
				return				
		else:
			#both Parameter album and artist are necessary
			results.EstimatedCount = 0;
		
	
	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
	
		

