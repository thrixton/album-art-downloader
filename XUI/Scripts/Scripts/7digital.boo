import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class sevendigital(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "7digital"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://7digital.com/search/products?searchDisplay=albums&page=1&search=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("<a href=\"(?<info>[^\"]+)\"(?:.(?!</a))+<img src=\"(?<image>http://cdn.7static.com/static/img/sleeveart/[^_]+)_50.jpg\" alt=\"(?<title>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			image = match.Groups["image"].Value;
			fullSize as string;
			size as int;

			//Detect if 800x800 size is available
			if CheckResponse(image, "800"):
				fullSize = image + "_800.jpg"
				size = 800;
			elif CheckResponse(image, "500"):
				//fall back on 500x500
				fullSize = image + "_500.jpg"
				size = 500;
			else:
				//fall back on 350x350 image
				fullSize = image + "_350.jpg"
				size = 350;

			results.Add(image + "_50.jpg", System.Web.HttpUtility.HtmlDecode(match.Groups["title"].Value), "http://7digital.com" + match.Groups["info"].Value, size, size, fullSize, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;

	def CheckResponse(image, size):
		checkRequest = System.Net.HttpWebRequest.Create(image + "_" + size + ".jpg") as System.Net.HttpWebRequest
		checkRequest.Method = "HEAD"
		checkRequest.AllowAutoRedirect = false
		try:
			response = checkRequest.GetResponse() as System.Net.HttpWebResponse
			return response.StatusCode == System.Net.HttpStatusCode.OK
		except e as System.Net.WebException:
			return false;
		ensure:
			if response != null:
				response.Close()
		