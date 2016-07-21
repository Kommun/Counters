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
    public sealed partial class Counters : MyToolkit.Paging.MtPage
    {
        AppSettings settings = new AppSettings();
        QueryResult selectedCounter;

        public Counters()
        {
            this.InitializeComponent();
        }

        private void refreshCounters()
        {
            try
            {
                var query = App.QueryManager.GetCountersWithLastData();
                lbCounters.ItemsSource = query;
                tbSumm.Text = string.Format("{0} {1}", query.Sum(counter => counter.Summ), settings.Currency);
                tbHelp.Visibility = query.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                btnMoney.IsEnabled = query.Count > 0;
            }
            catch
            {
                new MessageDialog("Конфигурация базы данных повреждена или устарела").ShowAsync();
                //App.QueryManager.ClearCounters();
            }
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            refreshCounters();
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!settings.IsFullVersion && lbCounters.Items.Count >= 2)
                await new MessageDialog("В бесплатной версии невозможно создать более 2 счетчиков").ShowAsync();
            else
                await Frame.NavigateAsync(typeof(AddCounter), null);
        }

        private async void btnChange_Click(object sender, RoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(AddCounter), new AddCounterParameter()
                { CounterId = selectedCounter.CounterId });
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgbox = new MessageDialog("Вы действительно хотите удалить счетчик? Все показания будут также удалены.");

            msgbox.Commands.Add(new UICommand("Да", c =>
            {
                if (selectedCounter != null)
                {
                    App.QueryManager.DeleteCounter(selectedCounter.CounterId);
                    refreshCounters();
                }
            }));
            msgbox.Commands.Add(new UICommand("Нет"));
            await msgbox.ShowAsync();
        }

        private async void grdData_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(Data), ((sender as Grid).DataContext as QueryResult).CounterId);
        }

        private void grdData_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase.ShowAttachedFlyout(senderElement);
            selectedCounter = senderElement.DataContext as QueryResult;
        }

        private void btnMoney_Click(object sender, RoutedEventArgs e)
        {
            grdSumm.Visibility = grdSumm.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
