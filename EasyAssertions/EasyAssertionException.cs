using System;

namespace EasyAssertions
{
    public class EasyAssertionException : Exception
    {
        public override string ToString()
        {
            return Message + Environment.NewLine + Environment.NewLine + StackTrace;
        }

        public EasyAssertionException(string message)
            : base(message)
        {
        }

        public EasyAssertionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}