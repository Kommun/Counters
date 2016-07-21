using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Counters
{
    class BackgroundTaskManager
    {
#if WINDOWS_PHONE_APP
        public const string BackgroundTaskEntryPoint = "BackgroundTask.NotificationBackgroundTask";
#endif

#if WINDOWS_APP
        public const string BackgroundTaskEntryPoint = "BackgroundTaskWin.NotificationBackgroundTask";
#endif
        public const string BackgroundTaskName = "Communalka.Notifications";


        public static async Task RegisterBackgroundTask()
        {
            if (!IsExistsTask())
            {
                await BackgroundExecutionManager.RequestAccessAsync();

                var builder = new BackgroundTaskBuilder();

                builder.Name = BackgroundTaskName;
                builder.TaskEntryPoint = BackgroundTaskEntryPoint;
                builder.SetTrigger(new TimeTrigger(60, false));

                builder.Register();
            }
        }

        public static void UnregisterBackgroundTasks()
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
                if (cur.Value.Name == BackgroundTaskName)
                    cur.Value.Unregister(true);
        }

        public static bool IsExistsTask()
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
                if (cur.Value.Name == BackgroundTaskName)
                    return true;
            return false;
        }
    }
}
