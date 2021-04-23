using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Maneuver
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string Narrative { get; set; }
        public double Distance { get; set; }

    }
}
