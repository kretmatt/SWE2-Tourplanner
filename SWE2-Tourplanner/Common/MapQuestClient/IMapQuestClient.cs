using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.MapQuestClient
{
    public interface IMapQuestClient
    {
        Task<string> RetrieveRouteImage(string storeId, string sessionId, string boundingBox);
        Task GetRouteDataFromMapQuest(Tour tour);
    }
}
