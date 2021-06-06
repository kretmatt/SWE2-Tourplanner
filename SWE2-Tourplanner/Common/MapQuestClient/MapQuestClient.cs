using Common.Config;
using Common.Entities;
using Common.Logging;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.MapQuestClient
{
    /// <summary>
    /// Concrete implementation of interface IMapQuestClient
    /// </summary>
    public class MapQuestClient:IMapQuestClient
    {
        /// <summary>
        /// HTTPClient used for sending requests and receiving responses from MapQuest API
        /// </summary>
        private HttpClient client;
        /// <summary>
        /// Directions base URL used for retrieving tour data
        /// </summary>
        private static string routeBaseURL = "http://www.mapquestapi.com/directions/v2/route";
        /// <summary>
        /// Staticmap base URL used for retrieving map pictures
        /// </summary>
        private static string staticmapBaseURL = "http://www.mapquestapi.com/staticmap/v5/map";
        /// <summary>
        /// Width of map pictures
        /// </summary>
        private static int pictureWidth = 1920;
        /// <summary>
        /// Height of map pictures
        /// </summary>
        private static int pictureHeight = 1080;
        /// <summary>
        /// TourPlannerConfig object that provides MapQuestClient with MapQuestKey and PictureDirectory values
        /// </summary>
        private ITourPlannerConfig config;
        /// <summary>
        /// ILog instance used for logging errors, warnings etc.
        /// </summary>
        private ILog logger;

        /// <summary>
        /// Default MapQuestClient constructor
        /// </summary>
        public MapQuestClient()
        {
            client = new HttpClient();
            config = TourPlannerConfig.GetTourPlannerConfig();
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        /// <summary>
        /// Retrieves data from the MapQuest API for a specific tour and sets the data in the tour.
        /// </summary>
        /// <param name="tour">Tour that needs data from the MapQuest API</param>
        /// <returns>Task, where important data for the tour is retrieved from the MapQuest API and inserted into the tour.</returns>
        /// <exception cref="ArgumentException">Thrown, when MapQuest can't create tour with passed parameters</exception>
        /// <exception cref="HttpRequestException">Thrown, when the request doesn't have success status code</exception>
        public async Task GetRouteDataFromMapQuest(Tour tour)
        {
            string routeURL = $"{routeBaseURL}?key={config.MapQuestKey}&from={tour.StartLocation}&to={tour.EndLocation}&routeType={tour.RouteType}";
            logger.Info($"Send request to mapquestapi. Parameters: {tour.StartLocation},{tour.EndLocation},{tour.RouteType} ");
            HttpResponseMessage responseMessage = await client.GetAsync(routeURL);
            responseMessage.EnsureSuccessStatusCode();
            logger.Info($"Request was successful. Next step is iterating through the response.");
            JObject responseContent = JObject.Parse(await responseMessage.Content.ReadAsStringAsync());
            if (responseContent["route"]["distance"] != null && responseContent["route"]["legs"] != null)
            {
                JToken route = responseContent.SelectToken("route");
                tour.Distance = route.SelectToken("distance").ToObject<double>();
                foreach (JObject leg in route.SelectToken("legs"))
                {
                    if (leg["maneuvers"] != null)
                    {
                        tour.Maneuvers.Clear();
                        foreach (JObject maneuver in leg.SelectToken("maneuvers"))
                        {
                            Maneuver m = new Maneuver()
                            {
                                Narrative = maneuver.SelectToken("narrative").ToString(),
                                Distance = maneuver.SelectToken("distance").ToObject<double>()
                            };
                            tour.Maneuvers.Add(m);
                        }
                    }
                }
                if (responseContent["route"]["sessionId"] != null && responseContent["route"]["boundingBox"] != null)
                {
                    JToken boundingBox = route.SelectToken("boundingBox");
                    NumberFormatInfo numberFormat = new NumberFormatInfo();
                    numberFormat.NumberDecimalSeparator = ".";
                    string ul_lng = boundingBox.SelectToken("ul").SelectToken("lng").ToObject<double>().ToString(numberFormat);
                    string ul_lat = boundingBox.SelectToken("ul").SelectToken("lat").ToObject<double>().ToString(numberFormat);
                    string lr_lng = boundingBox.SelectToken("lr").SelectToken("lng").ToObject<double>().ToString(numberFormat);
                    string lr_lat = boundingBox.SelectToken("lr").SelectToken("lat").ToObject<double>().ToString(numberFormat);
                    string boundingBoxString = $"{ul_lat},{ul_lng},{lr_lat},{lr_lng}";
                    tour.RouteInfo = await RetrieveRouteImage(Guid.NewGuid().ToString(), responseContent.SelectToken("route").SelectToken("sessionId").ToString(), boundingBoxString);
                    logger.Info("The tour data was successfully retrieved from map quest!");
                }
            }
            else
                throw new ArgumentException($"With the passed parameters {tour.StartLocation}, {tour.EndLocation} and {tour.RouteType} no route could be created by mapquest. Try again with other values!");
        }
        /// <summary>
        /// RetrieveRouteImage retrieves the associated map image of a previous MapQuest API request.
        /// </summary>
        /// <param name="storeId">Name of the route image file</param>
        /// <param name="sessionId">SessionId of the previous request to the MapQuest API</param>
        /// <param name="boundingBox">BoundingBox of the map</param>
        /// <returns>Task, where a route image is retrieved from MapQuest</returns>
        /// <exception cref="ArgumentException">Thrown, when the MapQuest Request with parameters boundingBox, sessionId and size is not successful</exception>
        /// <exception cref="HttpRequestException">Thrown, when the request doesn't have success status code</exception>
        public async Task<string> RetrieveRouteImage(string storeId, string sessionId, string boundingBox)
        {
            string fileLocation = $@"{config.PictureDirectory}{storeId}.png";
            logger.Info($"Send request to mapquestapi staticmap. Parameters: {boundingBox}");
            HttpResponseMessage responseMessage = await client.GetAsync($"{staticmapBaseURL}?key={config.MapQuestKey}&size={pictureWidth},{pictureHeight}&session={sessionId}&boundingBox={boundingBox}");
            responseMessage.EnsureSuccessStatusCode();
            logger.Info($"Request was successful. Image is going to be saved at the following location: {fileLocation}");
            Stream fileStream = await responseMessage.Content.ReadAsStreamAsync();

            if (fileStream.Length == 0)
                throw new ArgumentException($"With the passed parameters sessionId (due to safety reasons not in the message), size={pictureWidth},{pictureHeight} and boundingBox={boundingBox} no map could be created by mapquest.");

            using (FileStream fs = File.Create(fileLocation))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                fileStream.CopyTo(fs);
            }
            return fileLocation;
        }
    }
}
