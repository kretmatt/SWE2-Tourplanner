 using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SWE2_Tourplanner.ValidationRules
{
    public class TextInputValidationRule : ValidationRule
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string inputText = (string)value;

            if (inputText.Length < Min || inputText.Length > Max)
                return new ValidationResult(false, $"Please enter a text with length in the following range: {Min}-{Max}");
            return ValidationResult.ValidResult;
        }
    }
}
