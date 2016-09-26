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
using Counters.Utils;

namespace Counters
{
    public sealed partial class Flats : MyToolkit.Paging.MtPage
    {
        Flat selectedFlat;
        AppSettings settings = new AppSettings();

        public Flats()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            refresh();
        }

        private void refresh()
        {
            var flats = App.QueryManager.Connection.Table<Flat>().ToList();
            lbFlats.ItemsSource = flats;
            lbFlats.SelectedItem = flats.FirstOrDefault(f => f.FlatId == settings.CurrentFlatId);
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(AddFlat), null);
        }

        private async void btnChange_Click(object sender, RoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(AddFlat), selectedFlat);
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((lbFlats.ItemsSource as List<Flat>).Count > 1)
            {
                MessageDialog msgbox = new MessageDialog("Вы действительно хотите удалить квартиру? Будут удалены все связанные счетчики, услуги и счета.");

                msgbox.Commands.Add(new UICommand("Да", c =>
                {
                    App.QueryManager.DeleteFlat(selectedFlat.FlatId);
                    refresh();
                    if (settings.CurrentFlatId == selectedFlat.FlatId)
                        settings.CurrentFlatId = (lbFlats.ItemsSource as List<Flat>).First().FlatId;
                }));
                msgbox.Commands.Add(new UICommand("Нет"));

                await msgbox.ShowAsync();
            }
            else await new MessageDialog("Невозможно удалить единственную квартиру.").ShowAsync();
        }

        private async void btnPin_Click(object sender, RoutedEventArgs e)
        {
            await TileHelper.Pin(selectedFlat);
        }

        private async void grdFlat_Tapped(object sender, TappedRoutedEventArgs e)
        {
            selectedFlat = lbFlats.SelectedItem as Flat;

            settings.CurrentFlatId = selectedFlat.FlatId;
            await Frame.GoBackAsync();
        }

        private void grdFlat_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase.ShowAttachedFlyout(senderElement);
            selectedFlat = senderElement.DataContext as Flat;
        }
    }
}
