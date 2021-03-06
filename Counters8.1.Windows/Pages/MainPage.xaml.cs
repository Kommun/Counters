﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.Graphics.Display;
using Counters.Utils;

namespace Counters
{
    public sealed partial class MainPage : Page
    {
        private AppSettings _settings;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Регистрируем фоновую задачу
            await BackgroundTaskManager.RegisterBackgroundTask();

            _settings = Resources["settings"] as AppSettings;
            _settings.Runs++;
            _settings.notificationCountSetting = 0;
            _settings.NeedToSaveBackup = true;

            TileHelper.RefreshTile();
            DisplayInformation.AutoRotationPreferences = _settings.AllowOrientation ? DisplayOrientations.None : DisplayOrientations.Portrait;

            var firstLaunch = _settings.isFirstLaunchSetting;
            if (firstLaunch)
                InitializeApp();
            else
                await OnLaunch();

            // Переходим на стартовую страницу
            await frameContent.NavigateAsync(Type.GetType(_settings.DefaultPageType));

            // Отображаем список квартир, если это установлено в настройках и их больше 1
            if (_settings.ShowFlatsOnLaunch && App.QueryManager.RowsCount<Flat>() > 1)
                await frameContent.NavigateAsync(typeof(Flats));

            // При первом запуске предлагаем ознакомиться со справкой
            if (firstLaunch && await App.PopupManager.ShowDialogPopupAsync("Вы хотите ознакомиться со справкой? В дальнейшем ее можно будет найти в разделе 'О программе'."))
                await frameContent.NavigateAsync(typeof(Help));

            if (CurrentApp.LicenseInformation.ProductLicenses["FullVersion"].IsActive)
                _settings.IsFullVersion = true;

            if (_settings.IsFullVersion)
                await CheckNewBackup();
        }

        /// <summary>
        /// Проверяет наличие новой резервной копии в облаке
        /// </summary>
        /// <returns></returns>
        private async Task CheckNewBackup()
        {
            var backups = await LiveLogin.GetOrderedBackups();
            if (backups != null && backups.First().CreationDate > DateTime.Parse(_settings.LastBackupDate))
            {
                if (await App.PopupManager.ShowDialogPopupAsync("Обнаружена новая резервная копия. Восстановить данные?"))
                {
                    var appBar = frameContent.CurrentPage.Page.BottomAppBar;
                    if (appBar != null)
                        appBar.Visibility = Visibility.Collapsed;

                    await new OneDriveHelper().RestoreAsync();

                    if (appBar != null)
                        appBar.Visibility = Visibility.Visible;
                }

                _settings.LastBackupDate = backups.First().CreationDate.ToString();
            }
        }

        /// <summary>
        /// Действия при запуске приложения
        /// </summary>
        /// <returns></returns>
        private async Task OnLaunch()
        {
            // Обновление схемы базы данных
            App.QueryManager.RefreshDbScheme();
            await UpdateManager.ShowUpdateMessage();

            if (_settings.NotRated && _settings.Runs == 4)
            {
                if (await App.PopupManager.ShowDialogPopupAsync("Вы пользуетесь приложением уже достаточно долго. Не могли бы Вы оставить о нем отзыв?"))
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
                    _settings.NotRated = false;
                }

                _settings.Runs = 0;
            }
        }

        /// <summary>
        /// Инициализация приложения при первом запуске
        /// </summary>
        /// <returns></returns>
        private void InitializeApp()
        {
            App.QueryManager.CreateDatabase();

            _settings.isFirstLaunchSetting = false;
            _settings.dbVersionSetting = QueryManager.dbVersion;
            _settings.AppVersion = UpdateManager.CurrentVersion;
        }

        private void frameContent_Navigated(object sender, MyToolkit.Paging.MtNavigationEventArgs e)
        {
            frameContent.IsEnabled = true;
            var addPages = new List<Type> { typeof(AddCounter), typeof(AddData), typeof(AddFlat), typeof(AddNotification), typeof(AddScore), typeof(AddService) };
            var notAddPage = addPages.IndexOf(frameContent.CurrentPage.Type) == -1;
            mainMenu.Visibility = notAddPage ? Visibility.Visible : Visibility.Collapsed;

            var appBar = frameContent.CurrentPage.Page.BottomAppBar;
            if (appBar != null)
            {
                appBar.Style = Resources["appBarStyle"] as Style;
                appBar.Margin = new Thickness(mainMenu.Visibility == Visibility.Visible ? mainMenu.ActualWidth : 0, 0, 0, 0);
            }

            btnFlats.Text = App.QueryManager.GetCurrentFlat().Name;
            var header = frameContent.CurrentPage.Page.Tag;
            if (header != null)
                tbHeader.Text = header.ToString();
            btnBack.IsEnabled = frameContent.CanGoBack;
        }

        private async void grdData_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var selectedItem = (sender as Grid).DataContext as MainMenuItem;
            await frameContent.NavigateAsync(Type.GetType(selectedItem.Page));
            if (selectedItem.IsLocked)
            {
                frameContent.IsEnabled = false;
                await new MessageDialog("Данный раздел доступен только в полной версии приложения.").ShowAsync();
            }
        }

        private async void btnFlats_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await frameContent.NavigateAsync(typeof(Flats));
        }

        private async void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentApp.LicenseInformation.ProductLicenses["FullVersion"].IsActive)
            {
                try
                {
                    var result = await CurrentApp.RequestProductPurchaseAsync("FullVersion");
                    if (result.Status == ProductPurchaseStatus.Succeeded)
                        _settings.IsFullVersion = true;
                }
                catch { }
            }
            else _settings.IsFullVersion = true;
        }

        private async void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await frameContent.GoBackAsync();
            }
            catch { }
        }
    }
}
