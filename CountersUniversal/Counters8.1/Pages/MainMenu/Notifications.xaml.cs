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
    public sealed partial class Notifications : MyToolkit.Paging.MtPage
    {
        Notification selectedNotification;

        public Notifications()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            refresh();
        }

        private void refresh()
        {
            lbNotifications.ItemsSource = App.QueryManager.GetNotifications();
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(AddNotification), null);
        }

        private async void btnChange_Click(object sender, RoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(AddNotification), selectedNotification);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            App.QueryManager.Connection.Delete<Notification>(selectedNotification.Id);
            refresh();
        }

        private void lbNotifications_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbNotifications.SelectedItem != null)
                selectedNotification = lbNotifications.SelectedItem as Notification;
            btnChange.IsEnabled = lbNotifications.SelectedIndex != -1;
            btnDelete.IsEnabled = lbNotifications.SelectedIndex != -1;
        }
    }
}
