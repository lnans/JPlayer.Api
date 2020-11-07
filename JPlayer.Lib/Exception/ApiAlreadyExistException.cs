using System;
using System.Runtime.Serialization;

namespace JPlayer.Lib.Exception
{
    [Serializable]
    public class ApiAlreadyExistException : ApiException
    {
        public ApiAlreadyExistException(string message) : base(message)
        {
        }

        public ApiAlreadyExistException(string message, System.Exception e) : base(message, e)
        {
        }

        protected ApiAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}