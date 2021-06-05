using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.MapQuestClient
{
    /// <summary>
    /// The IMapQuestClient interface defines two methods for retrieving data from the MapQuest API
    /// </summary>
    public interface IMapQuestClient
    {
        /// <summary>
        /// RetrieveRouteImage retrieves the associated map image of a previous MapQuest API request.
        /// </summary>
        /// <param name="storeId">Name of the route image file</param>
        /// <param name="sessionId">SessionId of the previous request to the MapQuest API</param>
        /// <param name="boundingBox">BoundingBox of the map</param>
        /// <returns>Task, where a route image is retrieved from MapQuest</returns>
        Task<string> RetrieveRouteImage(string storeId, string sessionId, string boundingBox);
        /// <summary>
        /// Retrieves data from the MapQuest API for a specific tour and sets the data in the tour.
        /// </summary>
        /// <param name="tour">Tour that needs data from the MapQuest API</param>
        /// <returns>Task, where important data for the tour is retrieved from the MapQuest API and inserted into the tour.</returns>
        Task GetRouteDataFromMapQuest(Tour tour);
    }
}
