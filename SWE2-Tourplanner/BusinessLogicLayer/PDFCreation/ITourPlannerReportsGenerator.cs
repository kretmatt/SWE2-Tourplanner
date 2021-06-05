using Common.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.PDFCreation
{
    /// <summary>
    /// An interface that defines methods for generating different reports
    /// </summary>
    public interface ITourPlannerReportsGenerator
    {
        Task GenerateTourReport(Tour tour);
        Task GenerateSummaryReport(List<Tour> tours);
    }
}
