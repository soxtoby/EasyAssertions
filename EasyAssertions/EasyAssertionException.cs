using System;

namespace EasyAssertions
{
    public class EasyAssertionException : Exception
    {
        public EasyAssertionException()
        {
        }

        public EasyAssertionException(string message)
            : base(message)
        {
        }
    }
}