using System;

namespace EasyAssertions
{
    class ErrorFactory : IErrorFactory
    {
        public static readonly ErrorFactory Instance = new ErrorFactory();

        private ErrorFactory() {}

        private Func<string, Exception> createMessageException;
        private Func<string, Exception, Exception> createInnerExceptionException;

        /// <summary>
        /// Overrides the exceptions used when assertions fail.
        /// Test frameworks will detect their own exception types and display the correct assertion failure messages.
        /// </summary>
        public void UseFrameworkExceptions(Func<string, Exception> messageExceptionFactory, Func<string, Exception, Exception> innerExceptionExceptionFactory)
        {
            createMessageException = messageExceptionFactory;
            createInnerExceptionException = innerExceptionExceptionFactory;
        }

        /// <summary>
        /// Throw <see cref="EasyAssertionException"/>s when assertions fail.
        /// </summary>
        public void UseEasyAssertionExceptions()
        {
            createMessageException = null;
            createInnerExceptionException = null;
        }

        private Exception Failure(string failureMessage)
        {
            return createMessageException != null
                ? createMessageException(failureMessage)
                : new EasyAssertionException(failureMessage);
        }

        private Exception Failure(string failureMessage, Exception innerException)
        {
            return createInnerExceptionException != null
                ? createInnerExceptionException(failureMessage, innerException)
                : new EasyAssertionException(failureMessage, innerException);
        }

        /// <summary>
        /// Creates an <see cref="Exception"/> to be thrown for a failed assertion that includes the source representation of the actual value.
        /// </summary>
        public Exception WithActualExpression(string message)
        {
            return Failure(FailureMessage.ActualExpression + Environment.NewLine + message);
        }

        /// <summary>
        /// Creates an <see cref="Exception"/> to be thrown for a failed assertion that includes the source representation of the actual value.
        /// </summary>
        public Exception WithActualExpression(string message, Exception innerException)
        {
            return Failure(FailureMessage.ActualExpression + Environment.NewLine + message, innerException);
        }

        /// <summary>
        /// Creates an <see cref="Exception"/> to be thrown for a failed assertion.
        /// </summary>
        public Exception Custom(string message)
        {
            return Failure(message);
        }

        /// <summary>
        /// Creates an <see cref="Exception"/> to be thrown for a failed assertion.
        /// </summary>
        public Exception Custom(string message, Exception innerException)
        {
            return Failure(message, innerException);
        }
    }
}