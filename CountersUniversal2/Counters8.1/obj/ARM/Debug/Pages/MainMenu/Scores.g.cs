﻿

#pragma checksum "C:\Users\khmelenko\Desktop\Новая папка\CountersUniversal2\Counters8.1\Pages\MainMenu\Scores.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E3283AA27D585192018E90C58140FED9"
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
    partial class Scores : global::MyToolkit.Paging.MtPage, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 16 "..\..\..\..\Pages\MainMenu\Scores.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.grdData_Tapped;
                 #line default
                 #line hidden
                #line 16 "..\..\..\..\Pages\MainMenu\Scores.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Holding += this.grdData_Holding;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 22 "..\..\..\..\Pages\MainMenu\Scores.xaml"
                ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target)).Click += this.btnDelete_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 23 "..\..\..\..\Pages\MainMenu\Scores.xaml"
                ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target)).Click += this.btnSendByMail_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 24 "..\..\..\..\Pages\MainMenu\Scores.xaml"
                ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target)).Click += this.btnSendBySMS_Click;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 36 "..\..\..\..\Pages\MainMenu\Scores.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnAdd_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


