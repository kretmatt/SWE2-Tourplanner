using System.Threading.Tasks;
using Common.Entities;

namespace BusinessLogicLayer.Factories
{
    /// <summary>
    /// ITourLogFactory defines several methods for managing TourLog entities
    /// </summary>
    public interface ITourLogFactory
    {
        /// <summary>
        /// CreateTourLog is a method for crating new TourLog entities in the datastore
        /// </summary>
        /// <param name="tourLog">TourLog to be created</param>
        /// <returns>Task, which creates a new TourLog entity in the datastore</returns>
        public Task CreateTourLog(TourLog tourLog);
        /// <summary>
        /// UpdateTourLog is a method for updating TourLog entities in the datastore
        /// </summary>
        /// <param name="tourLog">Updated TourLog entity</param>
        /// <returns>Task, which updates a TourLog entity in the database</returns>
        public Task UpdateTourLog(TourLog tourLog);
        /// <summary>
        /// DeleteTourLog is a method for deleting TourLog entities in the datastore
        /// </summary>
        /// <param name="tourLog">TourLog to be deleted</param>
        /// <returns>Task, which deletes a TourLog entity in the datastore</returns>
        public Task DeleteTourLog(TourLog tourLog);
    }
}
