﻿#pragma checksum "..\..\..\..\Windows\Controls\LTextBox_double_Arrow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1C50B757A66BEE55492CFBB837629EC1"
//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
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
using System.Windows.Forms.Integration;
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


namespace PTL.Windows.Controls {
    
    
    /// <summary>
    /// LTextBox_double_Arrow
    /// </summary>
    public partial class LTextBox_double_Arrow : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        /// <summary>
        /// _TextBox Name Field
        /// </summary>
        
        #line 9 "..\..\..\..\Windows\Controls\LTextBox_double_Arrow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public System.Windows.Controls.TextBox _TextBox;
        
        #line default
        #line hidden
        
        /// <summary>
        /// Increase_Button Name Field
        /// </summary>
        
        #line 15 "..\..\..\..\Windows\Controls\LTextBox_double_Arrow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public System.Windows.Controls.Primitives.RepeatButton Increase_Button;
        
        #line default
        #line hidden
        
        /// <summary>
        /// Decrease_Button Name Field
        /// </summary>
        
        #line 16 "..\..\..\..\Windows\Controls\LTextBox_double_Arrow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public System.Windows.Controls.Primitives.RepeatButton Decrease_Button;
        
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
            System.Uri resourceLocater = new System.Uri("/PTL;component/windows/controls/ltextbox_double_arrow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Windows\Controls\LTextBox_double_Arrow.xaml"
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
            this._TextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.Increase_Button = ((System.Windows.Controls.Primitives.RepeatButton)(target));
            
            #line 15 "..\..\..\..\Windows\Controls\LTextBox_double_Arrow.xaml"
            this.Increase_Button.Click += new System.Windows.RoutedEventHandler(this.Button_minus_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Decrease_Button = ((System.Windows.Controls.Primitives.RepeatButton)(target));
            
            #line 16 "..\..\..\..\Windows\Controls\LTextBox_double_Arrow.xaml"
            this.Decrease_Button.Click += new System.Windows.RoutedEventHandler(this.Button_add_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

