﻿#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "D84FCAC30216C2FBBD591C2F0A09DA93A0BE7AB253DFFBAC686E82FF423F8317"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ten kod został wygenerowany przez narzędzie.
//     Wersja wykonawcza:4.0.30319.42000
//
//     Zmiany w tym pliku mogą spowodować nieprawidłowe zachowanie i zostaną utracone, jeśli
//     kod zostanie ponownie wygenerowany.
// </auto-generated>
//------------------------------------------------------------------------------

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
using System.Windows.Shell;
using WarsztatV2;


namespace WarsztatV2 {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton Aktualonosci;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton Zlecenia;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton NoweZlecenie;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton DoNaprawy;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton DoOdbioru;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton HistoriaZlecen;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton Klienci;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton Pracownicy;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton Samochody;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton Czesci;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton OFirmie;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Frame RightContent;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WarsztatV2;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Aktualonosci = ((System.Windows.Controls.RadioButton)(target));
            
            #line 33 "..\..\MainWindow.xaml"
            this.Aktualonosci.Click += new System.Windows.RoutedEventHandler(this.AktualnosciClick);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Zlecenia = ((System.Windows.Controls.RadioButton)(target));
            
            #line 40 "..\..\MainWindow.xaml"
            this.Zlecenia.Click += new System.Windows.RoutedEventHandler(this.ZleceniaClick);
            
            #line default
            #line hidden
            return;
            case 3:
            this.NoweZlecenie = ((System.Windows.Controls.RadioButton)(target));
            
            #line 48 "..\..\MainWindow.xaml"
            this.NoweZlecenie.Click += new System.Windows.RoutedEventHandler(this.NoweZlecenieClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this.DoNaprawy = ((System.Windows.Controls.RadioButton)(target));
            
            #line 56 "..\..\MainWindow.xaml"
            this.DoNaprawy.Click += new System.Windows.RoutedEventHandler(this.DoNaprawyClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.DoOdbioru = ((System.Windows.Controls.RadioButton)(target));
            
            #line 64 "..\..\MainWindow.xaml"
            this.DoOdbioru.Click += new System.Windows.RoutedEventHandler(this.DoOdbioruClick);
            
            #line default
            #line hidden
            return;
            case 6:
            this.HistoriaZlecen = ((System.Windows.Controls.RadioButton)(target));
            
            #line 72 "..\..\MainWindow.xaml"
            this.HistoriaZlecen.Click += new System.Windows.RoutedEventHandler(this.HistoriaZlecenClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.Klienci = ((System.Windows.Controls.RadioButton)(target));
            
            #line 79 "..\..\MainWindow.xaml"
            this.Klienci.Click += new System.Windows.RoutedEventHandler(this.KlienciClick);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Pracownicy = ((System.Windows.Controls.RadioButton)(target));
            
            #line 86 "..\..\MainWindow.xaml"
            this.Pracownicy.Click += new System.Windows.RoutedEventHandler(this.PracownicyClick);
            
            #line default
            #line hidden
            return;
            case 9:
            this.Samochody = ((System.Windows.Controls.RadioButton)(target));
            
            #line 93 "..\..\MainWindow.xaml"
            this.Samochody.Click += new System.Windows.RoutedEventHandler(this.SamochodyClick);
            
            #line default
            #line hidden
            return;
            case 10:
            this.Czesci = ((System.Windows.Controls.RadioButton)(target));
            
            #line 100 "..\..\MainWindow.xaml"
            this.Czesci.Click += new System.Windows.RoutedEventHandler(this.CzesciClick);
            
            #line default
            #line hidden
            return;
            case 11:
            this.OFirmie = ((System.Windows.Controls.RadioButton)(target));
            
            #line 107 "..\..\MainWindow.xaml"
            this.OFirmie.Click += new System.Windows.RoutedEventHandler(this.OFirmieClick);
            
            #line default
            #line hidden
            return;
            case 12:
            this.RightContent = ((System.Windows.Controls.Frame)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

