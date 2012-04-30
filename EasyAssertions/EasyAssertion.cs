using System.Collections.Generic;

namespace EasyAssertions
{
    public static class EasyAssertion
    {
        public static Actual<TActual> ShouldEqual<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TActual : TExpected
        {
            if (!Compare.ObjectsAreEqual(actual, expected))
            {
                string actualString = actual as string;
                string expectedString = expected as string;
                if (actualString != null && expectedString != null)
                    throw new EasyAssertionException(FailureMessageFormatter.Current.NotEqual(expectedString, actualString, message));
                throw new EasyAssertionException(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
            }

            return new Actual<TActual>(actual);
        }

        public static Actual<TActual> ShouldBe<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TActual : TExpected
        {
            if (!ReferenceEquals(actual, expected))
                throw new EasyAssertionException(FailureMessageFormatter.Current.NotSame(expected, actual, message));

            return new Actual<TActual>(actual);
        }

        public static Actual<TActual> ShouldMatch<TActual, TExpected>(this TActual actual, IEnumerable<TExpected> expected, string message = null) where TActual : IEnumerable<TExpected>
        {
            if (!Compare.CollectionsMatch(actual, expected))
                throw new EasyAssertionException(FailureMessageFormatter.Current.DoNotMatch(expected, actual, message));

            return new Actual<TActual>(actual);
        }

        public static Actual<TExpected> ShouldBeA<TExpected>(this object actual, string message = null)
        {
            if (!(actual is TExpected))
                throw new EasyAssertionException(FailureMessageFormatter.Current.NotEqual(typeof(TExpected), actual == null ? null : actual.GetType(), message));

            return new Actual<TExpected>((TExpected)actual);
        }

        public static Actual<string> ShouldContain(this string actual, string expectedToContain, string message = null)
        {
            if (!actual.Contains(expectedToContain))
                throw new EasyAssertionException(FailureMessageFormatter.Current.DoesNotContain(expectedToContain, actual, message));
            return new Actual<string>(actual);
        }
    }
}