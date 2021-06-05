using System;

namespace DataAccessLayer.Exceptions
{
    [Serializable]
    public class DALUnitOfWorkException:Exception
    {
        public DALUnitOfWorkException()
        {

        }

        public DALUnitOfWorkException(string message):base(message)
        {

        }

        public DALUnitOfWorkException(string message, Exception inner):base(message,inner)
        {

        }
    }
}
