using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DBCommands;
using DataAccessLayer.DBConnection;
using DataAccessLayer.Repositories;
using DataAccessLayer.Entities;
using DataAccessLayer.DBCommands.TourCommands;

namespace SWE2_Tourplanner_Tests.DALTests
{
    class TourRepositoryTests
    {

        private Mock<IDBConnection> mockDb;
        private List<IDBCommand> mockCommandList;
        private Mock<IManeuverRepository> maneuverRepository;
        private Mock<ITourLogRepository> tourLogRepository;
        private ITourRepository tourRepository;


        [SetUp]
        public void Setup()
        {
            mockCommandList = new List<IDBCommand>();
            mockDb = new Mock<IDBConnection>();
            mockDb.Setup(d => d.ExecuteStatement(It.IsAny<IDbCommand>())).Returns(1);
            object[] arr = new object[8];
            arr[0] = 1;
            arr[1] = "Mock Tour";
            arr[2] = "Vienna";
            arr[3] = "Graz";
            arr[4] = "Just a mock tour";
            arr[5] = 123;
            arr[6] = 0;
            arr[7] = "Mock description";
            mockDb.Setup(d => d.QueryDatabase(It.IsAny<IDbCommand>())).Returns(new List<object[]>() { arr });
            maneuverRepository = new Mock<IManeuverRepository>();
            Maneuver m = new Maneuver()
            {
                Id = 1,
                TourId = 1,
                Narrative = "A narrative.",
                Distance = 10.0
            };
            maneuverRepository.Setup(mr => mr.Read(It.IsAny<int>())).Returns(m);
            maneuverRepository.Setup(mr => mr.ReadAll()).Returns(new List<Maneuver>() { m });
            tourLogRepository = new Mock<ITourLogRepository>();
            TourLog tl = new TourLog()
            {
                Id=1,
                TourId = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                Distance = 1,
                TotalTime = 2,
                Temperature = 30,
                Rating = 10,
                AverageSpeed = 15,
                Weather = DataAccessLayer.Enums.EWeather.SUNNY,
                TravelMethod = DataAccessLayer.Enums.ETravelMethod.BIKING,
                Report = "Mock report"
            };
            tourLogRepository.Setup(tlr => tlr.Read(It.IsAny<int>())).Returns(tl);
            tourLogRepository.Setup(tlr => tlr.ReadAll()).Returns(new List<TourLog>() { tl });
            tourRepository = new TourRepository(mockDb.Object, mockCommandList, tourLogRepository.Object, maneuverRepository.Object);
        }

        [Test]
        public void InsertCorrectCallsMock()
        {
            //arrange
            Tour t = new Tour()
            {
                Name ="New tour",
                StartLocation="Palt",
                EndLocation="Vienna",
                RouteInfo="Mock tour",
                Distance=100,
                RouteType=DataAccessLayer.Enums.ERouteType.BICYCLE,
                Description="Mock description"
            };
            //act
            tourRepository.Insert(t);
            //assert
            Assert.AreEqual(1, mockCommandList.Count);
            Assert.IsInstanceOf(typeof(InsertTourCommand), mockCommandList[0]);
        }
        [Test]
        public void UpdateCorrectCallsMock()
        {
            //arrange
            Tour t = new Tour()
            {
                Id=1,
                Name ="New tour",
                StartLocation="Palt",
                EndLocation="Vienna",
                RouteInfo="Mock tour",
                Distance=100,
                RouteType=DataAccessLayer.Enums.ERouteType.BICYCLE,
                Description="Mock description"
            };
            //act
            tourRepository.Update(t);
            //assert
            Assert.AreEqual(1, mockCommandList.Count);
            Assert.IsInstanceOf(typeof(UpdateTourCommand), mockCommandList[0]);
            mockDb.Verify(mdb=>mdb.QueryDatabase(It.IsAny<IDbCommand>()),Times.Once);
            mockDb.Verify(mdb => mdb.OpenConnection(), Times.Once);
            mockDb.Verify(mdb => mdb.CloseConnection(), Times.Once);
            tourLogRepository.Verify(tlr => tlr.ReadAll(), Times.Once);
            maneuverRepository.Verify(mr => mr.ReadAll(), Times.Once);

        }
        [Test]
        public void DeleteCorrectCallsMock()
        {
            //arrange
            int id = 1;
            //act
            tourRepository.Delete(id);
            //assert
            Assert.AreEqual(1, mockCommandList.Count);
            Assert.IsInstanceOf(typeof(DeleteTourCommand), mockCommandList[0]);
            mockDb.Verify(mdb => mdb.QueryDatabase(It.IsAny<IDbCommand>()), Times.Once);
            mockDb.Verify(mdb => mdb.OpenConnection(), Times.Once);
            mockDb.Verify(mdb => mdb.CloseConnection(), Times.Once);
            tourLogRepository.Verify(tlr => tlr.ReadAll(), Times.Once);
            maneuverRepository.Verify(mr => mr.ReadAll(), Times.Once);
        }
        
        [Test]
        public void ReadCorrectCallsMock()
        {
            //arrange
            int id = 1;
            //act
            Tour t = tourRepository.Read(id);
            //assert
            Assert.NotNull(t);
            Assert.AreEqual(1, t.Maneuvers.Count);
            Assert.AreEqual(1, t.TourLogs.Count);
            mockDb.Verify(mdb => mdb.QueryDatabase(It.IsAny<IDbCommand>()), Times.Once);
            mockDb.Verify(mdb => mdb.OpenConnection(), Times.Once);
            mockDb.Verify(mdb => mdb.CloseConnection(), Times.Once);
            tourLogRepository.Verify(tlr => tlr.ReadAll(), Times.Once);
            maneuverRepository.Verify(mr => mr.ReadAll(), Times.Once);
        }
        
        [Test]
        public void ReadAllCorrectCallsMock()
        {
            //arrange
            List<Tour> tours;
            //act
            tours = tourRepository.ReadAll();
            //assert
            Assert.AreEqual(1, tours.Count);
            mockDb.Verify(mdb => mdb.QueryDatabase(It.IsAny<IDbCommand>()), Times.Once);
            mockDb.Verify(mdb => mdb.OpenConnection(), Times.Once);
            mockDb.Verify(mdb => mdb.CloseConnection(), Times.Once);
            tourLogRepository.Verify(tlr => tlr.ReadAll(), Times.Once);
            maneuverRepository.Verify(mr => mr.ReadAll(), Times.Once);
        }
    }
}
