using System;
using System.IO;
using System.Web.Hosting;
using System.Web.Script.Serialization;

namespace vk_search_v3.Util
{
    [Serializable]
    public class VkSettings
    {
        private const string FILE_NAME = "settings.json";
        private static VkSettings instance;

        public string DownloadPath { get; set; }

        // ReSharper disable once UnusedParameter.Local
        private VkSettings(bool newInstance)
        {
            var settingsFile = new FileInfo(FILE_NAME);

            if (settingsFile.Exists)
            {
                var fileContents = File.ReadAllText(settingsFile.FullName);
                var settings = new JavaScriptSerializer().Deserialize<VkSettings>(fileContents);

                DownloadPath = settings.DownloadPath;
            }
            else
            {
                DownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                Save();
            }
        }

        public VkSettings()
        {
            // Empty constructor for serialization
        }

        public static VkSettings GetInstance()
        {
            return instance ?? (instance = new VkSettings(true));
        }

        public void Save()
        {
            var json = new JavaScriptSerializer().Serialize(this);
            File.WriteAllText(FILE_NAME, json);
        }
    }
}