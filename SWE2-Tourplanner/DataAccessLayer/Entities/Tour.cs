using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Tour
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public string RouteInfo { get; set; }
        public double Distance { get; set; }
        public ERouteType RouteType { get; set; }
        public string Description { get; set; }
        public List<Maneuver> Maneuvers { get; set; }
        public List<TourLog> TourLogs { get; set; }
    }
}
