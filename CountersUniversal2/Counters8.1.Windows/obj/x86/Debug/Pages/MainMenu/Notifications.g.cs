﻿

#pragma checksum "C:\Users\khmelenko\Desktop\Новая папка\CountersUniversal2\Counters8.1.Windows\Pages\MainMenu\Notifications.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "24502C603DE83B0D2A8FCB1FE97D1A4E"
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
    partial class Notifications : global::MyToolkit.Paging.MtPage, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 35 "..\..\..\..\Pages\MainMenu\Notifications.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.lbNotifications_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 41 "..\..\..\..\Pages\MainMenu\Notifications.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnAdd_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 42 "..\..\..\..\Pages\MainMenu\Notifications.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnChange_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 43 "..\..\..\..\Pages\MainMenu\Notifications.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnDelete_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


