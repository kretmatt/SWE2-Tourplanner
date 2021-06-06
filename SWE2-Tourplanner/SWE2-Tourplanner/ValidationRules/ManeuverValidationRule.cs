using Common.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace SWE2_Tourplanner.ValidationRules
{
    /// <summary>
    /// ManeuverValidationRule is a ValidationRule for Maneuver entities inside a Datagrid.
    /// </summary>
    public class ManeuverValidationRule : ValidationRule
    {
        /// <summary>
        /// Validates whether a Maneuver has a Narrative and positive Distance or not.
        /// </summary>
        /// <param name="value">Maneuver to be validated</param>
        /// <param name="cultureInfo">CultureInfo used for the conversion</param>
        /// <returns>Returns valid result if Narrative isn't empty, null or only whitespace and if Distance isn't negative. Returns invalid result if those conditions aren't fulfilled</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Maneuver maneuver = (value as BindingGroup).Items[0] as Maneuver;
            if (string.IsNullOrWhiteSpace(maneuver.Narrative))
                return new ValidationResult(false, "Narrative can't be empty!");
            if (maneuver.Distance < 0)
                return new ValidationResult(false, "Distance can't be negative!");
            return ValidationResult.ValidResult;
        }
    }
}
