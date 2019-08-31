using System.IO;
using Newtonsoft.Json;

namespace SubtitleManager.Services
{
    public class ConfigurationService
    {
        private const string ConfigFilePath = "./config.json";
        private ConfigData ConfigData { get; }

        public ConfigurationService()
        {
            string json = string.Empty;
            if (File.Exists(ConfigFilePath))
            {
                json = File.ReadAllText(ConfigFilePath);
            }

            this.ConfigData = JsonConvert.DeserializeObject<ConfigData>(json);
        }

        public string LastOpenedPath
        {
            get => this.ConfigData.LastOpenedPath;
            set { this.ConfigData.LastOpenedPath = value; this.Save(); }
        }

        private void Save()
        {
            string json = JsonConvert.SerializeObject(this.ConfigData);
            File.WriteAllText(ConfigFilePath, json);
        }
    }

    public class ConfigData
    {
        public string LastOpenedPath { get; set; }
    }
}
