﻿

#pragma checksum "C:\Users\khmelenko\Desktop\Новая папка\CountersUniversal2\Counters8.1\Pages\Other\Chart.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A755577ECAD98AF6A2B14BF72D1278DF"
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
    partial class Chart : global::MyToolkit.Paging.MtPage, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 30 "..\..\..\..\Pages\Other\Chart.xaml"
                ((global::Windows.UI.Xaml.Controls.ListPickerFlyout)(target)).ItemsPicked += this.tbCounters_ItemsPicked;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 40 "..\..\..\..\Pages\Other\Chart.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnSettings_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


