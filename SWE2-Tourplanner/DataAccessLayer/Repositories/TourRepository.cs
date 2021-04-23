using DataAccessLayer.DBCommands;
using DataAccessLayer.DBCommands.TourCommands;
using DataAccessLayer.DBConnection;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class TourRepository : ITourRepository
    {
        private IDBConnection db;
        private List<IDBCommand> commitCommands;
        private ITourLogRepository tourLogRepository;
        private IManeuverRepository maneuverRepository;
        public TourRepository()
        {
            db = DatabaseConnection.GetDBConnection();
            commitCommands = new List<IDBCommand>();
            tourLogRepository = new TourLogRepository();
            maneuverRepository = new ManeuverRepository();
        }
        public TourRepository(IDBConnection db, List<IDBCommand> commitCommands)
        {
            this.db = db;
            this.commitCommands = commitCommands;
            maneuverRepository = new ManeuverRepository(this.db, this.commitCommands);
            tourLogRepository = new TourLogRepository(this.db, this.commitCommands);
        }

        public TourRepository(IDBConnection db, List<IDBCommand> commitCommands, ITourLogRepository tourLogRepository, IManeuverRepository maneuverRepository)
        {
            this.db = db;
            this.commitCommands = commitCommands;
            this.maneuverRepository = maneuverRepository;
            this.tourLogRepository = tourLogRepository;
        }
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

        public void Delete(int id)
        {
            Tour tour = Read(id);
            if (tour != null)
            {
                commitCommands.Add(new DeleteTourCommand(db,tour));
            }
        }

        public void Insert(Tour entity)
        {
            if ((!String.IsNullOrEmpty(entity.Name)) && (!String.IsNullOrEmpty(entity.RouteInfo)) && (!String.IsNullOrEmpty(entity.StartLocation)) && (!String.IsNullOrEmpty(entity.EndLocation)))
            {
                commitCommands.Add(new InsertTourCommand(db, entity.Name, entity.StartLocation, entity.EndLocation, entity.RouteInfo, entity.Distance, entity.RouteType, entity.Description));
            }
        }

        public Tour Read(int id)
        {
            Tour tour = null;
            if (id > 0)
            {
                INpgsqlCommand readTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@id;");
                readTourCommand.Parameters.AddWithValue("id", id);
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

        public List<Tour> ReadAll()
        {
            INpgsqlCommand readToursCommand = new NpgsqlCommand("SELECT * FROM tour;");
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

        public void Update(Tour entity)
        {
            Tour oldTour = Read(entity.Id);
            if (oldTour != null)
            {
                commitCommands.Add(new UpdateTourCommand(db, entity, oldTour));
            }
        }
    }
}
