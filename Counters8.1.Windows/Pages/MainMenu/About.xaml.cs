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
using Windows.ApplicationModel.Store;
using Windows.ApplicationModel;

#if WINDOWS_PHONE_APP
using Windows.ApplicationModel.Email;
#endif

#if WINDOWS_APP
using Windows.System;
#endif

namespace Counters
{
    public sealed partial class About : MyToolkit.Paging.MtPage
    {
        AppSettings settings = new AppSettings();

        public About()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            var version = Package.Current.Id.Version;
            tbVersion.Text = string.Format("версия: {0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        private async void btnFeedback_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
            settings.NotRated = false;
        }

        private async void btnMail_Click(object sender, RoutedEventArgs e)
        {
#if WINDOWS_PHONE_APP
            EmailMessage mail = new EmailMessage();
            mail.Subject = "Отзыв на приложение \"Коммуналка\"";
            mail.To.Add(new EmailRecipient("f11kostya@hotmail.com"));

            await EmailManager.ShowComposeNewEmailAsync(mail);
#endif

#if WINDOWS_APP
            await Launcher.LaunchUriAsync(new Uri("mailto:f11kostya@hotmail.com?subject=Отзыв на приложение Коммуналка"));
#endif
        }

        private async void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            await UpdateManager.ShowUpdateMessage(false);
        }

        private async void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(Help));
        }
    }
}
