using DataAccessLayer.DBConnection;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands.TourLogCommands
{
    public class UpdateTourLogCommand : IDBCommand
    {
        private IDBConnection db;
        private TourLog tourLog;
        private TourLog oldTourLog;

        public UpdateTourLogCommand(IDBConnection db, TourLog tourLog, TourLog oldTourLog)
        {
            this.db = db;
            this.tourLog = tourLog;
            this.oldTourLog = oldTourLog;
        }

        public int Execute()
        {
            int updateTourLogResult = 0;

            INpgsqlCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
            checkForTourCommand.Parameters.AddWithValue("tourid", tourLog.TourId);
            List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

            if (tourResults.Count != 1)
                return updateTourLogResult;
            
            INpgsqlCommand updateTourLogCommand = new NpgsqlCommand("UPDATE tourlog SET tourid=@tourid, startdate=@startdate, enddate=@enddate, distance=@distance, totaltime=@totaltime, rating=@rating, averagespeed=@averagespeed, weather=@weather, travelmethod=@travelmethod, report=@report, temperature=@temperature WHERE id = @id;");
            updateTourLogCommand.Parameters.AddWithValue("id", tourLog.Id);
            updateTourLogCommand.Parameters.AddWithValue("tourid", tourLog.TourId);
            updateTourLogCommand.Parameters.AddWithValue("startdate", tourLog.StartDate);
            updateTourLogCommand.Parameters.AddWithValue("enddate", tourLog.EndDate);
            updateTourLogCommand.Parameters.AddWithValue("distance", tourLog.Distance);
            updateTourLogCommand.Parameters.AddWithValue("totaltime", tourLog.TotalTime);
            updateTourLogCommand.Parameters.AddWithValue("rating", tourLog.Rating);
            updateTourLogCommand.Parameters.AddWithValue("averagespeed", tourLog.AverageSpeed);
            updateTourLogCommand.Parameters.AddWithValue("weather", (int)tourLog.Weather);
            updateTourLogCommand.Parameters.AddWithValue("travelmethod", (int)tourLog.TravelMethod);
            updateTourLogCommand.Parameters.AddWithValue("report", tourLog.Report);
            updateTourLogCommand.Parameters.AddWithValue("temperature", tourLog.Temperature);

            updateTourLogResult = db.ExecuteStatement(updateTourLogCommand);

            return updateTourLogResult;
        }

        public int Undo()
        {
            int undoResult = 0;

            INpgsqlCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
            checkForTourCommand.Parameters.AddWithValue("tourid", tourLog.TourId);
            List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

            if (tourResults.Count != 1)
                return undoResult;

            INpgsqlCommand undoCommand = new NpgsqlCommand("UPDATE tourlog SET tourid=@tourid, startdate=@startdate, enddate=@enddate, distance=@distance, totaltime=@totaltime, rating=@rating, averagespeed=@averagespeed, weather=@weather, travelmethod=@travelmethod, report=@report, temperature=@temperature WHERE id = @id;");
            undoCommand.Parameters.AddWithValue("id", oldTourLog.Id);
            undoCommand.Parameters.AddWithValue("tourid", oldTourLog.TourId);
            undoCommand.Parameters.AddWithValue("startdate", oldTourLog.StartDate);
            undoCommand.Parameters.AddWithValue("enddate", oldTourLog.EndDate);
            undoCommand.Parameters.AddWithValue("distance", oldTourLog.Distance);
            undoCommand.Parameters.AddWithValue("totaltime", oldTourLog.TotalTime);
            undoCommand.Parameters.AddWithValue("rating", oldTourLog.Rating);
            undoCommand.Parameters.AddWithValue("averagespeed", oldTourLog.AverageSpeed);
            undoCommand.Parameters.AddWithValue("weather", (int)oldTourLog.Weather);
            undoCommand.Parameters.AddWithValue("travelmethod", (int)oldTourLog.TravelMethod);
            undoCommand.Parameters.AddWithValue("report", oldTourLog.Report);
            undoCommand.Parameters.AddWithValue("temperature", oldTourLog.Temperature);

            undoResult = db.ExecuteStatement(undoCommand);

            return undoResult;
        }
    }
}
