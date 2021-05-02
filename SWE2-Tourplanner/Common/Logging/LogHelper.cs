using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging
{
    /// <summary>
    /// The LogHelper class is responsible for creating ILog objects. Those objects are used throughout the different layers of the application for logging purposes. Is a singleton.
    /// </summary>
    public class LogHelper
    {
        private static LogHelper logHelper;
        private LogHelper([CallerFilePath] string filename = "")
        {
            if (!log4net.LogManager.GetRepository().Configured)
            {
                var configFile = new FileInfo(string.Format("{0}\\log4net.config", Path.GetDirectoryName(filename)));
                if (!configFile.Exists)
                    throw new FileLoadException(string.Format("The configuration file {0} does not exist", configFile));
                log4net.Config.XmlConfigurator.Configure(configFile);
            }
        }
        /// <summary>
        /// Global access point to the only instance of LogHelper.
        /// </summary>
        /// <returns>Only instance of LogHelper</returns>
        public static LogHelper GetLogHelper()
        {
            if (logHelper == null)
            {
                logHelper = new LogHelper();
            }
            return logHelper;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">Name of the file where the ILog instance is requested. This parameter doesn't need to be passed due to CallerFilePath</param>
        /// <returns>Properly configured ILog instance for corresponding files/classes</returns>
        public log4net.ILog GetLogger([CallerFilePath] string filename = "")
        {
            return log4net.LogManager.GetLogger(filename);
        }
    }
}
