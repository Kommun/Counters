using System;
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
using Windows.UI.Popups;
using Windows.Storage;
using Windows.Networking.Connectivity;

namespace Counters
{
    public sealed partial class Export : MyToolkit.Paging.MtPage
    {
        private OneDriveHelper _oneDriveHelper;

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

                case 2:
                    await _oneDriveHelper.ExportToCsvAsync();
                    break;
                case 3:
                    await _oneDriveHelper.SaveBackupAsync();
                    break;
                case 4:
                    await _oneDriveHelper.RestoreAsync();
                    break;
            }
        }
    }
}
