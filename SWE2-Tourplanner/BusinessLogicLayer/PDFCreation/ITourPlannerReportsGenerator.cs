using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.PDFCreation
{
    public interface ITourPlannerReportsGenerator
    {
        Task GenerateTourReport(Tour tour);
        Task GenerateSummaryReport(List<Tour> tours);
    }
}
