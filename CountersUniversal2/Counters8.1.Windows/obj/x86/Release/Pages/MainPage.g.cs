﻿

#pragma checksum "C:\Users\khmelenko\Desktop\Новая папка\CountersUniversal2\Counters8.1.Windows\Pages\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6736896254ED010292A7DB7FB2924363"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Counters
{
    partial class MainPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 11 "..\..\..\Pages\MainPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Page_Loaded;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 21 "..\..\..\Pages\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.grdData_Tapped;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 99 "..\..\..\Pages\MainPage.xaml"
                ((global::MyToolkit.Paging.MtFrame)(target)).Navigated += this.frameContent_Navigated;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 101 "..\..\..\Pages\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnBack_Click;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 84 "..\..\..\Pages\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.btnBuy_Click;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 63 "..\..\..\Pages\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.btnFlats_Tapped;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


