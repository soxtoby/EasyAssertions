using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EasyAssertions
{
    public static class EasyAssertion
    {
        public static Actual<TActual> ShouldBe<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TExpected : TActual
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

        public static Actual<TActual> ShouldNotBe<TActual, TNotExpected>(this TActual actual, TNotExpected notExpected, string message = null) where TNotExpected : TActual
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod();

            if (Compare.ObjectsAreEqual(actual, notExpected))
                throw new EasyAssertionException(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));

            return new Actual<TActual>(actual);
        }

        public static Actual<TActual> ShouldBeThis<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TExpected : TActual
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod();

            if (!ReferenceEquals(actual, expected))
                throw new EasyAssertionException(FailureMessageFormatter.Current.NotSame(expected, actual, message));

            return new Actual<TActual>(actual);
        }

        public static Actual<TActual> ShouldNotBeThis<TActual, TNotExpected>(this TActual actual, TNotExpected notExpected, string message = null) where TNotExpected : TActual
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod();

            if (ReferenceEquals(actual, notExpected))
                throw new EasyAssertionException(FailureMessageFormatter.Current.AreSame(actual, message));

            return new Actual<TActual>(actual);
        }

        public static Actual<TActual> ShouldBeEmpty<TActual>(this TActual actual, string message = null) where TActual : IEnumerable
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod();

            if (!Compare.IsEmpty(actual))
                throw new EasyAssertionException(FailureMessageFormatter.Current.NotEmpty(actual, message));

            return new Actual<TActual>(actual);
        }

        public static Actual<TActual> ShouldNotBeEmpty<TActual>(this TActual actual, string message = null) where TActual : IEnumerable
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod();

            if (Compare.IsEmpty(actual))
                throw new EasyAssertionException(FailureMessageFormatter.Current.IsEmpty(message));

            return new Actual<TActual>(actual);
        }

        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod();

            if (!Compare.CollectionsMatch(actual, expected))
                throw new EasyAssertionException(FailureMessageFormatter.Current.DoNotMatch(expected, actual, message));

            return new Actual<IEnumerable<TActual>>(actual);
        }

        public static Actual<IEnumerable<TItem>> ItemsSatisfy<TItem>(this IEnumerable<TItem> actual, params Action<TItem>[] assertions)
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod();

            List<TItem> actualList = actual.ToList();
            if (actualList.Count != assertions.Length)
                throw new EasyAssertionException(FailureMessageFormatter.Current.LengthMismatch(actual, assertions.Length));

            for (int i = 0; i < assertions.Length; i++)
            {
                SourceExpressionProvider.Instance.EnterIndexedAssertion(assertions[i].Method, i);
                assertions[i](actualList[i]);
                SourceExpressionProvider.Instance.ExitNestedAssertion();
            }

            return new Actual<IEnumerable<TItem>>(actual);
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
            SourceExpressionProvider.Instance.RegisterAssertionMethod();
            SourceExpressionProvider.Instance.EnterNestedAssertion(assert.Method);
            assert(actual.Value);
            SourceExpressionProvider.Instance.ExitNestedAssertion();
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