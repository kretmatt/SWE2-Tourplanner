using BusinessLogicLayer.Exceptions;
using Common.Config;
using Common.Entities;
using Common.Logging;
using DataAccessLayer.UnitOfWork;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using DataAccessLayer.Exceptions;
using System.IO;
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
                {
                    logger.Warn("Map picture could not be exported because it could not be found!");
                    throw new FileNotFoundException("Map picture could not be found! Stopping export!");
                }
            }
        }

        public Task Export(List<Tour> tours)
        {
            return Task.Run(() =>
            {
                try
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
                }
                catch(Exception e)
                {
                    if (e is UnauthorizedAccessException)
                        throw new BLExporterImporterException("Due to lacking access permissions the export could not be conducted!");
                    if (e is DirectoryNotFoundException)
                        throw new BLExporterImporterException("A directory vital to the export could not be found!");
                    if (e is ArgumentException || e is ArgumentNullException)
                        throw new BLExporterImporterException("Export could not be conducted due to bad arguments!");
                    if (e is FileNotFoundException)
                        throw new BLExporterImporterException("A file (e.g.: image) vital to the export could not be found!");
                    if (e is PathTooLongException)
                        throw new BLExporterImporterException("Somehow a path got too long");
                    if (e is NotSupportedException)
                        throw new BLExporterImporterException("An operation was not supported and therefore the export had to be stopped!");
                    throw new BLExporterImporterException("Some other unhandled error occured during the export! Please don't despair and keep calm!");
                }
            });   
        }

        public Task<List<Tour>> Import(string filePath)
        {
            return Task.Run(() =>
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
                    if(e is JsonSerializationException)
                    {
                        logger.Error($"{filePath} had a bad format and could therfore not be imported!");
                        throw new BLExporterImporterException($"The file {filePath} has an invalid format and could therefore not be imported!");
                    }
                    if(e is DALDBConnectionException || e is DALParameterException || e is DALRepositoryCommandException || e is DALUnitOfWorkException)
                    {
                        logger.Error($"DataAccessLayer had problems saving the data. Exception type: {e.GetType()}");
                        throw new BLExporterImporterException($"The import could not be conducted properly due to an error during the saving process: {e.Message}");
                    }
                    logger.Error($"Some other unhandled error occured: {e.GetType()} - {e.Message}");
                    throw new BLExporterImporterException($"An error occured during the import of file {filePath}. Try again with another file or check if there are some problems with the file.");
                }
                return importedTours;
            });
        }
    }
}
