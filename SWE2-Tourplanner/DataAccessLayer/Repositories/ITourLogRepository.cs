using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    /// <summary>
    /// The ITourLogRepository interface is a place, where specific functions for tourlogs can be defined. The basic methods of repositories are derived from IRepository<TourLog>
    /// </summary>
    public interface ITourLogRepository:IRepository<TourLog>
    {
    }
}
