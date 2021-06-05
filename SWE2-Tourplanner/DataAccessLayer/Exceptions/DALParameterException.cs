using System;

namespace DataAccessLayer.Exceptions
{
    [Serializable]
    public class DALParameterException:Exception
    {
        public DALParameterException()
        {

        }

        public DALParameterException(string message) : base(message)
        {

        }

        public DALParameterException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
