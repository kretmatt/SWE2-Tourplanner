using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DBConnection;
using DataAccessLayer.DBCommands;
using DataAccessLayer.Repositories;
using Common.Entities;
using DataAccessLayer.DBCommands.ManeuverCommands;
using DataAccessLayer.UnitOfWork;

namespace SWE2_Tourplanner_Tests.DALTests
{
    class ManeuverRepositoryTests
    {
        private Mock<IDBConnection> mockDb;
        private List<IDBCommand> mockCommandList;
        private IManeuverRepository maneuverRepository;

        [SetUp]
        public void Setup()
        {
            mockCommandList = new List<IDBCommand>();
            mockDb = new Mock<IDBConnection>();
            mockDb.Setup(d => d.ExecuteStatement(It.IsAny<IDbCommand>())).Returns(1);
            object[] arr = new object[4];
            arr[0] = 1;
            arr[1] = 2;
            arr[2] = "MOCK";
            arr[3] = 15.0;
            mockDb.Setup(d => d.QueryDatabase(It.IsAny<IDbCommand>())).Returns(new List<object[]>() { arr });
            maneuverRepository = new ManeuverRepository(mockDb.Object, mockCommandList);
        }
        [Test]
        public void InsertCorrectCallsMock()
        {
            //arrange
            Maneuver m = new Maneuver()
            {
                TourId=3,
                Narrative="A narrative.",
                Distance=10.0
            };
            //act
            maneuverRepository.Insert(m);
            //assert
            Assert.AreEqual(1, mockCommandList.Count);
            Assert.IsInstanceOf(typeof(InsertManeuverCommand), mockCommandList[0]);
        }
        [Test]
        public void UpdateCorrectCallsMock()
        {
            //arrange
            Maneuver m = new Maneuver()
            {
                Id = 1,
                TourId = 3,
                Narrative = "A narrative.",
                Distance = 10.0
            };
            //act
            maneuverRepository.Update(m);
            //assert
            Assert.AreEqual(1, mockCommandList.Count);
            Assert.IsInstanceOf(typeof(UpdateManeuverCommand), mockCommandList[0]);
            mockDb.Verify(mdb => mdb.QueryDatabase(It.IsAny<IDbCommand>()), Times.Once);
            mockDb.Verify(mdb => mdb.OpenConnection(), Times.Once);
            mockDb.Verify(mdb => mdb.CloseConnection(), Times.Once);
        }
        
        [Test]
        public void DeleteCorrectCallsMock()
        { 
            //arrange
            int id = 1;
            //act
            maneuverRepository.Delete(id);
            //assert
            Assert.AreEqual(1, mockCommandList.Count);
            Assert.IsInstanceOf(typeof(DeleteManeuverCommand), mockCommandList[0]);
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
            Maneuver m = maneuverRepository.Read(id);
            //assert
            Assert.NotNull(m);
            mockDb.Verify(mdb => mdb.QueryDatabase(It.IsAny<IDbCommand>()), Times.Once);
            mockDb.Verify(mdb => mdb.OpenConnection(), Times.Once);
            mockDb.Verify(mdb => mdb.CloseConnection(), Times.Once);
        }
        
        [Test]
        public void ReadAllCorrectCallsMock()
        {
            //arrange
            List<Maneuver> tourLogs;
            //act
            tourLogs = maneuverRepository.ReadAll();
            //assert
            Assert.AreEqual(1, tourLogs.Count);
            mockDb.Verify(mdb => mdb.QueryDatabase(It.IsAny<IDbCommand>()), Times.Once);
            mockDb.Verify(mdb => mdb.OpenConnection(), Times.Once);
            mockDb.Verify(mdb => mdb.CloseConnection(), Times.Once);
        }
    }
}
