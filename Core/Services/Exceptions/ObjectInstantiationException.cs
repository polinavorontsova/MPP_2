using System;

namespace Core.Services.Exceptions
{
    public class ObjectInstantiationException : Exception
    {
        public ObjectInstantiationException(string message) : base(message)
        {
        }

        public ObjectInstantiationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}