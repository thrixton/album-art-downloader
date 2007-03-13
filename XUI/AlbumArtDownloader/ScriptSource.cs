using System;
using System.Collections.Generic;
using System.Text;
using AlbumArtDownloader.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;
using AlbumArtDownloader.Scripts;
using System.Drawing;

namespace AlbumArtDownloader
{
	internal class ScriptSource : Source
	{
		private IScript mScript;
		private string mAuthor;
		private string mVersion;
		
		private Thread mSearchThread;

		public ScriptSource(IScript script)
		{
			mScript = script;
		}

		public override string Name
		{
			get { return mScript.Name; }
		}
		public override string Author
		{
			get { return mScript.Author; }
		}
		public override string Version
		{
			get { return mScript.Version; }
		}

		protected override void SearchInternal(string artist, string album, IScriptResults results)
		{
			try
			{
				mScript.Search(artist, album, results);
			}
			catch (ThreadAbortException) { } //Script was cancelled
			catch (Exception e)
			{
				string message;
				if (e is System.Reflection.TargetInvocationException)
				{
					message = ((System.Reflection.TargetInvocationException)e).InnerException.Message;
				}
				else
				{
					message = e.Message;
				}
				System.Diagnostics.Debug.Fail(String.Format("Script {0} threw an exception while searching: {1}", mScript.Name, message));
			}

		}

		internal override Bitmap RetrieveFullSizeImage(object fullSizeCallbackParameter)
		{
			object fullSizeImage = null;
			try
			{
				fullSizeImage = mScript.RetrieveFullSizeImage(fullSizeCallbackParameter);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.Fail(String.Format("Script {0} threw an exception while retreiving full sized image: {1}", mScript.Name, e.Message));
			}
			return BitmapHelpers.GetBitmap(fullSizeImage);
		}
	}
}