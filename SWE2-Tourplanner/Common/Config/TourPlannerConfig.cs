using Common.Logging;
using log4net;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Exceptions;

namespace Common.Config
{
    /// <summary>
    /// TourPlannerConfig is a concrete implementation of the ITourPlannerConfig interface. It is a singleton and provides the application with vital config data.
    /// </summary>
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
            try
            {
                if (!File.Exists(configPath))
                {
                    logger.Error("The config file could not be found and therefore the application could not be started!");
                    throw new FileNotFoundException($"The config file could not be found at {Path.GetFullPath(configPath)}");
                }
                IConfiguration config = new ConfigurationBuilder().AddJsonFile(Path.GetFullPath(configPath)).Build();
                if (config.GetChildren().Any(c => c.Key == "dbsettings") && config.GetChildren().Any(c => c.Key == "mapQuestKey") && config.GetChildren().Any(c => c.Key == "pictureDirectory") && config.GetChildren().Any(c => c.Key == "exportsDirectory"))
                {
                    DatabaseConnectionString = $"Host={config.GetSection("dbsettings:host").Value};Port={config.GetSection("dbsettings:port").Value};Username={config.GetSection("dbsettings:username").Value};Password={config.GetSection("dbsettings:password").Value};Database={config.GetSection("dbsettings:database").Value};";
                    MapQuestKey = $"{config.GetSection("mapQuestKey").Value}";
                    PictureDirectory = $"{config.GetSection("pictureDirectory").Value}";
                    ExportsDirectory = $"{config.GetSection("exportsDirectory").Value}";
                    SetUpEnvironment();
                }
                else
                {
                    logger.Error("The imported file has an invalid format and can therefore not be used!");
                    throw new FormatException("The file could be found, but it has an invalid format!");
                }
            }
            catch(Exception e)
            {
                if(e is FormatException || e is FileNotFoundException)
                    throw new CommonConfigException(e.Message);
                logger.Error($"Some other unhandled exception occured. Details: {e.Message}");
                throw new CommonConfigException("The configuration for the application could not be loaded properly!");
            }    
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
