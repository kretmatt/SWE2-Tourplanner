using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Exceptions
{
    [Serializable]
    public class BLFactoryException:Exception
    {
        public BLFactoryException()
        {

        }

        public BLFactoryException(string message) : base(message)
        {

        }

        public BLFactoryException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
