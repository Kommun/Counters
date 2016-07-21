using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.UI.Xaml.Controls;
using Microsoft.Live;
using Windows.UI.Popups;
using Windows.Storage;

namespace Counters
{
    public static class LiveLogin
    {
        private static LiveConnectClient _client;

        public static async Task<LiveConnectClient> GetClient(bool background = false)
        {
            if (_client == null)
            {
                LiveLoginResult res;
                if (background)
                    res = await new LiveAuthClient().InitializeAsync(new string[] { "wl.signin", "wl.basic", "wl.skydrive", "wl.skydrive_update" });
                else
                    res = await new LiveAuthClient().LoginAsync(new string[] { "wl.signin", "wl.basic", "wl.skydrive", "wl.skydrive_update" });

                if (res.Session != null)
                    _client = new LiveConnectClient(res.Session);
            }

            return _client;
        }

        public static async Task<bool> UploadToOnedrive(string folderName, string fileName, Stream stream, int maxFilesCount, bool background = false)
        {
            try
            {
                LiveConnectClient client = await GetClient(background);
                if (client == null)
                    return false;

                var path = await FindAndCreateDirectoryAsync(new List<string> { "Коммуналка", folderName }, true);
                await DeleteFiles(path, maxFilesCount - 1);
                await client.BackgroundUploadAsync(path, fileName, stream.AsInputStream(), OverwriteOption.Overwrite);
                stream.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<List<SkyDriveFile>> GetOrderedBackups()
        {
            try
            {
                LiveConnectClient client = await GetClient();
                if (client == null)
                    return null;

                string path = await LiveLogin.FindAndCreateDirectoryAsync(new List<string> { "Коммуналка", "Резервные копии" }, false);
                if (path == null)
                    return null;

                var files = await GetFiles(path);
                if (files.Count == 0)
                    return null;

                return files.OrderByDescending(f => f.CreationDate).ToList();
            }
            catch
            {
                return null;
            }
        }

        public static async Task<bool> DownloadBackup(string path)
        {
            try
            {
                LiveConnectClient client = await GetClient();
                if (client == null)
                    return false;

                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("dbCounters.sqlite", CreationCollisionOption.ReplaceExisting);
                var download = await client.CreateBackgroundDownloadAsync(path + "/content", file);
                await download.StartAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static async Task<List<SkyDriveFile>> GetFiles(string path)
        {
            LiveOperationResult result = await _client.GetAsync(path + "/files");
            var fileList = new List<SkyDriveFile>();
            dynamic data = result.Result;
            foreach (var dd in data)
            {
                foreach (var d in dd.Value)
                {
                    fileList.Add(new SkyDriveFile(d));
                }
            }
            return fileList;
        }

        private static async Task DeleteFiles(string path, int maxCount)
        {
            var files = await GetFiles(path);
            if (files.Count > maxCount)
            {
                files = files.OrderByDescending(f => f.CreationDate).ToList();
                files.RemoveRange(0, maxCount);
                foreach (var file in files)
                    await _client.DeleteAsync(file.ID);
            }
        }

        private async static Task<string> FindAndCreateDirectoryAsync(List<string> path, bool CreateMode)
        {
            string folderId = "me/skydrive";
            foreach (string dir in path)
            {
                string currentFolderId = null;
                // Retrieves all the directories.
                var queryFolder = folderId + "/files";
                var opResult = await _client.GetAsync(queryFolder);
                dynamic result = opResult.Result;

                foreach (dynamic folder in result.data)
                {
                    // Checks if current folder has the passed name.
                    if (folder.name.ToLowerInvariant() == dir.ToLowerInvariant())
                    {
                        currentFolderId = folder.id;
                        break;
                    }
                }

                if (currentFolderId == null && CreateMode)
                {
                    // Directory hasn't been found, so creates it using the PostAsync method.
                    var folderData = new Dictionary<string, object>();
                    folderData.Add("name", dir);
                    opResult = await _client.PostAsync(folderId, folderData);
                    result = opResult.Result;

                    // Retrieves the id of the created folder.
                    currentFolderId = result.id;
                }
                folderId = currentFolderId;
            }

            return folderId;
        }
    }

    public class SkyDriveFile
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }

        public SkyDriveFile(dynamic fileData)
        {
            Name = fileData.name;
            ID = fileData.id;
            CreationDate = Convert.ToDateTime(fileData.created_time);
        }
    }
}
