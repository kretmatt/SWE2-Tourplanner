using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enums
{
    /// <summary>
    /// Enum for the possible routetypes in mapquest. The route type influences the route itself.
    /// </summary>
    public enum ERouteType
    {
        FASTEST,
        SHORTEST,
        PEDESTRIAN,
        MULTIMODAL,
        BICYCLE
    }
}
