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
using MyToolkit.Messaging;
using System.Threading.Tasks;

namespace Counters
{
    public sealed partial class AddScore : MyToolkit.Paging.MtPage
    {
        int id;
        double Summ, recalculation, peni;
        bool allowGoBack, needToSave, showEmpty;
        Score currentScore;
        AppSettings settings = new AppSettings();
        List<QueryResult> counters;
        List<ServiceResult> services;

        /// <summary>
        /// Выбранные услуги
        /// </summary>
        public List<ServiceResult> CheckedServices
        {
            get
            {
                return (lbServices.ItemsSource as List<CheckedItem>)
                    .Where(s => s.IsChecked).Select(s => s.Item as ServiceResult).ToList();
            }
        }

        public AddScore()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            Loaded += AddScore_Loaded;
        }

        private void AddScore_Loaded(object sender, RoutedEventArgs e)
        {
            refreshSumm();
        }

        protected async override Task OnNavigatingFromAsync(MyToolkit.Paging.MtNavigatingCancelEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
                return;

            if ((id == 0 || needToSave) && !allowGoBack)
            {
                MessageDialog msgbox = new MessageDialog("Сохранить счет?");
                msgbox.Commands.Add(new UICommand("Да", null, 0));
                msgbox.Commands.Add(new UICommand("Нет", null, 1));
                msgbox.Commands.Add(new UICommand("Отмена", null, 2));
                var result = await msgbox.ShowAsync();

                // В отличие от WP, тут добавилась кнопка "Отмена", ее тоже нужно обрабатывать
                if (result == null || (int)result.Id == 2 || (int)result.Id == 0 && !await Save())
                {
                    e.Cancel = true;
                    return;
                }
            }
            App.QueryManager.Connection.Rollback();
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            try
            {
                //Начинаем транзакцию
                if (e.NavigationMode == NavigationMode.New)
                    App.QueryManager.Connection.BeginTransaction();

                id = (int)e.Parameter;
                if (id != 0)
                {
                    currentScore = App.QueryManager.Connection.Get<Score>(id);
                    tbDate.Date = currentScore.Date;
                    Tag = currentScore.stringDate;
                }
                else
                {
                    showEmpty = true;
                    //При открытии страницы для создания счета
                    if (e.NavigationMode == NavigationMode.New)
                    {
                        currentScore = new Score() { FlatId = settings.CurrentFlatId };
                        App.QueryManager.Connection.Insert(currentScore);
                    }
                    Tag = "Новый счет";
                }
                spPayment.DataContext = currentScore;

                counters = App.QueryManager.GetCounterDataByScoreId(currentScore.ScoreId, true);
                services = App.QueryManager.GetServiceDataByScoreId(currentScore.ScoreId, true);

                FilterDatas();

                btnShowEmpty.Visibility = id != 0 && (counters.Count != counters.Where(c => c.DataId != 0).Count()
                     || services.Count != services.Where(s => s.ServiceDataId != 0).Count()) ? Visibility.Visible : Visibility.Collapsed;
            }
            catch
            {
                Frame.GoBackAsync();
            }
        }

        private void FilterDatas()
        {
            if (showEmpty)
            {
                lbCounters.ItemsSource = counters;
                lbServices.ItemsSource = services
                    .Select(s => new CheckedItem() { Item = s, IsChecked = id == 0 || s.ServiceDataId != 0 }).ToList();
            }
            else
            {
                lbCounters.ItemsSource = counters.Where(c => c.DataId != 0);
                lbServices.ItemsSource = services.Where(s => s.ServiceDataId != 0)
                    .Select(s => new CheckedItem() { Item = s, IsChecked = true }).ToList();
            }
            refreshSumm();
        }

        /// <summary>
        /// Пересчитать начисленную сумму
        /// </summary>
        private void refreshSumm()
        {
            Summ = counters.Sum(c => c.Summ + c.SummODN) + CheckedServices.Sum(s => s.Summ);
            tbSumm.Text = string.Format("{0} {1}", Summ, settings.Currency);
            RecalculatePaymentSumm();
        }

        private void tbPayment_TextChanged(object sender, TextChangedEventArgs e)
        {
            RecalculatePaymentSumm();
        }

        /// <summary>
        /// Пересчитать сумму к оплате
        /// </summary>
        private void RecalculatePaymentSumm()
        {
            double.TryParse(tbRecalculation.Text, out recalculation);
            double.TryParse(tbPeni.Text, out peni);

            tbPaymentSumm.Text = (Summ + recalculation + peni).ToString();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (await Save())
                await Frame.GoBackAsync();
        }

        private async Task<bool> Save()
        {
            if (counters.Count(c => c.DataId != 0) == 0 && CheckedServices.Count == 0)
            {
                await new MessageDialog("Выберите хотя бы один счетчик или услугу").ShowAsync();
                return false;
            }
            else
            {
                currentScore.Date = tbDate.Date.Date;
                currentScore.Recalculation = recalculation;
                currentScore.Peni = peni;

                App.QueryManager.DeleteServiceDataByScoreId(currentScore.ScoreId);
                foreach (var service in CheckedServices)
                    App.QueryManager.Connection.Insert(new ServiceData
                    {
                        ServiceId = service.ServiceId,
                        TarifId = service.TarifId,
                        ScoreId = currentScore.ScoreId
                    });

                App.QueryManager.Connection.Update(currentScore);
                //Завершаем транзакцию
                App.QueryManager.Connection.Commit();
                allowGoBack = true;
                return true;
            }
        }

        private async void service_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var selectedItem = ((dynamic)sender).DataContext.Item as ServiceResult;
            await Frame.NavigateAsync(typeof(AddService), new AddServiceParameter
            {
                ServiceId = selectedItem.ServiceId,
                DataId = selectedItem.ServiceDataId
            });
            needToSave = true;
        }

        private async void counter_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var selectedItem = (sender as Grid).DataContext as QueryResult;
            await Frame.NavigateAsync(typeof(AddData), new AddDataParameter()
            {
                CounterId = selectedItem.CounterId,
                DataId = selectedItem.DataId,
                ScoreId = currentScore.ScoreId
            });
            needToSave = true;
        }

        private void service_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var selectedItem = (sender as Grid).DataContext as CheckedItem;
            selectedItem.IsChecked = !selectedItem.IsChecked;
            refreshSumm();
            needToSave = true;
        }

        private void btnShowEmpty_Click(object sender, RoutedEventArgs e)
        {
            showEmpty = !showEmpty;
            FilterDatas();
        }
    }
}