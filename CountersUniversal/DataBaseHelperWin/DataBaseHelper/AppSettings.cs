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

        public AppSettings()
        {
            settings = ApplicationData.Current.LocalSettings;
        }

        public void AddOrUpdateValue(string Key, Object value)
        {
            if (settings.Values[Key] != value)
                settings.Values[Key] = value;
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

        public bool isRememberOn
        {
            get { return GetValueOrDefault<bool>("remember", true); }
            set
            {
                if (value)
                    BackgroundTaskManager.RegisterBackgroundTask();
                else
                    BackgroundTaskManager.UnregisterBackgroundTasks();
                AddOrUpdateValue("remember", value);
                OnPropertyChanged("isRememberOn");
            }
        }

        public string Currency
        {
            get { return GetValueOrDefault<string>("Currency", "руб"); }
            set
            {
                AddOrUpdateValue("Currency", value);
                OnPropertyChanged("Currency");
            }
        }

        public int notificationCountSetting
        {
            get { return GetValueOrDefault<int>("notificationCount", 0); }
            set
            {
                AddOrUpdateValue("notificationCount", value);
                OnPropertyChanged("notificationCountSetting");
            }
        }

        public string dbVersionSetting
        {
            get { return GetValueOrDefault<string>("dbVersion", "1"); }
            set
            {
                AddOrUpdateValue("dbVersion", value);
                OnPropertyChanged("dbVersionSetting");
            }
        }

        public bool isFirstLaunchSetting
        {
            get { return GetValueOrDefault<bool>("isFirstLaunch", true); }
            set
            {
                AddOrUpdateValue("isFirstLaunch", value);
                OnPropertyChanged("isFirstLaunchSetting");
            }
        }

        public bool isFirstMessageSetting
        {
            get { return GetValueOrDefault<bool>("isFirstMessage", true); }
            set
            {
                AddOrUpdateValue("isFirstMessage", value);
                OnPropertyChanged("isFirstMessageSetting");
            }
        }

        public string emailSetting
        {
            get { return GetValueOrDefault<string>("email", ""); }
            set
            {
                AddOrUpdateValue("email", value);
                OnPropertyChanged("emailSetting");
            }
        }

        public string mailTitleSetting
        {
            get { return GetValueOrDefault<string>("mailTitle", ""); }
            set
            {
                AddOrUpdateValue("mailTitle", value);
                OnPropertyChanged("mailTitleSetting");
            }
        }

        public string phoneSetting
        {
            get { return GetValueOrDefault<string>("phone", ""); }
            set
            {
                AddOrUpdateValue("phone", value);
                OnPropertyChanged("phoneSetting");
            }
        }

        public int Runs
        {
            get { return GetValueOrDefault<int>("Runs", 0); }
            set
            {
                AddOrUpdateValue("Runs", value);
                OnPropertyChanged("Runs");
            }
        }

        public bool NotRated
        {
            get { return GetValueOrDefault<bool>("NotRated", true); }
            set
            {
                AddOrUpdateValue("NotRated", value);
                OnPropertyChanged("NotRated");
            }
        }

        public bool IsFullVersion
        {
            get { return GetValueOrDefault<bool>("IsFullVersion", false); }
            set
            {
                AddOrUpdateValue("IsFullVersion", value);
                OnPropertyChanged("IsFullVersion");
            }
        }

        public bool AllowOrientation
        {
            get { return GetValueOrDefault<bool>("AllowOrientation", true); }
            set
            {
                Windows.Graphics.Display.DisplayInformation.AutoRotationPreferences = value ? Windows.Graphics.Display.DisplayOrientations.None : Windows.Graphics.Display.DisplayOrientations.Portrait;

                AddOrUpdateValue("AllowOrientation", value);
                OnPropertyChanged("AllowOrientation");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler == null) return;
            handler(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}
