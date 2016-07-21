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
    public sealed partial class Settings : MyToolkit.Paging.MtPage
    {
        AppSettings settings;

        public Settings()
        {
            this.InitializeComponent();
            settings = Resources["settings"] as AppSettings;
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            tbPage.SelectedItem = (tbPage.ItemsSource as List<MainMenuItem>).FirstOrDefault(mi => mi.Page == settings.DefaultPageType);
            tbCurrency.Text = settings.Currency;
        }

        protected override void OnNavigatedFrom(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            settings.DefaultPageType = (tbPage.SelectedItem as MainMenuItem).Page;
            settings.Currency = tbCurrency.Text;
        }

        private async void btnSaveBackupOnExit_Toggled(object sender, RoutedEventArgs e)
        {
            // Если включили автосохранение - производим авторизацию в облаке
            if (btnSaveBackupOnExit != null && btnSaveBackupOnExit.IsOn)
                // Если авторизация не удалась - сбрасываем автоматическое сохранение
                if (await LiveLogin.GetClient() == null)
                    btnSaveBackupOnExit.IsOn = false;
        }
    }
}
