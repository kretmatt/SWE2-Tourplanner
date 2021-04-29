using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    ///    <summary>
    ///        The tourlog is an entry from a person, who participated in a tour. It's basically a short review of the tour.
    ///    </summary>
    public class TourLog
    {
        /// <value>Id of the tour log.</value>
        public int Id { get; set; }

        /// <value>Id of the associated tour.</value>
        public int TourId { get; set; }

        /// <value>Start time and date of the tour log.</value>
        public DateTime StartDate { get; set; }

        /// <value>End time and date of the tour log.</value>
        public DateTime EndDate { get; set; }

        /// <value>Distance travelled by user.</value>
        public double Distance { get; set; }

        /// <value>Total time needed.</value>
        public double TotalTime { get; set; }

        /// <value>Temperature during the tour.</value>
        public double Temperature { get; set; }

        /// <value>Rating of the tour.</value>
        public double Rating { get; set; }

        /// <value>Average speed of the user.</value>
        public double AverageSpeed { get; set; }

        /// <value>Weather during the tour.</value>
        public EWeather Weather { get; set; }

        /// <value>Travel method of the user.</value>
        public ETravelMethod TravelMethod { get; set; }

        /// <value>Short comment on the tour.</value>
        public string Report { get; set; }
    }
}
