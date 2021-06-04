using System;

namespace Common.Exceptions
{
    [Serializable]
    public class CommonConfigException:Exception
    {
        public CommonConfigException():base()
        {

        }

        public CommonConfigException(string message):base(message)
        {

        }

        public CommonConfigException(string message, Exception inner):base(message, inner)
        {

        }
    }
}
