using System;

namespace DataAccessLayer.Exceptions
{
    [Serializable]
    public class DALDBConnectionException:Exception
    {
        public DALDBConnectionException()
        {

        }

        public DALDBConnectionException(string message) : base(message)
        {

        }

        public DALDBConnectionException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
