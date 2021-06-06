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
    /// <summary>
    /// TourPlannerReportsGenerator is a concrete implementation of ITourPlannerReportsGenerator. It generates tour- and summary reports as PDF files.
    /// </summary>
    public class TourPlannerReportsGenerator : ITourPlannerReportsGenerator
    {
        /// <summary>
        /// Object of ITourPlannerConfig which provides TourPlannerReportsGenerator with ExportsDirectory.
        /// </summary>
        ITourPlannerConfig config;
        /// <summary>
        /// ILog instance used for logging errors, warnings etc.
        /// </summary>
        ILog logger;
        /// <summary>
        /// Default constructor of TourPlannerReportsGenerator
        /// </summary>
        public TourPlannerReportsGenerator()
        {
            config = TourPlannerConfig.GetTourPlannerConfig();
            logger = LogHelper.GetLogHelper().GetLogger();
        }
        /// <summary>
        /// GenerateSummaryReport creates a summary report for the passed tours.
        /// </summary>
        /// <param name="tours">Tours used for summary report generation</param>
        /// <returns>Task, which creates and automatically opens an summary report</returns>
        /// <exception cref="BLPDFCreationException">Thrown, if there are any errors during creation, saving and opening process of summary report generation</exception>
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
        /// <summary>
        /// GenerateTourReport generates a tour report for the passed tour
        /// </summary>
        /// <param name="tour">Tour used for tour report generation</param>
        /// <returns>Task, which generates a tour report and opens it automatically</returns>
        /// <exception cref="BLPDFCreationException">Thrown, if there are problems during creation, saving and opening process of the pdf file</exception>
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
