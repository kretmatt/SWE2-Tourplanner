using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    ///    <summary>
    ///         The tour is the most important entity of the project. It represents a route. 
    ///    </summary>
    public class Tour
    {
        /// <value>Id of the tour.</value> 
        public int Id { get; set; }

        /// <value>Name of the tour. Is unique.</value>
        public string Name { get; set; }

        /// <value>Location where the tour is supposed to start.</value>
        public string StartLocation { get; set; }

        /// <value>Location where the tour is supposed to end.</value>
        public string EndLocation { get; set; }

        /// <value>Path to the route image.</value>
        public string RouteInfo { get; set; }

        /// <value>Overall length of the tour.</value>
        public double Distance { get; set; }

        /// <value>Type of the route. Is an important parameter that decides how the route is constructed by MapQuest.</value> 
        public ERouteType RouteType { get; set; }

        /// <value>Description of the tour.</value> 
        public string Description { get; set; }

        /// <value>All associated maneuvers of the tour.</value> 
        public List<Maneuver> Maneuvers { get; set; }

        /// <value>All associated tour logs of the tour.</value> 
        public List<TourLog> TourLogs { get; set; }
    }
}
