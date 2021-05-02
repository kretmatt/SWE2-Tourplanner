using Common.Logging;
using DataAccessLayer.DBConnection;
using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands.ManeuverCommands
{
    /// <summary>
    /// The DeleteManeuverCommand is used for deleting specific maneuvers.
    /// </summary>
    public class DeleteManeuverCommand : IDBCommand
    {
        /// <summary>
        /// The connection to the database.
        /// </summary>
        private IDBConnection db;
        /// <summary>
        /// The maneuver to be deleted/inserted again.
        /// </summary>
        private Maneuver maneuver;

        private log4net.ILog logger;

        /// <summary>
        /// Creates the new DeleteManeuverCommand and sets its values.
        /// </summary>
        /// <param name="db">Database connection.</param>
        /// <param name="maneuver">The maneuver to be deleted</param>
        public DeleteManeuverCommand(IDBConnection db, Maneuver maneuver)
        {
            this.db = db;
            this.maneuver = maneuver;
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        /// <summary>
        /// Removes a specific maneuver from the maneuver-table.
        /// </summary>
        /// <returns>Amount of rows affected by the delete-statement. Expected: 1</returns>
        public int Execute()
        {
            int deleteManeuverResult = 0;

            if (maneuver.Id > 0)
            {
                IDbCommand deleteManeuverCommand = new NpgsqlCommand("DELETE FROM maneuver WHERE id=@id;");
                db.DefineParameter(deleteManeuverCommand, "@id", System.Data.DbType.Int32,maneuver.Id);

                deleteManeuverResult = db.ExecuteStatement(deleteManeuverCommand);
            }

            return deleteManeuverResult;
        }
        /// <summary>
        /// Inserts the previously deleted maneuver into the database again.
        /// </summary>
        /// <returns>Amount of rows affected by the insert of the previously deleted data. Normally expected value: 1</returns>
        public int Undo()
        {
            int undoResult = 0;
            if (maneuver.Id > 0)
            {
                IDbCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
                db.DefineParameter(checkForTourCommand, "@tourid", System.Data.DbType.Int32, maneuver.TourId);

                List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

                if (tourResults.Count != 1)
                {
                    logger.Warn($"Tour with the id {maneuver.TourId} could not be found. A rollback could be necessary to ensure data consistency.");
                    return undoResult;
                }

                IDbCommand insertManeuverCommand = new NpgsqlCommand("INSERT INTO maneuver (id,tourid,narrative,distance) VALUES (@id,@tourid,@narrative,@distance);");
                db.DefineParameter(insertManeuverCommand, "@id", System.Data.DbType.Int32, maneuver.Id);
                db.DefineParameter(insertManeuverCommand, "@tourid", System.Data.DbType.Int32, maneuver.TourId);
                db.DefineParameter(insertManeuverCommand, "@narrative", System.Data.DbType.String, maneuver.Narrative);
                db.DefineParameter(insertManeuverCommand, "@distance", System.Data.DbType.Decimal, maneuver.Distance);

                undoResult = db.ExecuteStatement(insertManeuverCommand);
            }
            return undoResult;
        }
    }
}
