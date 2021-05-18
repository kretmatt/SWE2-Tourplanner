using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    ///    <summary>
    ///     The maneuver entity is basically a single part of the overall route description of a tour. It consists of a narrative, describing the part of the route in detail, and the length (km) of the section.
    ///    </summary>
    public class Maneuver
    {
        [JsonIgnore]
        /// <value>Id of the maneuver.</value>
        public int Id { get; set; }

        [JsonIgnore]
        /// <value>Id of the associated tour.</value>
        public int TourId { get; set; }

        /// <value>Description of the section</value>
        public string Narrative { get; set; }

        /// <value>Length of the section. Unit is km.</value>
        public double Distance { get; set; }

        public override bool Equals(object obj)
        {
            if(obj.GetType() == GetType())
            {
                Maneuver m = (Maneuver)obj;
                return (m.Distance==this.Distance&&m.Narrative==this.Narrative) ? true : false;
            }
            return false;
        }

    }
}
