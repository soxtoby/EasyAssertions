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
            if (actual == null) throw new ArgumentNullException("actual");
        }

        /// <summary>
        /// Asserts that the thrown exception is not of a particular type.
        /// </summary>
        public ActualException<T> AndShouldNotBeA<TUnexpected>(string message = null) where TUnexpected : T
        {
            And.RegisterAssert(() =>
                {
                    if (And is TUnexpected)
                        throw EasyAssertion.Failure(FailureMessageFormatter.Current.AreEqual(typeof(TUnexpected), And == null ? null : And.GetType(), message));
                });
            return this;
        }
    }
}