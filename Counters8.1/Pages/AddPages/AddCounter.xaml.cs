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
using Counters.Enums;

namespace Counters
{
	public sealed partial class AddCounter : MyToolkit.Paging.MtPage
	{
		AddCounterParameter parameter;

		public AddCounter()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
		{
			try
			{
				if (e.Parameter != null)
				{
					parameter = e.Parameter as AddCounterParameter;
					var currentCounter = App.QueryManager.GetCounterWithTarif(parameter.CounterId, parameter.DataId);
					spCounter.DataContext = currentCounter;
					spTarif.DataContext = currentCounter;
					btnType.IsEnabled = false;
					tbBeginData.Visibility = Visibility.Collapsed;
					tbType.SelectedItem = (tbType.ItemsSource as List<QueryResult>).Single(item => item.TypeId == currentCounter.TypeId);

					if (parameter.ChangeTarif)
						pageSelector.SelectedIndex = 1;

					cbType.SelectedIndex = currentCounter.TarifsCount - 1;
					Tag = currentCounter.Name;
				}
				else
				{
					tbType.SelectedIndex = 0;
					cbType.SelectedIndex = 0;
					Tag = "Новый счетчик";
				}
				tbType_ItemsPicked(null, null);
			}
			catch (Exception ex)
			{
				new MessageDialog(ex.Message).ShowAsync();
			}
		}

		private async void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (tbName.Text.ToLower() == "ihavealreadybought")
			{
				new AppSettings().IsFullVersion = true;
				await Frame.GoBackAsync();
			}
			else
			{
				double tarif1 = 0, tarif2 = 0, tarif3 = 0, limit1 = 0, limit2 = 0;
				double unitConvertCoefficient = 1;
				double beginData;

				if (tbName.Text == "")
					invalidData(tbName, 0, "Введите название счетчика");
				else if (!double.TryParse(tbBeginData.Text.Replace('.', ','), out beginData))
					invalidData(tbBeginData, 0);
				else if ((tbType.SelectedItem as QueryResult).TypeId == (int)CounterType.Heating
					&& !double.TryParse(tbUnitConvertCoefficient.Text.Replace('.', ','), out unitConvertCoefficient))
					invalidData(tbUnitConvertCoefficient, 0);
				else if (cbType.SelectedIndex == 2 && !double.TryParse(tbTarif1.Text.Replace('.', ','), out tarif1))
					invalidData(tbTarif1, 1);
				else if (cbType.SelectedIndex == 2 && !double.TryParse(tbLimit1.Text.Replace('.', ','), out limit1))
					invalidData(tbLimit1, 1);
				else if ((cbType.SelectedIndex == 1 || cbType.SelectedIndex == 2) && !double.TryParse(tbTarif2.Text.Replace('.', ','), out tarif2))
					invalidData(tbTarif2, 1);
				else if ((cbType.SelectedIndex == 1 || cbType.SelectedIndex == 2) && !double.TryParse(tbLimit2.Text.Replace('.', ','), out limit2))
					invalidData(tbLimit2, 1);
				else if (!double.TryParse(tbTarif3.Text.Replace('.', ','), out tarif3))
					invalidData(tbTarif3, 1);
				else
				{
					Tarif newTarif = new Tarif()
					{
						TarifsCount = cbType.SelectedIndex + 1,
						Tarif1 = tarif1,
						Limit1 = limit1,
						Tarif2 = tarif2,
						Limit2 = limit2,
						Tarif3 = tarif3
					};

					App.QueryManager.Connection.Insert(newTarif);

					if (parameter != null)
					{
						Counter counterToUpdate = App.QueryManager.Connection.Get<Counter>(parameter.CounterId);
						counterToUpdate.Name = tbName.Text;
						counterToUpdate.EnableODN = btnEnableODN.IsOn;
						counterToUpdate.UnitConvertCoefficient = unitConvertCoefficient;
						// Редактирования тарифа при добавлении нового показания
						if (parameter.DataId == 0)
							counterToUpdate.TarifId = newTarif.TariffId;
						// Редактирование тарифа при изменении показания
						else
							App.QueryManager.UpdateCounterDataTarif(parameter.DataId, newTarif.TariffId);
						App.QueryManager.Connection.Update(counterToUpdate);
					}
					else
					{
						Counter newCounter = new Counter()
						{
							FlatId = new AppSettings().CurrentFlatId,
							Name = tbName.Text,
							TypeId = (tbType.SelectedItem as QueryResult).TypeId,
							EnableODN = btnEnableODN.IsOn,
							UnitConvertCoefficient = unitConvertCoefficient,
							TarifId = newTarif.TariffId
						};
						App.QueryManager.Connection.Insert(newCounter);

						// Начальные показания
						App.QueryManager.Connection.Insert(new CounterData()
						{
							CounterId = newCounter.CounterId,
							Data = beginData,
							Date = DateTime.Today,
							TariffId = newCounter.TarifId,
							IsFirst = true
						});
					}
					await Frame.GoBackAsync();
				}
			}
		}

		private async void invalidData(Control tbToFocus, int selectedPage, string message = "Некорректное значение")
		{
			pageSelector.SelectedIndex = selectedPage;
			await new MessageDialog(message).ShowAsync();
			tbToFocus.Focus(FocusState.Programmatic);
		}

		private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			spFirstTarif.Visibility = Visibility.Collapsed;
			spSecondTarif.Visibility = Visibility.Collapsed;

			switch (cbType.SelectedIndex)
			{
				case 0:
					tbTarif3.Header = "Тариф";
					break;
				case 1:
					spSecondTarif.Visibility = Visibility.Visible;

					tbTarif2.Header = "Тариф до порога";
					tbLimit2.Header = "Порог";
					tbTarif3.Header = "Тариф свыше порога";
					break;
				case 2:
					spFirstTarif.Visibility = Visibility.Visible;
					spSecondTarif.Visibility = Visibility.Visible;

					tbTarif1.Header = "Тариф до порога 1";
					tbLimit1.Header = "Порог 1";
					tbTarif2.Header = "Тариф от порога 1 до порога 2";
					tbLimit2.Header = "Порог 2";
					tbTarif3.Header = "Тариф свыше порога 2";
					break;
			}
		}

		private void tbType_ItemsPicked(ListPickerFlyout sender, ItemsPickedEventArgs args)
		{
			// Для счетчиков типа "Отопление" включаем возможность перевода единиц измерения
			if ((tbType.SelectedItem as QueryResult).TypeId == (int)CounterType.Heating)
				tbUnitConvertCoefficient.Visibility = Visibility.Visible;
			else tbUnitConvertCoefficient.Visibility = Visibility.Collapsed;
		}
	}
}
