using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.ApplicationModel.Store;
using Windows.Phone.UI.Input;
using Windows.Graphics.Display;
using Counters.Utils;

namespace Counters
{
    public sealed partial class MainPage : Page
    {
        private AppSettings _settings;
        private Task _task;

        public MainPage()
        {
            this.InitializeComponent();
            mainMenu.InitializeDrawerLayout();
            mainMenu.OpenDrawer();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
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

                    _task = new OneDriveHelper().RestoreAsync();
                    await _task;

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
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
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

        private void btnMainMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (mainMenu.IsDrawerOpen)
                mainMenu.CloseDrawer();
            else
                mainMenu.OpenDrawer();
        }

        private void frameContent_Navigated(object sender, MyToolkit.Paging.MtNavigationEventArgs e)
        {
            frameContent.IsEnabled = true;

            if (mainMenu.IsDrawerOpen)
                mainMenu.CloseDrawer();

            var addPages = new List<Type> { typeof(AddCounter), typeof(AddData), typeof(AddFlat), typeof(AddNotification), typeof(AddScore), typeof(AddService) };
            var notAddPage = addPages.IndexOf(frameContent.CurrentPage.Type) == -1;
            mainMenu.IsDrawerVisible = notAddPage;
            btnMainMenu.Visibility = notAddPage ? Visibility.Visible : Visibility.Collapsed;

            btnFlats.Text = App.QueryManager.GetCurrentFlat().Name;
            var header = frameContent.CurrentPage.Page.Tag;
            if (header != null)
                tbHeader.Text = header.ToString();
        }

        private async void btnFlats_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await frameContent.NavigateAsync(typeof(Flats));
        }

        private async void grdData_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mainMenu.CloseDrawer();
            var selectedItem = (sender as Grid).DataContext as MainMenuItem;
            await frameContent.NavigateAsync(Type.GetType(selectedItem.Page));
            if (selectedItem.IsLocked)
            {
                frameContent.IsEnabled = false;
                await new MessageDialog("Данный раздел доступен только в полной версии приложения.").ShowAsync();
            }
        }

        private async void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;

            if (_task != null && !_task.IsCompleted)
                return;

            if (mainMenu.IsDrawerOpen)
                mainMenu.CloseDrawer();
            else if (frameContent.CanGoBack && !frameContent.IsNavigating)
                try
                {
                    await frameContent.GoBackAsync();
                }
                catch { }
            else if (await App.PopupManager.ShowDialogPopupAsync("Вы действительно хотите выйти?"))
                Application.Current.Exit();
        }

        private void btnExit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
