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
    /// TextInputValidationRule is a ValidationRule for the length of text input
    /// </summary>
    public class TextInputValidationRule : ValidationRule
    {
        /// <value>
        /// Minimum character count of text
        /// </value>
        public int Min { get; set; }
        /// <value>
        /// Maximum character count of text
        /// </value>
        public int Max { get; set; }
        /// <summary>
        /// Validates whether a string surpasses Min character count and is lower than Max character count.
        /// </summary>
        /// <param name="value">Text to be validated</param>
        /// <param name="cultureInfo">CultureInfo used for conversion</param>
        /// <returns>Valid result if text doesn't violate bounds, otherwise invalid result is returned</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string inputText = (string)value;

            if (inputText.Length < Min || inputText.Length > Max)
                return new ValidationResult(false, $"Please enter a text with length in the following range: {Min}-{Max}");
            return ValidationResult.ValidResult;
        }
    }
}
