using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using MyToolkit.Paging;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Counters
{
    public sealed partial class ExportSettings : MyToolkit.Paging.MtPage
    {
        public ExportSettings()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(MtNavigationEventArgs args)
        {
            tbTemplate.Text = App.Settings.Template;
        }

        protected override void OnNavigatedFrom(MtNavigationEventArgs args)
        {
            App.Settings.Template = tbTemplate.Text;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            await Frame.GoBackAsync();
        }
    }
}
