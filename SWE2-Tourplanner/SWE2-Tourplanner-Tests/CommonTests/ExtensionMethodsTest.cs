using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using NUnit.Framework;

namespace SWE2_Tourplanner_Tests.CommonTests
{
    class ExtensionMethodsTest
    {
        [Test]
        public void CIContainsTest()
        {
            //arrange
            string text = "CAPSLOCK";
            string searchedText = "lOcK";
            bool contained;
            //act
            contained = text.CIContains(searchedText);
            //assert
            Assert.True(contained);
        }
    }
}
