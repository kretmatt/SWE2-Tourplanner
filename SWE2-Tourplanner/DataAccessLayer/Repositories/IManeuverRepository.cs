using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    /// <summary>
    /// The IManeuverRepository interface is a place, where specific functions for maneuvers can be defined. The basic methods of repositories are derived from IRepository<Maneuver>
    /// </summary>
    public interface IManeuverRepository : IRepository<Maneuver>
    {
    }
}
