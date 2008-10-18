namespace CoverSources
import System
import System.Text
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class CoverIsland:
	static SourceName as string:
		get: return "CoverIsland"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as string:
		get: return "0.5"
	static def GetThumbs(coverart,artist,album):
		if not String.IsNullOrEmpty(artist):
			firstLetter = artist[0]
		elif not String.IsNullOrEmpty(album):
			firstLetter = album[0]
		else:
			return //Nothing to search for
		
		//Get results (trying each page in turn)
		sequence = 1		
		while sequence < 100: //Sanity check, don't expect there to be more than 100 pages per letter
			try:
				sequenceChar = ""
				if(sequence > 1):
					sequenceChar = sequence.ToString()
				
				listPage = GetPage(String.Format("http://www.coverisland.com/copertine/Audio/{0}{1}.asp", firstLetter, sequenceChar))
			except e as System.Net.WebException: //Catch the 404 and break out of the loop - no more pages left
				return //No results found
			//Check if results are found here
			resultsRegex = Regex(String.Format("<option value=\"(?<value>[^\"]+)\">(?<title>[^<]*{0}[^<]+{1}[^<]*)(?=<)", artist.Replace(' ','_'), album.Replace(' ','_')), RegexOptions.Multiline | RegexOptions.IgnoreCase)
			resultMatches = resultsRegex.Matches(listPage)
			if resultMatches.Count > 0:
				break //Found some results
			//Otherwise, go round and try the next page	
			sequence++
		
		coverart.SetCountEstimate(resultMatches.Count)
		
		for resultMatch as Match in resultMatches:
			matchTitle = resultMatch.Groups["title"].Value
			value = resultMatch.Groups["value"].Value
			if value.StartsWith("-"):
				type = int.Parse(value.Substring(1, 3))
				segno = 1
				title = value.Substring(3)
			else:
				type = int.Parse(value.Substring(0, 2))
				segno = 0
				title = value.Substring(2)
			
			imageTypes = List(6)
			if type & 0x1 == 0x1:
				imageTypes.Add("front")
			if type & 0x2 == 0x2:
				imageTypes.Add("back")
			if type & 0x4 == 0x4:
				imageTypes.Add("inside")
			if type & 0x8 == 0x8:
				imageTypes.Add("inlay")
			if type & 0x10 == 0x10:
				imageTypes.Add("cd")
			if type & 0x20 == 0x20:
				imageTypes.Add("cd2")
		
			for typeName in imageTypes:
				imageResult = Post("http://www.coverisland.com/copertine/down.asp", String.Format("tipologia=Audio&title={0}&type=-{1}&segno={2}", title, typeName, segno))
				imageRegex = Regex("'(?<image>http\\://www\\.coverforum\\.net/view\\.php\\?[^']+)'", RegexOptions.Multiline)
				imageMatches = imageRegex.Matches(imageResult)
				for imageMatch as Match in imageMatches: //Only expecting one, really.
					url = imageMatch.Groups["image"].Value
					request = System.Net.HttpWebRequest.Create(url)
					response = request.GetResponse()
					if response.ContentType.StartsWith("image/"):
						coverart.Add(
							response.GetResponseStream(), 
							String.Format("{0} - {1}", matchTitle.Trim(), typeName),
							null,
							string2coverType(typeName)
							)

	static def Post(url as String, content as String):
		request = System.Net.HttpWebRequest.Create(url)
		request.Method="POST"
		request.ContentType = "application/x-www-form-urlencoded"
		bytes = System.Text.UTF8Encoding().GetBytes(content)
		request.ContentLength = bytes.Length
		stream = request.GetRequestStream()
		stream.Write(bytes,0,bytes.Length)
		stream.Close()
		streamresponse = request.GetResponse().GetResponseStream()
		return System.IO.StreamReader(streamresponse).ReadToEnd()
			
	static def GetResult(param):
		return param
		
	static def string2coverType(typeString as string):
		if(typeString.StartsWith("front")):
			return CoverType.Front;
		elif(typeString.StartsWith("back")):
			return CoverType.Back;
		elif(typeString.StartsWith("inlay")):
			return CoverType.Inlay;
		elif(typeString.StartsWith("cd")):
			return CoverType.CD;
		elif(typeString.StartsWith("inside")):
			return CoverType.Inlay;
		else:
			return CoverType.Unknown;
