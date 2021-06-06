using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SWE2_Tourplanner.ValidationRules
{
    /// <summary>
    /// DoubleRangeRule is a ValidationRule for validating whether a string converted to a double value is within a Min and Max Value
    /// </summary>
    public class DoubleRangeRule : ValidationRule
    {
        /// <value>
        /// Lower Bound of the validation rule
        /// </value>
        public double Min { get; set; }
        /// <value>
        /// Upper Bound of the validation rule
        /// </value>
        public double Max { get; set; }
        /// <summary>
        /// Validates if the input value is between Min and Max
        /// </summary>
        /// <param name="value">Input value</param>
        /// <param name="cultureInfo">CultureInfo used for convertig the value</param>
        /// <returns>ValidationResult. Returns valid result if value is within bounds, invalid result if value is out of bounds</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double inputValue;

            if (Double.TryParse((string)value, NumberStyles.Number, CultureInfo.InvariantCulture, out inputValue))
            {
                if ((inputValue < Min) || (inputValue > Max))
                    return new ValidationResult(false, $"Please enter a value within the range: {Min}-{Max}");
                return ValidationResult.ValidResult;
            }
            else
                return new ValidationResult(false, "The input is not a decimal value!");
        }
    }
}
