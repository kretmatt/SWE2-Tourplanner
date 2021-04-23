using DataAccessLayer.DBConnection;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands.ManeuverCommands
{
    public class UpdateManeuverCommand : IDBCommand
    {
        private IDBConnection db;
        private Maneuver maneuver;
        private Maneuver oldManeuver;

        public UpdateManeuverCommand(IDBConnection db, Maneuver maneuver, Maneuver oldManeuver)
        {
            this.db = db;
            this.maneuver = maneuver;
            this.oldManeuver = oldManeuver;
        }

        public int Execute()
        {
            int updateManeuverResult = 0;

            INpgsqlCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
            checkForTourCommand.Parameters.AddWithValue("tourid", maneuver.TourId);
            List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

            if (tourResults.Count != 1)
                return updateManeuverResult;

            INpgsqlCommand updateManeuverCommand = new NpgsqlCommand("UPDATE maneuver SET tourid=@tourid,narrative=@narrative,distance=@distance WHERE id=@id;");
            updateManeuverCommand.Parameters.AddWithValue("id", maneuver.Id);
            updateManeuverCommand.Parameters.AddWithValue("tourid", maneuver.TourId);
            updateManeuverCommand.Parameters.AddWithValue("narrative", maneuver.Narrative);
            updateManeuverCommand.Parameters.AddWithValue("distance", maneuver.Distance);

            updateManeuverResult = db.ExecuteStatement(updateManeuverCommand);

            return updateManeuverResult;
        }

        public int Undo()
        {
            int undoResult = 0;

            INpgsqlCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
            checkForTourCommand.Parameters.AddWithValue("tourid", oldManeuver.TourId);
            List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

            if (tourResults.Count != 1)
                return undoResult;

            INpgsqlCommand undoCommand = new NpgsqlCommand("UPDATE maneuver SET tourid=@tourid,narrative=@narrative,distance=@distance WHERE id=@id;");
            undoCommand.Parameters.AddWithValue("id", oldManeuver.Id);
            undoCommand.Parameters.AddWithValue("tourid", oldManeuver.TourId);
            undoCommand.Parameters.AddWithValue("narrative", oldManeuver.Narrative);
            undoCommand.Parameters.AddWithValue("distance", oldManeuver.Distance);

            undoResult = db.ExecuteStatement(undoCommand);

            return undoResult;
        }
    }
}
