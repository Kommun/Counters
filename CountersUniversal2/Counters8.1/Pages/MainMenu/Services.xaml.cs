﻿using System;
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
    public sealed partial class Services : MyToolkit.Paging.MtPage
    {
        ServiceResult selectedService;
        AppSettings settings = new AppSettings();

        public Services()
        {
            this.InitializeComponent();
        }

        private void refreshServices()
        {
            var services = App.QueryManager.GetServices();
            lbServices.ItemsSource = services;
            grdSumm.Visibility = services.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            tbSumm.Text = string.Format("{0} {1}", services.Sum(service => service.Summ), settings.Currency);
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            try
            {
                refreshServices();
            }
            catch
            {
                Frame.GoBackAsync();
            }
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!settings.IsFullVersion && lbServices.Items.Count >= 2)
                await new MessageDialog("В бесплатной версии невозможно создать более 2 услуг").ShowAsync();
            else
                await Frame.NavigateAsync(typeof(AddService));
        }

        private async void btnChange_Click(object sender, RoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(AddService), new AddServiceParameter { ServiceId = selectedService.ServiceId });
        }

        private void lbServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbServices.SelectedItem != null)
                selectedService = lbServices.SelectedItem as ServiceResult;
            btnChange.IsEnabled = lbServices.SelectedIndex != -1;
            btnDelete.IsEnabled = lbServices.SelectedIndex != -1;
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgbox = new MessageDialog("Вы действительно хотите удалить услугу?");

            msgbox.Commands.Add(new UICommand("Да", c =>
            {
                if (selectedService != null)
                {
                    App.QueryManager.DeleteService(selectedService.ServiceId);
                    refreshServices();
                }
            }));
            msgbox.Commands.Add(new UICommand("Нет"));
            await msgbox.ShowAsync();
        }
    }
}
