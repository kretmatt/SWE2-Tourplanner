using System;

namespace DataAccessLayer.Exceptions
{
    /// <summary>
    /// An exception that gets thrown when errors occur whilst declaring / setting parameters of database commands
    /// </summary>
    [Serializable]
    public class DALParameterException:Exception
    {
        /// <summary>
        /// Default constructor for DALParameterException.
        /// </summary>
        public DALParameterException()
        {

        }
        /// <summary>
        /// Constructor that takes an exception message and passes it to the base constructor of Exception class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public DALParameterException(string message) : base(message)
        {

        }
        /// <summary>
        /// Constructor that takes an exception message and an inner exception and passes it to the base constructor of Exception class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Inner exception of DALParameterException</param>
        public DALParameterException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
