﻿using System;
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
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Contacts;
using Windows.System;

namespace Counters
{
    public sealed partial class ExportSend : MyToolkit.Paging.MtPage
    {
        ExportParameter parameter;

        public ExportSend()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            try
            {
                var settings = Resources["settings"] as AppSettings;
                parameter = (ExportParameter)e.Parameter;

                double summ = 0;
                var message = new StringBuilder();
                var counters = parameter.ScoreId == 0 ? App.QueryManager.GetCountersWithLastData() : App.QueryManager.GetCounterDataByScoreId(parameter.ScoreId);
                foreach (QueryResult c in counters)
                {
                    string.Join<QueryResult>("/r/n", counters);
                    message.Append(c.Name);
                    if (settings.AddData)
                        message.Append("   " + c.stringDataWithDelta);
                    if (settings.AddSumm)
                        message.Append("   " + c.stringDetailSumm);
                    message.AppendLine();
                    summ += c.Summ;
                }
                if (settings.AddServices)
                {
                    var services = parameter.ScoreId == 0 ? App.QueryManager.GetServices() : App.QueryManager.GetServiceDataByScoreId(parameter.ScoreId);
                    foreach (ServiceResult s in services)
                    {
                        message.AppendLine(string.Format("{0}   {1}", s.Name, s.stringSumm));
                        summ += s.Summ;
                    }
                }

                if (message.ToString() != "")
                    message.Append(string.Format("Итого: {0} {1}", summ, settings.Currency));
                tbMessage.Text = message.ToString();
            }
            catch
            {
                Frame.GoBackAsync();
            }
        }

        private async void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!new Regex(@"^(([\w-.]+)@([\w-]+).\w+)$").Match(tbEmail.Text).Success)
                    await invalidData("Неверно введен E-mail", tbEmail);
                else
                    await Launcher.LaunchUriAsync(new Uri(string.Format("mailto:{0}?subject={1}&body={2}", tbEmail.Text, tbTitle.Text, Uri.EscapeDataString(tbMessage.Text))));
            }
            catch { }
        }

        private async Task invalidData(string message, Control tbToFocus)
        {
            await new MessageDialog(message).ShowAsync();
            tbToFocus.Focus(FocusState.Programmatic);
        }

        private async void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(ExportSettings));
        }
    }
}