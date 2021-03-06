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
using Windows.Media.Capture;
using Windows.Devices.Enumeration;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Media.MediaProperties;

namespace Counters
{
    public sealed partial class AddData : MyToolkit.Paging.MtPage
    {
        QueryResult currentCounter;
        bool isFlashlightOn;
        AddDataParameter parameter;
        MediaCapture mc;

        public AddData()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            try
            {
                parameter = e.Parameter as AddDataParameter;
                currentCounter = App.QueryManager.GetCounterWithData(parameter.CounterId, parameter.DataId);

                if (parameter.ScoreId != 0 && currentCounter.EnableODN)
                    grdODN.Visibility = Visibility.Visible;
                tbTarif.Text = currentCounter.stringTariff;

                tbDate.MinYear = new DateTime(2001, 1, 1);
                if (parameter.DataId != 0)
                {
                    tbDate.Date = currentCounter.Date;
                    tbData.Text = currentCounter.Data.ToString();
                    tbDataODN.Text = currentCounter.DataODN.ToString();
                }
                else currentCounter.Date = tbDate.Date.Date;

                grdLastData.DataContext = currentCounter.PreviousData;

                Tag = currentCounter.Name;
            }
            catch
            {
                Frame.GoBackAsync();
            }
        }

        protected override void OnNavigatingFrom(MyToolkit.Paging.MtNavigatingCancelEventArgs args)
        {
            if (isFlashlightOn)
                flashlightChangeState();
        }

        private void tbDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            currentCounter.Date = e.NewDate.Date;
            grdLastData.DataContext = currentCounter.PreviousData;
        }

        private async void btnChangeTarif_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof(AddCounter), new AddCounterParameter()
            {
                CounterId = currentCounter.CounterId,
                DataId = parameter.DataId,
                ChangeTarif = true
            });
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            List<CounterData> datas;
            double data, odn;
            // Если значение начинается со знака "+" - введена разница в показаниях
            bool isDelta = !string.IsNullOrEmpty(tbData.Text) && tbData.Text[0] == '+';

            if (App.QueryManager.IsCounterDatasByDateExist(currentCounter))
                invalidData("Показания с указанной датой уже имеются в базе", tbData);
            else if (!double.TryParse(tbData.Text.Replace('.', ','), out data))
                invalidData("Поле содержит недопустимые символы", tbData);
            else if (!double.TryParse(tbDataODN.Text.Replace('.', ','), out odn))
                invalidData("Поле содержит недопустимые символы", tbDataODN);
            else if (currentCounter.IsFirst
                && (datas = App.QueryManager.DatasBeforeDate(currentCounter.CounterId, tbDate.Date.Date.ToString("yyyy-MM-dd HH:mm:ss"), false)).Count > 0)
                invalidData($"Дата начальных показаний должна быть меньше {datas.First().Date.ToString("dd.MM.yyyy")}", tbDate);
            else if (!currentCounter.IsFirst
                && (datas = App.QueryManager.DatasBeforeDate(currentCounter.CounterId, tbDate.Date.Date.ToString("yyyy-MM-dd HH:mm:ss"), true)).Count == 0)
                invalidData("Дата не может быть меньше даты начальных показаний", tbDate);
            else if (!isDelta && data < currentCounter.PreviousData?.Data)
                invalidData("Данные показания меньше предыдущих", tbData);
            else
            {
                CounterData counterData;
                if (parameter.DataId != 0)
                    counterData = App.QueryManager.Connection.Get<CounterData>(parameter.DataId);
                else
                    counterData = new CounterData()
                    {
                        CounterId = parameter.CounterId,
                        ScoreId = parameter.ScoreId,
                        TariffId = currentCounter.TariffId
                    };

                counterData.Data = isDelta ? currentCounter.PreviousData.Data + data : data;
                counterData.DataODN = odn;
                counterData.Date = tbDate.Date.Date;

                // Редактирование показаний
                if (parameter.DataId != 0)
                {
                    counterData.DataId = parameter.DataId;
                    App.QueryManager.Connection.Update(counterData);
                }
                // Добавление показаний
                else
                    App.QueryManager.Connection.Insert(counterData);

                await Frame.GoBackAsync();
            }
        }

        private async void invalidData(string message, Control tbToFocus)
        {
            await new MessageDialog(message).ShowAsync();
            tbToFocus.Focus(FocusState.Programmatic);
        }

        private void btnLamp_Click(object sender, RoutedEventArgs e)
        {
            flashlightChangeState();
        }

        private async void flashlightChangeState()
        {
            try
            {
                if (!isFlashlightOn)
                {
                    mc = new MediaCapture();
                    var cameraID = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture))
                                    .FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back);
                    if (cameraID == null)
                        return;

                    await mc.InitializeAsync(new MediaCaptureInitializationSettings
                    {
                        StreamingCaptureMode = StreamingCaptureMode.Video,
                        PhotoCaptureSource = PhotoCaptureSource.VideoPreview,
                        AudioDeviceId = string.Empty,
                        VideoDeviceId = cameraID.Id
                    });


                    var torch = mc.VideoDeviceController.TorchControl;
                    if (torch.Supported)
                    {
                        isFlashlightOn = true;

                        // Start Preview
                        var captureElement = new CaptureElement();
                        captureElement.Source = mc;
                        await mc.StartPreviewAsync();

                        // Prep for video recording
                        // Get Application temporary folder to store temporary video file folder
                        StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;

                        // Create a temp Flash folder 
                        var folder = await tempFolder.CreateFolderAsync("TempFlashlightFolder", CreationCollisionOption.OpenIfExists);

                        // Create video encoding profile as MP4 
                        var videoEncodingProperties = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);

                        // Create new unique file in the Flash folder and record video
                        var videoStorageFile = await folder.CreateFileAsync("TempFlashlightFile", CreationCollisionOption.GenerateUniqueName);

                        // Start recording
                        await mc.StartRecordToStorageFileAsync(videoEncodingProperties, videoStorageFile);

                        torch.Enabled = isFlashlightOn;
                    }
                    else
                    {
                        await new MessageDialog("Фонарик не поддерживается данным устройством").ShowAsync();
                        mc.Dispose();
                    }
                }
                else
                {
                    isFlashlightOn = false;
                    mc.VideoDeviceController.TorchControl.Enabled = isFlashlightOn;
                    mc.Dispose();
                }
            }
            catch { }
        }
    }
}
