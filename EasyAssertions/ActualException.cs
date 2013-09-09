using System;

namespace EasyAssertions
{
    public class ActualException<T> : Actual<T> where T : Exception
    {
        public ActualException(T actual)
            : base(actual)
        {
            if (actual == null) throw new ArgumentNullException("actual");
        }

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