using System;

namespace EasyAssertions
{
    /// <summary>
    /// The default exception type thrown by failed assertions.
    /// The type of exceptions thrown can be changed using <see cref="EasyAssertion.UseFrameworkExceptions">EasyAssertion.UseFrameworkExceptions</see>.
    /// </summary>
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