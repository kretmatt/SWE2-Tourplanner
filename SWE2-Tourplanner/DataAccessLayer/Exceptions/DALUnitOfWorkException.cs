using System;

namespace DataAccessLayer.Exceptions
{
    /// <summary>
    /// An exception that gets thrown when something goes wrong during commits or rollbacks of IUnitOfWork classes
    /// </summary>
    [Serializable]
    public class DALUnitOfWorkException:Exception
    {
        /// <summary>
        /// Default constructor for DALUnitOfWorkException. Calls base constructor of Exception class.
        /// </summary>
        public DALUnitOfWorkException()
        {

        }
        /// <summary>
        /// Constructor that takes an exception message and passes it to the base constructor of Exception class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public DALUnitOfWorkException(string message):base(message)
        {

        }
        /// <summary>
        /// Constructor that takes an exception message and an inner exception and passes it to the base constructor of Exception class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Inner exception of DALUnitOfWorkException</param>
        public DALUnitOfWorkException(string message, Exception inner):base(message,inner)
        {

        }
    }
}
