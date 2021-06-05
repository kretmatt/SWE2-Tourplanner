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
    public class NoNegativeDistancesRule : ValidationRule
    {
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
