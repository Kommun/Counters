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
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.Contacts;

namespace Counters
{
    public sealed partial class ExportSend : MyToolkit.Paging.MtPage
    {
        AppSettings _settings;
        ExportParameter parameter;

        public ExportSend()
        {
            this.InitializeComponent();
            _settings = Resources["settings"] as AppSettings;
        }

        protected async override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            try
            {
                parameter = (ExportParameter)e.Parameter;
                switch (parameter.Type)
                {
                    case 0:
                        spMail.Visibility = Visibility.Visible;
                        break;
                    case 1:
                        if (_settings.isFirstMessageSetting)
                        {
                            await new MessageDialog("Внимание! Отправка SMS является платной услугой. Со счета будут списаны средства согласно тарифному плану мобильного оператора.").ShowAsync();
                            _settings.isFirstMessageSetting = false;
                        }
                        spSMS.Visibility = Visibility.Visible;
                        break;
                }

                if (_settings.TemplateType == 0)
                    tbMessage.Text = GenerateDefaultReport();
                else
                    tbMessage.Text = GenerateReportByTemplate();
            }
            catch
            {
                Frame.GoBackAsync();
            }
        }

        /// <summary>
        /// Сформировать отчет по стандартному шаблону
        /// </summary>
        /// <returns></returns>
        private string GenerateDefaultReport()
        {
            double summ = 0;
            var message = new StringBuilder();

            var counters = parameter.ScoreId == 0 ? App.QueryManager.GetCountersWithLastData() : App.QueryManager.GetCounterDataByScoreId(parameter.ScoreId);
            foreach (QueryResult c in counters)
            {
                string.Join<QueryResult>("/r/n", counters);
                message.Append(c.Name);
                if (_settings.AddData)
                    message.Append("   " + c.stringDataWithDelta);
                if (_settings.AddSumm)
                    message.Append("   " + c.stringDetailSumm);
                message.AppendLine();
                summ += c.Summ;
            }
            if (_settings.AddServices)
            {
                var services = parameter.ScoreId == 0 ? App.QueryManager.GetServices() : App.QueryManager.GetServiceDataByScoreId(parameter.ScoreId);
                foreach (ServiceResult s in services)
                {
                    message.AppendLine(string.Format("{0}   {1}", s.Name, s.stringSumm));
                    summ += s.Summ;
                }
            }

            if (message.ToString() != "")
                message.Append(string.Format("Итого: {0} {1}", summ, _settings.Currency));

            return message.ToString();
        }

        /// <summary>
        /// Сформировать отчет по пользовательскому шаблону
        /// </summary>
        /// <returns></returns>
        private string GenerateReportByTemplate()
        {
            var message = _settings.Template;

            var counters = parameter.ScoreId == 0 ? App.QueryManager.GetCountersWithLastData() : App.QueryManager.GetCounterDataByScoreId(parameter.ScoreId);
            foreach (QueryResult c in counters)
            {
                message = Regex.Replace(message, string.Format("{{{0}:п}}", c.Name), c.stringData, RegexOptions.IgnoreCase);
                message = Regex.Replace(message, string.Format("{{{0}:пп}}", c.Name), c.stringDataWithDelta, RegexOptions.IgnoreCase);
                message = Regex.Replace(message, string.Format("{{{0}:с}}", c.Name), c.stringFullSumm, RegexOptions.IgnoreCase);
                message = Regex.Replace(message, string.Format("{{{0}:дс}}", c.Name), c.stringDetailSumm, RegexOptions.IgnoreCase);
            }

            var services = parameter.ScoreId == 0 ? App.QueryManager.GetServices() : App.QueryManager.GetServiceDataByScoreId(parameter.ScoreId);
            foreach (ServiceResult s in services)
                message = Regex.Replace(message, string.Format("{{{0}:с}}", s.Name), s.stringSumm, RegexOptions.IgnoreCase);

            return message;
        }

        private async void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (parameter.Type)
                {
                    case 0:
                        if (!new Regex(@"^(([\w-.]+)@([\w-]+).\w+)$").Match(tbEmail.Text).Success)
                            await invalidData("Неверно введен E-mail", tbEmail);
                        else
                        {
                            EmailMessage mail = new EmailMessage();
                            mail.Subject = tbTitle.Text;
                            mail.Body = tbMessage.Text;
                            mail.To.Add(new EmailRecipient(tbEmail.Text));

                            await EmailManager.ShowComposeNewEmailAsync(mail);
                        }
                        break;

                    case 1:
                        if (!new Regex(@"^(\+?\d+)$").Match(tbPhone.Text).Success)
                            await invalidData("Неверно введен телефон", tbPhone);
                        else
                        {
                            ChatMessage message = new ChatMessage();
                            message.Body = tbMessage.Text;
                            message.Recipients.Add(tbPhone.Text);

                            await ChatMessageManager.ShowComposeSmsMessageAsync(message);
                        }
                        break;
                }
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

        private async void btnSelectContact_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                var contactPicker = new ContactPicker();
                contactPicker.DesiredFieldsWithContactFieldType.Add(parameter.Type == 0 ? ContactFieldType.Email : ContactFieldType.PhoneNumber);
                Contact contact = await contactPicker.PickContactAsync();
                if (contact != null)
                {
                    if (parameter.Type == 0)
                        tbEmail.Text = contact.Emails[0].Address;
                    else
                        tbPhone.Text = contact.Phones[0].Number;
                }
            }
            catch { }
        }
    }
}
