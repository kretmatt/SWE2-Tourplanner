using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using DataAccessLayer.DBCommands;
using DataAccessLayer.DBConnection;
using DataAccessLayer.Repositories;
using Common.Entities;
using DataAccessLayer.DBCommands.TourLogCommands;

namespace SWE2_Tourplanner_Tests.DALTests
{
    class TourLogRepositoryTests
    {
        private Mock<IDBConnection> mockDb;
        private List<IDBCommand> mockCommandList;
        private ITourLogRepository tourLogRepository;

        [SetUp]
        public void Setup()
        {
            mockCommandList = new List<IDBCommand>();
            mockDb = new Mock<IDBConnection>();
            mockDb.Setup(d => d.ExecuteStatement(It.IsAny<IDbCommand>())).Returns(1);
            object[] arr = new object[12];
            arr[0] = 1;
            arr[1] = 2;
            arr[2] = DateTime.Now;
            arr[3] = DateTime.Now;
            arr[4] = 1.0;
            arr[5] = 2.0;
            arr[6] = 30.0;
            arr[7] = 10.0;
            arr[8] = 15.0;
            arr[9] = 0;
            arr[10] = 0;
            arr[11] = "Test";
            mockDb.Setup(d => d.QueryDatabase(It.IsAny<IDbCommand>())).Returns(new List<object[]>() { arr });
            tourLogRepository = new TourLogRepository(mockDb.Object, mockCommandList);
        }

        [Test]
        public void InsertCorrectCallsMock()
        {
            //arrange
            TourLog tl = new TourLog()
            {
                TourId=1,
                StartDate=DateTime.Now,
                EndDate = DateTime.Now,
                Distance=1,
                TotalTime=2,
                Temperature=30,
                Rating=10,
                AverageSpeed=15,
                Weather=EWeather.SUNNY,
                TravelMethod=ETravelMethod.BIKING,
                Report="12345"
            };
            //act
            tourLogRepository.Insert(tl);
            //assert
            Assert.AreEqual(1, mockCommandList.Count);
            Assert.IsInstanceOf(typeof(InsertTourLogCommand), mockCommandList[0]);
        }
        [Test]
        public void UpdateCorrectCallsMock()
        {
            //arrange
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
                Weather = EWeather.SUNNY,
                TravelMethod = ETravelMethod.BIKING,
                Report = "12345"
            };
            //act
            tourLogRepository.Update(tl);
            //assert
            Assert.AreEqual(1, mockCommandList.Count);
            Assert.IsInstanceOf(typeof(UpdateTourLogCommand), mockCommandList[0]);
            mockDb.Verify(mdb=>mdb.QueryDatabase(It.IsAny<IDbCommand>()),Times.Once);
            mockDb.Verify(mdb => mdb.OpenConnection(), Times.Once);
            mockDb.Verify(mdb => mdb.CloseConnection(), Times.Once);
        }
        [Test]
        public void DeleteCorrectCallsMock()
        {
            //arrange
            int id = 1;
            //act
            tourLogRepository.Delete(id);
            //assert
            Assert.AreEqual(1, mockCommandList.Count);
            Assert.IsInstanceOf(typeof(DeleteTourLogCommand), mockCommandList[0]);
            mockDb.Verify(mdb => mdb.QueryDatabase(It.IsAny<IDbCommand>()), Times.Once);
            mockDb.Verify(mdb => mdb.OpenConnection(), Times.Once);
            mockDb.Verify(mdb => mdb.CloseConnection(), Times.Once);
        }
        [Test]
        public void ReadCorrectCallsMock()
        {
            //arrange
            int id = 1;
            //act
            TourLog tl = tourLogRepository.Read(id);
            //assert
            Assert.NotNull(tl);
            mockDb.Verify(mdb => mdb.QueryDatabase(It.IsAny<IDbCommand>()), Times.Once);
            mockDb.Verify(mdb => mdb.OpenConnection(), Times.Once);
            mockDb.Verify(mdb => mdb.CloseConnection(), Times.Once);
        }
        [Test]
        public void ReadAllCorrectCallsMock()
        {
            //arrange
            List<TourLog> tourLogs;
            //act
            tourLogs = tourLogRepository.ReadAll();
            //assert
            Assert.AreEqual(1, tourLogs.Count);
            mockDb.Verify(mdb => mdb.QueryDatabase(It.IsAny<IDbCommand>()), Times.Once);
            mockDb.Verify(mdb => mdb.OpenConnection(), Times.Once);
            mockDb.Verify(mdb => mdb.CloseConnection(), Times.Once);
        }

    }
}
