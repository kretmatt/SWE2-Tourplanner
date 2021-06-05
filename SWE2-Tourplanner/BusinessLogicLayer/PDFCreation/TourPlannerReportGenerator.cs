using Common.Config;
using Common.Entities;
using log4net;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Common.Logging;
using BusinessLogicLayer.Exceptions;

namespace BusinessLogicLayer.PDFCreation
{
    public class TourPlannerReportsGenerator : ITourPlannerReportsGenerator
    {
        ITourPlannerConfig config;
        ILog logger;
        public TourPlannerReportsGenerator()
        {
            config = TourPlannerConfig.GetTourPlannerConfig();
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        public Task GenerateSummaryReport(List<Tour> tours)
        {
            return Task.Run(()=>
            {
                try
                {
                    logger.Info($"Summary report of {tours.Count} tours is being generated!");
                    var summaryReport = new SummaryReport(tours);
                    string pdfLocation = $@"{config.ExportsDirectory}Summary{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf";
                    summaryReport.GeneratePdf(pdfLocation);
                    Process.Start("explorer.exe", pdfLocation);
                    logger.Info("Summary Report was successfully generated and opened!");
                }
                catch(Exception e)
                {
                    if (e is InvalidOperationException || e is Win32Exception || e is FileNotFoundException || e is ObjectDisposedException)
                    {
                        logger.Error("Couldn't open the generated file! Check in the exports directory!");
                        throw new BLPDFCreationException($"Could not open generated file. Check in the exports directory {config.ExportsDirectory}");
                    }
                    logger.Error($"Summary report PDF creation could not be conducted properly. Further information: {e.Message}");
                    throw new BLPDFCreationException("Summary report PDF creation could not be conducted properly! Try again later with other data or contact the developer!");
                }
            });
        }

        public Task GenerateTourReport(Tour tour)
        {
            return Task.Run(() =>
            {
                try
                {
                    logger.Info($"{tour.Name} report is being generated!");
                    var summaryReport = new TourReport(tour);
                    string pdfLocation = $@"{config.ExportsDirectory}{string.Join("",tour.Name.Split(Path.GetInvalidFileNameChars()))}Report.pdf";
                    summaryReport.GeneratePdf(pdfLocation);
                    Process.Start("explorer.exe", pdfLocation);
                    logger.Info("Tour Report was successfully generated and opened!");
                }
                catch (Exception e)
                {
                    if(e is InvalidOperationException || e is Win32Exception || e is FileNotFoundException || e is ObjectDisposedException)
                    {
                        logger.Error("Couldn't open the generated file! Check in the exports directory!");
                        throw new BLPDFCreationException($"Could not open generated file. Check in the exports directory {config.ExportsDirectory}");
                    }
                    logger.Error($"Tour report PDF creation could not be conducted properly. Further information: {e.Message}");
                    throw new BLPDFCreationException("Tour report PDF creation could not be conducted properly! Try again later with other data or contact the developer!");
                }
            });
        }
    }
}
