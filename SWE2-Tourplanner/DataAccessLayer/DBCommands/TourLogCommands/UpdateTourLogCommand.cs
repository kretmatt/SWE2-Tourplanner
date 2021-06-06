using Common.Logging;
using DataAccessLayer.DBConnection;
using Common.Entities;
using System.Collections.Generic;

namespace DataAccessLayer.DBCommands.TourLogCommands
{
    /// <summary>
    /// UpdateTourLogCommand is used for updating tourlogs.
    /// </summary>
    public class UpdateTourLogCommand : IDBCommand
    {
        /// <summary>
        /// Connection to the database.
        /// </summary>
        private IDBConnection db;
        /// <summary>
        /// New state of the log.
        /// </summary>
        private TourLog tourLog;
        /// <summary>
        /// Old state of the log.
        /// </summary>
        private TourLog oldTourLog;
        /// <summary>
        /// ILog instance used for logging errors, warnings etc.
        /// </summary>
        private log4net.ILog logger;

        /// <summary>
        /// Creates the UpdateTourLogCommand instance.
        /// </summary>
        /// <param name="db">Connection to the database.</param>
        /// <param name="tourLog">The new state of the log.</param>
        /// <param name="oldTourLog">The old state of the log.</param>
        public UpdateTourLogCommand(IDBConnection db, TourLog tourLog, TourLog oldTourLog)
        {
            this.db = db;
            this.tourLog = tourLog;
            this.oldTourLog = oldTourLog;
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        /// <summary>
        /// Updates the data of the log in the database according to the new state of the log.
        /// </summary>
        /// <returns>Amount of rows affected by the update-statement. Expected: 1</returns>
        public int Execute()
        {
            int updateTourLogResult = 0;

            IDbCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
            db.DefineParameter(checkForTourCommand, "@tourid", System.Data.DbType.Int32, tourLog.TourId);
            List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

            if (tourResults.Count != 1)
            {
                logger.Warn($"Tour with the id {tourLog.TourId} could not be found. A rollback could be necessary to ensure data consistency.");
                return updateTourLogResult;
            }

            IDbCommand updateTourLogCommand = new NpgsqlCommand("UPDATE tourlog SET tourid=@tourid, startdate=@startdate, enddate=@enddate, distance=@distance, totaltime=@totaltime, rating=@rating, averagespeed=@averagespeed, weather=@weather, travelmethod=@travelmethod, report=@report, temperature=@temperature WHERE id = @id;");
            db.DefineParameter(updateTourLogCommand, "@id", System.Data.DbType.Int32, tourLog.Id);
            db.DefineParameter(updateTourLogCommand, "@tourid", System.Data.DbType.Int32, tourLog.TourId);
            db.DefineParameter(updateTourLogCommand, "@startdate", System.Data.DbType.DateTime, tourLog.StartDate);
            db.DefineParameter(updateTourLogCommand, "@enddate", System.Data.DbType.DateTime, tourLog.EndDate);
            db.DefineParameter(updateTourLogCommand, "@distance", System.Data.DbType.Decimal, tourLog.Distance);
            db.DefineParameter(updateTourLogCommand, "@totaltime", System.Data.DbType.Decimal, tourLog.TotalTime);
            db.DefineParameter(updateTourLogCommand, "@rating", System.Data.DbType.Decimal, tourLog.Rating);
            db.DefineParameter(updateTourLogCommand, "@averagespeed", System.Data.DbType.Decimal, tourLog.AverageSpeed);
            db.DefineParameter(updateTourLogCommand, "@weather", System.Data.DbType.Int32, (int)tourLog.Weather);
            db.DefineParameter(updateTourLogCommand, "@travelmethod", System.Data.DbType.Int32, (int)tourLog.TravelMethod);
            db.DefineParameter(updateTourLogCommand, "@report", System.Data.DbType.String, tourLog.Report);
            db.DefineParameter(updateTourLogCommand, "@temperature", System.Data.DbType.Decimal, tourLog.Temperature);

            updateTourLogResult = db.ExecuteStatement(updateTourLogCommand);

            return updateTourLogResult;
        }
        /// <summary>
        /// Reverts the data of the log in the database back to its original state.
        /// </summary>
        /// <returns>Amount of rows affected by the update statement. Expected: 1</returns>
        public int Undo()
        {
            int undoResult = 0;

            IDbCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
            db.DefineParameter(checkForTourCommand, "@tourid", System.Data.DbType.Int32, oldTourLog.TourId);
            List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

            if (tourResults.Count != 1)
            {
                logger.Warn($"Tour with the id {oldTourLog.TourId} could not be found. A rollback could be necessary to ensure data consistency.");
                return undoResult;
            }

            IDbCommand undoUpdateTourLogCommand = new NpgsqlCommand("UPDATE tourlog SET tourid=@tourid, startdate=@startdate, enddate=@enddate, distance=@distance, totaltime=@totaltime, rating=@rating, averagespeed=@averagespeed, weather=@weather, travelmethod=@travelmethod, report=@report, temperature=@temperature WHERE id = @id;");
            db.DefineParameter(undoUpdateTourLogCommand, "@id", System.Data.DbType.Int32, oldTourLog.Id);
            db.DefineParameter(undoUpdateTourLogCommand, "@tourid", System.Data.DbType.Int32, oldTourLog.TourId);
            db.DefineParameter(undoUpdateTourLogCommand, "@startdate", System.Data.DbType.DateTime, oldTourLog.StartDate);
            db.DefineParameter(undoUpdateTourLogCommand, "@enddate", System.Data.DbType.DateTime, oldTourLog.EndDate);
            db.DefineParameter(undoUpdateTourLogCommand, "@distance", System.Data.DbType.Decimal, oldTourLog.Distance);
            db.DefineParameter(undoUpdateTourLogCommand, "@totaltime", System.Data.DbType.Decimal, oldTourLog.TotalTime);
            db.DefineParameter(undoUpdateTourLogCommand, "@rating", System.Data.DbType.Decimal, oldTourLog.Rating);
            db.DefineParameter(undoUpdateTourLogCommand, "@averagespeed", System.Data.DbType.Decimal, oldTourLog.AverageSpeed);
            db.DefineParameter(undoUpdateTourLogCommand, "@weather", System.Data.DbType.Int32, (int)oldTourLog.Weather);
            db.DefineParameter(undoUpdateTourLogCommand, "@travelmethod", System.Data.DbType.Int32, (int)oldTourLog.TravelMethod);
            db.DefineParameter(undoUpdateTourLogCommand, "@report", System.Data.DbType.String, oldTourLog.Report);
            db.DefineParameter(undoUpdateTourLogCommand, "@temperature", System.Data.DbType.Decimal, oldTourLog.Temperature);

            undoResult = db.ExecuteStatement(undoUpdateTourLogCommand);

            return undoResult;
        }
    }
}
