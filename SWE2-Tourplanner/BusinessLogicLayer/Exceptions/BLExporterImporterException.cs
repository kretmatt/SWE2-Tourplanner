using System;

namespace BusinessLogicLayer.Exceptions
{
    /// <summary>
    /// An exception that gets thrown if errors occur during the import / export process of tour data
    /// </summary>
    [Serializable]
    public class BLExporterImporterException:Exception
    {
        /// <summary>
        /// Default constructor of BLExporterImporterException
        /// </summary>
        public BLExporterImporterException()
        {

        }
        /// <summary>
        /// A constructor that takes a message as a string parameter and passes it to the base constructor
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public BLExporterImporterException(string message) : base(message)
        {

        }
        /// <summary>
        /// A constructor that takes a message and exception and passes it to the base constructor
        /// </summary>
        /// <param name="message">Message of the exception</param>
        /// <param name="inner">Inner exception</param>
        public BLExporterImporterException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
