using DataAccessLayer.DBConnection;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands.TourCommands
{
    public class UpdateTourCommand : IDBCommand
    {
        private IDBConnection db;
        private Tour tour;
        private Tour oldTour;

        public UpdateTourCommand(IDBConnection db, Tour tour, Tour oldTour)
        {
            this.db = db;
            this.tour = tour;
            this.oldTour = oldTour;
        }
        public int Execute()
        {
            int updateTourResult = 0;
            if (tour.Id > 0)
            {
                INpgsqlCommand updateTourCommand = new NpgsqlCommand("UPDATE tour SET name=@name, startlocation=@startlocation, endlocation=@endlocation, routeinfo=@routeinfo, distance=@distance, routetype=@routetype, description=@description WHERE id=@id;");
                updateTourCommand.Parameters.AddWithValue("id", tour.Id);
                updateTourCommand.Parameters.AddWithValue("name", tour.Name);
                updateTourCommand.Parameters.AddWithValue("startlocation", tour.StartLocation);
                updateTourCommand.Parameters.AddWithValue("endlocation", tour.EndLocation);
                updateTourCommand.Parameters.AddWithValue("routeinfo", tour.RouteInfo);
                updateTourCommand.Parameters.AddWithValue("distance", tour.Distance);
                updateTourCommand.Parameters.AddWithValue("routetype", (int)tour.RouteType);
                updateTourCommand.Parameters.AddWithValue("description", tour.Description);

                updateTourResult = db.ExecuteStatement(updateTourCommand);
            }

            return updateTourResult;
        }

        public int Undo()
        {
            int undoUpdateTourResult = 0;

            if (oldTour.Id > 0)
            {
                INpgsqlCommand undoUpdateTourCommand = new NpgsqlCommand("UPDATE tour SET name=@name, startlocation=@startlocation, endlocation=@endlocation, routeinfo=@routeinfo, distance=@distance, routetype=@routetype, description=@description WHERE id=@id;");
                undoUpdateTourCommand.Parameters.AddWithValue("id", oldTour.Id);
                undoUpdateTourCommand.Parameters.AddWithValue("name", oldTour.Name);
                undoUpdateTourCommand.Parameters.AddWithValue("startlocation", oldTour.StartLocation);
                undoUpdateTourCommand.Parameters.AddWithValue("endlocation", oldTour.EndLocation);
                undoUpdateTourCommand.Parameters.AddWithValue("routeinfo", oldTour.RouteInfo);
                undoUpdateTourCommand.Parameters.AddWithValue("distance", oldTour.Distance);
                undoUpdateTourCommand.Parameters.AddWithValue("routetype", (int)oldTour.RouteType);
                undoUpdateTourCommand.Parameters.AddWithValue("description", oldTour.Description);

                undoUpdateTourResult = db.ExecuteStatement(undoUpdateTourCommand);
            }

            return undoUpdateTourResult;
        }
    }
}
