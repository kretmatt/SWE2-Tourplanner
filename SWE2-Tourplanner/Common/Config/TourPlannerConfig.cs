using Common.Logging;
using log4net;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Config
{
    public class TourPlannerConfig : ITourPlannerConfig
    {
        private static ITourPlannerConfig tourPlannerConfig;
        private string originalConfigPath = "../../../../../config.json";
        private ILog logger;
        private TourPlannerConfig()
        {
            logger = LogHelper.GetLogHelper().GetLogger();
            LoadConfigFromFile(originalConfigPath);
        }

        public static ITourPlannerConfig GetTourPlannerConfig()
        {
            if (tourPlannerConfig == null)
            {
                tourPlannerConfig = new TourPlannerConfig();
            }
            return tourPlannerConfig;
        }

        public void LoadConfigFromFile(string configPath)
        {
            if(!File.Exists(configPath))
            {
                logger.Error("The config file could not be found and therefore the application could not be started!");
                throw new FileNotFoundException($"The config file could not be found at {Path.GetFullPath(configPath)}");
            }
            IConfiguration config = new ConfigurationBuilder().AddJsonFile(Path.GetFullPath(configPath)).Build();
            DatabaseConnectionString = $"Host={config["dbsettings:host"]};Port={config["dbsettings:port"]};Username={config["dbsettings:username"]};Password={config["dbsettings:password"]};Database={config["dbsettings:database"]};";
            MapQuestKey = $"{config["mapQuestKey"]}";
            PictureDirectory = $"{config["pictureDirectory"]}";
            ExportsDirectory = $"{config["exportsDirectory"]}";
            SetUpEnvironment();
        }

        private void SetUpEnvironment()
        {
            Directory.CreateDirectory($@"{ExportsDirectory}");
            Directory.CreateDirectory($@"{PictureDirectory}");
        }
        public string MapQuestKey { get; private set; }

        public string PictureDirectory { get; private set; }

        public string DatabaseConnectionString { get; private set; }

        public string ExportsDirectory { get; private set; }
    }
}
