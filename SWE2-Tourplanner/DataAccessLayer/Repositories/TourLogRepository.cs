using System;
using System.Collections.Generic;
using Common.Entities;
using DataAccessLayer.DBCommands.TourLogCommands;
using DataAccessLayer.DBConnection;
using DataAccessLayer.DBCommands;
using Common.Enums;
using Common.Logging;
using DataAccessLayer.Exceptions;

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
        /// ILog object used for logging errors etc.
        /// </summary>
        private log4net.ILog logger;

        /// <summary>
        /// Creates the TourLogRepository instance.
        /// </summary>
        public TourLogRepository()
        {
            db = DatabaseConnection.GetDBConnection();
            commitCommands = new List<IDBCommand>();
            logger = LogHelper.GetLogHelper().GetLogger();
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
            logger = LogHelper.GetLogHelper().GetLogger();
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
        /// CheckDBConstraints is used to check whether a tourlog object complies with db constraints or not.
        /// </summary>
        /// <param name="tourLog">The tourlog that needs to be checked.</param>
        /// <returns>True if constraints are adhered to, false if constraints are not complied with.</returns>
        private bool CheckDBConstraints(TourLog tourLog)
        {
            if (DateTime.Compare(tourLog.StartDate, tourLog.EndDate)<0 && tourLog.Distance>=0 && tourLog.AverageSpeed>0 && tourLog.Temperature>-273.15 && tourLog.Rating>=0 && tourLog.Rating<=10 )
                return true;
            return false;
        }
        /// <summary>
        /// Creates a DeleteTourLogCommand object, if a tourlog with the specified id exists.
        /// </summary>
        /// <param name="id">Id of the tourlog to be deleted</param>
        /// <exception cref="DALRepositoryCommandException">Thrown, when DeleteTourLogCommand can't be created</exception>
        public void Delete(int id)
        {
            TourLog tourLog = Read(id);
            if (tourLog != null)
            {
                commitCommands.Add(new DeleteTourLogCommand(db,tourLog));
                logger.Info($"DeleteTourLogCommand queued. Amount of commands in the next commit is {commitCommands.Count}");
            }
            else
            {
                logger.Warn("Delete is not possible because the tourlog entity does not exist in the data store!");
                throw new DALRepositoryCommandException("Delete is not possible because tourlog data does not exist in data store");
            }
        }
        /// <summary>
        /// Checks if log data is ok. If so, creates a new InsertTourLogCommand
        /// </summary>
        /// <param name="entity">TourLog to be inserted.</param>
        /// <exception cref="DALRepositoryCommandException">Thrown, when InsertTourLogCommand can't be created</exception>
        public void Insert(TourLog entity)
        {
            if (CheckDBConstraints(entity))
            {
                commitCommands.Add(new InsertTourLogCommand(db, entity));
                logger.Info($"InsertTourLogCommand queued. Amount of commands in the next commit is {commitCommands.Count}");
            }
            else
            {
                logger.Warn("Insert of tourlog data is not possible because constraints are being violated!");
                throw new DALRepositoryCommandException("Saving the tourlog data is not possible, because constraints are being violated!");
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
        /// <exception cref="DALRepositoryCommandException">Thrown, when UpdateTourLogCommand can't be created</exception>
        public void Update(TourLog entity)
        {
            TourLog oldTourLog = Read(entity.Id);

            if (oldTourLog != null && CheckDBConstraints(entity))
            {
                commitCommands.Add(new UpdateTourLogCommand(db, entity, oldTourLog));
                logger.Info($"UpdateTourLogCommand queued. Amount of commands in the next commit is {commitCommands.Count}");
            }
            else
            {
                logger.Warn("Updating the tourlog data is not possible, because constraints are being violated or the entity does not exist in the data store!");
                throw new DALRepositoryCommandException("Updating the tourlog data is not possible, because constraints are being violated or the associated data does not exist in the data store!");
            }
        }
    }
}
