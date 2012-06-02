using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EasyAssertions
{
    public static class EasyAssertion
    {
        public static Actual<TActual> ShouldBe<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TActual : TExpected
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod();

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

        public static Actual<TActual> ShouldBeThis<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TActual : TExpected
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod();

            if (!ReferenceEquals(actual, expected))
                throw new EasyAssertionException(FailureMessageFormatter.Current.NotSame(expected, actual, message));

            return new Actual<TActual>(actual);
        }

        public static Actual<TActual> ShouldBeEmpty<TActual>(this TActual actual, string message = null) where TActual : IEnumerable
        {
            IEnumerator enumerator = actual.GetEnumerator();
            bool empty = !enumerator.MoveNext();
            Dispose(enumerator);

            if (!empty)
                throw new EasyAssertionException(FailureMessageFormatter.Current.NotEmpty(actual, message));

            return new Actual<TActual>(actual);
        }

        private static void Dispose(IEnumerator enumerator)
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        public static Actual<TActual> ShouldMatch<TActual, TExpected>(this TActual actual, IEnumerable<TExpected> expected, string message = null) where TActual : IEnumerable<TExpected>
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod();

            if (!Compare.CollectionsMatch(actual, expected))
                throw new EasyAssertionException(FailureMessageFormatter.Current.DoNotMatch(expected, actual, message));

            return new Actual<TActual>(actual);
        }

        public static Actual<TExpected> ShouldBeA<TExpected>(this object actual, string message = null)
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod();

            if (!(actual is TExpected))
                throw new EasyAssertionException(FailureMessageFormatter.Current.NotEqual(typeof(TExpected), actual == null ? null : actual.GetType(), message));

            return new Actual<TExpected>((TExpected)actual);
        }

        public static Actual<string> ShouldContain(this string actual, string expectedToContain, string message = null)
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod();

            if (!actual.Contains(expectedToContain))
                throw new EasyAssertionException(FailureMessageFormatter.Current.DoesNotContain(expectedToContain, actual, message));

            return new Actual<string>(actual);
        }

        public static Actual<TActual> And<TActual>(this Actual<TActual> actual, Action<TActual> assert)
        {
            SourceExpressionProvider.Instance.EnterNestedContinuation(assert.Method);
            assert(actual.Value);
            SourceExpressionProvider.Instance.ExitNestedContinuation();
            return actual;
        }
    }

    public static class Should
    {
        public static Actual<TException> Throw<TException>(Expression<Action> expression, string message = null) where TException : Exception
        {
            try
            {
                expression.Compile()();
            }
            catch (TException e)
            {
                return new Actual<TException>(e);
            }
            catch (Exception actual)
            {
                throw new EasyAssertionException(FailureMessageFormatter.Current.WrongException(typeof(TException), actual.GetType(), expression, message), actual);
            }

            throw new EasyAssertionException(FailureMessageFormatter.Current.NoException(typeof(TException), expression, message));
        }
    }
}