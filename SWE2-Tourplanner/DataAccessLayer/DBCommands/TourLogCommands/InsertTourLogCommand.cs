using DataAccessLayer.DBConnection;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands.TourLogCommands
{
    public class InsertTourLogCommand : IDBCommand
    {
        private IDBConnection db;
        private TourLog tourLog;

        public InsertTourLogCommand(IDBConnection db, int tourId, DateTime startDate, DateTime endDate, double distance, double totalTime, double rating, double averageSpeed, EWeather weather, ETravelMethod travelMethod, string report, double temperature)
        {
            this.db = db;
            this.tourLog = new TourLog()
            {
                Id=-1,
                TourId=tourId,
                StartDate=startDate,
                EndDate=endDate,
                Distance = distance,
                TotalTime=totalTime,
                Rating=rating,
                AverageSpeed=averageSpeed,
                Weather=weather,
                TravelMethod=travelMethod,
                Report=report,
                Temperature=temperature
            };
        }


        public int Execute()
        {
            int insertTourLogResult = 0;

            if (tourLog.TourId > 0)
            {
                INpgsqlCommand checkForTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE id=@tourid;");
                checkForTourCommand.Parameters.AddWithValue("tourid", tourLog.TourId);
                List<object[]> tourResults = db.QueryDatabase(checkForTourCommand);

                if (tourResults.Count != 1)
                    return insertTourLogResult;

                INpgsqlCommand retrieveNextIdCommand = new NpgsqlCommand("SELECT nextval(pg_get_serial_sequence('tourlog','id')) AS newid;");
                List<object[]> retrieveNextIdResult = db.QueryDatabase(retrieveNextIdCommand);

                tourLog.Id = Convert.ToInt32(retrieveNextIdResult[0][0]);

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

                insertTourLogResult = db.ExecuteStatement(undoCommand);
            }
            return insertTourLogResult;
        }

        public int Undo()
        {
            int undoResult = 0;

            if (tourLog.Id != -1)
            {
                INpgsqlCommand undoCommand = new NpgsqlCommand("DELETE FROM tourlog WHERE id=@id;");
                undoCommand.Parameters.AddWithValue("id", tourLog.Id);

                undoResult = db.ExecuteStatement(undoCommand);
            }

            return undoResult;
            
        }
    }
}
