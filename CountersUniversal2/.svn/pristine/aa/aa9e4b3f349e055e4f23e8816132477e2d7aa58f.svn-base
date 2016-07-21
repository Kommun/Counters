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
    public sealed partial class AddData : MyToolkit.Paging.MtPage
    {
        QueryResult currentCounter;
        AddDataParameter parameter;

        public AddData()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            try
            {
                parameter = e.Parameter as AddDataParameter;
                currentCounter = App.QueryManager.GetCounterWithData(parameter.CounterId, parameter.DataId);

                if (parameter.ScoreId != 0 && currentCounter.EnableODN)
                    grdODN.Visibility = Visibility.Visible;
                tbTarif.Text = currentCounter.stringTariff;

                tbDate.MinYear = new DateTime(2001, 1, 1);
                if (parameter.DataId != 0)
                {
                    tbDate.Date = currentCounter.Date;
                    tbData.Text = currentCounter.Data.ToString();
                    tbDataODN.Text = currentCounter.DataODN.ToString();
                }
                else currentCounter.Date = tbDate.Date.Date;

                grdLastData.DataContext = currentCounter.PreviousData;

                Tag = currentCounter.Name;
            }
            catch
            {
                Frame.GoBackAsync();
            }
        }

        private void tbDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            currentCounter.Date = e.NewDate.Date;
            grdLastData.DataContext = currentCounter.PreviousData;
        }

        private async void btnChangeTarif_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(AddCounter), new AddCounterParameter()
            {
                CounterId = currentCounter.CounterId,
                DataId = parameter.DataId,
                ChangeTarif = true
            });
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            List<CounterData> datas;
            double data, odn;
            // Если значение начинается со знака "+" - введена разница в показаниях
            bool isDelta = !string.IsNullOrEmpty(tbData.Text) && tbData.Text[0] == '+';

            if (App.QueryManager.IsCounterDatasByDateExist(currentCounter))
                invalidData("Показания с указанной датой уже имеются в базе", tbData);
            else if (!double.TryParse(tbData.Text.Replace('.', ','), out data))
                invalidData("Поле содержит недопустимые символы", tbData);
            else if (!double.TryParse(tbDataODN.Text.Replace('.', ','), out odn))
                invalidData("Поле содержит недопустимые символы", tbDataODN);
            else if (currentCounter.IsFirst
                && (datas = App.QueryManager.DatasBeforeDate(currentCounter.CounterId, tbDate.Date.Date.ToString("yyyy-MM-dd HH:mm:ss"), false)).Count > 0)
                invalidData($"Дата начальных показаний должна быть меньше {datas.First().Date.ToString("dd.MM.yyyy")}", tbDate);
            else if (!currentCounter.IsFirst
                && (datas = App.QueryManager.DatasBeforeDate(currentCounter.CounterId, tbDate.Date.Date.ToString("yyyy-MM-dd HH:mm:ss"), true)).Count == 0)
                invalidData("Дата не может быть меньше даты начальных показаний", tbDate);
            else if (!isDelta && data < currentCounter.PreviousData?.Data)
                invalidData("Данные показания меньше предыдущих", tbData);
            else
            {
                CounterData counterData;
                if (parameter.DataId != 0)
                    counterData = App.QueryManager.Connection.Get<CounterData>(parameter.DataId);
                else
                    counterData = new CounterData()
                    {
                        CounterId = parameter.CounterId,
                        ScoreId = parameter.ScoreId,
                        TariffId = currentCounter.TariffId
                    };

                counterData.Data = isDelta ? currentCounter.PreviousData.Data + data : data;
                counterData.DataODN = odn;
                counterData.Date = tbDate.Date.Date;

                // Редактирование показаний
                if (parameter.DataId != 0)
                {
                    counterData.DataId = parameter.DataId;
                    App.QueryManager.Connection.Update(counterData);
                }
                // Добавление показаний
                else
                    App.QueryManager.Connection.Insert(counterData);

                await Frame.GoBackAsync();
            }
        }

        private void invalidData(string message, Control tbToFocus)
        {
            new MessageDialog(message).ShowAsync();
            tbToFocus.Focus(FocusState.Programmatic);
        }
    }
}
