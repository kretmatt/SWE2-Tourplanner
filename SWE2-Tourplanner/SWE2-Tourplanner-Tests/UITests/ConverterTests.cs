using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWE2_Tourplanner.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Data;

namespace SWE2_Tourplanner_Tests.UITests
{
    class ConverterTests
    {
        [Test]
        public void StringToDateTimeConverterTest()
        {
            //arrange
            string dateTimeString = DateTime.Now.ToString();
            DateTime convertedDateTime;
            string convertedString;
            StringToDateTimeConverter stringToDateTimeConverter = new StringToDateTimeConverter();
            //act
            convertedDateTime = (DateTime)stringToDateTimeConverter.ConvertBack(dateTimeString, null,null,System.Globalization.CultureInfo.CurrentCulture);
            convertedString = (string)stringToDateTimeConverter.Convert(convertedDateTime, null,null,System.Globalization.CultureInfo.CurrentCulture);
            //assert
            Assert.AreEqual(dateTimeString, convertedString);
        }

        [Test]
        public void RouteInfoImageConverter()
        {
            //arrange
            BitmapImage bmi = new BitmapImage();
            RouteInfoImageConverter routeInfoImageConverter = new RouteInfoImageConverter();
            //act
            bmi = (BitmapImage)routeInfoImageConverter.Convert("testpic.png", null, null, System.Globalization.CultureInfo.CurrentCulture);
            //assert
            Assert.AreNotEqual(new BitmapImage(), bmi);
            Assert.AreEqual(Binding.DoNothing, routeInfoImageConverter.ConvertBack(bmi, null, null, System.Globalization.CultureInfo.CurrentCulture));
        }
    }
}
