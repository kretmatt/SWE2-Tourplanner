using DataAccessLayer.DBConnection;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands.TourCommands
{
    public class InsertTourCommand : IDBCommand
    {
        private IDBConnection db;
        private Tour tour;

        public InsertTourCommand(IDBConnection db, string name, string startLocation, string endLocation, string routeInfo, double distance, ERouteType routeType, string description)
        {
            this.db = db;
            tour = new Tour()
            {
                Id=-1,
                Name = name,
                StartLocation = startLocation,
                EndLocation = endLocation,
                RouteInfo = routeInfo,
                Distance = distance,
                RouteType = routeType,
                Description = description
            };
        }

        public int Execute()
        {
            int insertTourResult = 0;
            INpgsqlCommand checkNameUniqueCommand = new NpgsqlCommand("SELECT * FROM tour WHERE name=@name;");
            checkNameUniqueCommand.Parameters.AddWithValue("name", tour.Name);
            
            if (db.QueryDatabase(checkNameUniqueCommand).Count == 0)
            {
                INpgsqlCommand retrieveNextIdCommand = new NpgsqlCommand("SELECT nextval(pg_get_serial_sequence('tour','id')) AS newid;");
                List<object[]> retrieveNextIdResult = db.QueryDatabase(retrieveNextIdCommand);

                tour.Id = Convert.ToInt32(retrieveNextIdResult[0][0]);

                INpgsqlCommand insertTourCommand = new NpgsqlCommand("INSERT INTO tour (id,name, startlocation, endlocation, routeinfo, distance, routetype, description) VALUES (@id,@name, @startlocation, @endlocation, @routeinfo, @distance, @routetype, @description);");
                insertTourCommand.Parameters.AddWithValue("id", tour.Id);
                insertTourCommand.Parameters.AddWithValue("name", tour.Name);
                insertTourCommand.Parameters.AddWithValue("startlocation", tour.StartLocation);
                insertTourCommand.Parameters.AddWithValue("endlocation", tour.EndLocation);
                insertTourCommand.Parameters.AddWithValue("routeinfo", tour.RouteInfo);
                insertTourCommand.Parameters.AddWithValue("distance", tour.Distance);
                insertTourCommand.Parameters.AddWithValue("routetype", (int)tour.RouteType);
                insertTourCommand.Parameters.AddWithValue("description", tour.Description);

                insertTourResult = db.ExecuteStatement(insertTourCommand);

                INpgsqlCommand readTourCommand = new NpgsqlCommand("SELECT * FROM tour WHERE name=@name;");
                readTourCommand.Parameters.AddWithValue("name", tour.Name);
                List<object[]> readTourResults = db.QueryDatabase(readTourCommand);
            }
            return insertTourResult;
        }

        public int Undo()
        {
            int undoResult = 0;
            if (tour.Id != -1)
            {
                INpgsqlCommand removeTourCommand = new NpgsqlCommand("DELETE FROM tour WHERE id=@id;");
                removeTourCommand.Parameters.AddWithValue("id", tour.Id);
                undoResult = db.ExecuteStatement(removeTourCommand);
            }
            return undoResult;
        }
    }
}
