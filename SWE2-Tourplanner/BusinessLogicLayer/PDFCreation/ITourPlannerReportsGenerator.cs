using Common.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.PDFCreation
{
    public interface ITourPlannerReportsGenerator
    {
        Task GenerateTourReport(Tour tour);
        Task GenerateSummaryReport(List<Tour> tours);
    }
}
