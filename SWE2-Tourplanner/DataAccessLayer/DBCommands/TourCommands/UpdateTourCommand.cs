using DataAccessLayer.DBConnection;
using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands.TourCommands
{
    /// <summary>
    /// UpdateTourCommand is used for updating tours.
    /// </summary>
    public class UpdateTourCommand : IDBCommand
    {
        /// <summary>
        /// Connection to the database.
        /// </summary>
        private IDBConnection db;
        /// <summary>
        /// New state of the tour.
        /// </summary>
        private Tour tour;
        /// <summary>
        /// Old state of the tour.
        /// </summary>
        private Tour oldTour;
        /// <summary>
        /// Creates the UpdateTourCommand instance.
        /// </summary>
        /// <param name="db">Connection to the database</param>
        /// <param name="tour">New state of the tour</param>
        /// <param name="oldTour">Old state of the tour</param>
        public UpdateTourCommand(IDBConnection db, Tour tour, Tour oldTour)
        {
            this.db = db;
            this.tour = tour;
            this.oldTour = oldTour;
        }
        /// <summary>
        /// Sets the data of the tour in the datbase to the new state.
        /// </summary>
        /// <returns>Amount of rows affected by the update-statement. Expected: 1</returns>
        public int Execute()
        {
            int updateTourResult = 0;
            if (tour.Id > 0)
            {
                IDbCommand updateTourCommand = new NpgsqlCommand("UPDATE tour SET name=@name, startlocation=@startlocation, endlocation=@endlocation, routeinfo=@routeinfo, distance=@distance, routetype=@routetype, description=@description WHERE id=@id;");
                db.DefineParameter(updateTourCommand, "@id", System.Data.DbType.Int32, tour.Id);
                db.DefineParameter(updateTourCommand, "@name", System.Data.DbType.String, tour.Name);
                db.DefineParameter(updateTourCommand, "@startlocation", System.Data.DbType.String, tour.StartLocation);
                db.DefineParameter(updateTourCommand, "@endlocation", System.Data.DbType.String, tour.EndLocation);
                db.DefineParameter(updateTourCommand, "@routeinfo", System.Data.DbType.String, tour.RouteInfo);
                db.DefineParameter(updateTourCommand, "@distance", System.Data.DbType.Decimal, tour.Distance);
                db.DefineParameter(updateTourCommand, "@routetype", System.Data.DbType.Int32, (int)tour.RouteType);
                db.DefineParameter(updateTourCommand, "@description", System.Data.DbType.String, tour.Description);

                updateTourResult = db.ExecuteStatement(updateTourCommand);
            }

            return updateTourResult;
        }
        /// <summary>
        /// Reverts the tour to its original state.
        /// </summary>
        /// <returns>Amount of rows affected by the rollback to the original state. Expected: 1</returns>
        public int Undo()
        {
            int undoUpdateTourResult = 0;

            if (oldTour.Id > 0)
            {
                IDbCommand undoUpdateTourCommand = new NpgsqlCommand("UPDATE tour SET name=@name, startlocation=@startlocation, endlocation=@endlocation, routeinfo=@routeinfo, distance=@distance, routetype=@routetype, description=@description WHERE id=@id;");
                db.DefineParameter(undoUpdateTourCommand, "@id", System.Data.DbType.Int32, oldTour.Id);
                db.DefineParameter(undoUpdateTourCommand, "@name", System.Data.DbType.String, oldTour.Name);
                db.DefineParameter(undoUpdateTourCommand, "@startlocation", System.Data.DbType.String, oldTour.StartLocation);
                db.DefineParameter(undoUpdateTourCommand, "@endlocation", System.Data.DbType.String, oldTour.EndLocation);
                db.DefineParameter(undoUpdateTourCommand, "@routeinfo", System.Data.DbType.String, oldTour.RouteInfo);
                db.DefineParameter(undoUpdateTourCommand, "@distance", System.Data.DbType.Decimal, oldTour.Distance);
                db.DefineParameter(undoUpdateTourCommand, "@routetype", System.Data.DbType.Int32, (int)oldTour.RouteType);
                db.DefineParameter(undoUpdateTourCommand, "@description", System.Data.DbType.String, oldTour.Description);

                undoUpdateTourResult = db.ExecuteStatement(undoUpdateTourCommand);
            }

            return undoUpdateTourResult;
        }
    }
}
