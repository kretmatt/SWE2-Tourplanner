using System;

namespace DataAccessLayer.Exceptions
{
    /// <summary>
    /// An exception that gets thrown when errors regarding database connection / queries / execution of statements occur
    /// </summary>
    [Serializable]
    public class DALDBConnectionException:Exception
    {
        /// <summary>
        /// Default constructor for DALDBConnectionException.
        /// </summary>
        public DALDBConnectionException()
        {

        }
        /// <summary>
        /// Constructor that takes an exception message and passes it to the base constructor of Exception class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public DALDBConnectionException(string message) : base(message)
        {

        }
        /// <summary>
        /// Constructor that takes an exception message and an inner exception and passes it to the base constructor of Exception class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Inner exception of DALDBConnectionException</param>
        public DALDBConnectionException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
