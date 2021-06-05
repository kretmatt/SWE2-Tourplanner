using NUnit.Framework;
using DataAccessLayer.UnitOfWork;
using DataAccessLayer.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Factories;
using Common.Entities;

namespace SWE2_Tourplanner_Tests.BLTests
{
    class TourLogFactoryTest
    {
        private Mock<IUnitOfWork> mockUOW;
        private Mock<ITourLogRepository> mockTourLogRepository;
        private Mock<ITourRepository> mockTourRepository;
        private Mock<ManeuverRepository> mockManeuverRepository;

        [SetUp]
        public void SetUp()
        {
            mockTourLogRepository = new Mock<ITourLogRepository>();
            mockTourRepository = new Mock<ITourRepository>();
            mockManeuverRepository = new Mock<ManeuverRepository>();
            mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(uow => uow.Commit()).Returns(1);
            mockUOW.SetupGet(uow => uow.ManeuverRepository).Returns(mockManeuverRepository.Object);
            mockUOW.SetupGet(uow => uow.TourRepository).Returns(mockTourRepository.Object);
            mockUOW.SetupGet(uow => uow.TourLogRepository).Returns(mockTourLogRepository.Object);
        }

        [Test]
        public async Task CreateTourLogTest()
        {
            //arrange
            ITourLogFactory tourLogFactory = new TourLogFactory(mockUOW.Object);
            //act
            await tourLogFactory.CreateTourLog(new TourLog());
            //assert
            mockUOW.Verify(uow => uow.Commit(),Times.Once);
            mockTourLogRepository.Verify(tlr => tlr.Insert(It.IsAny<TourLog>()), Times.Once);
        }

        [Test]
        public async Task UpdateTourLogTest()
        {
            //arrange
            ITourLogFactory tourLogFactory = new TourLogFactory(mockUOW.Object);
            //act
            await tourLogFactory.UpdateTourLog(new TourLog());
            //assert
            mockUOW.Verify(uow => uow.Commit(), Times.Once);
            mockTourLogRepository.Verify(tlr => tlr.Update(It.IsAny<TourLog>()), Times.Once);
        }

        [Test]
        public async Task DeleteTourLogTest()
        {
            //arrange
            ITourLogFactory tourLogFactory = new TourLogFactory(mockUOW.Object);
            //act
            await tourLogFactory.DeleteTourLog(new TourLog());
            //assert
            mockUOW.Verify(uow => uow.Commit(), Times.Once);
            mockTourLogRepository.Verify(tlr => tlr.Delete(It.IsAny<int>()), Times.Once);
        }
    }
}
