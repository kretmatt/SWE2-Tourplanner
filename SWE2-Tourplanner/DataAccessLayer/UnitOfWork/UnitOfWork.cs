﻿using Common.Logging;
using DataAccessLayer.DBCommands;
using DataAccessLayer.DBConnection;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.UnitOfWork
{
    /// <summary>
    /// Concrete implementation of the IUnitOfWork interface. 
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// commitCommands is a collection of every issued command for a commit.
        /// </summary>
        private List<IDBCommand> commitCommands;
        /// <summary>
        /// rollbackCommands is a collection of every commited command.
        /// </summary>
        private List<IDBCommand> rollbackCommands;
        /// <summary>
        /// IDbConnection is the database connection.
        /// </summary>
        private IDBConnection db;
        /// <summary>
        /// The provided Tour repository for the user. Insert-, Update- and Delete-TourCommands produced by Insert(), Update() and Delete() are saved into the commitCommands collection.
        /// </summary>
        private ITourRepository tourRepository;
        /// <summary>
        /// The provided TourLog repository for the user. Insert-, Update- and Delete-TourLogCommands produced by Insert(), Update() and Delete() are saved into the commitCommands collection.
        /// </summary>
        private ITourLogRepository tourLogRepository;
        /// <summary>
        /// The provided Maneuver repository for the user. Insert-, Update- and Delete-ManeuverCommands produced by Insert(), Update() and Delete() are saved into the commitCommands collection.
        /// </summary>
        private IManeuverRepository maneuverRepository;
        /// <summary>
        /// ILog object used for logging errors etc.
        /// </summary>
        private log4net.ILog logger;
        /// <summary>
        /// A public property for setting and getting the private property tourRepository.
        /// </summary>
        public ITourRepository TourRepository
        {
            get 
            {
                return tourRepository;
            } 
            set
            {
                if (value != tourRepository)
                    tourRepository = value;
            } 
        }
        /// <summary>
        /// A public property for setting and getting the private property tourLogRepository.
        /// </summary>
        public ITourLogRepository TourLogRepository 
        {
            get
            {
                return tourLogRepository;
            }
            set 
            {
                if (value != tourLogRepository)
                    tourLogRepository = value;
            }
        }
        /// <summary>
        /// A public property for setting and getting the private property maneuverRepository.
        /// </summary>
        public IManeuverRepository ManeuverRepository
        {
            get
            {
                return maneuverRepository;
            }
            set
            {
                if (value != maneuverRepository)
                    maneuverRepository = value;
            }
        }
        /// <summary>
        /// Default constructor for UnitOfWork. Retrieves the database connection from a singleton and properly creates the 3 repositories.
        /// </summary>
        public UnitOfWork()
        {
            db = DatabaseConnection.GetDBConnection();
            commitCommands = new List<IDBCommand>();
            rollbackCommands = new List<IDBCommand>();
            tourLogRepository = new TourLogRepository(db, commitCommands);
            maneuverRepository = new ManeuverRepository(db, commitCommands);
            tourRepository = new TourRepository(db,commitCommands);
            logger = LogHelper.GetLogHelper().GetLogger();
        }

        public UnitOfWork(ITourLogRepository tourLogRepository, ITourRepository tourRepository, IManeuverRepository maneuverRepository)
        {
            db = DatabaseConnection.GetDBConnection();
            commitCommands = new List<IDBCommand>();
            rollbackCommands = new List<IDBCommand>();

            logger = LogHelper.GetLogHelper().GetLogger();
        }

        /// <summary>
        /// A constructor just for testing purposes.
        /// </summary>
        /// <param name="db">The database connection. For testing purposes, a mock can be passed.</param>
        /// <param name="commitCommands">An IDBCommand collection. Can be used to verify if the amount of commands and their types are correct in test-methods.</param>
        /// <param name="rollbackCommands">An IDbCommand collection. Can be used to verify if the Commit() function puts executed commands into rhe rollbackCommands-Collection.</param>
        public UnitOfWork(IDBConnection db, List<IDBCommand> commitCommands, List<IDBCommand> rollbackCommands)
        {
            this.db = db;
            this.commitCommands = commitCommands;
            this.rollbackCommands = rollbackCommands;
            tourLogRepository = new TourLogRepository(db, commitCommands);
            maneuverRepository = new ManeuverRepository(db, commitCommands);
            tourRepository = new TourRepository(db, commitCommands);
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        /// <summary>
        /// Concrete imlementation of the Commit() function of the IUnitOfWork interface. Takes every issued command for the commit and executes them together.
        /// </summary>
        /// <returns>The amount of rows affected by the commit.</returns>
        public int Commit()
        {
            int commitCount = 0;
            logger.Info($"Starting database transaction. Commiting {commitCommands.Count} commands.");
            db.OpenConnection();
            commitCommands.ForEach(cc =>
            {
                commitCount += cc.Execute();
                rollbackCommands.Add(cc);
            });
            db.CloseConnection();
            logger.Info($"Finished database transaction. {commitCount} rows were affected by the issued commands.");
            return commitCount;
        }
        /// <summary>
        /// The dispose method is implemented for UnitsOfWork to be used in a using statement.
        /// </summary>
        public void Dispose()
        {
            commitCommands.Clear();
            rollbackCommands.Clear();
        }
        /// <summary>
        /// Concrete implementation of the Rollback() function of the IUnitOfWork interface. Takes every commited command and executes their Undo() function.
        /// </summary>
        /// <returns>The amount of rows affected by the rollback.</returns>
        public int Rollback()
        {
            int rollbackCount = 0;
            logger.Info($"Starting database rollback. Rolling back {rollbackCommands.Count} commands.");
            db.OpenConnection();
            rollbackCommands.Reverse<IDBCommand>().ToList().ForEach(rc => 
            {
                rollbackCount+= rc.Undo();
            });
            db.CloseConnection();
            rollbackCommands.Clear();
            logger.Info($"Finished database rollback. {rollbackCount} rows were affected by the rollback.");
            return rollbackCount;
        }
    }
}
