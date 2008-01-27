﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AlbumArtDownloader.Scripts;
using System.IO;
using System.Net;
using System.Drawing.Imaging;

namespace AlbumArtDownloader
{
	internal class ScriptResult
	{
		private IScript mScript;
		private object mThumbnail;
		private string mName;
		private int mWidth, mHeight;
		private object mFullSizeImageCallbackParameter;
		private Bitmap mImage;
		private bool mImageDownloaded;

		public ScriptResult(IScript script, object thumbnail, string name, int width, int height, object fullSizeImageCallbackParameter)
		{
			mScript = script;
			mThumbnail = thumbnail;
			mName = name;
			mWidth = width;
			mHeight = height;
			mFullSizeImageCallbackParameter = fullSizeImageCallbackParameter;
		}

		private void DownloadImage()
		{
			if (!mImageDownloaded)
			{
				object fullSizeImage = null;
				try
				{
					fullSizeImage = mScript.RetrieveFullSizeImage(mFullSizeImageCallbackParameter);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.Fail(String.Format("Script {0} threw an exception while retreiving full sized image: {1}", mScript.Name, e.Message));
				}
				mImage = GetBitmap(fullSizeImage);

				if (mImage == null) //If it is null, just use the thumbnail image
				{
					mImage = GetBitmap(mThumbnail);
				}
				mWidth = mImage.Width;
				mHeight = mImage.Height;
				mImageDownloaded = true;
			}
		}

		public int GetMinImageDimension(bool forceDownload)
		{
			if (mWidth < 0 || mHeight < 0 || forceDownload)
				DownloadImage(); //Must download the image to determine the width and height;

			return Math.Min(mWidth, mHeight);
		}

		public bool Save(string pathPattern, int sequence)
		{
			DownloadImage(); //Ensure image is downloaded

			if (mImage == null)
				return false; //No image to save

			Console.WriteLine(); //New line after all the source searching

			//Find the image file format extension
			string extension;
			//Find the codec
			Guid bitmapFormatGuid = mImage.RawFormat.Guid;
			ImageCodecInfo info = ImageCodecInfo.GetImageEncoders().FirstOrDefault(i => i.FormatID == bitmapFormatGuid);
			if (info != null)
			{
				//Use the first filename extension of the codec, with *. removed from it, in lower case
				extension = info.FilenameExtension.Split(';')[0].Substring(2).ToLower();
			}
			else
			{
				System.Diagnostics.Trace.WriteLine("Could not determine image file format for: " + mName);
				//Use .bmp as a general image file format indicator
				extension = "bmp";
			}

			//Construct the file path
			string path = String.Format(pathPattern.Replace("%name%", "{0}").Replace("%source%", "{1}").Replace("%size%", "{2} x {3}").Replace("%extension%", "{4}").Replace("%sequence%", "{5}"),
										mName,
										mScript.Name,
										mWidth, mHeight,
										extension);

			//Ensure path is absolute, if relative
			path = Path.GetFullPath(path);

			try
			{
				DirectoryInfo folder = new DirectoryInfo(Path.GetDirectoryName(path));
				if (!folder.Exists)
					folder.Create();

				//Image.Save has rubbish error reporting, so detect any errors pre-emptively by creating a file
				File.Create(path, 1, FileOptions.DeleteOnClose).Close();

				mImage.Save(path); //If an exception is thrown, let it pass back up.
			}
			catch (Exception)
			{
				Console.WriteLine("Could not save image to: \"{0}\"", path);
				throw; //Let the exception bubble up to be reported as an unexpected faliure
			}

			Console.WriteLine("Saved \"{0}\" from {1} to: \"{2}\"", mName, mScript.Name, path);
			return true;
		}

		#region Copied from AlbumArtDownloader/BitmapHelpers.cs
		//TODO: Refactor this out into a common library? Or at least shared file?

		/// <summary>
		/// Synchronously downloads or converts a bitmap from an object which may be any of:
		/// <para>System.Drawing.Bitmap</para>
		/// <para>Uri</para>
		/// <para>String (containing a url)</para>
		/// <para>Stream</para>
		/// </summary>
		/// <param name="from"></param>
		/// <returns></returns>
		public static Bitmap GetBitmap(object from)
		{
			Bitmap bitmap = null;

			try
			{
				if (from is Bitmap)
				{
					bitmap = (Bitmap)from;
				}
				else
				{
					Stream stream = null;
					if (from is Stream)
					{
						stream = (Stream)from;
					}
					else
					{
						Uri uri = null;
						if (from is string)
						{
							uri = new Uri((string)from, UriKind.Absolute);
						}
						else if (from is Uri)
						{
							uri = (Uri)from;
						}
						if (uri != null)
						{
							WebRequest request = HttpWebRequest.Create(uri);
							stream = request.GetResponse().GetResponseStream();
						}
					}
					if (stream != null)
					{
						bitmap = (Bitmap)Bitmap.FromStream(stream);
					}
				}
			}
			catch (Exception)
			{
				System.Diagnostics.Trace.Write("Could not get a bitmap for: ");
				System.Diagnostics.Trace.WriteLine(from);
			}
			return bitmap;
		}
		#endregion
	}
}
