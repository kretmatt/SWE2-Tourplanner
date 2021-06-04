using System.Threading.Tasks;
using Common.Entities;

namespace BusinessLogicLayer.Factories
{
    public interface ITourLogFactory
    {
        public Task CreateTourLog(TourLog tourLog);
        public Task UpdateTourLog(TourLog tourLog);
        public Task DeleteTourLog(TourLog tourLog);
    }
}
