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
    public class ManeuverValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Maneuver maneuver = (value as BindingGroup).Items[0] as Maneuver;
            if (string.IsNullOrEmpty(maneuver.Narrative))
                return new ValidationResult(false, "Narrative can't be empty!");
            if (maneuver.Distance < 0)
                return new ValidationResult(false, "Distance can't be negative!");
            return ValidationResult.ValidResult;
        }
    }
}
