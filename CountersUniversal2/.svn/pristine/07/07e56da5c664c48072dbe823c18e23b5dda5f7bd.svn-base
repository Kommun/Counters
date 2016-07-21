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
using Windows.UI.Popups;

namespace Counters
{
    public sealed partial class Charts : MyToolkit.Paging.MtPage
    {
        public Charts()
        {
            this.InitializeComponent();
        }

        private async void btnChart_Click(object sender, RoutedEventArgs e)
        {
            var selectedChart = (sender as Button).DataContext as MenuItem;
            if (Enabled(selectedChart.Type))
                await Frame.NavigateAsync(typeof(Chart), selectedChart);
            else
                await new MessageDialog("Для начала работы с графиками недостаточно данных").ShowAsync();
        }

        private bool Enabled(int chartType)
        {
            if (chartType == 4)
                return App.QueryManager.RowsCount<Score>() > 0;
            else
                return App.QueryManager.RowsCount<Counter>() > 0;
        }
    }
}
