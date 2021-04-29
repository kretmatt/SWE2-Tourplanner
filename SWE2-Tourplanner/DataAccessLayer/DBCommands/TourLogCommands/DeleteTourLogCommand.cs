using BusinessLogicLayer.Logging;
using DataAccessLayer.DBConnection;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands.TourLogCommands
{
    /// <summary>
    /// DeleteTourLogCommand is used for creating new logs of tours.
    /// </summary>
    public class DeleteTourLogCommand:IDBCommand
    {
        /// <summary>
        /// Connection to the database.
        /// </summary>
        private IDBConnection db;
        /// <summary>
        /// Log of a tour to be deleted.
        /// </summary>
        private TourLog tourLog;

        private log4net.ILog logger;

        /// <summary>
        /// Creates the DeleteTourLogCommand instance.
        /// </summary>
        /// <param name="db">Connection to the database</param>
        /// <param name="tourLog">Log to be deleted.</param>
        public DeleteTourLogCommand(IDBConnection db, TourLog tourLog)
        {
            this.db = db;
            this.tourLog = tourLog;
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        /// <summary>
        /// Deletes the log of a tour from the tourlog table.
        /// </summary>
        /// <returns>Amount of rows affected by the delete-statement. Expected: 1</returns>
        public int Execute()
        {
            int deleteTourLogResult = 0;

            if (tourLog.Id > 0) 
            {
                IDbCommand deleteTourLogCommand = new NpgsqlCommand("DELETE FROM tourlog WHERE id=@id;");
                db.DefineParameter(deleteTourLogCommand, "@id", System.Data.DbType.Int32, tourLog.Id);

                deleteTourLogResult += db.ExecuteStatement(deleteTourLogCommand);
            }

            return deleteTourLogResult;
        }
        /// <summary>
        /// Inserts the previously deleted log back into the tourlog table.
        /// </summary>
        /// <returns>Amount of rows affected by the insert-statement. Expected: 1</returns>
        public int Undo()
        {
            int undoResult = 0;

            if (tourLog.TourId > 0)
            {
                IDbCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
                db.DefineParameter(checkForTourCommand, "@tourid", System.Data.DbType.Int32, tourLog.TourId);
                List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

                if (tourResults.Count != 1)
                {
                    logger.Warn($"Tour with the id {tourLog.TourId} could not be found. A rollback could be necessary to ensure data consistency.");
                    return undoResult;
                }

                IDbCommand insertTourLogCommand = new NpgsqlCommand("INSERT INTO tourlog (id,tourid,startdate,enddate,distance,totaltime,rating,averagespeed,weather,travelmethod,report,temperature) VALUES (@id,@tourid,@startdate,@enddate,@distance,@totaltime,@rating,@averagespeed,@weather,@travelmethod,@report,@temperature);");
                db.DefineParameter(insertTourLogCommand, "@id", System.Data.DbType.Int32, tourLog.Id);
                db.DefineParameter(insertTourLogCommand, "@tourid", System.Data.DbType.Int32, tourLog.TourId);
                db.DefineParameter(insertTourLogCommand, "@startdate", System.Data.DbType.DateTime, tourLog.StartDate);
                db.DefineParameter(insertTourLogCommand, "@enddate", System.Data.DbType.DateTime, tourLog.EndDate);
                db.DefineParameter(insertTourLogCommand, "@distance", System.Data.DbType.Decimal, tourLog.Distance);
                db.DefineParameter(insertTourLogCommand, "@totaltime", System.Data.DbType.Decimal, tourLog.TotalTime);
                db.DefineParameter(insertTourLogCommand, "@rating", System.Data.DbType.Decimal, tourLog.Rating);
                db.DefineParameter(insertTourLogCommand, "@averagespeed", System.Data.DbType.Decimal, tourLog.AverageSpeed);
                db.DefineParameter(insertTourLogCommand, "@weather", System.Data.DbType.Int32, (int)tourLog.Weather);
                db.DefineParameter(insertTourLogCommand, "@travelmethod", System.Data.DbType.Int32, (int)tourLog.TravelMethod);
                db.DefineParameter(insertTourLogCommand, "@report", System.Data.DbType.String, tourLog.Report);
                db.DefineParameter(insertTourLogCommand, "@temperature", System.Data.DbType.Decimal, tourLog.Temperature);

                undoResult = db.ExecuteStatement(insertTourLogCommand);
            }
            return undoResult;
        }
    }
}
