using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using Moq;
using NUnit.Framework;
using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Factories;
using BusinessLogicLayer.Exceptions;

namespace SWE2_Tourplanner_Tests.BLTests
{
    class TourFactoryTests
    {
        private Mock<IUnitOfWork> mockUOW;
        private Mock<ITourLogRepository> mockTourLogRepository;
        private Mock<ITourRepository> mockTourRepository;
        private Mock<IManeuverRepository> mockManeuverRepository;
        private Tour testTour;

        [SetUp]
        public void SetUp()
        {
            mockTourLogRepository = new Mock<ITourLogRepository>();
            mockTourRepository = new Mock<ITourRepository>();
            mockTourRepository.Setup(tr => tr.Read(It.IsAny<int>())).Returns(new Tour());
            mockManeuverRepository = new Mock<IManeuverRepository>();
            mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(uow => uow.Commit()).Returns(2);
            mockUOW.SetupGet(uow => uow.ManeuverRepository).Returns(mockManeuverRepository.Object);
            mockUOW.SetupGet(uow => uow.TourRepository).Returns(mockTourRepository.Object);
            mockUOW.SetupGet(uow => uow.TourLogRepository).Returns(mockTourLogRepository.Object);
            testTour = new Tour()
            {
                Id = 1,
                Description = "test",
                Distance = 123.4,
                StartLocation = "Wien",
                EndLocation = "Krems an der Donau",
                RouteInfo = "testpic.png",
                Maneuvers = new List<Maneuver>()
                {
                    new Maneuver()
                    {
                        Id=1,
                        TourId=1,
                        Narrative="test",
                        Distance=123.4
                    }
                },
                Name="Testtour",
                RouteType = Common.Enums.ERouteType.BICYCLE
            };
        }

        [Test]
        public async Task CreateTourTest()
        {
            //arrange
            ITourFactory tourFactory = new TourFactory(mockUOW.Object);
            //act
            await tourFactory.CreateTour(testTour);
            //assert
            mockManeuverRepository.Verify(mr => mr.Insert(testTour.Maneuvers[0]), Times.Once);
            mockTourRepository.Verify(tr => tr.Insert(testTour));
            mockUOW.Verify(uow => uow.Commit());
        }

        [Test]
        public async Task UpdateTourTest()
        {
            //arrange
            ITourFactory tourFactory = new TourFactory(mockUOW.Object);
            //act
            await tourFactory.UpdateTour(testTour);
            //assert
            mockTourRepository.Verify(tr => tr.Read(testTour.Id));
            mockManeuverRepository.Verify(mr => mr.Insert(testTour.Maneuvers[0]), Times.Once);
            mockTourRepository.Verify(tr => tr.Update(testTour));
            mockUOW.Verify(uow => uow.Commit());
        }

        [Test]
        public async Task DeleteTourTest()
        {
            //arrange
            ITourFactory tourFactory = new TourFactory(mockUOW.Object);
            //act
            await tourFactory.DeleteTour(testTour);
            //assert
            mockTourRepository.Verify(tr => tr.Delete(testTour.Id));
            mockUOW.Verify(uow => uow.Commit());
        }
    }
}
