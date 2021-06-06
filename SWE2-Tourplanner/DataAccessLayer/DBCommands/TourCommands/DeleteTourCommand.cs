using DataAccessLayer.DBConnection;
using Common.Entities;

namespace DataAccessLayer.DBCommands.TourCommands
{
    /// <summary>
    /// The DeleteTourCommand is used for deleting tours.
    /// </summary>
    public class DeleteTourCommand:IDBCommand
    {
        /// <summary>
        /// Connection to the database
        /// </summary>
        private IDBConnection db;
        /// <summary>
        /// Tour to be deleted/inserted again.
        /// </summary>
        private Tour tour;
        /// <summary>
        /// Creates the DeleteTourCommand instance.
        /// </summary>
        /// <param name="db">Connection to the database</param>
        /// <param name="tour">Tour to be deleted</param>
        public DeleteTourCommand(IDBConnection db, Tour tour)
        {
            this.db = db;
            this.tour = tour;
        }
        /// <summary>
        /// Deletes the specified tour.
        /// </summary>
        /// <returns>Amount of rows affected by the delete-statement. Expected: 1 + amount of associated maneuvers + amount of associated tourlogs</returns>
        public int Execute()
        {
            int deleteTourResult = 0;
            if (tour.Id > 0)
            {
                IDbCommand deleteTourCommand = new NpgsqlCommand("DELETE FROM tour WHERE id=@id;");
                db.DefineParameter(deleteTourCommand, "@id", System.Data.DbType.Int32, tour.Id);

                deleteTourResult = db.ExecuteStatement(deleteTourCommand);
            }

            return deleteTourResult;
        }
        /// <summary>
        /// Inserts the previously deleted tour again (and the previously associated data).
        /// </summary>
        /// <returns>Amount of rows affected by the insert-statement(s). Expected: 1 + amount of associated maneuvers + amount of associated tourlogs</returns>
        public int Undo()
        {
            int undoResult = 0;

            if (tour.Id > 0)
            {
                IDbCommand insertTourCommand = new NpgsqlCommand("INSERT INTO tour (id,name,startlocation,endlocation,routeinfo,distance,routetype,description) VALUES (@id,@name,@startlocation,@endlocation,@routeinfo,@distance,@routetype,@description);");
                db.DefineParameter(insertTourCommand, "@id", System.Data.DbType.Int32, tour.Id);
                db.DefineParameter(insertTourCommand, "@name", System.Data.DbType.String, tour.Name);
                db.DefineParameter(insertTourCommand, "@startlocation", System.Data.DbType.String, tour.StartLocation);
                db.DefineParameter(insertTourCommand, "@endlocation", System.Data.DbType.String, tour.EndLocation);
                db.DefineParameter(insertTourCommand, "@routeinfo", System.Data.DbType.String, tour.RouteInfo);
                db.DefineParameter(insertTourCommand, "@distance", System.Data.DbType.Decimal, tour.Distance);
                db.DefineParameter(insertTourCommand, "@routetype", System.Data.DbType.Int32, (int)tour.RouteType);
                db.DefineParameter(insertTourCommand, "@description", System.Data.DbType.String, tour.Description);

                undoResult = db.ExecuteStatement(insertTourCommand);
                //On cascade delete leads to removal of maneuvers and tourlogs if the respective tour is deleted. Undo needs to insert those pieces of data again
                tour.Maneuvers.ForEach(m =>
                {
                    IDbCommand insertManeuverCommand = new NpgsqlCommand("INSERT INTO maneuver (tourid, narrative, distance) VALUES (@tourid, @narrative, @distance);");
                    db.DefineParameter(insertManeuverCommand, "@id", System.Data.DbType.Int32, m.Id);
                    db.DefineParameter(insertManeuverCommand, "@tourid", System.Data.DbType.Int32, m.TourId);
                    db.DefineParameter(insertManeuverCommand, "@narrative", System.Data.DbType.String, m.Narrative);
                    db.DefineParameter(insertManeuverCommand, "@distance", System.Data.DbType.Decimal, m.Distance);

                    undoResult += db.ExecuteStatement(insertManeuverCommand);
                });

                tour.TourLogs.ForEach(t =>
                {
                    IDbCommand insertTourLogCommand = new NpgsqlCommand("INSERT INTO tourlog (id,tourid,startdate,enddate,distance,totaltime,rating,averagespeed,weather,travelmethod,report,temperature) VALUES (@id,@tourid,@startdate,@enddate,@distance,@totaltime,@rating,@averagespeed,@weather,@travelmethod,@report,@temperature);");
                    db.DefineParameter(insertTourLogCommand, "@id", System.Data.DbType.Int32, t.Id);
                    db.DefineParameter(insertTourLogCommand, "@tourid", System.Data.DbType.Int32, t.TourId);
                    db.DefineParameter(insertTourLogCommand, "@startdate", System.Data.DbType.DateTime, t.StartDate);
                    db.DefineParameter(insertTourLogCommand, "@enddate", System.Data.DbType.DateTime, t.EndDate);
                    db.DefineParameter(insertTourLogCommand, "@distance", System.Data.DbType.Decimal, t.Distance);
                    db.DefineParameter(insertTourLogCommand, "@totaltime", System.Data.DbType.Decimal, t.TotalTime);
                    db.DefineParameter(insertTourLogCommand, "@rating", System.Data.DbType.Decimal, t.Rating);
                    db.DefineParameter(insertTourLogCommand, "@averagespeed", System.Data.DbType.Decimal, t.AverageSpeed);
                    db.DefineParameter(insertTourLogCommand, "@weather", System.Data.DbType.Int32, (int)t.Weather);
                    db.DefineParameter(insertTourLogCommand, "@travelmethod", System.Data.DbType.Int32, (int)t.TravelMethod);
                    db.DefineParameter(insertTourLogCommand, "@report", System.Data.DbType.String, t.Report);
                    db.DefineParameter(insertTourLogCommand, "@temperature", System.Data.DbType.Decimal, t.Temperature);

                    undoResult += db.ExecuteStatement(insertTourLogCommand);
                });
            }

            return undoResult;
        }
    }
}
