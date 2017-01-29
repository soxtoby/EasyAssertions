using System;

namespace EasyAssertions
{
    class ErrorFactory : IErrorFactory
    {
        public static readonly ErrorFactory Instance = new ErrorFactory();

        private ErrorFactory() {}

        private Func<string, Exception> createMessageException;
        private Func<string, Exception, Exception> createInnerExceptionException;
        
        public void UseFrameworkExceptions(Func<string, Exception> messageExceptionFactory, Func<string, Exception, Exception> innerExceptionExceptionFactory)
        {
            createMessageException = messageExceptionFactory;
            createInnerExceptionException = innerExceptionExceptionFactory;
        }

        public void UseEasyAssertionExceptions()
        {
            createMessageException = null;
            createInnerExceptionException = null;
        }
        
        public Exception WithActualExpression(string message)
        {
            return Failure(FailureMessage.ActualExpression + Environment.NewLine + message.TrimStart('\r', '\n'));
        }
        
        public Exception WithActualExpression(string message, Exception innerException)
        {
            return Failure(FailureMessage.ActualExpression + Environment.NewLine + message, innerException);
        }
        
        public Exception Custom(string message)
        {
            return Failure(message);
        }
        
        public Exception Custom(string message, Exception innerException)
        {
            return Failure(message, innerException);
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
    }
}