using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Exceptions
{
    [Serializable]
    public class BLPDFCreationException:Exception
    {
        public BLPDFCreationException()
        {

        }

        public BLPDFCreationException(string message):base(message)
        {

        }

        public BLPDFCreationException(string message, Exception inner):base(message,inner)
        {

        }
    }
}
