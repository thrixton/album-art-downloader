﻿#pragma checksum "..\..\FileBrowser.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9342F3B1EA253A82AFFE6FD699154798"
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
    /// FileBrowser
    /// </summary>
    public partial class FileBrowser : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 58 "..\..\FileBrowser.xaml"
        internal System.Windows.Controls.TextBox mFilePathBox;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\FileBrowser.xaml"
        internal System.Windows.Controls.Button mBrowse;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\FileBrowser.xaml"
        internal System.Windows.Controls.Button mSearch;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\FileBrowser.xaml"
        internal System.Windows.Controls.CheckBox mIncludeSubfolders;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\FileBrowser.xaml"
        internal System.Windows.Controls.RadioButton mUseFilePathPattern;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\FileBrowser.xaml"
        internal AlbumArtDownloader.Controls.ArtPathPatternBox mFilePathPattern;
        
        #line default
        #line hidden
        
        
        #line 118 "..\..\FileBrowser.xaml"
        internal AlbumArtDownloader.Controls.ArtPathPatternBox mImagePathPatternBox;
        
        #line default
        #line hidden
        
        
        #line 151 "..\..\FileBrowser.xaml"
        internal AlbumArtDownloader.BrowserResults mResults;
        
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
            System.Uri resourceLocater = new System.Uri("/AlbumArt;component/filebrowser.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\FileBrowser.xaml"
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
            this.mFilePathBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.mBrowse = ((System.Windows.Controls.Button)(target));
            return;
            case 3:
            this.mSearch = ((System.Windows.Controls.Button)(target));
            return;
            case 4:
            this.mIncludeSubfolders = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 5:
            this.mUseFilePathPattern = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 6:
            this.mFilePathPattern = ((AlbumArtDownloader.Controls.ArtPathPatternBox)(target));
            return;
            case 7:
            this.mImagePathPatternBox = ((AlbumArtDownloader.Controls.ArtPathPatternBox)(target));
            return;
            case 8:
            this.mResults = ((AlbumArtDownloader.BrowserResults)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
