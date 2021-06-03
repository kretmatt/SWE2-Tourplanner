using System;

namespace DataAccessLayer.Exceptions
{
    [Serializable]
    public class DALRepositoryException:Exception
    {
        public DALRepositoryException()
        {

        }

        public DALRepositoryException(string message):base(message)
        {

        }

        public DALRepositoryException(string message, Exception inner):base(message,inner)
        {

        }
    }
}
