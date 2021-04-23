using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.DBCommands.TourLogCommands;
using DataAccessLayer.DBConnection;
using DataAccessLayer.DBCommands;
using DataAccessLayer.Enums;

namespace DataAccessLayer.Repositories
{
    public class TourLogRepository : ITourLogRepository
    {
        private IDBConnection db;
        private List<IDBCommand> commitCommands;

        public TourLogRepository()
        {
            db = DatabaseConnection.GetDBConnection();
            commitCommands = new List<IDBCommand>();
        }

        public TourLogRepository(IDBConnection db, List<IDBCommand> commitCommands)
        {
            this.db = db;
            this.commitCommands = commitCommands;
        }

        private TourLog ConvertToTourLog(object[] row)
        {
            TourLog tourLog = new TourLog()
            {
                Id = Convert.ToInt32(row[0]),
                TourId = Convert.ToInt32(row[1]),
                StartDate = Convert.ToDateTime(row[2]),
                EndDate = Convert.ToDateTime(row[3]),
                Distance = Convert.ToDouble(row[4]),
                TotalTime = Convert.ToDouble(row[5]),
                Rating = Convert.ToDouble(row[6]),
                AverageSpeed = Convert.ToDouble(row[7]),
                Weather = (EWeather)Convert.ToInt32(row[8]),
                Temperature = Convert.ToDouble(row[9]),
                TravelMethod = (ETravelMethod)Convert.ToInt32(row[10]),
                Report = row[11].ToString()
            };

            return tourLog;
        }

        public void Delete(int id)
        {
            TourLog tourLog = Read(id);
            if (tourLog != null)
            {
                commitCommands.Add(new DeleteTourLogCommand(db,tourLog));
            }
        }

        public void Insert(TourLog entity)
        {
            if (!(String.IsNullOrEmpty(entity.Report)))
            {
                commitCommands.Add(new InsertTourLogCommand(db, entity.TourId, entity.StartDate,entity.EndDate, entity.Distance, entity.TotalTime, entity.Rating, entity.AverageSpeed, entity.Weather, entity.TravelMethod, entity.Report, entity.Temperature));
            }
        }

        public TourLog Read(int id)
        {
            TourLog tourLog = null;

            if (id > 0)
            {
                INpgsqlCommand readTourLogCommand = new NpgsqlCommand("SELECT * FROM tourlog WHERE id=@id;");
                readTourLogCommand.Parameters.AddWithValue("id", id);
                db.OpenConnection();
                List<object[]> readTourLogResults = db.QueryDatabase(readTourLogCommand);
                db.CloseConnection();

                if (readTourLogResults.Count > 0)
                {
                    tourLog = ConvertToTourLog(readTourLogResults[0]);
                }
            }

            return tourLog;
        }

        public List<TourLog> ReadAll()
        {
            List<TourLog> tourLogs = new List<TourLog>();
            INpgsqlCommand readTourLogsCommand = new NpgsqlCommand("SELECT * FROM tourlog;");
            db.OpenConnection();
            List<object[]> readTourLogsResults = db.QueryDatabase(readTourLogsCommand);
            db.CloseConnection();

            foreach(object[] row in readTourLogsResults)
            {
                TourLog tourLog = ConvertToTourLog(row);
                tourLogs.Add(tourLog);
            }

            return tourLogs;
        }

        public void Update(TourLog entity)
        {
            TourLog oldTourLog = Read(entity.Id);

            if (oldTourLog != null)
            {
                commitCommands.Add(new UpdateTourLogCommand(db, entity, oldTourLog));
            }
        }
    }
}
