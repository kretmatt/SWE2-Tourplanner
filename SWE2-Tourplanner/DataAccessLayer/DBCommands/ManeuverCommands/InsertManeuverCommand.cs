using Common.Logging;
using DataAccessLayer.DBConnection;
using Common.Entities;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBCommands.ManeuverCommands
{
    /// <summary>
    /// The InsertManeuverCommand is used for inserting new maneuvers.
    /// </summary>
    public class InsertManeuverCommand : IDBCommand
    {
        /// <summary>
        /// The connection to the database.
        /// </summary>
        private IDBConnection db;
        /// <summary>
        /// The maneuver to be created/deleted.
        /// </summary>
        private Maneuver maneuver;
        /// <summary>
        /// ILog instance used for logging errors, warnings etc.
        /// </summary>
        private log4net.ILog logger;

        /// <summary>
        /// Creates a new InsertManeuverCommand instance.
        /// </summary>
        /// <param name="maneuver">Maneuver to be inserted</param>
        /// <param name="db">Connection to the database</param>
        public InsertManeuverCommand(IDBConnection db, Maneuver maneuver)
        {
            this.db = db;
            this.maneuver = maneuver;
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        /// <summary>
        /// Inserts the new maneuver into the maneuver table.
        /// </summary>
        /// <returns>Amount of rows affected by the insert of the new maneuver. Expected: 1</returns>
        public int Execute()
        {
            int insertManeuverResult = 0;

            IDbCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
            db.DefineParameter(checkForTourCommand, "@tourid", System.Data.DbType.Int32, maneuver.TourId);

            List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

            if (tourResults.Count != 1)
            {
                logger.Warn($"A single tour with the id {maneuver.TourId} could not be found. A rollback could be necessary to ensure data consistency.");
                return insertManeuverResult;
            }

            IDbCommand retrieveNextIdCommand = new NpgsqlCommand("SELECT nextval(pg_get_serial_sequence('maneuver','id')) AS newid;");
            List<object[]> retrieveNextIdResult = db.QueryDatabase(retrieveNextIdCommand);

            maneuver.Id = Convert.ToInt32(retrieveNextIdResult[0][0]);

            IDbCommand insertManeuverCommand = new NpgsqlCommand("INSERT INTO maneuver (id,tourid,narrative,distance) VALUES (@id,@tourid,@narrative,@distance);");
            db.DefineParameter(insertManeuverCommand, "@id", System.Data.DbType.Int32, maneuver.Id);
            db.DefineParameter(insertManeuverCommand, "@tourid", System.Data.DbType.Int32, maneuver.TourId);
            db.DefineParameter(insertManeuverCommand, "@narrative", System.Data.DbType.String, maneuver.Narrative);
            db.DefineParameter(insertManeuverCommand, "@distance", System.Data.DbType.Decimal, maneuver.Distance);

            insertManeuverResult = db.ExecuteStatement(insertManeuverCommand);

            return insertManeuverResult;
        }
        /// <summary>
        /// Removes the previously inserted maneuver from the maneuver table.
        /// </summary>
        /// <returns>Amount of rows affected by the delete-statement of the previously inserted maneuver</returns>
        public int Undo()
        {
            int undoResult = 0;

            if (maneuver.Id > 0)
            {
                IDbCommand deleteManeuverCommand = new NpgsqlCommand("DELETE FROM maneuver WHERE id=@id;");
                db.DefineParameter(deleteManeuverCommand, "@id", System.Data.DbType.Int32, maneuver.Id);

                undoResult = db.ExecuteStatement(deleteManeuverCommand);
            }

            return undoResult;
        }
    }
}
