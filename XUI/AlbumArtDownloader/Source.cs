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
using System.Windows.Controls;
using System.Collections.Specialized;

namespace AlbumArtDownloader
{
	internal abstract class Source : ISource
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler SearchCompleted;

		private ObservableCollectionOfDisposables<IAlbumArt> mResults;
		private SourceSettings mSettings;
		private Control mCustomSettingsUI; 

		private Thread mSearchThread;

		public Source()
		{
			mResults = new ObservableCollectionOfDisposables<IAlbumArt>();

			mCustomSettingsUI = CreateCustomSettingsUI();
			if(mCustomSettingsUI != null)
				mCustomSettingsUI.DataContext = this;
		}

		/// <summary>
		/// Override this to create the custom settings UI control to be displayed
		/// to allow editing of custom settings for the source.
		/// </summary>
		protected virtual System.Windows.Controls.Control CreateCustomSettingsUI()
		{
			return null;
		}

		public void LoadSettings()
		{
			mSettings = GetSettings();
			LoadSettingsInternal(mSettings);
		}

		public void SaveSettings()
		{
			SaveSettingsInternal(mSettings);
		}

		private bool mSettingsChanged = false;
		/// <summary>
		/// This flag is set true whenever a setting is changed, and
		/// reset to false when a search is performed. It can then be
		/// used to determine whether or not a new search is needed to
		/// be performed for this source (if the artist and album haven't)
		/// chagned.
		/// </summary>
		public bool SettingsChanged
		{
			get
			{
				return mSettingsChanged;
			}
			protected set
			{
				mSettingsChanged = value;
			}
		}

		/// <summary>
		/// Load the SourceSettings object for this source
		/// </summary>
		protected virtual SourceSettings GetSettings()
		{
			if (String.IsNullOrEmpty(Name))
				throw new InvalidOperationException("Cannot load settings for a source with no name");

			return ((App)Application.Current).GetSourceSettings(Name);
		}

		/// <summary>
		/// Reads the values from the SourceSettings object
		/// </summary>
		/// <param name="settings"></param>
		protected virtual void LoadSettingsInternal(SourceSettings settings)
		{
			this.IsEnabled = settings.Enabled;
			this.UseMaximumResults = settings.UseMaximumResults;
			this.MaximumResults = settings.MaximumResults;
		}

		/// <summary>
		/// Saves the values to the SourceSettings object
		/// </summary>
		/// <param name="settings"></param>
		protected virtual void SaveSettingsInternal(SourceSettings settings)
		{
			settings.Enabled = this.IsEnabled;
			settings.UseMaximumResults = this.UseMaximumResults;
			settings.MaximumResults = this.MaximumResults;
		}

		#region Abstract members
		public abstract string Name {get;}
		public abstract string Author { get;}
		public abstract string Version { get;}

		/// <summary>
		/// Perform the actual internal searching operation
		/// This should not update any WPF controls, or
		/// perform any direct modification of property values.
		/// </summary>
		protected abstract void SearchInternal(string artist, string album, IScriptResults results);

		internal abstract Bitmap RetrieveFullSizeImage(object fullSizeCallbackParameter);
		#endregion

		#region Basic properties
		/// <summary>
		/// Null for no custom settings, or a control which should be displayed to allow custom
		/// settings for this source to be edited.
		/// </summary>
		public Control CustomSettingsUI
		{
			get
			{
				return mCustomSettingsUI;
			}
		}

		private bool mIsEnabled = true;
		public bool IsEnabled
		{
			get
			{
				return mIsEnabled;
			}
			set
			{
				if (mIsEnabled != value)
				{
					mIsEnabled = value;
					NotifyPropertyChanged("IsEnabled");

					if (!mIsEnabled && IsSearching)
					{
						AbortSearch();
					}
				}
			}
		}

		private bool mUseMaximumResults = true;
		public bool UseMaximumResults
		{
			get
			{
				return mUseMaximumResults;
			}
			set
			{
				if (mUseMaximumResults != value)
				{
					mUseMaximumResults = value;
					
					SettingsChanged = true;
					NotifyPropertyChanged("UseMaximumResults");
					if (UseMaximumResults && MaximumResults < EstimatedResultsCount)
						NotifyPropertyChanged("EstimatedResultsCount"); //This will be coerced to be no more than maximum results
				}
			}
		}

		private int mMaximumResults;
		public int MaximumResults
		{
			get
			{
				return mMaximumResults;
			}
			set
			{
				if (mMaximumResults != value)
				{
					mMaximumResults = value;

					SettingsChanged = true;
					NotifyPropertyChanged("MaximumResults");
					if(UseMaximumResults && MaximumResults < EstimatedResultsCount)
						NotifyPropertyChanged("EstimatedResultsCount"); //This will be coerced to be no more than maximum results
				}
			}
		}

		public ObservableCollection<IAlbumArt> Results
		{
			get { return mResults; }
		}

		private bool mIsSearching;
		public bool IsSearching
		{
			get
			{
				return mIsSearching;
			}
			private set
			{
				mIsSearching = value;
				NotifyPropertyChanged("IsSearching");
			}
		}

		private int mEstimatedResultsCount;
		public int EstimatedResultsCount
		{
			get
			{
				//If there is a maximum set, then the estimated results count should never be more than that.
				if (UseMaximumResults && MaximumResults < mEstimatedResultsCount)
					return MaximumResults;

				return mEstimatedResultsCount;
			}
			private set
			{
				mEstimatedResultsCount = value;
				NotifyPropertyChanged("EstimatedResultsCount");
			}
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler temp = PropertyChanged;
			if (temp != null)
			{
				temp(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion

		/// <summary>
		/// Begins an asynchronous search. Results are raised by <see cref="FoundAlbumArt"/> events.
		/// </summary>
		public void Search(string artist, string album)
		{
			AbortSearch(); //Abort any existing search
			mSearchThread = new Thread(new ParameterizedThreadStart(SearchWorker));
			mSearchThread.Name = String.Format("{0} search", Name);
			mSearchThread.Start(new SearchThreadParameters(Dispatcher.CurrentDispatcher, artist, album));
		}

		/// <summary>
		/// Aborts an asynchronous search, if one is running.
		/// </summary>
		public void AbortSearch()
		{
			if (mSearchThread != null)
			{
				mSearchThread.Abort();
			}
		}

		private struct SearchThreadParameters
		{
			private Dispatcher mDispatcher;
			private string mArtist;
			private string mAlbum;
			public SearchThreadParameters(Dispatcher dispatcher, string artist, string album)
			{
				mDispatcher = dispatcher;
				mArtist = artist;
				mAlbum = album;
			}
			public Dispatcher Dispatcher { get { return mDispatcher; } }
			public string Artist { get { return mArtist; } }
			public string Album { get { return mAlbum; } }
		}
		private void SearchWorker(object state)
		{
			SearchThreadParameters parameters = (SearchThreadParameters)state;

			try
			{
				parameters.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(delegate
				{
					Results.Clear();
					IsSearching = true;
				}));

				SearchInternal(parameters.Artist, parameters.Album, new ScriptResults(this, parameters.Dispatcher));
			}
			finally
			{
				parameters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(delegate
				{
					IsSearching = false;
					RaiseSearchCompleted();
				}));
				SettingsChanged = false;
			}
		}

		private void RaiseSearchCompleted()
		{
			EventHandler temp = SearchCompleted;
			if (temp != null)
			{
				temp(this, EventArgs.Empty);
			}
		}

		
		private class ScriptResults : IScriptResults
		{
			private Source mSource;
			private Dispatcher mDispatcher;

			public ScriptResults(Source source, Dispatcher dispatcher)
			{
				mSource = source;
				mDispatcher = dispatcher;
			}

			#region Redirects for obsolete members
			//This region can be copied and pasted for reuse
			public void SetCountEstimate(int count)
			{
				EstimatedCount = count;
			}
			public void AddThumb(string thumbnailUri, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
			{
				Add(thumbnailUri, name, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback);
			}
			public void AddThumb(System.IO.Stream thumbnailStream, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
			{
				Add(thumbnailStream, name, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback);
			}
			public void AddThumb(System.Drawing.Image thumbnailImage, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
			{
				Add(thumbnailImage, name, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback);
			}
			#endregion

			public int EstimatedCount
			{
				get
				{
					return mSource.EstimatedResultsCount;
				}
				set
				{
					mDispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(delegate
					{
						mSource.EstimatedResultsCount = value;
					}));
				}
			}

			public void Add(object thumbnail, string name, object fullSizeImageCallback)
			{
				Add(thumbnail, name, -1, -1, fullSizeImageCallback);
			}
			public void Add(object thumbnail, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
			{
				//TODO: does downloading the thumbnail need to be asynch?
				Bitmap thumbnailBitmap = BitmapHelpers.GetBitmap(thumbnail);

				if (thumbnailBitmap != null)
				{
					mDispatcher.Invoke(DispatcherPriority.Input, new ThreadStart(delegate
					{
						mSource.Results.Add(new AlbumArt(mSource,
							thumbnailBitmap,
							name,
							fullSizeImageWidth,
							fullSizeImageHeight,
							fullSizeImageCallback));
					}));
				}

				if (mSource.UseMaximumResults && mSource.Results.Count >= mSource.MaximumResults)
				{
					//Break out of this search
					Thread.CurrentThread.Abort();
				}
			}
		}
	}
}
