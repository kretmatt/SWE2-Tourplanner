using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    /// <summary>
    /// Generic repository that defines basic functions for querying, inserting, updating and deleting data.
    /// </summary>
    /// <typeparam name="T">Class from Entities-Namespace: Tour, TourLog, Maneuver</typeparam>
    public interface IRepository<T> where T:class
    {
        /// <summary>
        /// Inserts a new entity into the corresponding table.
        /// </summary>
        /// <param name="entity">Entity to be inserted</param>
        void Insert(T entity);
        /// <summary>
        /// Retrieves an entity with the specified Id
        /// </summary>
        /// <param name="id">Id of the wanted entity</param>
        /// <returns>Entity with the specified id or null if no such entity exists.</returns>
        T Read(int id);
        /// <summary>
        /// Retrieves every entity of the type T.
        /// </summary>
        /// <returns>Collection of every entity in the corresponding table.</returns>
        List<T> ReadAll();
        /// <summary>
        /// Updates the data in the respective table according to the new state.
        /// </summary>
        /// <param name="entity">New state of the entity</param>
        void Update(T entity);
        /// <summary>
        /// Removes an entity with the specified id from the T table.
        /// </summary>
        /// <param name="id">Id of the entity that is supposed to be deleted.</param>
        void Delete(int id);
    }
}
