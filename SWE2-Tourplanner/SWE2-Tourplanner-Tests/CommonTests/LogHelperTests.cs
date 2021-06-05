using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using log4net;
using NUnit.Framework;

namespace SWE2_Tourplanner_Tests.CommonTests
{
    class LogHelperTests
    {
        [Test]
        public void GetLogHelperTest()
        {
            //arrange
            LogHelper logHelper;
            //act
            logHelper = LogHelper.GetLogHelper();
            //assert
            Assert.AreEqual(LogHelper.GetLogHelper(), logHelper);
        }

        [Test]
        public void GetLoggerTest()
        {
            //arrange
            ILog logger;
            //act
            logger = LogHelper.GetLogHelper().GetLogger();
            //assert
            Assert.AreEqual(LogHelper.GetLogHelper().GetLogger(), logger);
        }
    }
}
