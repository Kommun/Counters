﻿

#pragma checksum "C:\Users\khmelenko\Desktop\Новая папка\CountersUniversal2\Counters8.1.Windows\Pages\Other\Chart.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5D7F77F46EEB5C06CED33C0191BC1AAF"
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
    partial class Chart : global::MyToolkit.Paging.MtPage
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.ComboBox tbCounters; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::OxyPlot.Windows.PlotView chart; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton btnSettings; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///Pages/Other/Chart.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            tbCounters = (global::Windows.UI.Xaml.Controls.ComboBox)this.FindName("tbCounters");
            chart = (global::OxyPlot.Windows.PlotView)this.FindName("chart");
            btnSettings = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("btnSettings");
        }
    }
}



