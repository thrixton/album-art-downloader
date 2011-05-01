﻿#pragma checksum "..\..\ArtSearchWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C410BBC8114FC2400A3137B88248C078"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AlbumArtDownloader;
using AlbumArtDownloader.Controls;
using AlbumArtDownloader.Properties;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace AlbumArtDownloader {
    
    
    /// <summary>
    /// ArtSearchWindow
    /// </summary>
    public partial class ArtSearchWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\ArtSearchWindow.xaml"
        internal AlbumArtDownloader.ArtSearchWindow @this;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\ArtSearchWindow.xaml"
        internal System.Windows.Controls.TextBox mArtist;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\ArtSearchWindow.xaml"
        internal System.Windows.Controls.TextBox mAlbum;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\ArtSearchWindow.xaml"
        internal System.Windows.Controls.Button mSearch;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\ArtSearchWindow.xaml"
        internal System.Windows.Controls.Expander mOptionsBox;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\ArtSearchWindow.xaml"
        internal System.Windows.Controls.Grid mNormalSaveFolderControls;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\ArtSearchWindow.xaml"
        internal AlbumArtDownloader.Controls.ArtPathPatternBox mDefaultSaveFolder;
        
        #line default
        #line hidden
        
        
        #line 134 "..\..\ArtSearchWindow.xaml"
        internal System.Windows.Controls.StackPanel mReadOnlySaveFolderControls;
        
        #line default
        #line hidden
        
        
        #line 262 "..\..\ArtSearchWindow.xaml"
        internal System.Windows.Controls.ComboBox mGroupingPicker;
        
        #line default
        #line hidden
        
        
        #line 272 "..\..\ArtSearchWindow.xaml"
        internal AlbumArtDownloader.Controls.ArtPanelListSortPicker mSortPicker;
        
        #line default
        #line hidden
        
        
        #line 278 "..\..\ArtSearchWindow.xaml"
        internal AlbumArtDownloader.Controls.ArtPanelList mResultsViewer;
        
        #line default
        #line hidden
        
        
        #line 323 "..\..\ArtSearchWindow.xaml"
        internal System.Windows.Controls.CheckBox mSelectAll;
        
        #line default
        #line hidden
        
        
        #line 327 "..\..\ArtSearchWindow.xaml"
        internal System.Windows.Controls.ItemsControl mSourcesViewer;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/AlbumArt;component/artsearchwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ArtSearchWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.@this = ((AlbumArtDownloader.ArtSearchWindow)(target));
            return;
            case 2:
            this.mArtist = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.mAlbum = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.mSearch = ((System.Windows.Controls.Button)(target));
            return;
            case 5:
            this.mOptionsBox = ((System.Windows.Controls.Expander)(target));
            return;
            case 6:
            this.mNormalSaveFolderControls = ((System.Windows.Controls.Grid)(target));
            return;
            case 7:
            this.mDefaultSaveFolder = ((AlbumArtDownloader.Controls.ArtPathPatternBox)(target));
            return;
            case 8:
            this.mReadOnlySaveFolderControls = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 9:
            
            #line 165 "..\..\ArtSearchWindow.xaml"
            ((System.Windows.Controls.GroupBox)(target)).AddHandler(System.Windows.Controls.Primitives.ToggleButton.CheckedEvent, new System.Windows.RoutedEventHandler(this.OnAutoDownloadFullSizeImagesChanged));
            
            #line default
            #line hidden
            return;
            case 10:
            this.mGroupingPicker = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 11:
            this.mSortPicker = ((AlbumArtDownloader.Controls.ArtPanelListSortPicker)(target));
            return;
            case 12:
            this.mResultsViewer = ((AlbumArtDownloader.Controls.ArtPanelList)(target));
            return;
            case 13:
            this.mSelectAll = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 14:
            this.mSourcesViewer = ((System.Windows.Controls.ItemsControl)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
