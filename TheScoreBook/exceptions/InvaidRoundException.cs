using System;

namespace TheScoreBook.exceptions
{
    public class InvalidRoundException : Exception
    {
        public InvalidRoundException(string message) : base(message) { }
        public InvalidRoundException(string message, Exception inner) : base(message, inner) { }
    }
}