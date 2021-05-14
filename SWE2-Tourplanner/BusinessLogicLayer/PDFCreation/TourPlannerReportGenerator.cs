using Common.Config;
using Common.Entities;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.PDFCreation
{
    public class TourPlannerReportsGenerator : ITourPlannerReportsGenerator
    {

        ITourPlannerConfig config;
        public TourPlannerReportsGenerator()
        {
            config = TourPlannerConfig.GetTourPlannerConfig();
        }
        public async Task GenerateSummaryReport(List<Tour> tours)
        {
            await Task.Run(()=>
            {
                var summaryReport = new SummaryReport(tours);
                string pdfLocation = $@"{config.ExportsDirectory}Summary{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf";
                summaryReport.GeneratePdf(pdfLocation);
                Process.Start("explorer.exe", pdfLocation);
            });
        }

        public async Task GenerateTourReport(Tour tour)
        {
            await Task.Run(() =>
            {
                var summaryReport = new TourReport(tour);
                string pdfLocation = $@"{config.ExportsDirectory}{tour.Name}Report.pdf";
                summaryReport.GeneratePdf(pdfLocation);
                Process.Start("explorer.exe", pdfLocation);
            });
        }
    }
}
