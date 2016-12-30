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
using Windows.UI.Popups;

namespace Counters
{

    public sealed partial class Scores : MyToolkit.Paging.MtPage
    {
        Score selectedScore;
        AppSettings settings = new AppSettings();

        public Scores()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            refreshScores();
        }

        private void refreshScores()
        {
            lbScores.ItemsSource = App.QueryManager.GetScores();
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (App.QueryManager.DataSourcesCount() == 0)
                await new MessageDialog("Для начала добавьте хотя бы один счетчик или услугу").ShowAsync();
            else
                await Frame.NavigateAsync(typeof(AddScore), new AddScoreParameter { Change = true });
        }

        private async void grdData_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(AddScore), new AddScoreParameter { ScoreId = ((sender as Grid).DataContext as Score).ScoreId });
        }

        private void grdData_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase.ShowAttachedFlyout(senderElement);
            selectedScore = senderElement.DataContext as Score;
        }

        private async void btnChange_Click(object sender, RoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(AddScore), new AddScoreParameter { ScoreId = selectedScore.ScoreId, Change = true });
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgbox = new MessageDialog("Вы действительно хотите удалить счет? Все связанные показания также будут удалены.");

            msgbox.Commands.Add(new UICommand("Да", c =>
            {
                if (selectedScore != null)
                {
                    App.QueryManager.DeleteScore(selectedScore.ScoreId);
                    refreshScores();
                }
            }));
            msgbox.Commands.Add(new UICommand("Нет"));
            await msgbox.ShowAsync();
        }

        private async void btnSendByMail_Click(object sender, RoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(ExportSend), new ExportParameter { Type = 0, ScoreId = selectedScore.ScoreId });
        }
    }
}
