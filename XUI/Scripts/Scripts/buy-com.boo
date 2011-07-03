import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class BuyDotCom(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Buy.com"
	Version as string:
		get: return "0.7"
	Author as string:
		get: return "alsaan, DRata, Alex Vallat"

	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
	  	
		searchParameter = EncodeUrl(artist + " " + album)
		  	  
		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.buy.com/sr/srajax.aspx?qu=${searchParameter}&from=2&sid=6&page=1&pv=1")
		
		//Check whether we actually got any relevant result or not
		if(searchResultsHtml.IndexOf("did not return an exact match") > -1):
			return

		//Remove "Results ... in All Categories"
		allCategories = searchResultsHtml.IndexOf(" in all categories ")
		if(allCategories > -1):
			searchResultsHtml = searchResultsHtml.Substring(0, allCategories)

		//Remove "Similar Products in General"
		similar = searchResultsHtml.IndexOf(">Similar Products in <span class=\"searchHeaderTerm\">general<")
		if(similar > -1):
			searchResultsHtml = searchResultsHtml.Substring(0, similar)
		
		//Extract all the thumbnails and the links to product pages
		itemsRegex = Regex("<a href=\"(?<productPageUrl>[^\"]*/(?<sku>[^\"]*)\\.html)\"[^>]*><img[^>]*src=\"(?<thumbnailUrl>[^\"]*)\"[^>]*title=\"(?<title>[^\"]*)\"[^>]")
		itemsMatches = itemsRegex.Matches(searchResultsHtml)
		
		//Set the estimated number of covers available (approx. 1 cover per product page)
		results.EstimatedCount = itemsMatches.Count
		
		for itemMatch as Match in itemsMatches:
  			AddThumbnail(results,itemMatch)
									
	def RetrieveFullSizeImage(fullSizeCallbackParameter): 
		return fullSizeCallbackParameter

	def AddThumbnail(results as IScriptResults, itemMatch as Match):
		
		imageSize = -1
		title = itemMatch.Groups["title"].Value
		productPageUrl = "http://www.buy.com" + itemMatch.Groups["productPageUrl"].Value
		thumbnailUrl = itemMatch.Groups["thumbnailUrl"].Value
		productNumber = itemMatch.Groups["sku"].Value
		
		if(thumbnailUrl != "http://ak.buy.com/buy_assets/v6/img/icons/imagena_prod_109.gif"):
		  
			dirName = productNumber.Substring(productNumber.Length-3)					
			fullImageUrl = ""
			
			fullImageUrl = String.Format("http://ak.buy.com/db_assets/large_images/{0}/{1}.jpg",dirName,productNumber)  
			if(ImageExists(fullImageUrl)):
				imageSize = 500
			else:  
				fullImageUrl = String.Format("http://ak.buy.com/db_assets/prod_lrg_images/{0}/{1}.jpg",dirName,productNumber)  
				if(ImageExists(fullImageUrl)):
					imageSize = 250
				else:  
					fullImageUrl = String.Format("http://ak.buy.com/db_assets/prod_images/{0}/{1}.jpg",dirName,productNumber)  
					imageSize = 125
							

		results.Add(thumbnailUrl, title, productPageUrl, imageSize, imageSize, fullImageUrl, CoverType.Front)

	

	//Checks if a image exists in the specified URL
	//Return true if it exists or false if it doesn't, or if it's a 49-byte transparent GIF
	def ImageExists(url as string):
		try:  
			if(GetContentLength(url) == 49):
				return false
			else:
				return true
		except e:
			return false

	//Gets the content-length of the content located in the specified URL
	def GetContentLength(url as string):
		request = System.Net.HttpWebRequest.Create(url) as System.Net.HttpWebRequest
		response = request.GetResponse()
		length = response.ContentLength
		response.Close()
		return length