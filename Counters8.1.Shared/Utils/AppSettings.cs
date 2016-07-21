using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.ComponentModel;
using Windows.ApplicationModel.Background;

namespace Counters
{
    public class AppSettings : INotifyPropertyChanged
    {
        ApplicationDataContainer settings;
        public event PropertyChangedEventHandler PropertyChanged;

        public AppSettings()
        {
            settings = ApplicationData.Current.LocalSettings;
        }

        public void AddOrUpdateValue(string Key, Object value)
        {
            if (settings.Values[Key] != value)
                settings.Values[Key] = value;
            OnPropertyChanged();
        }

        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            if (settings.Values.ContainsKey(Key))
                value = (T)settings.Values[Key];
            else
                value = defaultValue;

            return value;
        }

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler == null) return;
            handler(null, new PropertyChangedEventArgs(propertyName));
        }

        #region StartSettings

        public bool isFirstLaunchSetting
        {
            get { return GetValueOrDefault<bool>("isFirstLaunch", true); }
            set { AddOrUpdateValue("isFirstLaunch", value); }
        }

        public int Runs
        {
            get { return GetValueOrDefault<int>("Runs", 0); }
            set { AddOrUpdateValue("Runs", value); }
        }

        public bool NotRated
        {
            get { return GetValueOrDefault<bool>("NotRated", true); }
            set { AddOrUpdateValue("NotRated", value); }
        }

        public bool IsFullVersion
        {
            get { return GetValueOrDefault<bool>("IsFullVersion", false); }
            set { AddOrUpdateValue("IsFullVersion", value); }
        }

        #endregion

        #region SettingsPage

        public string Currency
        {
            get { return GetValueOrDefault<string>("Currency", "руб"); }
            set { AddOrUpdateValue("Currency", value); }
        }

        public bool AllowOrientation
        {
            get { return GetValueOrDefault<bool>("AllowOrientation", true); }
            set
            {
                Windows.Graphics.Display.DisplayInformation.AutoRotationPreferences = value ? Windows.Graphics.Display.DisplayOrientations.None : Windows.Graphics.Display.DisplayOrientations.Portrait;

                AddOrUpdateValue("AllowOrientation", value);
            }
        }

        public bool isRememberOn
        {
            get { return GetValueOrDefault<bool>("remember", true); }
            set { AddOrUpdateValue("remember", value); }
        }

        public string DefaultPageType
        {
            get { return GetValueOrDefault<string>("DefaultPageType", "Counters.Counters"); }
            set { AddOrUpdateValue("DefaultPageType", value); }
        }

        #endregion

        #region ExportPage

        public bool isFirstMessageSetting
        {
            get { return GetValueOrDefault<bool>("isFirstMessage", true); }
            set { AddOrUpdateValue("isFirstMessage", value); }
        }

        public string emailSetting
        {
            get { return GetValueOrDefault<string>("email", ""); }
            set { AddOrUpdateValue("email", value); }
        }

        public string mailTitleSetting
        {
            get { return GetValueOrDefault<string>("mailTitle", ""); }
            set { AddOrUpdateValue("mailTitle", value); }
        }

        public string phoneSetting
        {
            get { return GetValueOrDefault<string>("phone", ""); }
            set { AddOrUpdateValue("phone", value); }
        }

        public bool AddData
        {
            get { return GetValueOrDefault<bool>("AddData", true); }
            set { AddOrUpdateValue("AddData", value); }
        }

        public bool AddSumm
        {
            get { return GetValueOrDefault<bool>("AddSumm", true); }
            set { AddOrUpdateValue("AddSumm", value); }
        }

        public bool AddServices
        {
            get { return GetValueOrDefault<bool>("AddServices", false); }
            set { AddOrUpdateValue("AddServices", value); }
        }

        #endregion

        #region Backup

        public string dbVersionSetting
        {
            get { return GetValueOrDefault<string>("dbVersion", "1"); }
            set { AddOrUpdateValue("dbVersion", value); }
        }

        public string LastBackupDate
        {
            get { return GetValueOrDefault<string>("LastBackup", new DateTime().ToString()); }
            set { AddOrUpdateValue("LastBackup", value); }
        }

        public bool SaveBackupOnExit
        {
            get { return GetValueOrDefault<bool>("SaveBackupOnExit", false); }
            set { AddOrUpdateValue("SaveBackupOnExit", value); }
        }

        public bool NeedToSaveBackup
        {
            get { return GetValueOrDefault<bool>("NeedToSaveBackup", false); }
            set { AddOrUpdateValue("NeedToSaveBackup", value); }
        }

        #endregion

        #region Other

        public int notificationCountSetting
        {
            get { return GetValueOrDefault<int>("notificationCount", 0); }
            set { AddOrUpdateValue("notificationCount", value); OnPropertyChanged("notificationCountSetting"); }
        }

        public int PlotType
        {
            get { return GetValueOrDefault<int>("PlotType", 0); }
            set { AddOrUpdateValue("PlotType", value); }
        }

        public int CurrentFlatId
        {
            get { return GetValueOrDefault<int>("CurrentFlatId", 0); }
            set { AddOrUpdateValue("CurrentFlatId", value); }
        }

        public int AppVersion
        {
            get { return GetValueOrDefault<int>("AppVersion", 0); }
            set { AddOrUpdateValue("AppVersion", value); }
        }

        #endregion
    }
}