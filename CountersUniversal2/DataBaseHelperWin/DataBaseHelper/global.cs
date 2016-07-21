using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml.Controls;

namespace Counters
{
    public static class global
    {
        public static string db_path = System.IO.Path.Combine(System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "dbCounters.sqlite"));
        public static SQLiteConnection db;
        public const string dbVersion = "1.8";
        public static AppSettings settings = new AppSettings();

        public static void openConnection()
        {
            db = new SQLiteConnection(db_path);
        }

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

        public static bool refreshApp()
        {
            try
            {
                while (settings.dbVersionSetting != dbVersion)
                    switch (settings.dbVersionSetting)
                    {
                        case "1.7":
                            var query2 = db.Query<Score>("select * from Score");
                            db.DropTable<Score>();
                            db.CreateTable<Score>();
                            foreach (Score s in query2)
                                db.Execute(string.Format("insert into Score Values ({0},'{1}','{2}','{3}','{4}')",
                                    s.ScoreId, s.Data.ToString().Replace(',', '.'), s.Date.Date.ToString("yyyy-MM-dd ss:mm:hh"), s.lstServiceTarifs, ""));

                            settings.dbVersionSetting = "1.8";
                            break;
                        case "1.6":
                            var query22 = db.Query<Counter>("select * from Counter");
                            db.DropTable<Counter>();
                            db.CreateTable<Counter>();
                            foreach (Counter c in query22)
                                db.Execute(string.Format("insert into Counter Values ({0},{1},'{2}',{3},{4})",
                                   c.CounterId, c.TypeId, c.Name, c.TarifId, 0));

                            settings.dbVersionSetting = "1.7";
                            break;
                        case "1.5":
                            var tmpData2 = db.Query<CounterData>("select * from CounterData");
                            global.db.DropTable<CounterData>();
                            global.db.CreateTable<CounterData>();
                            foreach (CounterData cd in tmpData2)
                            {
                                db.Execute(string.Format("Insert Into CounterData Values({0},{1},{2},{3},'{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                                  cd.DataId, cd.CounterId, cd.TariffId, cd.ScoreId, cd.Data.ToString().Replace(',', '.'), cd.DataODN.ToString().Replace(',', '.'),
                                  cd.Date.Date.ToString("yyyy-MM-dd ss:mm:hh"), cd.Delta.ToString().Replace(',', '.'), cd.Summ.ToString().Replace(',', '.'), cd.SummODN.ToString().Replace(',', '.'),
                                  cd.stringSumm == null ? "" : cd.stringSumm.IndexOf('+') != -1 ? cd.stringSumm.Replace("руб", "").Trim() : ""));
                            }

                            var query12 = global.db.Table<Score>().ToList();
                            foreach (Score s in query12)
                            {
                                if (s.lstDatas != null && s.lstDatas != "")
                                    global.db.Execute(string.Format("update CounterData set ScoreId={0} where DataId in({1})", s.ScoreId, s.lstDatas));
                            }

                            settings.dbVersionSetting = "1.6";
                            break;
                        default:
                            global.db.DropTable<Counter>();
                            global.db.DropTable<CounterData>();
                            global.db.DropTable<Service>();
                            global.db.DropTable<Score>();
                            global.db.DropTable<Notification>();
                            global.db.DropTable<Tarif>();
                            global.db.DropTable<Property>();

                            global.db.CreateTable<Counter>();
                            global.db.CreateTable<CounterData>();
                            global.db.CreateTable<Service>();
                            global.db.CreateTable<Score>();
                            global.db.CreateTable<Notification>();
                            global.db.CreateTable<Tarif>();
                            global.db.CreateTable<Property>();
                            return false;
                    }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
