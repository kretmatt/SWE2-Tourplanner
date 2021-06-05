using Common.Logging;
using DataAccessLayer.DBConnection;
using Common.Entities;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBCommands.TourLogCommands
{
    /// <summary>
    /// InsertTourLogCommand is used for inserting new logs for tours.
    /// </summary>
    public class InsertTourLogCommand : IDBCommand
    {
        /// <summary>
        /// Connection to the database.
        /// </summary>
        private IDBConnection db;
        /// <summary>
        /// The new log to be inserted.
        /// </summary>
        private TourLog tourLog;

        private log4net.ILog logger;

        /// <summary>
        /// Creates a new InsertTourLogCommand instance.
        /// </summary>
        /// <param name="db">Connection to the database.</param>
        /// <param name="tourId">Id of the associated tour.</param>
        /// <param name="startDate">Start date+time of the log.</param>
        /// <param name="endDate">End date+time of the log.</param>
        /// <param name="distance">Travelled distance in km.</param>
        /// <param name="totalTime">Total time needed in h.</param>
        /// <param name="rating">Rating of the tour.</param>
        /// <param name="averageSpeed">Average speed of the user.</param>
        /// <param name="weather">Weather during the tour.</param>
        /// <param name="travelMethod">Way of travelling.</param>
        /// <param name="report">Short comment on the tour.</param>
        /// <param name="temperature">Temperature during the tour.</param>
        public InsertTourLogCommand(IDBConnection db, TourLog tourLog)
        {
            this.db = db;
            this.tourLog = tourLog;
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        /// <summary>
        /// Inserts a new log of a tour into the tourlog table
        /// </summary>
        /// <returns>Amount of rows affected by the insert statement. Expected: 1</returns>
        public int Execute()
        {
            int insertTourLogResult = 0;

            if (tourLog.TourId > 0)
            {
                IDbCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
                db.DefineParameter(checkForTourCommand, "@tourid", System.Data.DbType.Int32, tourLog.TourId);

                List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

                if (tourResults.Count != 1)
                {
                    logger.Warn($"Tour with the id {tourLog.TourId} could not be found. A rollback could be necessary to ensure data consistency.");
                    return insertTourLogResult;
                }

                IDbCommand retrieveNextIdCommand = new NpgsqlCommand("SELECT nextval(pg_get_serial_sequence('tourlog','id')) AS newid;");
                List<object[]> retrieveNextIdResult = db.QueryDatabase(retrieveNextIdCommand);

                tourLog.Id = Convert.ToInt32(retrieveNextIdResult[0][0]);

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

                insertTourLogResult = db.ExecuteStatement(insertTourLogCommand);
            }
            return insertTourLogResult;
        }
        /// <summary>
        /// Deletes the previously inserted tourlog.
        /// </summary>
        /// <returns>Amount of rows affected by the delete-statement. Expected: 1</returns>
        public int Undo()
        {
            int undoResult = 0;

            if (tourLog.Id > 0)
            {
                IDbCommand deleteTourLogCommand = new NpgsqlCommand("DELETE FROM tourlog WHERE id=@id;");
                db.DefineParameter(deleteTourLogCommand, "@id", System.Data.DbType.Int32, tourLog.Id);

                undoResult = db.ExecuteStatement(deleteTourLogCommand);
            }

            return undoResult;
            
        }
    }
}
