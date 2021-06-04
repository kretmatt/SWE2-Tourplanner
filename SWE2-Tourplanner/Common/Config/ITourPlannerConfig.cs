using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Config
{
    /// <summary>
    /// The ITourPlannerConfig interface ensures that all derived TourPlannerConfigs provide a MapQuestKey, a PictureDirectory, an ExportsDirectory, a DatabaseConnectionstring and the option to load another config file during the execution of the program. 
    /// </summary>
    public interface ITourPlannerConfig
    {
        /// <summary>
        /// The MapQuestKey is used to retrieve data about tours from mapquest.
        /// </summary>
        string MapQuestKey { get; }
        /// <summary>
        /// PictureDirectory is a path to a directory where MapQuest maps can be saved.
        /// </summary>
        string PictureDirectory { get; }
        /// <summary>
        /// The DatabaseConnectionString defines the database where the data is going to be stored.
        /// </summary>
        string DatabaseConnectionString { get; }
        /// <summary>
        /// ExportsDirectory is a path to a directory where exports and associated images are saved.
        /// </summary>
        string ExportsDirectory { get; }
        /// <summary>
        /// Method used for loading different configurations during the execution of the program.
        /// </summary>
        /// <param name="configPath">Path to the config file</param>
        void LoadConfigFromFile(string configPath);
    }
}
