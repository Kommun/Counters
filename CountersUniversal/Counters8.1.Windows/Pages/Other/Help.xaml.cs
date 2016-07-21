using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Counters
{
    public sealed partial class Help : MyToolkit.Paging.MtPage
    {
        public Help()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            var folder = await Package.Current.InstalledLocation.GetFolderAsync("Resources");
            var file = (await folder.GetFilesAsync()).FirstOrDefault();

            if (file != null)
                tbHelp.Text = await FileIO.ReadTextAsync(file);
            else
            {
                await new MessageDialog("Не удалось загрузить файл справки").ShowAsync();
                await Frame.GoBackAsync();
            }
        }
    }
}
