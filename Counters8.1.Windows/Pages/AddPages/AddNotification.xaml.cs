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
using Windows.ApplicationModel.Background;

namespace Counters
{
    public sealed partial class AddNotification : MyToolkit.Paging.MtPage
    {
        Notification currentNotification;

        public AddNotification()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                currentNotification = e.Parameter as Notification;
                spNotification.DataContext = currentNotification;
                dpDay.Date = currentNotification.Date;
                tpTime.Time = currentNotification.Date.TimeOfDay;
            }
            else tpTime.Time = new TimeSpan(DateTime.Now.Hour, 0, 0);
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (tbMessage.Text == "")
            {
                await new MessageDialog("Введите текст напоминания").ShowAsync();
                tbMessage.Focus(FocusState.Programmatic);
            }
            else
            {
                var newNotification = new Notification()
                {
                    Date = dpDay.Date.Date.Add(tpTime.Time),
                    Message = tbMessage.Text,
                    IsRepeatable = tsRepeat.IsOn
                };

                if (currentNotification != null)
                {
                    newNotification.Id = currentNotification.Id;
                    App.QueryManager.Connection.Update(newNotification);
                }
                else
                    App.QueryManager.Connection.Insert(newNotification);

                await Frame.GoBackAsync();
            }
        }
    }
}
