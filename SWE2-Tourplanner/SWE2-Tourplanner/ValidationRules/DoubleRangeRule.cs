using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SWE2_Tourplanner.ValidationRules
{
    public class DoubleRangeRule : ValidationRule
    {
        public double Min { get; set; }
        public double Max { get; set; }

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
