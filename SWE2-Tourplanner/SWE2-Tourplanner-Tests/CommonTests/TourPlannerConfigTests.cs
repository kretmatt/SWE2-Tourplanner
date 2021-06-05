using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Config;
using NUnit.Framework;
using Common.Exceptions;

namespace SWE2_Tourplanner_Tests.CommonTests
{
    class TourPlannerConfigTests
    {
        [Test]
        public void GetTourPlannerConfigTest()
        {
            //arrange
            ITourPlannerConfig tourPlannerConfig;
            //act
            tourPlannerConfig = TourPlannerConfig.GetTourPlannerConfig();
            //assert
            Assert.AreEqual(TourPlannerConfig.GetTourPlannerConfig(), tourPlannerConfig);
            Assert.IsTrue(Directory.Exists(tourPlannerConfig.PictureDirectory));
            Assert.IsTrue(Directory.Exists(tourPlannerConfig.ExportsDirectory));
        }
        [Test]
        public void LoadConfigFromFileThrowsCommonConfigExceptionTest()
        {
            //arrange
            ITourPlannerConfig tourPlannerConfig;
            //act
            tourPlannerConfig = TourPlannerConfig.GetTourPlannerConfig();
            //assert
            // Throw CommonConfigException due to bad format
            Assert.Throws<CommonConfigException>(()=>tourPlannerConfig.LoadConfigFromFile("badformat.json"));
            // Throw CommonConfigException because file doesn't exist
            Assert.Throws<CommonConfigException>(() => tourPlannerConfig.LoadConfigFromFile("doesntexist.json"));
        }
    }
}
