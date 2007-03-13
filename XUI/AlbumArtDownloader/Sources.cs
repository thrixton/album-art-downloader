using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AlbumArtDownloader.Controls;
using System.ComponentModel;
using System.Windows.Threading;

namespace AlbumArtDownloader
{
	internal class Sources : ObservableCollection<Source>
	{
		private ObservableCollection<IAlbumArt> mCombinedResults = null;

		public ObservableCollection<IAlbumArt> CombinedResults
		{
			get
			{
				if (mCombinedResults == null)
					mCombinedResults = new ObservableCollection<IAlbumArt>();
				return mCombinedResults;
			}
		}

		private bool mSettingAllEnabled = false; //Flag to prevent listening to IsEnabled changes when setting them all
		private bool? mAllEnabled;
		/// <summary>
		/// This can be set to true, to enable all sources, false, to disable them all,
		/// or null to leave them as they are. It will return true if all sources are
		/// enabled, false if they are all disabled, or null if they are mixed.
		/// </summary>
		public bool? AllEnabled
		{
			get
			{
				return mAllEnabled;
			}
			set
			{
				if (value != mAllEnabled)
				{
					if (value.HasValue)
					{
						mSettingAllEnabled = true;
						foreach (Source source in this)
						{
							source.IsEnabled = value.Value;
						}
						mSettingAllEnabled = false;
					}

					mAllEnabled = value;
					OnPropertyChanged(new PropertyChangedEventArgs("AllEnabled"));
				}
			}
		}

		protected override void InsertItem(int index, Source item)
		{
			base.InsertItem(index, item);

			HookSource(item);
		}

		protected override void RemoveItem(int index)
		{
			UnhookSource(this[index]);

			base.RemoveItem(index);
		}

		protected override void SetItem(int index, Source item)
		{
			UnhookSource(this[index]);

			base.SetItem(index, item);

			HookSource(item);
		}

		protected override void ClearItems()
		{
			foreach (Source source in this)
			{
				UnhookSource(source);
			}
			base.ClearItems();
		}

		private void HookSource(Source source)
		{
			if (Count == 1) //First item to be inserted
			{
				AllEnabled = source.IsEnabled;
			}
			else if (AllEnabled.HasValue && AllEnabled.Value != source.IsEnabled)
			{
				AllEnabled = null;
			}

			source.PropertyChanged += new PropertyChangedEventHandler(OnSourcePropertyChanged);
			source.Results.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSourceResultsChanged);

			foreach (IAlbumArt albumArt in source.Results)
			{
				CombinedResults.Add(albumArt);
			}
		}

		private void UnhookSource(Source source)
		{
			if (!AllEnabled.HasValue)
			{
				//It's possible that all other sources have the same value now
				RecalculateAllEnabled();
			}

			source.PropertyChanged -= new PropertyChangedEventHandler(OnSourcePropertyChanged);
			source.Results.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnSourceResultsChanged);

			foreach (IAlbumArt albumArt in source.Results)
			{
				CombinedResults.Remove(albumArt);
			}
		}

		private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Results")
			{
				//Recalc all, as we can't find the old value
				RecreateCombinedResults();
			}
			else if (e.PropertyName == "IsEnabled" && !mSettingAllEnabled)
			{
				RecalculateAllEnabled();
			}
		}

		private void OnSourceResultsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add ||
				e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (IAlbumArt albumArt in e.NewItems)
				{
					TryAddResultToCombinedResults(albumArt);
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove ||
				e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (IAlbumArt albumArt in e.OldItems)
				{
					//TODO: Does this need a TryRemoveResultFromCombinedResults helper?
					CombinedResults.Remove(albumArt);
				}
			}
			
			//No change needed for move

			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				//For reset, there is no way of knowing that the items removed are,
				//so recreate the whole thing
				RecreateCombinedResults();
			}
		}

		private delegate void TryAddResultToCombinedResultsDelegate(IAlbumArt albumArt);
		private void TryAddResultToCombinedResults(IAlbumArt albumArt)
		{
			//Perform the add asynchronously, in case it fails due to the CombinedResults having a deferred update
			try
			{
				CombinedResults.Add(albumArt);
				return; //Successfully added.
			}
			catch (InvalidOperationException)
			{
				//This will occur if the collection has a view on it that is defer refreshed.
				
				//Try to do it again, as background
				Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new TryAddResultToCombinedResultsDelegate(TryAddResultToCombinedResults), albumArt);
			}
		}

		private void RecreateCombinedResults()
		{
			CombinedResults.Clear();
			foreach (Source source in this)
			{
				foreach (IAlbumArt albumArt in source.Results)
				{
					CombinedResults.Add(albumArt);
				}
			}
		}

		private void RecalculateAllEnabled()
		{
			bool first = true;
			bool? allEnabled = null; //Avoid switching the real AllEnabled on and off until calculation is complete
			foreach (Source source in this)
			{
				if (first)
				{
					allEnabled = source.IsEnabled;
					first = false;
				}
				else if(allEnabled.Value != source.IsEnabled)
				{
					allEnabled = null;
					break;
				}
			}
			AllEnabled = allEnabled;
		}
	}
}