namespace EasyAssertions
{
    public class FailureMessageExpected : Expected
    {
        private readonly FailureMessage failureMessage;

        public FailureMessageExpected(FailureMessage failureMessage)
        {
            this.failureMessage = failureMessage;
        }

        /// <summary>
        /// The source representation of the expected value, as provided by the parent <see cref="FailureMessage"/>.
        /// </summary>
        public string Expression { get { return failureMessage.ExpectedExpression; } }

        /// <summary>
        /// The expected value, as provided by the parent <see cref="FailureMessage"/>.
        /// </summary>
        public object Value { get { return failureMessage.ExpectedValue; } }
    }
}