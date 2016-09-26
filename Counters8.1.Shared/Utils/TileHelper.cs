using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.UI.StartScreen;

namespace Counters.Utils
{
    public static class TileHelper
    {
        /// <summary>
        /// Закрепить плитку на рабочем столе
        /// </summary>
        /// <returns></returns>
        public async static Task Pin(Flat flat)
        {
            var tileToUpdate = (await SecondaryTile.FindAllForPackageAsync()).FirstOrDefault(t => t.TileId == flat.FlatId.ToString());

            if (tileToUpdate != null)
                await tileToUpdate.RequestDeleteAsync();


            var secondaryTile = new SecondaryTile()
            {
                TileId = flat.FlatId.ToString(),
                DisplayName = flat.Name,
                Arguments = $"tile://{flat.FlatId}"
            };
            secondaryTile.VisualElements.Square150x150Logo = new Uri("ms-appx:///Assets/Logo.png");
            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
            await secondaryTile.RequestCreateAsync();
        }

        /// <summary>
        /// Обновить счетчик уведомлений на плитке
        /// </summary>
        public static void RefreshTile()
        {
            try
            {
                var badgeXML = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
                var badge = badgeXML.SelectSingleNode("/badge") as XmlElement;
                badge.SetAttribute("value", new AppSettings().notificationCountSetting.ToString());
                var badgeNotification = new BadgeNotification(badgeXML);
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeNotification);
            }
            catch { }
        }
    }
}
