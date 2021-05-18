using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Config
{
    public interface ITourPlannerConfig
    {
        string MapQuestKey { get; }
        string PictureDirectory { get; }
        string DatabaseConnectionString { get; }
        string ExportsDirectory { get; }
        void LoadConfigFromFile(string configPath);
    }
}
