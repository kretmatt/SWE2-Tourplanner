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
        /// <summary>
        /// The one and only TourPlannerConfig object during execution. Can be accessed through static methods and populated through LoadConfigFromFile() method.
        /// </summary>
        private static ITourPlannerConfig tourPlannerConfig;
        /// <summary>
        /// Path to default configuration file
        /// </summary>
        private string originalConfigPath = "config.json";
        /// <summary>
        /// ILog instance used for logging errors, warnings etc.
        /// </summary>
        private ILog logger;
        /// <summary>
        /// Private constructor that is only called once. Automatically loads default config.
        /// </summary>
        private TourPlannerConfig()
        {
            logger = LogHelper.GetLogHelper().GetLogger();
            LoadConfigFromFile(originalConfigPath);
        }
        /// <summary>
        /// Static access method for the only instance of TourPlannerConfig.
        /// </summary>
        /// <returns>The only TourPlannerConfig object during execution</returns>
        public static ITourPlannerConfig GetTourPlannerConfig()
        {
            if (tourPlannerConfig == null)
            {
                tourPlannerConfig = new TourPlannerConfig();
            }
            return tourPlannerConfig;
        }
        /// <summary>
        /// LoadConfigFromFile can be used to load different configurations during execution.
        /// </summary>
        /// <param name="configPath">Path to the configuration file.</param>
        /// <exception cref="CommonConfigException">Throw when config file can't be found, when format isn't valid and when config file could't be loaded properly.</exception>
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
                    string dbConnectionString = $"Host={config.GetSection("dbsettings:host").Value};Port={config.GetSection("dbsettings:port").Value};Username={config.GetSection("dbsettings:username").Value};Password={config.GetSection("dbsettings:password").Value};Database={config.GetSection("dbsettings:database").Value};";
                    string mapQuestKey = $"{config.GetSection("mapQuestKey").Value}";
                    string exportsDirectory = $"{config.GetSection("exportsDirectory").Value}";
                    string pictureDirectory = $"{config.GetSection("pictureDirectory").Value}";
                    SetUpEnvironment(exportsDirectory, pictureDirectory);
                    DatabaseConnectionString = dbConnectionString;
                    MapQuestKey = mapQuestKey;
                    PictureDirectory = pictureDirectory;
                    ExportsDirectory = exportsDirectory;
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
        /// <summary>
        /// SetUpEnvironment ensures that exports and picture directory are created if they don't exist yet.
        /// </summary>
        /// <param name="exportsDirectory">Path to new exports directory</param>
        /// <param name="pictureDirectory">Path to new picture directory</param>
        private void SetUpEnvironment(string exportsDirectory, string pictureDirectory)
        {
            Directory.CreateDirectory($@"{exportsDirectory}");
            Directory.CreateDirectory($@"{pictureDirectory}");
        }
        public string MapQuestKey { get; private set; }

        public string PictureDirectory { get; private set; }

        public string DatabaseConnectionString { get; private set; }

        public string ExportsDirectory { get; private set; }
    }
}
