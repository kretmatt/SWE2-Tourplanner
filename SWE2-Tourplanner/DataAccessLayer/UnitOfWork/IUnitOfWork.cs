using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.UnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        int Commit();
        int Rollback();
        ITourRepository TourRepository { get; set; }
        ITourLogRepository TourLogRepository { get; set; }
        IManeuverRepository ManeuverRepository { get; set; }
    }
}
