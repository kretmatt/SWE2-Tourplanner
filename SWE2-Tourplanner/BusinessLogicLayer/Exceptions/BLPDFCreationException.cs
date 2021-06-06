using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Exceptions
{
    /// <summary>
    /// An exception that gets thrown if errors occur during the pdf creation process
    /// </summary>
    [Serializable]
    public class BLPDFCreationException:Exception
    {
        /// <summary>
        /// Default constructor of BLPDFCreationException
        /// </summary>
        public BLPDFCreationException()
        {

        }
        /// <summary>
        /// A constructor that takes a message and passes it to the base constructor
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public BLPDFCreationException(string message):base(message)
        {

        }
        /// <summary>
        /// A constructor that takes a message and exception and passes them to the base constructor
        /// </summary>
        /// <param name="message">Message of the exception</param>
        /// <param name="inner">Inner exception</param>
        public BLPDFCreationException(string message, Exception inner):base(message,inner)
        {

        }
    }
}
