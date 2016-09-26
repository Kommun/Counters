using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.Connectivity;
using Counters;
using Counters.Utils;

namespace BackgroundTaskWin
{
    public sealed class NotificationBackgroundTask : IBackgroundTask
    {
        AppSettings settings = new AppSettings();

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            try
            {
                DateTime today = DateTime.Today;
                int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
                QueryManager.Instance.OpenConnection();
                var notifications = QueryManager.Instance.Connection.Table<Notification>().ToList();
                foreach (Notification not in notifications)
                    if (DateTime.Now.TimeOfDay >= not.Date.TimeOfDay)
                        if (not.IsRepeatable)
                        {
                            if (today != not.LastDate && (not.Date.Day == today.Day || (today.Day == daysInMonth && daysInMonth < not.Date.Day)))
                            {
                                Notificate(not);
                                not.LastDate = DateTime.Today;
                                QueryManager.Instance.Connection.Update(not);
                            }
                        }
                        else
                        {
                            if (not.Date.Date == today)
                            {
                                Notificate(not);
                                QueryManager.Instance.Connection.Delete(not);
                            }
                        }
                TileHelper.RefreshTile();

                // Сохраняем резервную копию, если выставлены соответствующие настройки и есть соединение с интернетом
                if (settings.SaveBackupOnExit && settings.NeedToSaveBackup && NetworkInformation.GetInternetConnectionProfile() != null)
                {
                    var success = await (new OneDriveHelper().SaveBackup(true));
                    if (success)
                        settings.NeedToSaveBackup = false;
                }
            }
            catch { }
            deferral.Complete();
        }

        private void Notificate(Notification not)
        {
            settings.notificationCountSetting++;
            global.showToast(not.Message);
        }
    }
}
