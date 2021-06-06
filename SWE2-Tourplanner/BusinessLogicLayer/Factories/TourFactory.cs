using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Entities;
using System.IO;
using DataAccessLayer.UnitOfWork;
using Common.MapQuestClient;
using log4net;
using Common.Logging;
using BusinessLogicLayer.Exceptions;
using DataAccessLayer.Exceptions;

namespace BusinessLogicLayer.Factories
{
    /// <summary>
    /// TourFactory is a concrete implementation of ITourFactory.
    /// </summary>
    public class TourFactory:ITourFactory
    {
        /// <summary>
        /// IMapQuestClient instance used for retrieving data from MapQuest
        /// </summary>
        private IMapQuestClient mapQuestClient;
        /// <summary>
        /// ILog instance used for logging errors, warnings etc.
        /// </summary>
        private ILog logger;
        /// <summary>
        /// IUnitOfWork instance used for accessing the datastore
        /// </summary>
        private IUnitOfWork uow;
        /// <summary>
        /// Default constructor of TourFactory
        /// </summary>
        public TourFactory()
        {
            mapQuestClient = new MapQuestClient();
            logger = LogHelper.GetLogHelper().GetLogger();
            uow = new UnitOfWork();
        }

        public TourFactory(IUnitOfWork uow)
        {
            mapQuestClient = new MapQuestClient();
            logger = LogHelper.GetLogHelper().GetLogger();
            this.uow = uow;
        }
        /// <summary>
        /// RetrieveMapQuestData is a helper function for retrieving MapQuest data using the MapQuestClient
        /// </summary>
        /// <param name="tour">Tour that needs MapQuest data</param>
        /// <returns>Task, which retrieves data from MapQuest</returns>
        /// <exception cref="BLFactoryException">Thrown, when the MapQuest request was not successful or if MapQuest could not generate a route</exception>
        private async Task RetrieveMapQuestData(Tour tour) => await Task.Run(async () =>
                                                                  {
                                                                      try
                                                                      {
                                                                          await mapQuestClient.GetRouteDataFromMapQuest(tour);
                                                                      }
                                                                      catch (Exception e)
                                                                      {
                                                                          if (e is ArgumentException || e is HttpRequestException)
                                                                          {
                                                                              logger.Error($"{e.GetType()}: {e.Message}");
                                                                          }
                                                                          throw new BLFactoryException($"The request to MapQuest was not successful. Details: {e.Message}");
                                                                      }
                                                                  });
        /// <summary>
        /// CreateMapQuestTour is a method for creating new Tours with data from MapQuest
        /// </summary>
        /// <param name="tour">Tour to be created</param>
        /// <returns>Task, which retrieves data from MapQuest API and creates a Tour</returns>
        public async Task CreateMapQuestTour(Tour tour)
        {
                await RetrieveMapQuestData(tour);
                await CreateTour(tour); 
        }
        /// <summary>
        /// CreateTour is a method for inserting new Tour entities in the datastore
        /// </summary>
        /// <param name="tour">Tour that is supposed to be created</param>
        /// <returns>Task, which inserts a new Tour entity in the datastore</returns>
        /// <exception cref="BLFactoryException">Thrown, when the map image doesn't exist or if DAL exceptions occur</exception>
        public async Task CreateTour(Tour tour) => await Task.Run(() =>
                                                       {
                                                           try
                                                           {
                                                               if (File.Exists(tour.RouteInfo))
                                                               {
                                                                   using (uow)
                                                                   {
                                                                       uow.TourRepository.Insert(tour);
                                                                       tour.Maneuvers.ForEach(m => uow.ManeuverRepository.Insert(m));
                                                                       if (uow.Commit() != 1 + tour.Maneuvers.Count)
                                                                       {
                                                                           uow.Rollback();
                                                                           logger.Error("The commit did not affect as many rows as expected. Rollback to previous state to ensure data consistency.");
                                                                       }
                                                                   }
                                                               }
                                                               else
                                                               {
                                                                   logger.Error("Tour map could not be found. Aborting tour creation process.");
                                                                   throw new FileNotFoundException("Tour map could not be found!");
                                                               }
                                                           }
                                                           catch(Exception e)
                                                           {
                                                               if(e is FileNotFoundException)
                                                               {
                                                                   logger.Error("There seems to have been an error downloading the mapquest map image. Stop Insert process!");
                                                                   throw new BLFactoryException("Tour can't be created because the map image does not exist!");
                                                               }
                                                               if(e is DALDBConnectionException || e is DALParameterException || e is DALRepositoryCommandException || e is DALUnitOfWorkException)
                                                               {
                                                                   logger.Error($"Tour data could not be saved. Details: {e.Message}");
                                                                   throw new BLFactoryException($"Tour data could not be saved due to the following reason: {e.Message}");
                                                               }
                                                               logger.Error($"An unhandled exception ocurred! {e.GetType()}: {e.Message}");
                                                               throw new BLFactoryException("The new tour could not be created! Look up the logs for further information!");
                                                           }                                                           
                                                       });
        /// <summary>
        /// DeleteTour is a method for deleting Tour entities in the datastore
        /// </summary>
        /// <param name="tour">Tour that is supposed to be deleted</param>
        /// <returns>Task, which deletes a Tour in the datastore</returns>
        /// <exception cref="BLFactoryException">Thrown, when DAL exceptions occur or if there are problems whilst deleting the map image</exception>
        public Task DeleteTour(Tour tour) => Task.Run(() =>
                                                      {
                                                          try
                                                          {
                                                              using (uow)
                                                              {
                                                                  uow.TourRepository.Delete(tour.Id);
                                                                  int affectedRows = uow.Commit();
                                                                  logger.Info($"Expected max affected rows: {1 + tour.Maneuvers.Count + tour.TourLogs.Count}");
                                                                  if (affectedRows > 1 + tour.Maneuvers.Count + tour.TourLogs.Count)
                                                                  {
                                                                      uow.Rollback();
                                                                      logger.Error("Delete statement affected more rows than expected. Reroll to ensure data consistency.");
                                                                  }
                                                                  else
                                                                  {
                                                                      if (File.Exists(tour.RouteInfo))
                                                                          File.Delete(tour.RouteInfo);
                                                                  }
                                                              }
                                                          }
                                                          catch(Exception e)
                                                          {
                                                              if (e is DALDBConnectionException || e is DALParameterException || e is DALRepositoryCommandException || e is DALUnitOfWorkException)
                                                              {
                                                                  logger.Error($"Tour could not be deleted. Details: {e.Message}");
                                                                  throw new BLFactoryException($"Tour data could not be deleted due to the following reason: {e.Message}");
                                                              }
                                                              if(e is UnauthorizedAccessException)
                                                              {
                                                                  logger.Error($"Tour could be deleted, but the associated map image could not be deleted due to unauthorized access!");
                                                                  throw new BLFactoryException($"Tour could be deleted, but the associated map image could not be deleted due to access problems!");
                                                              }
                                                              logger.Error($"An unhandled exception ocurred whilst deleting the tour! {e.GetType()}: {e.Message}");
                                                              throw new BLFactoryException("The tour could not be deleted! Look up the logs for further information!");
                                                          }
                                                          
                                                      });
        /// <summary>
        /// UpdateMapQuestTour is a method for updating a MapQuest Tour
        /// </summary>
        /// <param name="tour">Updated tour</param>
        /// <returns>Task, which retrieves data from MapQuest API and updates Tour entity in datastore</returns>
        public async Task UpdateMapQuestTour(Tour tour)
        {
            await RetrieveMapQuestData(tour);
            await UpdateTour(tour);
        }
        /// <summary>
        /// UpdateTour is a method for updating Tour entities in the datastore
        /// </summary>
        /// <param name="tour">Updated tour data</param>
        /// <returns>Task, which updates a Tour entity</returns>
        /// <exception cref="BLFactoryException">Thrown, when there are DAL exceptions or problems copying / deleting map images</exception>
        public Task UpdateTour(Tour tour) => Task.Run(() =>
                                                       {
                                                           try
                                                           {
                                                               Tour oldTour = uow.TourRepository.Read(tour.Id);
                                                               if (oldTour != null)
                                                               {
                                                                   using (uow)
                                                                   {
                                                                       int expectedAffectedRows = 1;
                                                                       uow.TourRepository.Update(tour);

                                                                       if (!Enumerable.SequenceEqual(oldTour.Maneuvers, tour.Maneuvers))
                                                                       {
                                                                           oldTour.Maneuvers.ForEach(m => uow.ManeuverRepository.Delete(m.Id));
                                                                           tour.Maneuvers.ForEach(m =>
                                                                           {
                                                                               m.TourId = tour.Id;
                                                                               uow.ManeuverRepository.Insert(m);
                                                                           });
                                                                           expectedAffectedRows += oldTour.Maneuvers.Count + tour.Maneuvers.Count;
                                                                       }

                                                                       if (uow.Commit() != expectedAffectedRows)
                                                                       {
                                                                           uow.Rollback();
                                                                           logger.Error("Update did not affect as many rows as expected. Rollback to ensure data consistency.");
                                                                           if (oldTour.RouteInfo != tour.RouteInfo && File.Exists(tour.RouteInfo))
                                                                               File.Delete(tour.RouteInfo);
                                                                       }
                                                                       else
                                                                       {
                                                                           if (tour.RouteInfo != oldTour.RouteInfo && File.Exists(oldTour.RouteInfo))
                                                                               File.Delete(oldTour.RouteInfo);
                                                                       }
                                                                   }
                                                               }
                                                           }
                                                           catch(Exception e)
                                                           {
                                                               if (e is DALDBConnectionException || e is DALParameterException || e is DALRepositoryCommandException || e is DALUnitOfWorkException)
                                                               {
                                                                   logger.Error($"Tour could not be updated. Details: {e.Message}");
                                                                   throw new BLFactoryException($"Tour data could not be updated due to the following reason: {e.Message}");
                                                               }
                                                               if (e is UnauthorizedAccessException)
                                                               {
                                                                   logger.Error($"Obsolete map image could not be deleted");
                                                                   throw new BLFactoryException($"Tour could be updated, but obsolete map image could not be deleted!");
                                                               }
                                                               logger.Error($"An unhandled exception ocurred whilst updating the tour! {e.GetType()}: {e.Message}");
                                                               throw new BLFactoryException("The tour could not be updated properly! Look up the logs for further information!");
                                                           }
                                                           
                                                       });
        /// <summary>
        /// GetTours is a method for retrieving Tour entities from the datastore
        /// </summary>
        /// <returns>All tours of the data store</returns>
        /// <exception cref="BLFactoryException">Thrown, if there are DAL exceptions or if other errors occur</exception>
        public List<Tour> GetTours()
        {
            List<Tour> tours = new List<Tour>();

            try
            {
                tours = uow.TourRepository.ReadAll();
            }
            catch (Exception e)
            {
                if (e is DALDBConnectionException || e is DALParameterException || e is DALRepositoryCommandException || e is DALUnitOfWorkException)
                {
                    logger.Error($"Tour data could not be retrieved because: {e.Message}");
                    throw new BLFactoryException($"Tours could not be retrieved due to following resons: {e.Message}");
                }
                logger.Error($"An unhandled exception ocurred whilst retrieving tour data! {e.GetType()}: {e.Message}");
                throw new BLFactoryException("Tours could not be retrieved from data store!");
            }

            return tours;
        }
    }
}
