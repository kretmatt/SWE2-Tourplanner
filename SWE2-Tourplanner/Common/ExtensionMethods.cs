using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Class Extensions is used for providing new extension methods to the application.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// CIContains checks if a string contains another string (case insensitive).
        /// </summary>
        /// <param name="text">Some string that may contain parameter value</param>
        /// <param name="value">String that may be part of parameter text</param>
        /// <param name="stringComparison">String comparison rules. In this case case is ignored.</param>
        /// <returns>True if value is contained in text, false if not</returns>
        public static bool CIContains(this string text, string value, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }
    }
}
