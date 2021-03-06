﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Counters
{
    public sealed partial class Export : MyToolkit.Paging.MtPage
    {
        private OneDriveHelper _oneDriveHelper;
        private Task _task;

        public Export()
        {
            this.InitializeComponent();
            _oneDriveHelper = new OneDriveHelper();
        }

        private async void btnExport_Click(object sender, RoutedEventArgs e)
        {
            switch (((sender as Button).DataContext as MenuItem).Type)
            {
                case 0:
                    await Frame.NavigateAsync(typeof(ExportSend), new ExportParameter { Type = 0 });
                    break;
                case 1:
                    await Frame.NavigateAsync(typeof(ExportSend), new ExportParameter { Type = 1 });
                    break;
                case 2:
                    _task = _oneDriveHelper.ExportToCsvAsync();
                    await _task;
                    break;
                case 3:
                    _task = _oneDriveHelper.SaveBackupAsync();
                    await _task;
                    break;
                case 4:
                    _task = _oneDriveHelper.RestoreAsync();
                    await _task;
                    break;
            }
        }

        protected async override Task OnNavigatingFromAsync(MyToolkit.Paging.MtNavigatingCancelEventArgs args)
        {
            if (_task != null && !_task.IsCompleted)
                args.Cancel = true;
        }
    }
}
