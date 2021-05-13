using Common.Logging;
using DataAccessLayer.DBCommands;
using DataAccessLayer.DBCommands.TourCommands;
using DataAccessLayer.DBConnection;
using Common.Entities;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    /// <summary>
    /// TourRepositories are used for querying, inserting, updating and deleting tours. DML commands are not executed in this class. The only way to truly insert, update and delete tours is through the UnitOfWork class, which ensures that database connections get opened/closed only when really needed. Retrieving data is possible everywhere.
    /// </summary>
    public class TourRepository : ITourRepository
    {
        /// <summary>
        /// Database connection
        /// </summary>
        private IDBConnection db;
        /// <summary>
        /// Commands to be commited to the database
        /// </summary>
        private List<IDBCommand> commitCommands;
        /// <summary>
        /// Repository for retrieving associated tourlogs
        /// </summary>
        private ITourLogRepository tourLogRepository;
        /// <summary>
        /// Repositories for retrieving associated maneuvers
        /// </summary>
        private IManeuverRepository maneuverRepository;

        private log4net.ILog logger;

        /// <summary>
        /// Creates the TourRepository object
        /// </summary>
        public TourRepository()
        {
            db = DatabaseConnection.GetDBConnection();
            commitCommands = new List<IDBCommand>();
            tourLogRepository = new TourLogRepository();
            maneuverRepository = new ManeuverRepository();
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        /// <summary>
        /// Creates the TourRepository instance and "connects" it to the wrapping UnitOfWork class
        /// </summary>
        /// <param name="db">Database connection</param>
        /// <param name="commitCommands">Commands for a commit (UnitOfWork)</param>
        public TourRepository(IDBConnection db, List<IDBCommand> commitCommands)
        {
            this.db = db;
            this.commitCommands = commitCommands;
            maneuverRepository = new ManeuverRepository(this.db, this.commitCommands);
            tourLogRepository = new TourLogRepository(this.db, this.commitCommands);
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        /// <summary>
        /// Constructor solely for testing purposes
        /// </summary>
        /// <param name="db">Database connection. For tests, a mock can be passed.</param>
        /// <param name="commitCommands">Commands to be commited. </param>
        /// <param name="tourLogRepository">A repository for tourlogs. A mock can be passed for testing puproses.</param>
        /// <param name="maneuverRepository">Repository for maneuvers. Mock object can be passed</param>
        public TourRepository(IDBConnection db, List<IDBCommand> commitCommands, ITourLogRepository tourLogRepository, IManeuverRepository maneuverRepository)
        {
            this.db = db;
            this.commitCommands = commitCommands;
            this.maneuverRepository = maneuverRepository;
            this.tourLogRepository = tourLogRepository;
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        /// <summary>
        /// Converts object arrays to tours.
        /// </summary>
        /// <param name="row">Result (row) of a query</param>
        /// <returns>Converted tour.</returns>
        private Tour ConvertToTour(object[] row)
        {
            Tour tour = new Tour()
            {
                Id = Convert.ToInt32(row[0]),
                Name = row[1].ToString(),
                StartLocation = row[2].ToString(),
                EndLocation = row[3].ToString(),
                RouteInfo = row[4].ToString(),
                Distance = Convert.ToDouble(row[5]),
                RouteType = (ERouteType)Convert.ToInt32(row[6]),
                Description = row[7].ToString()
            };

            return tour;
        }

        private bool CheckDBConstraints(Tour tour)
        {
            if (tour.Name.Length<=75 && !string.IsNullOrWhiteSpace(tour.Name) && tour.RouteInfo.Length<=250 && !string.IsNullOrWhiteSpace(tour.RouteInfo) &&
                tour.StartLocation.Length<=150 && !string.IsNullOrWhiteSpace(tour.StartLocation) && tour.EndLocation.Length<=150 && !string.IsNullOrWhiteSpace(tour.EndLocation)
                &&tour.Distance>=0)
                return true;
            return false;
        }

        /// <summary>
        /// Creates a DeleteTourCommand object if a tour with the specified id exists.
        /// </summary>
        /// <param name="id">Id of the tour to be deleted</param>
        public void Delete(int id)
        {
            Tour tour = Read(id);
            if (tour != null)
            {
                commitCommands.Add(new DeleteTourCommand(db,tour));
                logger.Info($"DeleteTourCommand queued. Amount of commands in the next commit is {commitCommands.Count}");
            }
        }
        /// <summary>
        /// Checks if the data of the tour is ok. If so, a InsertTourCommand object gets created.
        /// </summary>
        /// <param name="entity">Tour that is supposed to be inserted into the database</param>
        public void Insert(Tour entity)
        {
            if (CheckDBConstraints(entity))
            {
                commitCommands.Add(new InsertTourCommand(db, entity));
                logger.Info($"InsertTourCommand queued. Amount of commands in the next commit is {commitCommands.Count}");
            }
        }
        /// <summary>
        /// Retrieves a tour with the specified id.
        /// </summary>
        /// <param name="id">Id of the wanted tour.</param>
        /// <returns>Tour with the specified id (null if id doesn't exist in table). Associated data (maneuvers, tourlogs) are already loaded</returns>
        public Tour Read(int id)
        {
            Tour tour = null;
            if (id > 0)
            {
                IDbCommand readTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@id;");
                db.DefineParameter(readTourCommand, "@id", System.Data.DbType.Int32, id);
                db.OpenConnection();
                List<object[]> readTourResults = db.QueryDatabase(readTourCommand);
                db.CloseConnection();
                if (readTourResults.Count > 0)
                {
                    tour = ConvertToTour(readTourResults[0]);
                    tour.Maneuvers = maneuverRepository.ReadAll().Where(m => m.TourId == tour.Id).ToList();
                    tour.TourLogs = tourLogRepository.ReadAll().Where(tl => tl.TourId == tour.Id).ToList();
                }
            }
            return tour;
        }
        /// <summary>
        /// Retrieves all tours in the tour table.
        /// </summary>
        /// <returns>Collection of all tours. Associated data (maneuvers, tourlogs) are already loaded.</returns>
        public List<Tour> ReadAll()
        {
            IDbCommand readToursCommand = new NpgsqlCommand("SELECT * FROM tour;");
            db.OpenConnection();
            List<object[]> readToursResults = db.QueryDatabase(readToursCommand);
            db.CloseConnection();
            List<Tour> tours = new List<Tour>();

            foreach(object[] row in readToursResults)
            {
                Tour tour = ConvertToTour(row);
                tour.Maneuvers = maneuverRepository.ReadAll().Where(m => m.TourId == tour.Id).ToList();
                tour.TourLogs = tourLogRepository.ReadAll().Where(tl => tl.TourId == tour.Id).ToList();
                tours.Add(tour);
            }

            return tours;
        }
        /// <summary>
        /// Creates UpdateTourCommand if the specified entity exists.
        /// </summary>
        /// <param name="entity">New state of the tour.</param>
        public void Update(Tour entity)
        {
            Tour oldTour = Read(entity.Id);
            if (oldTour != null && CheckDBConstraints(entity))
            {
                commitCommands.Add(new UpdateTourCommand(db, entity, oldTour));
                logger.Info($"InsertTourCommand queued. Amount of commands in the next commit is {commitCommands.Count}");
            }
        }
    }
}
