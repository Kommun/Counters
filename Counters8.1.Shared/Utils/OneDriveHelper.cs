﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.Networking.Connectivity;
using Counters.Utils;

namespace Counters
{
	public class OneDriveHelper
	{
		private AppSettings _settings;

		public OneDriveHelper()
		{
			_settings = new AppSettings();
		}

		private async Task OneDriveOperation(Func<Task> action, string message)
		{
			if (NetworkInformation.GetInternetConnectionProfile() == null)
			{
				await new MessageDialog("Отстутствует интернет-соединение").ShowAsync();
				return;
			}
			var popup = new Popup { IsLightDismissEnabled = false };
			var sp = new StackPanel
			{
				VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
				HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center
			};
			sp.Children.Add(new ProgressBar { IsIndeterminate = true });
			sp.Children.Add(new TextBlock
			{
				FontSize = 20,
				Text = message,
				Foreground = new SolidColorBrush(Windows.UI.Colors.White)
			});
			var grd = new Grid
			{
				Width = Window.Current.Bounds.Width,
				Height = Window.Current.Bounds.Height,
			};
			grd.Children.Add(new Grid { Background = new SolidColorBrush(Windows.UI.Colors.Black), Opacity = 0.9 });
			grd.Children.Add(sp);
			popup.Child = grd;
			popup.IsOpen = true;

			await action();

			popup.IsOpen = false;
		}

		public async Task SaveBackupAsync()
		{
			await OneDriveOperation(async () =>
			{
				var success = await SaveBackup();
				await new MessageDialog(success ? "Резервная копия сохранена" : "Не удалось сохранить резервную копию").ShowAsync();
			}, "Создание резервной копии...");
		}

		public async Task<bool> SaveBackup(bool background = false)
		{
			bool success = false;

			try
			{
				//Сохраняем в настройки текущую версию базы данных
				QueryManager.Instance.UpdatePropertyValue("Version", QueryManager.dbVersion);

				StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
				StorageFile storageFile = await applicationFolder.GetFileAsync("dbCounters.sqlite");
				var backupName = string.Format("Backup {0}", DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss"));
				var tmpFile = await storageFile.CopyAsync(ApplicationData.Current.TemporaryFolder,
					storageFile.Name, NameCollisionOption.GenerateUniqueName);
				success = await LiveLogin.UploadToOnedrive("Резервные копии", backupName, await tmpFile.OpenStreamForReadAsync(), 5, background);

				//Сохраняем дату последней резервной копии
				if (success)
					_settings.LastBackupDate = DateTime.Now.ToString();
			}
			catch { }

			return success;
		}

		public async Task RestoreAsync()
		{
			await OneDriveOperation(async () =>
			{
				bool restored = false;
				SkyDriveFile lastBackup = null;

				MessageDialog msgbox = new MessageDialog("Текущие данные будут заменены данными из резервной копии. Продолжить?");

				msgbox.Commands.Add(new UICommand("Да", null, 0));
				msgbox.Commands.Add(new UICommand("Нет", null, 1));
				var msgRes = await msgbox.ShowAsync();

				if ((int?)msgRes?.Id != 0)
					return;

				try
				{
					QueryManager.Instance.Connection.Close();

					lastBackup = (await LiveLogin.GetOrderedBackups())?.FirstOrDefault();
					if (lastBackup != null)
						restored = await LiveLogin.DownloadBackup(lastBackup.ID);
				}
				catch (Exception ex)
				{
					new MessageDialog(ex.Message).ShowAsync();
				}

				QueryManager.Instance.OpenConnection();

				if (!restored)
				{
					await new MessageDialog("Не удалось восстановить резервную копию данных").ShowAsync();
					return;
				}

				//Сохраняем дату последней резервной копии
				_settings.LastBackupDate = lastBackup.CreationDate.ToString();
				_settings.dbVersionSetting = QueryManager.Instance.GetPropertyValue("Version", "1");
				await new MessageDialog(QueryManager.Instance.RefreshDbScheme() ? "Данные успешно восстановлены из резервной копии" : "Во время восстановления данных произошла ошибка").ShowAsync();

			}, "Восстановление данных...");
		}

		public async Task ExportToCsvAsync()
		{
			await OneDriveOperation(async () =>
			{
				try
				{
					string currentName = "";
					var message = new StringBuilder();
					var query = QueryManager.Instance.ExportData();
					message.AppendLine("Название;Тариф;Дата;Показания;Стоимость");
					foreach (QueryResult qr in query)
					{
						if (currentName != qr.Name)
							message.AppendLine();
						message.AppendLine(string.Format("{0};{1};{2};{3};{4}", qr.Name == currentName ? "" : qr.Name, qr.stringTariff, qr.Date.ToString("dd.MM.yyyy"), qr.stringDataWithDelta, qr.stringFullSumm));
						currentName = qr.Name;
					}

					bool success = await LiveLogin.UploadToOnedrive("Показания", string.Format("Показания({0}).csv", DateTime.Today.ToString("dd-MMM-yy")),
							new MemoryStream(Portable.Text.Encoding.GetEncoding(1251).GetBytes(message.ToString())), 10);

					await new MessageDialog(success ? "Файл успешно загружен в OneDrive" : "Не удалось сохранить файл").ShowAsync();
				}
				catch (Exception ex)
				{
					new MessageDialog(ex.Message).ShowAsync();
				}
			}, "Экспорт показаний...");
		}
	}
}
