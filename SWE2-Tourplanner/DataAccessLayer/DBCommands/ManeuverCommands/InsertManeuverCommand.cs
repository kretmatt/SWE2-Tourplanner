using DataAccessLayer.DBConnection;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands.ManeuverCommands
{
    public class InsertManeuverCommand : IDBCommand
    {
        private IDBConnection db;
        private Maneuver maneuver;

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

        public int Execute()
        {
            int insertManeuverResult = 0;

            INpgsqlCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
            checkForTourCommand.Parameters.AddWithValue("tourid", maneuver.TourId);
            List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

            if (tourResults.Count != 1)
                return insertManeuverResult;

            INpgsqlCommand retrieveNextIdCommand = new NpgsqlCommand("SELECT nextval(pg_get_serial_sequence('maneuver','id')) AS newid;");
            List<object[]> retrieveNextIdResult = db.QueryDatabase(retrieveNextIdCommand);

            maneuver.Id = Convert.ToInt32(retrieveNextIdResult[0][0]);

            INpgsqlCommand insertManeuverCommand = new NpgsqlCommand("INSERT INTO maneuver (id,tourid,narrative,distance) VALUES (@id,@tourid,@narrative,@distance);");
            insertManeuverCommand.Parameters.AddWithValue("id", maneuver.Id);
            insertManeuverCommand.Parameters.AddWithValue("tourid",maneuver.TourId);
            insertManeuverCommand.Parameters.AddWithValue("narrative",maneuver.Narrative);
            insertManeuverCommand.Parameters.AddWithValue("distance", maneuver.Distance);

            insertManeuverResult = db.ExecuteStatement(insertManeuverCommand);

            return insertManeuverResult;
        }

        public int Undo()
        {
            int undoResult = 0;

            if (maneuver.Id != -1)
            {
                INpgsqlCommand undoCommand = new NpgsqlCommand("DELETE FROM maneuver WHERE id=@id;");
                undoCommand.Parameters.AddWithValue("id", maneuver.Id);

                undoResult = db.ExecuteStatement(undoCommand);
            }

            return undoResult;
        }
    }
}
