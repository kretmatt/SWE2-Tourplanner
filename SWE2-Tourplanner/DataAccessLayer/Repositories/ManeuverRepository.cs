﻿using DataAccessLayer.DBCommands;
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
    /// <summary>
    /// ManeuverRepositories are used for querying, inserting, updating and deleting maneuvers. DML commands are not executed in this class. The only way to truly insert, update and delete maneuvers is through the UnitOfWork class, which ensures that database connections get opened/closed only when really needed. Retrieving data is possible everywhere.
    /// </summary>
    public class ManeuverRepository : IManeuverRepository
    {
        /// <summary>
        /// Database connection
        /// </summary>
        private IDBConnection db;
        /// <summary>
        /// Commands to be committed to the database.
        /// </summary>
        private List<IDBCommand> commitCommands;
        /// <summary>
        /// Creates the ManeuverRepository instance.
        /// </summary>
        public ManeuverRepository()
        {
            db = DatabaseConnection.GetDBConnection();
            commitCommands = new List<IDBCommand>();
        }
        /// <summary>
        /// Creates the ManeuverRepository instance and "connects" it to the UnitOfWork class
        /// </summary>
        /// <param name="db">Database connection</param>
        /// <param name="commitCommands">Commands for a commit (UnitOfWork)</param>
        public ManeuverRepository(IDBConnection db, List<IDBCommand> commitCommands)
        {
            this.db = db;
            this.commitCommands = commitCommands;
        }
        /// <summary>
        /// Converts object arrays to maneuvers.
        /// </summary>
        /// <param name="row">Result of a query</param>
        /// <returns>Converted maneuver.</returns>
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
        /// <summary>
        /// Deletes a maneuver with a specific id.
        /// </summary>
        /// <param name="id">Id of the maneuver to be deleted</param>
        public void Delete(int id)
        {
            Maneuver maneuver = Read(id);
            if (maneuver != null)
            {
                commitCommands.Add(new DeleteManeuverCommand(db,maneuver));
            }
        }
        /// <summary>
        /// Checks if properties are ok. If so, creates a InsertManeuverCommand instance with the specified data.
        /// </summary>
        /// <param name="entity">Maneuver to be created</param>
        public void Insert(Maneuver entity)
        {
            if (!String.IsNullOrEmpty(entity.Narrative))
            {
                commitCommands.Add(new InsertManeuverCommand(db,entity.TourId,entity.Narrative,entity.Distance));
            }
        }
        /// <summary>
        /// Function for retrieving a maneuver with a specific id.
        /// </summary>
        /// <param name="id">Id of the wanted maneuver.</param>
        /// <returns>Maneuver with the specified id.</returns>
        public Maneuver Read(int id)
        {
            Maneuver maneuver = null;
            if (id > 0)
            {
                IDbCommand readManeuverCommand = new NpgsqlCommand("SELECT * FROM maneuver WHERE id=@id;");
                db.DefineParameter(readManeuverCommand, "@id", System.Data.DbType.Int32, id);

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
        /// <summary>
        /// Function for retrieval of all available maneuvers.
        /// </summary>
        /// <returns>All maneuvers in the maneuver table</returns>
        public List<Maneuver> ReadAll()
        {
            List<Maneuver> maneuvers = new List<Maneuver>();
            IDbCommand readManeuversCommand = new NpgsqlCommand("SELECT * FROM maneuver;");
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
        /// <summary>
        /// Checks whether a maneuver with the same id as the parameter exists. If so, creates an UpdateManeuverCommand with the old and new state of the maneuver.
        /// </summary>
        /// <param name="entity">The new state of a maneuver.</param>
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