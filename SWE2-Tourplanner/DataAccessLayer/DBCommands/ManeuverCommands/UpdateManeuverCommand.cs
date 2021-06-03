using Common.Logging;
using DataAccessLayer.DBConnection;
using Common.Entities;
using System.Collections.Generic;

namespace DataAccessLayer.DBCommands.ManeuverCommands
{
    /// <summary>
    /// UpdateManeuverCommand is used for updating maneuvers.
    /// </summary>
    public class UpdateManeuverCommand : IDBCommand
    {
        /// <summary>
        /// Connection to the database
        /// </summary>
        /// <value>Connection to the database.</value>
        private IDBConnection db;
        /// <summary>
        /// New state of the maneuver.
        /// </summary>
        private Maneuver maneuver;
        /// <summary>
        /// Old state of the maneuver.
        /// </summary>
        private Maneuver oldManeuver;

        private log4net.ILog logger;

        /// <summary>
        /// Creates the UpdateManeuverCommand instance.
        /// </summary>
        /// <param name="db">Database connection.</param>
        /// <param name="maneuver">The new wanted state of the maneuver.</param>
        /// <param name="oldManeuver">The old deprecated state of the maneuver.</param>
        public UpdateManeuverCommand(IDBConnection db, Maneuver maneuver, Maneuver oldManeuver)
        {
            this.db = db;
            this.maneuver = maneuver;
            this.oldManeuver = oldManeuver;
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        /// <summary>
        /// Updates the data of a specific maneuver.
        /// </summary>
        /// <returns>Amount of rows affected by the update statement. Expected: 1</returns>
        public int Execute()
        {
            int updateManeuverResult = 0;

            IDbCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
            db.DefineParameter(checkForTourCommand, "@tourid", System.Data.DbType.Int32, maneuver.TourId);
            List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

            if (tourResults.Count != 1)
            {
                logger.Warn($"Tour with the id {maneuver.TourId} could not be found. A rollback could be necessary to ensure data consistency.");
                return updateManeuverResult;
            }

            IDbCommand updateManeuverCommand = new NpgsqlCommand("UPDATE maneuver SET tourid=@tourid,narrative=@narrative,distance=@distance WHERE id=@id;");
            db.DefineParameter(updateManeuverCommand, "@id", System.Data.DbType.Int32, maneuver.Id);
            db.DefineParameter(updateManeuverCommand, "@tourid", System.Data.DbType.Int32, maneuver.TourId);
            db.DefineParameter(updateManeuverCommand, "@narrative", System.Data.DbType.String, maneuver.Narrative);
            db.DefineParameter(updateManeuverCommand, "@distance", System.Data.DbType.Decimal, maneuver.Distance);

            updateManeuverResult = db.ExecuteStatement(updateManeuverCommand);

            return updateManeuverResult;
        }
        /// <summary>
        /// Reverts the maneuver back to its original state.
        /// </summary>
        /// <returns>Amount of rows affected by the rollback to the original maneuver. Expected: 1</returns>
        public int Undo()
        {
            int undoResult = 0;

            IDbCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
            db.DefineParameter(checkForTourCommand, "@tourid", System.Data.DbType.Int32, oldManeuver.TourId);

            List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

            if (tourResults.Count != 1)
            {
                logger.Warn($"Tour with the id {maneuver.TourId} could not be found. A rollback could be necessary to ensure data consistency.");
                return undoResult;
            }

            IDbCommand undoUpdateCommand = new NpgsqlCommand("UPDATE maneuver SET tourid=@tourid,narrative=@narrative,distance=@distance WHERE id=@id;");
            db.DefineParameter(undoUpdateCommand, "@id", System.Data.DbType.Int32, oldManeuver.Id);
            db.DefineParameter(undoUpdateCommand, "@tourid", System.Data.DbType.Int32, oldManeuver.TourId);
            db.DefineParameter(undoUpdateCommand, "@narrative", System.Data.DbType.String, oldManeuver.Narrative);
            db.DefineParameter(undoUpdateCommand, "@distance", System.Data.DbType.Decimal, oldManeuver.Distance);

            undoResult = db.ExecuteStatement(undoUpdateCommand);

            return undoResult;
        }
    }
}
