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
    /// <summary>
    /// TourLogRepositories are used for querying, inserting, updating and deleting tourlogs. DML commands are not executed in this class. The only way to truly insert, update and delete tourlogs is through the UnitOfWork class, which ensures that database connections get opened/closed only when really needed. Retrieving data is possible everywhere.
    /// </summary>
    public class TourLogRepository : ITourLogRepository
    {
        /// <summary>
        /// Database connection
        /// </summary>
        private IDBConnection db;
        /// <summary>
        /// Commands to be committed to the database
        /// </summary>
        private List<IDBCommand> commitCommands;
        /// <summary>
        /// Creates the TourLogRepository instance.
        /// </summary>
        public TourLogRepository()
        {
            db = DatabaseConnection.GetDBConnection();
            commitCommands = new List<IDBCommand>();
        }
        /// <summary>
        /// Creates the TourLogRepository instance and "connects" it to the UnitOfWork class
        /// </summary>
        /// <param name="db">Database connection</param>
        /// <param name="commitCommands">Commands for a commit (UnitOfWork)</param>
        public TourLogRepository(IDBConnection db, List<IDBCommand> commitCommands)
        {
            this.db = db;
            this.commitCommands = commitCommands;
        }
        /// <summary>
        /// Converts object arrays to tourlogs.
        /// </summary>
        /// <param name="row">Result of a query</param>
        /// <returns>Converted tourlog.</returns>
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
        /// <summary>
        /// Creates a DeleteTourLogCommand object, if a tourlog with the specified id exists.
        /// </summary>
        /// <param name="id">Id of the tourlog to be deleted</param>
        public void Delete(int id)
        {
            TourLog tourLog = Read(id);
            if (tourLog != null)
            {
                commitCommands.Add(new DeleteTourLogCommand(db,tourLog));
            }
        }
        /// <summary>
        /// Checks if log data is ok. If so, creates a new InsertTourLogCommand
        /// </summary>
        /// <param name="entity">TourLog to be inserted.</param>
        public void Insert(TourLog entity)
        {
            if (!(String.IsNullOrEmpty(entity.Report)))
            {
                commitCommands.Add(new InsertTourLogCommand(db, entity.TourId, entity.StartDate,entity.EndDate, entity.Distance, entity.TotalTime, entity.Rating, entity.AverageSpeed, entity.Weather, entity.TravelMethod, entity.Report, entity.Temperature));
            }
        }
        /// <summary>
        /// Function for the retrieval of a log with the specified id
        /// </summary>
        /// <param name="id">Id of the wanted log</param>
        /// <returns>Log with the specified id (null if id doesn't exist in the table)</returns>
        public TourLog Read(int id)
        {
            TourLog tourLog = null;

            if (id > 0)
            {
                IDbCommand readTourLogCommand = new NpgsqlCommand("SELECT * FROM tourlog WHERE id=@id;");
                db.DefineParameter(readTourLogCommand, "@id", System.Data.DbType.Int32, id);
                //readTourLogCommand.Parameters.AddWithValue("id", id);
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
        /// <summary>
        /// Function for retrieving all tourlogs in the table.
        /// </summary>
        /// <returns>Collection of all logs in the tourlog table</returns>
        public List<TourLog> ReadAll()
        {
            List<TourLog> tourLogs = new List<TourLog>();
            IDbCommand readTourLogsCommand = new NpgsqlCommand("SELECT * FROM tourlog;");
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
        /// <summary>
        /// Creates an UpdateTourLogCommand object if the specified entity (id) exists in the table.
        /// </summary>
        /// <param name="entity">New state of the log</param>
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
