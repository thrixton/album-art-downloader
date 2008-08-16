using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace AlbumArtDownloader.Controls
{
	[TemplatePart(Name = "PART_ItemsPresenter", Type = typeof(ItemsPresenter))]
	[TemplatePart(Name = "PART_ArtPanel", Type = typeof(ArtPanel))]
	public class ArtPanelList : ItemsControl
	{
		/// <summary>This is the delay after an image's size changes before a re-sort and re-filter takes place.
		/// This gives further changes a chance to occur before a costly refresh is done.</summary>
		private static readonly TimeSpan sRefreshDelay = TimeSpan.FromMilliseconds(1000);

		public static class Commands
		{
			public static RoutedUICommand ToggleInformationLocation = new RoutedUICommand("ToggleInformationLocation", "ToggleInformationLocation", typeof(Commands));
			public static RoutedUICommand Preview = new RoutedUICommand("Preview", "Preview", typeof(Commands));
		}

		static ArtPanelList()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ArtPanelList), new FrameworkPropertyMetadata(typeof(ArtPanelList)));
			ItemsSourceProperty.OverrideMetadata(typeof(ArtPanelList), new FrameworkPropertyMetadata(null, new CoerceValueCallback(CoerceItemsSource)));
		}

		public ArtPanelList()
		{
			CommandBindings.Add(new CommandBinding(EditingCommands.AlignJustify, new ExecutedRoutedEventHandler(AlignJustifyCommandHandler)));
			AddHandler(ArtPanel.ImageSizeChangedEvent, new RoutedEventHandler(OnImageSizeChanged));
		}

		protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
		{
			base.OnTemplateChanged(oldTemplate, newTemplate);

			//Check if this template specifies a GroupStyle (which can't be done in Xaml directly)
			if (newTemplate != null)
			{
				GroupStyle groupStyle = newTemplate.Resources["PART_GroupStyle"] as GroupStyle;
				if (groupStyle != null)
				{
					GroupStyle.Clear();
					GroupStyle.Add(groupStyle);
				}
			}
		}

		#region Mouse shifting
		private Point mPanelResizeDragOffset;
		protected override void OnGotMouseCapture(MouseEventArgs e)
		{
			base.OnGotMouseCapture(e);

			Suspended = true;

			ArtPanel panel = GetArtPanel(e.OriginalSource);
			if (panel != null)
			{
				mPanelResizeDragOffset = panel.TranslatePoint(new Point(), this);
				PreviewMouseMove += OnPreviewMouseMoveWhileCaptured;
			}
		}

		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			base.OnLostMouseCapture(e);
			PreviewMouseMove -= OnPreviewMouseMoveWhileCaptured;

			Suspended = false;
		}

		private void OnPreviewMouseMoveWhileCaptured(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				if (Mouse.Captured != null)
					Mouse.Captured.ReleaseMouseCapture();

				OnLostMouseCapture(e);
			}
			else
			{
				//Shift the mouse with the panel being moved, so that the resize remains relative to it.
				ArtPanel panel = GetArtPanel(e.OriginalSource);
				if (panel != null)
				{
					panel.BringIntoView(); //Ensure the panel remains in view
					Point newOffset = panel.TranslatePoint(new Point(), this);
					Vector offsetDelta = newOffset - mPanelResizeDragOffset;
					if (offsetDelta.Length > 1)
					{
						MoveMousePosition(offsetDelta);
						mPanelResizeDragOffset = newOffset;
						e.Handled = true;
					}
				}
			}
		}

		private static ArtPanel GetArtPanel(object originalSource)
		{
			//Get the art panel being resized
			FrameworkElement source = originalSource as FrameworkElement;
			if (!(source is ArtPanel)) //If the source isn't the panel itself, then it must come from some control in the panels template.
				source = source.TemplatedParent as Controls.ArtPanel;

			return (ArtPanel)source;
		}

		[DllImport("user32.dll")]
		static extern bool SetCursorPos(int X, int Y);
		[DllImport("user32.dll")]
		static extern bool GetCursorPos(ref POINTAPI lpPoint);
		private struct POINTAPI
		{
			public int X;
			public int Y;
		}
		private void MoveMousePosition(Vector offsetDelta)
		{
			POINTAPI point = new POINTAPI();
			if (GetCursorPos(ref point))
			{
				point.X += (int)offsetDelta.X;
				point.Y += (int)offsetDelta.Y;
				SetCursorPos(point.X, point.Y);
			}
		}
		#endregion

		#region Auto Size panels
		private void AlignJustifyCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			AutoSizePanels();
		}

		public void AutoSizePanels()
		{
			//Auto-size the panel widths by finding the closest width that fits neatly
			if (ItemsPresenter != null)
			{
				PanelWidth = GetNearestArrangedPanelWidth(PanelWidth);
			}
		}

		private double GetNearestArrangedPanelWidth(double panelWidth)
		{
			int numberOfPanels = (int)Math.Round(ItemsPresenter.ActualWidth / panelWidth, MidpointRounding.AwayFromZero);
			double newPanelWidth = ItemsPresenter.ActualWidth / numberOfPanels;
			ContentPresenter firstItemContentPresenter = (ContentPresenter)ItemContainerGenerator.ContainerFromIndex(0);
			if (firstItemContentPresenter != null) //Will be null if there are no items shown.
			{
				ArtPanel firstArtPanel = firstItemContentPresenter.ContentTemplate.FindName("PART_ArtPanel", firstItemContentPresenter) as ArtPanel;
				if (numberOfPanels > 1 && newPanelWidth < firstArtPanel.MinWidth)
				{
					//Can't fit the panels at this size, as it is under the minimum size, so decrease the number of panels by one, and recalc.
					newPanelWidth = ItemsPresenter.ActualWidth / --numberOfPanels;
					//Should always now fit, as this is the equivalent of rounding up instead of down.
				}
			}
			return newPanelWidth;
		}
		#endregion

		#region Image Size Changed
		private void OnImageSizeChanged(object sender, RoutedEventArgs e)
		{
			IAlbumArt albumArt = ((ArtPanel)e.OriginalSource).AlbumArt; //Or should this get it from the ItemContainerGenerator?

			//As this panel's size has changed, it must be removed and re-added, so the list re-filters and re-sorts it
			if (ItemsSource is IList)
			{
				IList itemsSource = (IList)ItemsSource;
				itemsSource.Remove(albumArt);
				itemsSource.Add(albumArt);
			}
			else if (ItemsSource == null) //If there is no items source, then Items might be directly assigned
			{
				Items.Remove(albumArt);
				Items.Add(albumArt);
			}
			else
			{
				System.Diagnostics.Debug.Fail("Can't re-add the album art for re-sorting and filtering, as ItemsSource is not an IList");
			}
		}
		#endregion

		#region Properties
		public static readonly DependencyProperty UseMinimumImageSizeProperty = DependencyProperty.Register("UseMinimumImageSize", typeof(bool), typeof(ArtPanelList),
			new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnImageSizeLimitChanged)));
		/// <summary>Whether to use the Minimum size setting.</summary>		
		public bool UseMinimumImageSize
		{
			get { return (bool)GetValue(UseMinimumImageSizeProperty); }
			set { SetValue(UseMinimumImageSizeProperty, value); }
		}
		public static readonly DependencyProperty UseMaximumImageSizeProperty = DependencyProperty.Register("UseMaximumImageSize", typeof(bool), typeof(ArtPanelList),
			new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnImageSizeLimitChanged)));
		/// <summary>Whether to use the Maximum size setting.</summary>		
		public bool UseMaximumImageSize
		{
			get { return (bool)GetValue(UseMaximumImageSizeProperty); }
			set { SetValue(UseMaximumImageSizeProperty, value); }
		}
		public static readonly DependencyProperty MinimumImageSizeProperty = DependencyProperty.Register("MinimumImageSize", typeof(int), typeof(ArtPanelList),
			new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnImageSizeLimitChanged)));
		/// <summary>The minimum size of image to display. Images smaller than this will be filtered out.</summary>		
		public int MinimumImageSize
		{
			get { return (int)GetValue(MinimumImageSizeProperty); }
			set { SetValue(MinimumImageSizeProperty, value); }
		}
		public static readonly DependencyProperty MaximumImageSizeProperty = DependencyProperty.Register("MaximumImageSize", typeof(int), typeof(ArtPanelList),
			new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnImageSizeLimitChanged)));
		/// <summary>The Maximum size of image to display. Images larger than this will be filtered out.</summary>		
		public int MaximumImageSize
		{
			get { return (int)GetValue(MaximumImageSizeProperty); }
			set { SetValue(MaximumImageSizeProperty, value); }
		}
		private static void OnImageSizeLimitChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			ArtPanelList artPanelList = (ArtPanelList)sender;

			if (!artPanelList.UseMaximumImageSize && !artPanelList.UseMinimumImageSize)
			{
				artPanelList.Items.Filter = null; //No filtering required
			}
			else
			{
				artPanelList.Items.Filter = new Predicate<object>(artPanelList.SizeFilter); //Apply the new minimum image size filter
			}
		}
		private bool SizeFilter(object item)
		{
			IAlbumArt albumArt = item as IAlbumArt;
			if (albumArt == null)
				return true; //Can't filter it, don't know what it is

			if (!UseMaximumImageSize && !UseMinimumImageSize)
				return true; //No filtering required

			//Both width and height must be bigger, so use the smallest of the two
			double size = Math.Min(albumArt.ImageWidth, albumArt.ImageHeight);
			if (size == -1 || albumArt.ImageHeight == -1)
			{
				//No size has been provided, so filter on the actual thumbnail size
				size = Math.Min(albumArt.Image.Width, albumArt.Image.Height);
			}

			//Valid if there is no limit specified, or the size is within the limit. Both limits must apply if both are present
			return (!UseMinimumImageSize || size >= MinimumImageSize) &&
				   (!UseMaximumImageSize || size <= MaximumImageSize);
		}

		public static readonly DependencyProperty ThumbSizeProperty = DependencyProperty.Register("ThumbSize", typeof(double), typeof(ArtPanelList), new FrameworkPropertyMetadata(50D, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		/// <summary>The size of the thumbnail images to display</summary>		
		public double ThumbSize
		{
			get { return (double)GetValue(ThumbSizeProperty); }
			set { SetValue(ThumbSizeProperty, value); }
		}

		public static readonly DependencyProperty PanelWidthProperty = DependencyProperty.Register("PanelWidth", typeof(double), typeof(ArtPanelList), new FrameworkPropertyMetadata(150D, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, new CoerceValueCallback(CoercePanelWidth)));
		/// <summary>The width of each album art panel</summary>		
		public double PanelWidth
		{
			get { return (double)GetValue(PanelWidthProperty); }
			set { SetValue(PanelWidthProperty, value); }
		}
		private static object CoercePanelWidth(DependencyObject sender, object newValue)
		{
			if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ||
				Keyboard.GetKeyStates(Key.Left) == KeyStates.Down ||
				Keyboard.GetKeyStates(Key.Up) == KeyStates.Down ||
				Keyboard.GetKeyStates(Key.Right) == KeyStates.Down ||
				Keyboard.GetKeyStates(Key.Down) == KeyStates.Down)
			{
				//Return the value unsnapped if Shift is held down (to mean unsnapped)
				//or if any of the direction keys are pressed (which means this is being
				//adjusted via the keyboard)
				return newValue;
			}
			ArtPanelList artPanelList = (ArtPanelList)sender;
			if (artPanelList.PanelWidthSnapping > 0 && artPanelList.ItemsPresenter != null)
			{
				//Check if the panel width is close (within PanelWidthSnapping) to a neat arrangment
				double value = (double)newValue;
				double nearestArrangedPanelWidth = artPanelList.GetNearestArrangedPanelWidth(value);
				if (Math.Abs(nearestArrangedPanelWidth - value) <= artPanelList.PanelWidthSnapping)
					return nearestArrangedPanelWidth;
			}
			//Return the value un-coerced
			return newValue;
		}

		public static readonly DependencyProperty PanelWidthSnappingProperty = DependencyProperty.Register("PanelWidthSnapping", typeof(double), typeof(ArtPanelList), new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsArrange));
		/// <summary>The width of each album art panel</summary>		
		public double PanelWidthSnapping
		{
			get { return (double)GetValue(PanelWidthSnappingProperty); }
			set { SetValue(PanelWidthSnappingProperty, value); }
		}

		public static readonly DependencyProperty SortDescriptionProperty = DependencyProperty.Register("SortDescription", typeof(SortDescription), typeof(ArtPanelList),
					new FrameworkPropertyMetadata(default(SortDescription), new PropertyChangedCallback(OnSortDescriptionChanged)));
		/// <summary>The sorting to be applied to the list</summary>		
		public SortDescription SortDescription
		{
			get { return (SortDescription)GetValue(SortDescriptionProperty); }
			set { SetValue(SortDescriptionProperty, value); }
		}
		private static void OnSortDescriptionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			ArtPanelList artPanelList = (ArtPanelList)sender;
			artPanelList.Items.SortDescriptions.Clear();

			SortDescription sortDescription = (SortDescription)e.NewValue;
			if (!String.IsNullOrEmpty(sortDescription.PropertyName))
			{
				artPanelList.Items.SortDescriptions.Add(sortDescription);
			}
		}

		public static readonly DependencyProperty GroupingProperty = DependencyProperty.Register("Grouping", typeof(Grouping), typeof(ArtPanelList),
					new FrameworkPropertyMetadata(Grouping.None, new PropertyChangedCallback(OnGroupingChanged)));
		/// <summary>The grouping to be applied to the list</summary>
		public Grouping Grouping
		{
			get { return (Grouping)GetValue(GroupingProperty); }
			set { SetValue(GroupingProperty, value); }
		}

		private static void OnGroupingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			ArtPanelList artPanelList = (ArtPanelList)sender;
			artPanelList.Items.GroupDescriptions.Clear();

			switch ((Grouping)e.NewValue)
			{
				case Grouping.Local:
					//Group by whether the source is local or not
					artPanelList.Items.GroupDescriptions.Add(new LocalGroupDescription());
					break;
				case Grouping.Source:
					//Group by source name
					artPanelList.Items.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("SourceName"));
					break;
			}
		}

		public static readonly DependencyProperty InformationLocationProperty = DependencyProperty.Register("InformationLocation", typeof(InformationLocation), typeof(ArtPanelList), new FrameworkPropertyMetadata(InformationLocation.Right, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		/// <summary>Where to position the information, relative to the thumbnail</summary>
		public InformationLocation InformationLocation
		{
			get { return (InformationLocation)GetValue(InformationLocationProperty); }
			set { SetValue(InformationLocationProperty, value); }
		}

		private static object CoerceItemsSource(DependencyObject sender, object newValue)
		{
			IList itemsSource = newValue as IList;
			if (itemsSource != null && itemsSource is INotifyCollectionChanged && !(itemsSource is SuspendedNotificationCollection))
			{
				return new SuspendedNotificationCollection(itemsSource);
			}
			return newValue;
		}
		#endregion

		#region Elements
		private ItemsPresenter mCachedItemsPresenter;
		protected ItemsPresenter ItemsPresenter
		{
			get
			{
				if (mCachedItemsPresenter == null)
				{
					if (Template != null)
					{
						mCachedItemsPresenter = Template.FindName("PART_ItemsPresenter", this) as ItemsPresenter;
					}
				}

				return mCachedItemsPresenter;
			}
		}
		#endregion

		#region Suspension and resuming of modifications to the list
		//TODO: Should this do ref counting? How about an IDisposable suspension pattern?
		//Currently this is only used from Mouse Captured and Lost Capture, so isn't necessary.
		private bool Suspended
		{
			get
			{
				SuspendedNotificationCollection suspender = ItemsSource as SuspendedNotificationCollection;
				if (suspender != null)
					return suspender.Suspended;

				return false;
			}
			set
			{
				SuspendedNotificationCollection suspender = ItemsSource as SuspendedNotificationCollection;
				if (suspender != null)
					suspender.Suspended = value;
			}
		}

		/// <summary>
		/// Wrapper around an <see cref="INotifyCollectionChanged"/> that can suspend
		/// the notifications that the collection has changed, batch them up, then
		/// release them when the suspension is lifted. The collection must also implement
		/// <see cref="IList"/>.
		/// </summary>
		private class SuspendedNotificationCollection : INotifyCollectionChanged, IList
		{
			public event NotifyCollectionChangedEventHandler CollectionChanged;

			private IList mWrappedCollection;
			private bool mResetPending = false;
			private ArrayList mAddedItems = new ArrayList();
			private ArrayList mRemovedItems = new ArrayList();
			
			public SuspendedNotificationCollection(IList collection)
			{
				mWrappedCollection = collection;

				INotifyCollectionChanged notify = collection as INotifyCollectionChanged;
				if (notify == null)
					throw new ArgumentException("The collection must implement INotifyCollectionChanged", "collection");
				
				notify.CollectionChanged += new NotifyCollectionChangedEventHandler(OnBaseCollectionChanged);
			}

			private void OnBaseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				if (Suspended)
				{
					if (!mResetPending) //If we're going to reset anyway, it hardly matters what else chagnes
					{
						//Batch up the change for later use
						switch (e.Action)
						{
							case NotifyCollectionChangedAction.Add:
								foreach (object item in e.NewItems)
								{
									mAddedItems.Add(item);
									mRemovedItems.Remove(item);
								}
								break;
							case NotifyCollectionChangedAction.Remove:
								foreach (object item in e.OldItems)
								{
									mRemovedItems.Add(item);
									mAddedItems.Remove(item);
								} 
								break;
							case NotifyCollectionChangedAction.Move:
							case NotifyCollectionChangedAction.Replace:
								System.Diagnostics.Debug.Fail("Moving and replacing not supported yet");
								break;
							case NotifyCollectionChangedAction.Reset:
								mResetPending = true; //Trumps all other changes
								mAddedItems.Clear();
								mRemovedItems.Clear();
								break;
							default:
								System.Diagnostics.Debug.Fail("Unknown collection changed action");
								break;
						}
					}
				}
				else
				{
					//Pass it on immediately
					RaiseCollectionChanged(e);
				}
			}

			private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
			{
				NotifyCollectionChangedEventHandler temp = CollectionChanged;
				if (temp != null)
				{
					temp(mWrappedCollection, e);
				}
			}

			private bool mSuspended;
			public bool Suspended
			{
				get { return mSuspended; }
				set 
				{
					if (value != mSuspended)
					{
						mSuspended = value;
						if (!mSuspended)
						{
							//Raise all the enqued changes
							if (mResetPending)
							{
								RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
							}
							else
							{
								//Range actions are not currently supported by the WPF controls, so have to raise an event for each one.
								foreach (object item in mRemovedItems)
								{
									RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
								}
								foreach (object item in mRemovedItems)
								{
									RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
								}
							}
							mResetPending = false;
							mAddedItems.Clear();
							mRemovedItems.Clear();
						}
					}
				}
			}

			#region IList wrappers
			public int Add(object value) { return mWrappedCollection.Add(value); }
			public void Clear() { mWrappedCollection.Clear(); }
			public bool Contains(object value) { return mWrappedCollection.Contains(value); }
			public int IndexOf(object value) { return mWrappedCollection.IndexOf(value); }
			public void Insert(int index, object value) { mWrappedCollection.Insert(index, value); }
			public bool IsFixedSize { get { return mWrappedCollection.IsFixedSize; } }
			public bool IsReadOnly { get { return mWrappedCollection.IsReadOnly; } }
			public void Remove(object value) { mWrappedCollection.Remove(value); }
			public void RemoveAt(int index) { mWrappedCollection.RemoveAt(index); }
			public void CopyTo(Array array, int index) { mWrappedCollection.CopyTo(array, index); }
			public int Count { get { return mWrappedCollection.Count; } }
			public bool IsSynchronized { get { return mWrappedCollection.IsSynchronized; } }
			public object SyncRoot { get { return mWrappedCollection.SyncRoot; } }
			public IEnumerator GetEnumerator() { return mWrappedCollection.GetEnumerator(); }
			public object this[int index]
			{
				get
				{
					return mWrappedCollection[index];
				}
				set
				{
					mWrappedCollection[index] = value;
				}
			}
			#endregion
		}
		#endregion

		/// <summary>
		/// Gets the IAlbumArt that corresponds to the source of the a RoutedEvent that came from it.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public IAlbumArt GetSourceAlbumArt(ExecutedRoutedEventArgs e)
		{
			FrameworkElement source = e.OriginalSource as FrameworkElement;
			if (source != null && !(source is ArtPanel)) //If the source isn't the panel itself, then it must come from some control in the panels template.
				source = source.TemplatedParent as ArtPanel;

			if (source == null)
				return null; //Couldn't find an art panel that triggered this command.

			return (IAlbumArt)ItemContainerGenerator.ItemFromContainer(source.TemplatedParent);
		}
	}
}