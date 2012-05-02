using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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

    public static class Function
    {
        public static AssertionFunction Call(Expression<Action> functionCall)
        {
            return new AssertionFunction(functionCall);
        }
    }

    public class AssertionFunction
    {
        private readonly Expression<Action> functionCall;

        public AssertionFunction(Expression<Action> functionCall)
        {
            this.functionCall = functionCall;
        }

        public Actual<TException> ShouldThrow<TException>(string message = null) where TException : Exception
        {
            try
            {
                functionCall.Compile()();
            }
            catch (TException e)
            {
                return new Actual<TException>(e);
            }
            catch (Exception actual)
            {
                // functionCall.Body + " should have thrown " + typeof(T).Name + ", but instead threw " + e.GetType().Name
                throw new EasyAssertionException(FailureMessageFormatter.Current.WrongException(typeof(TException), actual.GetType(), functionCall, message), actual);
            }

            //functionCall.Body + " should have thrown " + typeof(TException).Name + ", but didn't throw at all."
            throw new EasyAssertionException(FailureMessageFormatter.Current.NoException(typeof(TException), functionCall, message));
        }
    }
}