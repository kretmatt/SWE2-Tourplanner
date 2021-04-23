using DataAccessLayer.DBConnection;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands.ManeuverCommands
{
    public class DeleteManeuverCommand : IDBCommand
    {
        private IDBConnection db;
        private Maneuver maneuver;

        public DeleteManeuverCommand(IDBConnection db, Maneuver maneuver)
        {
            this.db = db;
            this.maneuver = maneuver;
        }

        public int Execute()
        {
            int deleteManeuverResult = 0;

            if (maneuver.Id > 0)
            {
                INpgsqlCommand deleteManeuverCommand = new NpgsqlCommand("DELETE FROM maneuver WHERE id=@id;");
                deleteManeuverCommand.Parameters.AddWithValue("id", maneuver.Id);

                deleteManeuverResult = db.ExecuteStatement(deleteManeuverCommand);
            }

            return deleteManeuverResult;
        }

        public int Undo()
        {
            int undoResult = 0;
            if (maneuver.Id > 0)
            {
                INpgsqlCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
                checkForTourCommand.Parameters.AddWithValue("tourid", maneuver.TourId);
                List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

                if (tourResults.Count != 1)
                    return undoResult;

                INpgsqlCommand insertManeuverCommand = new NpgsqlCommand("INSERT INTO maneuver (id,tourid,narrative,distance) VALUES (@id,@tourid,@narrative,@distance);");
                insertManeuverCommand.Parameters.AddWithValue("id", maneuver.Id);
                insertManeuverCommand.Parameters.AddWithValue("tourid", maneuver.TourId);
                insertManeuverCommand.Parameters.AddWithValue("narrative", maneuver.Narrative);
                insertManeuverCommand.Parameters.AddWithValue("distance", maneuver.Distance);

                undoResult = db.ExecuteStatement(insertManeuverCommand);
            }
            return undoResult;
        }
    }
}
