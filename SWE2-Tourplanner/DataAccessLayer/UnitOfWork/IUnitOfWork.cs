﻿using DataAccessLayer.Repositories;
using System;

namespace DataAccessLayer.UnitOfWork
{
    //<summary>
    //  The IUnitOfWork interface defines the needed functions and properties of a unit of work. As long as the interface is implemented correctly, any class can be used as a unit of work.
    //</summary>
    //
    public interface IUnitOfWork:IDisposable
    {
        /// <summary>
        /// The commit function takes every issued command and executes them together. That way, the database connection is only established once.
        /// </summary>
        /// <returns>Amount of affected rows by the commit. Can be used to check if the commit affected the expected amount of rows.</returns>
        int Commit();

        /// <summary>
        /// The rollback function reverts the data store to it's state before the commit. Like the Commit() method, only one database connection is established for every command in the rollback.
        /// </summary>
        /// <returns>Amount of affected rows by the rollback.</returns>
        int Rollback();

        /// <value>
        /// Provides the user with a tour repository. 
        /// </value>
        ITourRepository TourRepository { get; set; }

        /// <value>
        /// Provides the user with a tourlog repository. 
        /// </value>
        ITourLogRepository TourLogRepository { get; set; }

        /// <value>
        /// Provides the user with a maneuver repository. 
        /// </value>
        IManeuverRepository ManeuverRepository { get; set; }
    }
}
