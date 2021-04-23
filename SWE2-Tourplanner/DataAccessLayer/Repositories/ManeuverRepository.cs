using DataAccessLayer.DBCommands;
using DataAccessLayer.DBCommands.ManeuverCommands;
using DataAccessLayer.DBConnection;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ManeuverRepository : IManeuverRepository
    {
        private IDBConnection db;
        private List<IDBCommand> commitCommands;
        public ManeuverRepository()
        {
            db = DatabaseConnection.GetDBConnection();
            commitCommands = new List<IDBCommand>();
        }

        public ManeuverRepository(IDBConnection db, List<IDBCommand> commitCommands)
        {
            this.db = db;
            this.commitCommands = commitCommands;
        }

        private Maneuver ConvertToManeuver(object[] row)
        {
            Maneuver maneuver = new Maneuver() 
            {
                Id=Convert.ToInt32(row[0]),
                TourId = Convert.ToInt32(row[1]),
                Narrative = row[2].ToString(),
                Distance = Convert.ToDouble(row[3])
            };

            return maneuver;
        }

        public void Delete(int id)
        {
            Maneuver maneuver = Read(id);
            if (maneuver != null)
            {
                commitCommands.Add(new DeleteManeuverCommand(db,maneuver));
            }
        }

        public void Insert(Maneuver entity)
        {
            if (!String.IsNullOrEmpty(entity.Narrative))
            {
                commitCommands.Add(new InsertManeuverCommand(db,entity.TourId,entity.Narrative,entity.Distance));
            }
        }

        public Maneuver Read(int id)
        {
            Maneuver maneuver = null;
            if (id > 0)
            {
                INpgsqlCommand readManeuverCommand = new NpgsqlCommand("SELECT * FROM maneuver WHERE id=@id;");
                readManeuverCommand.Parameters.AddWithValue("id",id);
                db.OpenConnection();
                List<object[]> readManeuverResults = db.QueryDatabase(readManeuverCommand);
                db.CloseConnection();
                if (readManeuverResults.Count > 0)
                {
                    maneuver = ConvertToManeuver(readManeuverResults[0]);
                }
            }
            return maneuver;
        }

        public List<Maneuver> ReadAll()
        {
            List<Maneuver> maneuvers = new List<Maneuver>();
            INpgsqlCommand readManeuversCommand = new NpgsqlCommand("SELECT * FROM maneuver;");
            db.OpenConnection();
            List<object[]> readManeuversResults = db.QueryDatabase(readManeuversCommand);
            db.CloseConnection();

            foreach(object[] row in readManeuversResults)
            {
                Maneuver maneuver = ConvertToManeuver(row);
                maneuvers.Add(maneuver);
            }

            return maneuvers;
        }

        public void Update(Maneuver entity)
        {
            Maneuver oldManeuver = Read(entity.Id);

            if (oldManeuver != null)
            {
                commitCommands.Add(new UpdateManeuverCommand(db, entity, oldManeuver));
            }
        }
    }
}
