using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Factories
{
    public interface ITourPlannerFactory
    {
        public Task<Tour> CreateMapQuestTour(Tour tour);
        public Task<Tour> CreateTour(Tour tour);
    }
}
