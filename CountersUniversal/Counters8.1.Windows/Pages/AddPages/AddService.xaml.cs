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
    public sealed partial class AddService : MyToolkit.Paging.MtPage
    {
        AddServiceParameter parameter;
        AppSettings settings = new AppSettings();

        public AddService()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            try
            {
                tbCounters.ItemsSource = App.QueryManager.GetWaterCounters();
                if (e.Parameter != null)
                {
                    parameter = e.Parameter as AddServiceParameter;
                    var currentService = App.QueryManager.GetServiceWithTarif(parameter.ServiceId, parameter.DataId);
                    spService.DataContext = currentService;
                    tbType.SelectedIndex = currentService.Type;
                    tbType.IsEnabled = false;

                    var selectedCounters = App.QueryManager.GetCountersByIds(currentService.lstCounters);
                    if (currentService.Type == 2)
                    {
                        foreach (var c in selectedCounters)
                            tbCounters.SelectedItems.Add(c);
                    }
                    else if (currentService.Type == 3)
                        tbCounters.SelectedItem = (tbCounters.ItemsSource as List<QueryResult>)
                            .FirstOrDefault(c => c.CounterId == selectedCounters.FirstOrDefault().CounterId);
                    tbCounters_SelectionChanged(null, null);

                    Tag = currentService.Name;
                }
                else
                {
                    tbType.SelectedIndex = 0;
                    Tag = "Новая услуга";
                }
            }
            catch { }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var selectedType = tbType.SelectedIndex;
            double tarif, count = 0;

            if (tbName.Text == "")
                invalidData("Введите название услуги", tbName);
            else if (!double.TryParse(tbTarif.Text.Replace('.', ','), out tarif))
                invalidData("Тариф содержит недопустимые символы", tbTarif);
            else if ((selectedType == 1 || selectedType == 3) && !double.TryParse(tbData.Text.Replace('.', ','), out count))
                invalidData("Поле содержит недопустимые символы", tbData);
            else if ((selectedType == 2 || selectedType == 3) && (tbCounters.SelectedItems == null || tbCounters.SelectedItems.Count == 0))
                invalidData("Выберите счетчики");
            else
            {
                var newTarif = new ServiceTarif() { Tarif = tarif };
                App.QueryManager.Connection.Insert(newTarif);

                if (parameter != null)
                {
                    var serviceToUpdate = App.QueryManager.Connection.Get<Service>(parameter.ServiceId);
                    serviceToUpdate.Name = tbName.Text;
                    serviceToUpdate.Data = count;
                    serviceToUpdate.lstCounters = lstCounters();
                    // Редактирования тарифа при добавлении нового показания
                    if (parameter.DataId == 0)
                        serviceToUpdate.TarifId = newTarif.TarifId;
                    // Редактирование тарифа при изменении показания
                    else
                        App.QueryManager.UpdateServiceDataTarif(parameter.DataId, newTarif.TarifId);
                    App.QueryManager.Connection.Update(serviceToUpdate);
                }
                else
                {
                    var newService = new Service()
                    {
                        FlatId = settings.CurrentFlatId,
                        Name = tbName.Text,
                        Type = selectedType,
                        Data = count,
                        TarifId = newTarif.TarifId,
                        lstCounters = lstCounters()
                    };
                    App.QueryManager.Connection.Insert(newService);
                }

                await Frame.GoBackAsync();
            }
        }

        private string lstCounters()
        {
            if (tbType.SelectedIndex == 2 || tbType.SelectedIndex == 3)
                return string.Join(",", (tbCounters.SelectedItems.ToList().Cast<QueryResult>()).Select(qr => qr.CounterId));
            else
                return null;
        }

        private async void invalidData(string message, Control tbToFocus = null)
        {
            await new MessageDialog(message).ShowAsync();
            if (tbToFocus != null)
                tbToFocus.Focus(FocusState.Programmatic);

        }

        private void tbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                spData.Visibility = Visibility.Collapsed;
                spCounters.Visibility = Visibility.Collapsed;

                switch (tbType.SelectedIndex)
                {
                    case 1:
                        spData.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        spCounters.Visibility = Visibility.Visible;
                        tbCounters.SelectionMode = ListViewSelectionMode.Multiple;
                        break;
                    case 3:
                        spData.Visibility = Visibility.Visible;
                        spCounters.Visibility = Visibility.Visible;
                        tbCounters.SelectionMode = ListViewSelectionMode.Single;
                        break;
                }
                if (tbCounters.SelectionMode == ListViewSelectionMode.Single)
                    tbCounters.SelectedIndex = -1;
                else
                    tbCounters.SelectedItems.Clear();
            }
            catch { }
        }

        private void tbCounters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbCounters.SelectedItems.Count > 0)
                btnCounters.Content = String.Join(",", (tbCounters.SelectedItems.ToList().Cast<QueryResult>()).Select(qr => qr.Name));
            else
                btnCounters.Content = "Выбрать...";
        }
    }
}
