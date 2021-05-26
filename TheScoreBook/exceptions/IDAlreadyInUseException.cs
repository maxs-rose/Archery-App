using System;
using System.Runtime.Serialization;
using TheScoreBook.Annotations;

namespace TheScoreBook.exceptions
{
    public class IDAlreadyInUseException : Exception
    {
        public IDAlreadyInUseException()
        {
        }

        protected IDAlreadyInUseException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IDAlreadyInUseException(string message) : base(message)
        {
        }

        public IDAlreadyInUseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}