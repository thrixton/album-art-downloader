﻿#pragma checksum "..\..\AutoDownloadedScriptsViewer.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B193B3A16EF956F3BC4D44A24BE598F9"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AlbumArtDownloader.Controls;
using AlbumArtDownloader.Properties;
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
    /// AutoDownloadedScriptsViewer
    /// </summary>
    public partial class AutoDownloadedScriptsViewer : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 40 "..\..\AutoDownloadedScriptsViewer.xaml"
        internal System.Windows.Controls.TextBlock mLabel;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\AutoDownloadedScriptsViewer.xaml"
        internal System.Windows.Controls.Button mRestartButton;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\AutoDownloadedScriptsViewer.xaml"
        internal System.Windows.Controls.ItemsControl mDownloadedScriptViewer;
        
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
            System.Uri resourceLocater = new System.Uri("/AlbumArt;component/autodownloadedscriptsviewer.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\AutoDownloadedScriptsViewer.xaml"
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
            this.mLabel = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.mRestartButton = ((System.Windows.Controls.Button)(target));
            
            #line 49 "..\..\AutoDownloadedScriptsViewer.xaml"
            this.mRestartButton.Click += new System.Windows.RoutedEventHandler(this.OnRestartButtonClicked);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 50 "..\..\AutoDownloadedScriptsViewer.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OnCloseButtonClicked);
            
            #line default
            #line hidden
            return;
            case 4:
            this.mDownloadedScriptViewer = ((System.Windows.Controls.ItemsControl)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
