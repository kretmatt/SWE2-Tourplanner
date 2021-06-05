using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Factories
{
    public interface ITourFactory
    {
        public Task CreateMapQuestTour(Tour tour);
        public Task CreateTour(Tour tour);
        public Task DeleteTour(Tour tour);
        public Task UpdateMapQuestTour(Tour tour);
        public Task UpdateTour(Tour tour);
        public List<Tour> GetTours();
    }
}
