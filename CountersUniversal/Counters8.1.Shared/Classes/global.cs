using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml.Controls;
using Windows.Networking.PushNotifications;
using Microsoft.WindowsAzure.Messaging;

namespace Counters
{
    public static class global
    {
        public static AppSettings settings = new AppSettings();

        public static void refreshTile()
        {
            try
            {
                var badgeXML = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
                var badge = badgeXML.SelectSingleNode("/badge") as XmlElement;
                badge.SetAttribute("value", settings.notificationCountSetting.ToString());
                var badgeNotification = new BadgeNotification(badgeXML);
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeNotification);
            }
            catch { }
        }

        public static void showToast(string message)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("Коммуналка"));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(message));
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "long");
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public static async void InitNotificationsAsync(List<string> tags)
        {
            try
            {
                if (settings.IsFullVersion)
                    tags.Add("FullVersion");
                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                var hub = new NotificationHub("CommunalkaPushHub",
                    "Endpoint=sb://communalkapushhub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=ke8Shcfe82r3Ox4ZjTg/5gwvlYP1EbXZyIMU8URXBi4=");

                await hub.RegisterNativeAsync(channel.Uri, tags);
            }
            catch { }
        }      
    }
}
