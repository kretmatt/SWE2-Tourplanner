using System;

namespace BusinessLogicLayer.Exceptions
{
    [Serializable]
    public class BLExporterImporterException:Exception
    {
        public BLExporterImporterException()
        {

        }

        public BLExporterImporterException(string message) : base(message)
        {

        }

        public BLExporterImporterException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
