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


namespace BusinessLogicLayer.Factories
{
    public class TourPlannerFactory
    {
        private IMapQuestClient mapQuestClient;
        private ILog logger;
        private IUnitOfWork uow;
        public TourPlannerFactory()
        {
            mapQuestClient = new MapQuestClient();
            logger = LogHelper.GetLogHelper().GetLogger();
            uow = new UnitOfWork();
        }
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
                                                                          throw;
                                                                      }
                                                                  });
        public async Task CreateMapQuestTour(Tour tour)
        {
            await RetrieveMapQuestData(tour);
            await CreateTour(tour);
        }
        public async Task CreateTour(Tour tour) => await Task.Run(() =>
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
                                                           return tour;
                                                       });
        public async Task DeleteTour(Tour tour) => await Task.Run(() =>
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
                                                      });
        public async Task UpdateMapQuestTour(Tour tour)
        {
            await RetrieveMapQuestData(tour);
            await UpdateTour(tour);
        }
        public async Task UpdateTour(Tour tour) => await Task.Run(() =>
                                                       {
                                                           Tour oldTour = uow.TourRepository.Read(tour.Id);
                                                           if (oldTour != null)
                                                           {
                                                               using (uow)
                                                               {
                                                                   int expectedAffectedRows = 1;
                                                                   uow.TourRepository.Update(tour);

                                                                   if (oldTour.Maneuvers.Count != tour.Maneuvers.Count && Enumerable.SequenceEqual(oldTour.Maneuvers, tour.Maneuvers))
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
                                                                       if (File.Exists(tour.RouteInfo))
                                                                           File.Delete(tour.RouteInfo);
                                                                   }
                                                                   else
                                                                   {
                                                                       if (File.Exists(oldTour.RouteInfo))
                                                                           File.Delete(oldTour.RouteInfo);
                                                                   }
                                                               }
                                                           }
                                                       });
        public List<Tour> GetTours()
        {
            return uow.TourRepository.ReadAll();
        }
        public async Task CreateTourLog(TourLog tourLog) => await Task.Run(() =>
                                                                   {
                                                                       using (uow)
                                                                       {
                                                                           uow.TourLogRepository.Insert(tourLog);

                                                                           if (uow.Commit() != 1)
                                                                           {
                                                                               uow.Rollback();
                                                                               logger.Error("The amount of affected rows was not 1. Rollback to ensure data consistency.");
                                                                           }
                                                                       }
                                                                   });
        public async Task UpdateTourLog(TourLog tourLog) => await Task.Run(() =>
                                                                {
                                                                    using (uow)
                                                                    {
                                                                        uow.TourLogRepository.Update(tourLog);

                                                                        if (uow.Commit() != 1)
                                                                        {
                                                                            uow.Rollback();
                                                                            logger.Error("The amount of affected rows was not 1. Rollback to ensure data consistency.");
                                                                        }
                                                                    }
                                                                });
        public async Task DeleteTourLog(TourLog tourLog) => await Task.Run(() =>
                                                                {
                                                                    using (uow)
                                                                    {
                                                                        uow.TourLogRepository.Delete(tourLog.Id);
                                                                        if (uow.Commit() != 1)
                                                                        {
                                                                            uow.Rollback();
                                                                            logger.Error("Delete statement affected more than 1 row. Reroll to ensure data consistency.");
                                                                        }
                                                                    }
                                                                });
    }
}
