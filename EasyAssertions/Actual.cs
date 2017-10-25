namespace EasyAssertions
{
    /// <summary>
    /// A wrapper around the value returned by an assertion, used for chaining further assertions.
    /// </summary>
    public class Actual<T>
    {
        internal readonly T Value;

        /// <inheritdoc />
        public Actual(T actual)
        {
            Value = actual;
        }

        /// <summary>
        /// Provides access to the actual value, to allow assertions to be chained.
        /// </summary>
        public T And
        {
            get
            {
                SourceExpressionProvider.ForCurrentThread.RegisterContinuation(0);
                return Value;
            }
        }
    }
}