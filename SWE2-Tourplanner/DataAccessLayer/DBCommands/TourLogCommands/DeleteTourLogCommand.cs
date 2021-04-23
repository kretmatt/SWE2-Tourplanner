using DataAccessLayer.DBConnection;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands.TourLogCommands
{
    public class DeleteTourLogCommand:IDBCommand
    {
        private IDBConnection db;
        private TourLog tourLog;

        public DeleteTourLogCommand(IDBConnection db, TourLog tourLog)
        {
            this.db = db;
            this.tourLog = tourLog;
        }

        public int Execute()
        {
            int deleteTourLogResult = 0;

            if (tourLog.Id > 0) 
            {
                INpgsqlCommand deleteTourLogCommand = new NpgsqlCommand("DELETE FROM tourlog WHERE id=@id;");
                deleteTourLogCommand.Parameters.AddWithValue("id", tourLog.Id);

                deleteTourLogResult += db.ExecuteStatement(deleteTourLogCommand);
            }

            return deleteTourLogResult;
        }

        public int Undo()
        {
            int undoResult = 0;

            if (tourLog.TourId > 0)
            {
                INpgsqlCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
                checkForTourCommand.Parameters.AddWithValue("tourid", tourLog.TourId);
                List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

                if (tourResults.Count != 1)
                    return undoResult;

                INpgsqlCommand undoCommand = new NpgsqlCommand("INSERT INTO tourlog (id,tourid,startdate,enddate,distance,totaltime,rating,averagespeed,weather,travelmethod,report,temperature) VALUES (@id,@tourid,@startdate,@enddate,@distance,@totaltime,@rating,@averagespeed,@weather,@travelmethod,@report,@temperature);");
                undoCommand.Parameters.AddWithValue("id", tourLog.Id);
                undoCommand.Parameters.AddWithValue("tourid", tourLog.TourId);
                undoCommand.Parameters.AddWithValue("startdate", tourLog.StartDate);
                undoCommand.Parameters.AddWithValue("enddate", tourLog.EndDate);
                undoCommand.Parameters.AddWithValue("distance", tourLog.Distance);
                undoCommand.Parameters.AddWithValue("totaltime", tourLog.TotalTime);
                undoCommand.Parameters.AddWithValue("rating", tourLog.Rating);
                undoCommand.Parameters.AddWithValue("averagespeed", tourLog.AverageSpeed);
                undoCommand.Parameters.AddWithValue("weather", (int)tourLog.Weather);
                undoCommand.Parameters.AddWithValue("travelmethod", (int)tourLog.TravelMethod);
                undoCommand.Parameters.AddWithValue("report", tourLog.Report);
                undoCommand.Parameters.AddWithValue("temperature", tourLog.Temperature);

                undoResult = db.ExecuteStatement(undoCommand);
            }
            return undoResult;
        }
    }
}
