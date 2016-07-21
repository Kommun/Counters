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

    public sealed partial class Data : MyToolkit.Paging.MtPage
    {
        int id;
        QueryResult selectedData;

        public Data()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            id = (int)e.Parameter;
            Tag = App.QueryManager.Connection.Get<Counter>(id).Name;
            refresh();
        }

        private void refresh()
        {
            lbData.ItemsSource = App.QueryManager.GetCounterDatas(id);
        }

        private void lbData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbData.SelectedItem != null)
                selectedData = lbData.SelectedItem as QueryResult;
            btnChange.IsEnabled = lbData.SelectedIndex != -1;
            btnDelete.IsEnabled = lbData.SelectedIndex != -1 && !selectedData.IsFirst;
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(AddData), new AddDataParameter() { CounterId = id });
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedData.ScoreId != 0)
            {
                MessageDialog msgbox = new MessageDialog("Показания привязаны к счету. Удалить их?");

                msgbox.Commands.Add(new UICommand("Да", null, 0));
                msgbox.Commands.Add(new UICommand("Нет", null, 1));

                var res = await msgbox.ShowAsync();
                if (res == null || (int)res.Id == 1)
                    return;
            }

            App.QueryManager.Connection.Delete<CounterData>(selectedData.DataId);
            refresh();
        }

        private async void btnChange_Click(object sender, RoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(AddData), new AddDataParameter()
            {
                CounterId = id,
                DataId = selectedData.DataId
            });
        }

        private async void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgbox = new MessageDialog("Вы действительно хотите удалить все показания?");

            msgbox.Commands.Add(new UICommand("Да", c =>
            {
                App.QueryManager.DeleteCounterDataButFirst(id);
                refresh();
            }));
            msgbox.Commands.Add(new UICommand("Нет"));
            await msgbox.ShowAsync();
        }
    }
}
