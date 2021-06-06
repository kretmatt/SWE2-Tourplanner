using System;

namespace Common.Exceptions
{
    /// <summary>
    /// An exception that gets thrown when errors occur in TourPlannerConfig
    /// </summary>
    [Serializable]
    public class CommonConfigException:Exception
    {
        /// <summary>
        /// Default constructor for CommonConfigException.
        /// </summary>
        public CommonConfigException():base()
        {

        }

        /// <summary>
        /// Constructor that takes an exception message and passes it to the base constructor of Exception class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public CommonConfigException(string message):base(message)
        {

        }
        /// <summary>
        /// Constructor that takes an exception message and an inner exception and passes it to the base constructor of Exception class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Inner exception of CommonConfigException</param>
        public CommonConfigException(string message, Exception inner):base(message, inner)
        {

        }
    }
}
