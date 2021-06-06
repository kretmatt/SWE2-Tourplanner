using SWE2_Tourplanner.Dialogs;
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
    /// NoNegativeDistancesRule is a ValidationRule for validating that double values used for distance-like properties aren't negative
    /// </summary>
    public class NoNegativeDistancesRule : ValidationRule
    {
        /// <summary>
        /// Validates whether a distance (double) is negative or not.
        /// </summary>
        /// <param name="value">Distance to be validated</param>
        /// <param name="cultureInfo">CultureInfo used for conversion</param>
        /// <returns>Valid result if value is greater than or equal 0, invalid result if value is negative</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double inputValue;

            if (Double.TryParse((string)value, out inputValue))
            {
                if (inputValue < 0)
                {
                    return new ValidationResult(false, "Please enter a valid distance!");
                }
                return ValidationResult.ValidResult;
            }
            else
                return new ValidationResult(false, "The input is not a decimal value!");
        }
    }
}
