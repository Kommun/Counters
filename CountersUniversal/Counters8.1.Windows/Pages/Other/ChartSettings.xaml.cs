using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ChartSettings : MyToolkit.Paging.MtPage
    {
        ChartSettingsParameter settings;

        public ChartSettings()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                settings = e.Parameter as ChartSettingsParameter;
                if (settings.ChartType == 0)
                    spType.Visibility = Visibility.Collapsed;
                dpBegin.Date = settings.BeginDate;
                dpEnd.Date = settings.EndDate;
            }
            else
                await Frame.GoBackAsync();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            settings.BeginDate = dpBegin.Date.Date;
            settings.EndDate = dpEnd.Date.Date;
            await Frame.GoBackAsync();
        }
    }
}
