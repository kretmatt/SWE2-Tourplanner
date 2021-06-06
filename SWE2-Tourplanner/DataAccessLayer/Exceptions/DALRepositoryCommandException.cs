using System;

namespace DataAccessLayer.Exceptions
{
    /// <summary>
    /// An exception that gets thrown when repositories can't create insert / update / delete commands
    /// </summary>
    [Serializable]
    public class DALRepositoryCommandException:Exception
    {
        /// <summary>
        /// Default constructor for DALRepositoryCommandException.
        /// </summary>
        public DALRepositoryCommandException()
        {

        }
        /// <summary>
        /// Constructor that takes an exception message and passes it to the base constructor of Exception class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public DALRepositoryCommandException(string message):base(message)
        {

        }
        /// <summary>
        /// Constructor that takes an exception message and an inner exception and passes it to the base constructor of Exception class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Inner exception of DALRepositoryCommandException</param>
        public DALRepositoryCommandException(string message, Exception inner):base(message,inner)
        {

        }
    }
}
