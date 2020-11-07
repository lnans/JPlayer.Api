using System;
using System.Runtime.Serialization;

namespace JPlayer.Lib.Exception
{
    [Serializable]
    public class ApiException : System.Exception
    {
        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, System.Exception e) : base(message, e)
        {
        }

        protected ApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}