using DataAccessLayer.DBCommands;
using DataAccessLayer.DBConnection;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private List<IDBCommand> commitCommands;
        private List<IDBCommand> rollbackCommands;
        private IDBConnection db;
        private ITourRepository tourRepository;
        private ITourLogRepository tourLogRepository;
        private IManeuverRepository maneuverRepository;
        public ITourRepository TourRepository
        {
            get 
            {
                return tourRepository;
            } 
            set
            {
                if (value != tourRepository)
                    tourRepository = value;
            } 
        }
        public ITourLogRepository TourLogRepository 
        {
            get
            {
                return tourLogRepository;
            }
            set 
            {
                if (value != tourLogRepository)
                    tourLogRepository = value;
            }
        }
        public IManeuverRepository ManeuverRepository
        {
            get
            {
                return maneuverRepository;
            }
            set
            {
                if (value != maneuverRepository)
                    maneuverRepository = value;
            }
        }

        public UnitOfWork()
        {
            db = DatabaseConnection.GetDBConnection();
            commitCommands = new List<IDBCommand>();
            rollbackCommands = new List<IDBCommand>();
            tourLogRepository = new TourLogRepository(db, commitCommands);
            maneuverRepository = new ManeuverRepository(db, commitCommands);
            tourRepository = new TourRepository(db,commitCommands);
        }

        public UnitOfWork(IDBConnection db, List<IDBCommand> commitCommands, List<IDBCommand> rollbackCommands)
        {
            this.db = db;
            this.commitCommands = commitCommands;
            this.rollbackCommands = rollbackCommands;
            tourLogRepository = new TourLogRepository(db, commitCommands);
            maneuverRepository = new ManeuverRepository(db, commitCommands);
            tourRepository = new TourRepository(db, commitCommands);
        }

        public int Commit()
        {
            int commitCount = 0;
            db.OpenConnection();
            commitCommands.ForEach(cc =>
            {
                commitCount += cc.Execute();
                rollbackCommands.Add(cc);
            });
            db.CloseConnection();
            return commitCount;
        }

        public void Dispose()
        {
            commitCommands.Clear();
            rollbackCommands.Clear();
        }

        public int Rollback()
        {
            int rollbackCount = 0;
            db.OpenConnection();
            rollbackCommands.Reverse<IDBCommand>().ToList().ForEach(rc => 
            {
                rollbackCount+= rc.Undo();
            });
            db.CloseConnection();
            rollbackCommands.Clear();
            return rollbackCount;
        }
    }
}
