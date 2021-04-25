using DataAccessLayer.DBConnection;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Creates a new InsertManeuverCommand instance.
        /// </summary>
        /// <param name="db">Connection to the database</param>
        /// <param name="tourId">Id of the associated tour</param>
        /// <param name="narrative">Narrative of the new maneuver</param>
        /// <param name="distance">Distance/Length of the maneuver</param>
        public InsertManeuverCommand(IDBConnection db, int tourId, string narrative, double distance)
        {
            this.db = db;
            this.maneuver = new Maneuver()
            {
                Id=-1,
                TourId=tourId,
                Narrative=narrative,
                Distance=distance
            };
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
                return insertManeuverResult;

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

            if (maneuver.Id != -1)
            {
                IDbCommand deleteManeuverCommand = new NpgsqlCommand("DELETE FROM maneuver WHERE id=@id;");
                db.DefineParameter(deleteManeuverCommand, "@id", System.Data.DbType.Int32, maneuver.Id);

                //undoCommand.Parameters.AddWithValue("id", maneuver.Id);

                undoResult = db.ExecuteStatement(deleteManeuverCommand);
            }

            return undoResult;
        }
    }
}
