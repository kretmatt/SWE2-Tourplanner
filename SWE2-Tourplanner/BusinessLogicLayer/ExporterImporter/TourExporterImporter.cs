using Common.Config;
using Common.Entities;
using Common.Logging;
using DataAccessLayer.UnitOfWork;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ExporterImporter
{
    public class TourExporterImporter:IExporterImporter<Tour>
    {
        private ITourPlannerConfig tourPlannerConfig;
        private ILog logger;

        public TourExporterImporter()
        {
            tourPlannerConfig = TourPlannerConfig.GetTourPlannerConfig();
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        private void ExportPicture(string originalLocation, string exportLocation)
        {
            if (originalLocation == exportLocation)
                logger.Info("File location and export location are the same. Export of image will be skipped");
            else
            {
                logger.Info("Export map picture");
                if (File.Exists(originalLocation))
                    File.Copy(originalLocation, exportLocation, true);
                else
                    logger.Warn("Map picture could not be exported because it could not be found!");
            }
        }

        public async Task Export(List<Tour> tours)
        {
            await Task.Run(() =>
            {
                List<Tour> tourCopies = new List<Tour>();

                foreach (Tour tour in tours)
                {
                    string newRouteInfoPath = $@"{tourPlannerConfig.ExportsDirectory}{Path.GetFileName(tour.RouteInfo)}";
                    ExportPicture(tour.RouteInfo, newRouteInfoPath);
                    Tour tourCopy = new Tour()
                    {
                        Name = tour.Name,
                        StartLocation = tour.StartLocation,
                        EndLocation = tour.EndLocation,
                        RouteInfo = newRouteInfoPath,
                        Distance = tour.Distance,
                        Description = tour.Description,
                        RouteType = tour.RouteType,
                        Maneuvers = tour.Maneuvers,
                        TourLogs = tour.TourLogs
                    };
                    tourCopies.Add(tourCopy);
                }
                string jsonExportFileName = $@"{tourPlannerConfig.ExportsDirectory}Tours{DateTime.Now.ToString("yyyyMMddHHmmss")}.json";
                logger.Info("Export tours data");
                using (StreamWriter writer = File.CreateText(jsonExportFileName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, tourCopies);
                }
            });
        }

        public async Task<List<Tour>> Import(string filePath)
        {
            return await Task.Run(() =>
            {
                List<Tour> importedTours = new List<Tour>();
                try
                {
                    if (File.Exists(filePath))
                    {
                        using (StreamReader reader = File.OpenText(filePath))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            importedTours = (List<Tour>)serializer.Deserialize(reader, typeof(List<Tour>));

                            using (IUnitOfWork unitOfWork = new UnitOfWork())
                            {
                                int predictedAffectedRowCount = 0;
                                foreach (Tour t in importedTours)
                                {
                                    if (File.Exists(t.RouteInfo) && t.RouteInfo != $@"{tourPlannerConfig.PictureDirectory}{Path.GetFileName(t.RouteInfo)}")
                                    {
                                        File.Copy(t.RouteInfo, $@"{tourPlannerConfig.PictureDirectory}{Path.GetFileName(t.RouteInfo)}", true);
                                        t.RouteInfo = $@"{tourPlannerConfig.PictureDirectory}{Path.GetFileName(t.RouteInfo)}";
                                    }
                                    else
                                        logger.Warn("Map picture could not be imported to picture directory because it could not be found!");

                                    unitOfWork.TourRepository.Insert(t);
                                    t.Maneuvers.ForEach(m =>
                                    {
                                        unitOfWork.ManeuverRepository.Insert(m);
                                        predictedAffectedRowCount++;
                                    });
                                    t.TourLogs.ForEach(tl => {
                                        unitOfWork.TourLogRepository.Insert(tl);
                                        predictedAffectedRowCount++;
                                    });
                                    predictedAffectedRowCount++;
                                }

                                if (unitOfWork.Commit() != predictedAffectedRowCount)
                                {
                                    logger.Error("The commit did not affect as many rows as expected. Rollback to previous state to ensure data consistency. The import could not be conducted properly");
                                    unitOfWork.Rollback();
                                    importedTours.Clear();
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.Error("The file could not be imported because it couldn't be found!");
                        throw new FileNotFoundException("The JSON file could not be found!");
                    }
                }
                catch(Exception e)
                {

                    
                    
                }
                return importedTours;
            });
        }
    }
}
