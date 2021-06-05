using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SWE2_Tourplanner.ValidationRules;
using System.Globalization;
using Common.Entities;

namespace SWE2_Tourplanner_Tests.UITests
{
    class ValidationRulesTests
    {
        [Test]
        public void DoubleRangeRuleTest()
        {
            //arrange
            DoubleRangeRule doubleRangeRule = new DoubleRangeRule();
            doubleRangeRule.Max = 100.5;
            doubleRangeRule.Min = -22.3;

            bool underMinResult, overMaxResult, inRangeResult;
            //act
            underMinResult = doubleRangeRule.Validate("-1000.2", CultureInfo.InvariantCulture).IsValid;
            overMaxResult = doubleRangeRule.Validate("1000.89", CultureInfo.InvariantCulture).IsValid;
            inRangeResult = doubleRangeRule.Validate("50.34", CultureInfo.InvariantCulture).IsValid;
            //assert

            Assert.IsFalse(underMinResult);
            Assert.IsFalse(overMaxResult);
            Assert.IsTrue(inRangeResult);
        }

        [Test]
        public void NoNegativeDistancesRuleTest()
        {
            //arrange
            NoNegativeDistancesRule noNegativeDistancesRule = new NoNegativeDistancesRule();
            string negativeDistance = "-1";
            string positiveDistance = "23.123";
            //act & assert
            Assert.IsTrue(noNegativeDistancesRule.Validate(positiveDistance, CultureInfo.InvariantCulture).IsValid);
            Assert.IsFalse(noNegativeDistancesRule.Validate(negativeDistance, CultureInfo.InvariantCulture).IsValid);
        }

        [Test]
        public void TextInputValidationRuleTest()
        {
            //arrange
            TextInputValidationRule textInputValidationRule = new TextInputValidationRule();
            textInputValidationRule.Min = 5;
            textInputValidationRule.Max = 10;
            //act & assert
            Assert.IsFalse(textInputValidationRule.Validate("123", CultureInfo.InvariantCulture).IsValid);
            Assert.IsTrue(textInputValidationRule.Validate("1234567", CultureInfo.InvariantCulture).IsValid);
            Assert.IsFalse(textInputValidationRule.Validate("1234567890123", CultureInfo.InvariantCulture).IsValid);
        }
    }
}
