using System;

namespace DataAccessLayer.Exceptions
{
    [Serializable]
    public class DALRepositoryCommandException:Exception
    {
        public DALRepositoryCommandException()
        {

        }

        public DALRepositoryCommandException(string message):base(message)
        {

        }

        public DALRepositoryCommandException(string message, Exception inner):base(message,inner)
        {

        }
    }
}
