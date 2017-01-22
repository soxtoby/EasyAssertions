using System;

namespace EasyAssertions
{
    /// <summary>
    /// A wrapper around the exception returned by an exception assertion.
    /// </summary>
    public class ActualException<T> : Actual<T> where T : Exception
    {
        public ActualException(T actual)
            : base(actual)
        {
            if (actual == null) throw new ArgumentNullException(nameof(actual));
        }

        /// <summary>
        /// Asserts that the thrown exception is not of a particular type.
        /// </summary>
        public ActualException<T> AndShouldNotBeA<TUnexpected>(string message = null) where TUnexpected : T
        {
            And.RegisterAssert(() =>
                {
                    if (And is TUnexpected)
                        throw EasyAssertion.Failure(FailureMessage.Standard.AreEqual(typeof(TUnexpected), And?.GetType(), message));
                });
            return this;
        }
    }
}