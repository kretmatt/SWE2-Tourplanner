using DataAccessLayer.DBConnection;
using Common.Entities;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands.TourCommands
{
    /// <summary>
    /// InsertTourCommand is used for inserting new tours.
    /// </summary>
    public class InsertTourCommand : IDBCommand
    {
        /// <summary>
        /// The connection to the database.
        /// </summary>
        private IDBConnection db;
        /// <summary>
        /// The tour to be inserted.
        /// </summary>
        private Tour tour;
        /// <summary>
        /// Create the InsertTourCommand instance.
        /// </summary>
        /// <param name="db">Connection to the database.</param>
        /// <param name="name">Name of the tour.</param>
        /// <param name="startLocation">Start location of the tour.</param>
        /// <param name="endLocation">End location of the tour.</param>
        /// <param name="routeInfo">Path to the route image.</param>
        /// <param name="distance">Length of the tour in km.</param>
        /// <param name="routeType">Route type of the tour.</param>
        /// <param name="description">Tour description.</param>
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
        /// <summary>
        /// Inserts a new tour into the tour table.
        /// </summary>
        /// <returns>Amount of rows affected by the insert statement. Expected: 1</returns>
        public int Execute()
        {
            int insertTourResult = 0;
            DBConnection.IDbCommand checkNameUniqueCommand = new NpgsqlCommand("SELECT * FROM tour WHERE name=@name;");
            db.DefineParameter(checkNameUniqueCommand, "@name", System.Data.DbType.String, tour.Name);

            if (db.QueryDatabase(checkNameUniqueCommand).Count == 0)
            {
                DBConnection.IDbCommand retrieveNextIdCommand = new NpgsqlCommand("SELECT nextval(pg_get_serial_sequence('tour','id')) AS newid;");
                List<object[]> retrieveNextIdResult = db.QueryDatabase(retrieveNextIdCommand);

                tour.Id = Convert.ToInt32(retrieveNextIdResult[0][0]);

                DBConnection.IDbCommand insertTourCommand = new NpgsqlCommand("INSERT INTO tour (id,name, startlocation, endlocation, routeinfo, distance, routetype, description) VALUES (@id,@name, @startlocation, @endlocation, @routeinfo, @distance, @routetype, @description);");
                db.DefineParameter(insertTourCommand, "@id", System.Data.DbType.Int32, tour.Id);
                db.DefineParameter(insertTourCommand, "@name", System.Data.DbType.String, tour.Name);
                db.DefineParameter(insertTourCommand, "@startlocation", System.Data.DbType.String, tour.StartLocation);
                db.DefineParameter(insertTourCommand, "@endlocation", System.Data.DbType.String, tour.EndLocation);
                db.DefineParameter(insertTourCommand, "@routeinfo", System.Data.DbType.String, tour.RouteInfo);
                db.DefineParameter(insertTourCommand, "@distance", System.Data.DbType.Decimal, tour.Distance);
                db.DefineParameter(insertTourCommand, "@routetype", System.Data.DbType.Int32, (int)tour.RouteType);
                db.DefineParameter(insertTourCommand, "@description", System.Data.DbType.String, tour.Description);

                insertTourResult = db.ExecuteStatement(insertTourCommand);
            }
            return insertTourResult;
        }
        /// <summary>
        /// Deletes the previously inserted tour.
        /// </summary>
        /// <returns>Amount of rows affected by the delete statement. Expected: 1</returns>
        public int Undo()
        {
            int undoResult = 0;
            if (tour.Id != -1)
            {
                DBConnection.IDbCommand removeTourCommand = new NpgsqlCommand("DELETE FROM tour WHERE id=@id;");
                db.DefineParameter(removeTourCommand, "@id", System.Data.DbType.Int32, tour.Id);
                undoResult = db.ExecuteStatement(removeTourCommand);
            }
            return undoResult;
        }
    }
}
