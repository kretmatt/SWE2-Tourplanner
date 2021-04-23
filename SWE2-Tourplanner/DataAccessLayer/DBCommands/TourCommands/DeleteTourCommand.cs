using DataAccessLayer.DBConnection;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands.TourCommands
{
    public class DeleteTourCommand:IDBCommand
    {
        private IDBConnection db;
        private Tour tour;

        public DeleteTourCommand(IDBConnection db, Tour tour)
        {
            this.db = db;
            this.tour = tour;
        }
        public int Execute()
        {
            int deleteTourResult = 0;
            if (tour.Id > 0)
            {
                INpgsqlCommand deleteTourCommand = new NpgsqlCommand("DELETE FROM tour WHERE id=@id;");
                deleteTourCommand.Parameters.AddWithValue("id", tour.Id);

                deleteTourResult = db.ExecuteStatement(deleteTourCommand);
            }

            return deleteTourResult;
        }

        public int Undo()
        {
            int undoResult = 0;

            if (tour.Id > 0)
            {
                INpgsqlCommand undoDeleteTourCommand = new NpgsqlCommand("INSERT INTO tour (id,name,startlocation,endlocation,routeinfo,distance,routetype,description) VALUES (@id,@name,@startlocation,@endlocation,@routeinfo,@distance,@routetype,@description);");
                undoDeleteTourCommand.Parameters.AddWithValue("id", tour.Id);
                undoDeleteTourCommand.Parameters.AddWithValue("name", tour.Name);
                undoDeleteTourCommand.Parameters.AddWithValue("startlocation", tour.StartLocation);
                undoDeleteTourCommand.Parameters.AddWithValue("endlocation", tour.EndLocation);
                undoDeleteTourCommand.Parameters.AddWithValue("routeinfo", tour.RouteInfo);
                undoDeleteTourCommand.Parameters.AddWithValue("distance", tour.Distance);
                undoDeleteTourCommand.Parameters.AddWithValue("routetype", (int)tour.RouteType);
                undoDeleteTourCommand.Parameters.AddWithValue("description", tour.Description);

                undoResult = db.ExecuteStatement(undoDeleteTourCommand);
                //On cascade delete leads to removal of maneuvers and tourlogs if the respective tour is deleted. Undo needs to insert those pieces of data again
                tour.Maneuvers.ForEach(m =>
                {
                    INpgsqlCommand insertManeuverCommand = new NpgsqlCommand("INSERT INTO maneuver (tourid, narrative, distance) VALUES (@tourid, @narrative, @distance);");
                    insertManeuverCommand.Parameters.AddWithValue("tourid", tour.Id);
                    insertManeuverCommand.Parameters.AddWithValue("narrative", m.Narrative);
                    insertManeuverCommand.Parameters.AddWithValue("distance", m.Distance);

                    undoResult += db.ExecuteStatement(insertManeuverCommand);
                });

                tour.TourLogs.ForEach(t =>
                {
                    INpgsqlCommand insertTourLogCommand = new NpgsqlCommand("INSERT INTO tourlog (tourid,startdate,enddate,distance,totaltime,rating,averagespeed,weather,travelmethod,report,temperature) VALUES (@tourid,@startdate,@enddate,@distance,@totaltime,@rating,@averagespeed,@weather,@travelmethod,@report,@temperature);");
                    insertTourLogCommand.Parameters.AddWithValue("tourid",tour.Id);
                    insertTourLogCommand.Parameters.AddWithValue("startdate", t.StartDate);
                    insertTourLogCommand.Parameters.AddWithValue("enddate", t.EndDate);
                    insertTourLogCommand.Parameters.AddWithValue("distance", t.Distance);
                    insertTourLogCommand.Parameters.AddWithValue("totaltime", t.TotalTime);
                    insertTourLogCommand.Parameters.AddWithValue("rating", t.Rating);
                    insertTourLogCommand.Parameters.AddWithValue("averagespeed", t.AverageSpeed);
                    insertTourLogCommand.Parameters.AddWithValue("weather", (int)t.Weather);
                    insertTourLogCommand.Parameters.AddWithValue("travelmethod", (int)t.TravelMethod);
                    insertTourLogCommand.Parameters.AddWithValue("report", t.Report);
                    insertTourLogCommand.Parameters.AddWithValue("temperature", t.Temperature);

                    undoResult += db.ExecuteStatement(insertTourLogCommand);
                });
            }

            return undoResult;
        }
    }
}
