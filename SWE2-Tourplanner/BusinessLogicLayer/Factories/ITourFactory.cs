using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Factories
{
    /// <summary>
    /// ITourFactory defines several methods for managing and retrieving Tour data from datastore
    /// </summary>
    public interface ITourFactory
    {
        /// <summary>
        /// CreateMapQuestTour is a method for creating new Tours with data from MapQuest
        /// </summary>
        /// <param name="tour">Tour to be created</param>
        /// <returns>Task, which retrieves data from MapQuest API and creates a Tour</returns>
        public Task CreateMapQuestTour(Tour tour);
        /// <summary>
        /// CreateTour is a method for inserting new Tour entities in the datastore
        /// </summary>
        /// <param name="tour">Tour that is supposed to be created</param>
        /// <returns>Task, which inserts a new Tour entity in the datastore</returns>
        public Task CreateTour(Tour tour);
        /// <summary>
        /// DeleteTour is a method for deleting Tour entities in the datastore
        /// </summary>
        /// <param name="tour">Tour that is supposed to be deleted</param>
        /// <returns>Task, which deletes a Tour in the datastore</returns>
        public Task DeleteTour(Tour tour);
        /// <summary>
        /// UpdateMapQuestTour is a method for updating a MapQuest Tour
        /// </summary>
        /// <param name="tour">Updated tour</param>
        /// <returns>Task, which retrieves data from MapQuest API and updates Tour entity in datastore</returns>
        public Task UpdateMapQuestTour(Tour tour);
        /// <summary>
        /// UpdateTour is a method for updating Tour entities in the datastore
        /// </summary>
        /// <param name="tour">Updated tour data</param>
        /// <returns>Task, which updates a Tour entity</returns>
        public Task UpdateTour(Tour tour);
        /// <summary>
        /// GetTours is a method for retrieving Tour entities
        /// </summary>
        /// <returns>All Tour entities from the datastore</returns>
        public List<Tour> GetTours();
    }
}
