using System;
using System.Runtime.Serialization;

namespace JPlayer.Lib.Exception
{
    [Serializable]
    public class ApiNotFoundException : ApiException
    {
        public ApiNotFoundException(string message) : base(message)
        {
        }

        public ApiNotFoundException(string message, System.Exception e) : base(message, e)
        {
        }

        protected ApiNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}