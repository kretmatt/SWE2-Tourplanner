using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class TourLog
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Distance { get; set; }
        public double TotalTime { get; set; }
        public double Temperature { get; set; }
        public double Rating { get; set; }
        public double AverageSpeed { get; set; }
        public EWeather Weather { get; set; }
        public ETravelMethod TravelMethod { get; set; }
        public string Report { get; set; }
    }
}
