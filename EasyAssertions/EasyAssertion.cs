using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EasyAssertions
{
    public static class EasyAssertion
    {
        private static Func<string, Exception> createMessageException;
        private static Func<string, Exception, Exception> createInnerExceptionException;

        public static Actual<TActual> ShouldBe<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TExpected : TActual
        {
            return actual.Assert(() =>
                {
                    if (!Compare.ObjectsAreEqual(actual, expected))
                    {
                        string actualString = actual as string;
                        string expectedString = expected as string;
                        if (actualString != null && expectedString != null)
                            throw Failure(FailureMessageFormatter.Current.NotEqual(expectedString, actualString, message));

                        throw Failure(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
                    }
                });
        }

        public static Actual<TActual> ShouldNotBe<TActual, TNotExpected>(this TActual actual, TNotExpected notExpected, string message = null) where TNotExpected : TActual
        {
            return actual.Assert(() =>
                {
                    if (Compare.ObjectsAreEqual(actual, notExpected))
                        throw Failure(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
                });
        }

        public static Actual<float> ShouldBe(this float actual, float expected, float delta, string message = null)
        {
            return actual.Assert(() =>
                {
                    if (!Compare.AreWithinDelta(actual, expected, delta))
                        throw Failure(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
                });
        }

        public static Actual<float> ShouldNotBe(this float actual, float notExpected, float delta, string message = null)
        {
            return actual.Assert(() =>
                {
                    if (Compare.AreWithinDelta(actual, notExpected, delta))
                        throw Failure(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
                });
        }

        public static Actual<double> ShouldBe(this double actual, double expected, double delta, string message = null)
        {
            return actual.Assert(() =>
            {
                if (!Compare.AreWithinDelta(actual, expected, delta))
                    throw Failure(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
            });
        }

        public static Actual<double> ShouldNotBe(this double actual, double notExpected, double delta, string message = null)
        {
            return actual.Assert(() =>
            {
                if (Compare.AreWithinDelta(actual, notExpected, delta))
                    throw Failure(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
            });
        }

        public static void ShouldBeNull<TActual>(this TActual actual, string message = null)
        {
            actual.Assert(() =>
                {
                    if (!Equals(actual, null))
                        throw Failure(FailureMessageFormatter.Current.NotEqual(null, actual, message));
                });
        }

        public static Actual<TActual> ShouldNotBeNull<TActual>(this TActual actual, string message = null)
        {
            return actual.Assert(() =>
                {
                    if (Equals(actual, null))
                        throw Failure(FailureMessageFormatter.Current.IsNull(message));
                });
        }

        public static Actual<TActual> ShouldBeThis<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TExpected : TActual
        {
            return actual.Assert(() =>
                {
                    if (!ReferenceEquals(actual, expected))
                        throw Failure(FailureMessageFormatter.Current.NotSame(expected, actual, message));
                });
        }

        public static Actual<TActual> ShouldNotBeThis<TActual, TNotExpected>(this TActual actual, TNotExpected notExpected, string message = null) where TNotExpected : TActual
        {
            return actual.Assert(() =>
                {
                    if (ReferenceEquals(actual, notExpected))
                        throw Failure(FailureMessageFormatter.Current.AreSame(actual, message));
                });
        }

        public static Actual<TActual> ShouldBeEmpty<TActual>(this TActual actual, string message = null) where TActual : IEnumerable
        {
            return actual.Assert(() =>
                {
                    if (!Compare.IsEmpty(actual))
                        throw Failure(FailureMessageFormatter.Current.NotEmpty(actual, message));
                });
        }

        public static Actual<TActual> ShouldNotBeEmpty<TActual>(this TActual actual, string message = null) where TActual : IEnumerable
        {
            return actual.Assert(() =>
                {
                    if (Compare.IsEmpty(actual))
                        throw Failure(FailureMessageFormatter.Current.IsEmpty(message));
                });
        }

        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.Assert(() => AssertMatch(actual, expected, (a, e) => Compare.ObjectsMatch(a, e), message));
        }

        public static Actual<IEnumerable<float>> ShouldMatch(this IEnumerable<float> actual, IEnumerable<float> expected, float delta, string message = null)
        {
            return actual.Assert(() => AssertMatch(actual, expected, (a, e) => Compare.AreWithinDelta(a, e, delta), message));
        }

        public static Actual<IEnumerable<double>> ShouldMatch(this IEnumerable<double> actual, IEnumerable<double> expected, double delta, string message = null)
        {
            return actual.Assert(() => AssertMatch(actual, expected, (a, e) => Compare.AreWithinDelta(a, e, delta), message));
        }

        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> predicate, string message = null)
        {
            return actual.Assert(() => AssertMatch(actual, expected, predicate, message));
        }

        private static void AssertMatch<TActual, TExpected>(IEnumerable<TActual> actual, IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> predicate, string message)
        {
            List<TActual> actualList = actual.ToList();
            List<TExpected> expectedList = expected.ToList();

            if (!Compare.CollectionsMatch(actualList, expectedList, (a, e) => predicate((TActual)a, (TExpected)e)))
                throw Failure(FailureMessageFormatter.Current.DoNotMatch(expected, actual, message));
        }

        public static Actual<IEnumerable<TActual>> ShouldContain<TActual, TExpected>(this IEnumerable<TActual> actual, TExpected expected, string message = null) where TExpected : TActual
        {
            return actual.Assert(() =>
                {
                    if (!actual.Contains(expected))
                        throw Failure(FailureMessageFormatter.Current.DoesNotContain(expected, actual, message));
                });
        }

        public static Actual<IEnumerable<TActual>> ShouldContainItems<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.Assert(() =>
                {
                    if (!Compare.ContainsAllItems(actual, expected))
                        throw Failure(FailureMessageFormatter.Current.DoesNotContainItems(expected, actual, message));
                });
        }

        public static Actual<IEnumerable<TActual>> ShouldOnlyContain<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.Assert(() =>
                {
                    if (!Compare.ContainsOnlyExpectedItems(actual, expected))
                        throw Failure(FailureMessageFormatter.Current.DoesNotOnlyContain(expected, actual, message));
                });
        }

        public static Actual<IEnumerable<TActual>> ShouldBeThese<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null)
        {
            return actual.Assert(() =>
                {
                    List<TActual> actualList = actual.ToList();
                    List<TExpected> expectedList = expected.ToList();

                    if (actualList.Count != expectedList.Count)
                        throw Failure(FailureMessageFormatter.Current.LengthMismatch(expectedList.Count, actual, message));

                    if (!Compare.CollectionsMatch(actual, expected, ReferenceEquals))
                        throw Failure(FailureMessageFormatter.Current.ItemsNotSame(expected, actual, message));
                });
        }

        public static Actual<IEnumerable<TItem>> ItemsSatisfy<TItem>(this IEnumerable<TItem> actual, params Action<TItem>[] assertions)
        {
            return actual.Assert(() =>
                {
                    List<TItem> actualList = actual.ToList();
                    if (actualList.Count != assertions.Length)
                        throw Failure(FailureMessageFormatter.Current.LengthMismatch(assertions.Length, actual));

                    for (int i = 0; i < assertions.Length; i++)
                        IndexedAssert(i, assertions[i].Method, () => assertions[i](actualList[i]));
                });
        }

        public static Actual<IEnumerable<TItem>> AllItemsSatisfy<TItem>(this IEnumerable<TItem> actual, Action<TItem> assertion)
        {
            return actual.Assert(() =>
                {
                    int i = 0;
                    foreach (TItem item in actual)
                        IndexedAssert(i++, assertion.Method, () => assertion(item));
                });
        }

        public static Actual<TExpected> ShouldBeA<TExpected>(this object actual, string message = null)
        {
            actual.Assert(() =>
                {
                    if (!(actual is TExpected))
                        throw Failure(FailureMessageFormatter.Current.NotEqual(typeof(TExpected),
                            actual == null ? null : actual.GetType(), message));
                });

            return new Actual<TExpected>((TExpected)actual);
        }

        public static Actual<string> ShouldContain(this string actual, string expectedToContain, string message = null)
        {
            return actual.Assert(() =>
                {
                    if (!actual.Contains(expectedToContain))
                        throw Failure(FailureMessageFormatter.Current.DoesNotContain(expectedToContain, actual, message));
                });
        }

        public static Actual<string> ShouldStartWith(this string actual, string expectedStart, string message = null)
        {
            return actual.Assert(() =>
                {
                    if (!actual.StartsWith(expectedStart))
                        throw Failure(FailureMessageFormatter.Current.DoesNotStartWith(expectedStart, actual, message));
                });
        }

        public static Actual<string> ShouldEndWith(this string actual, string expectedEnd, string message = null)
        {
            return actual.Assert(() =>
                {
                    if (!actual.EndsWith(expectedEnd))
                        throw Failure(FailureMessageFormatter.Current.DoesNotEndWith(expectedEnd, actual, message));
                });
        }

        public static Actual<TActual> And<TActual>(this Actual<TActual> actual, Action<TActual> assert)
        {
            actual.Assert(() => InnerAssert(assert.Method, () => assert(actual.Value)));
            return actual;
        }

        public static Actual<TActual> Assert<TActual>(this TActual actual, Action assert)
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod(1);
            assert();
            return new Actual<TActual>(actual);
        }

        public static Actual<TActual> Assert<TActual>(this TActual actual, Action<TActual> assert)
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod(1);
            InnerAssert(assert.Method, () => assert(actual));
            return new Actual<TActual>(actual);
        }

        public static Actual<TActual> Assert<TActual>(this TActual actual, Func<TActual, Actual<TActual>> assert)
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod(1);
            Actual<TActual> ret = null;
            InnerAssert(assert.Method, () => ret = assert(actual));
            return ret;
        }

        public static void IndexedAssert(int index, MethodInfo itemAssertionMethod, Action assert)
        {
            SourceExpressionProvider.Instance.EnterIndexedAssertion(itemAssertionMethod, index);
            assert();
            SourceExpressionProvider.Instance.ExitNestedAssertion();
        }

        public static void InnerAssert(MethodInfo innerAssertionMethod, Action assert)
        {
            SourceExpressionProvider.Instance.EnterNestedAssertion(innerAssertionMethod);
            assert();
            SourceExpressionProvider.Instance.ExitNestedAssertion();
        }

        public static void UseFrameworkExceptions(Func<string, Exception> messageExceptionFactory, Func<string, Exception, Exception> innerExceptionExceptionFactory)
        {
            createMessageException = messageExceptionFactory;
            createInnerExceptionException = innerExceptionExceptionFactory;
        }

        public static void UseEasyAssertionExceptions()
        {
            createMessageException = null;
            createInnerExceptionException = null;
        }

        public static Exception Failure(string failureMessage)
        {
            return createMessageException != null
                ? createMessageException(failureMessage)
                : new EasyAssertionException(failureMessage);
        }

        public static Exception Failure(string failureMessage, Exception innerException)
        {
            return createInnerExceptionException != null
                ? createInnerExceptionException(failureMessage, innerException)
                : new EasyAssertionException(failureMessage, innerException);
        }
    }

    public static class Should
    {
        public static Actual<TException> Throw<TException>(Expression<Func<object>> expression, string message = null) where TException : Exception
        {
            return Test<TException>(expression, message, () => expression.Compile()());
        }

        public static Actual<TException> Throw<TException>(Expression<Action> expression, string message = null) where TException : Exception
        {
            return Test<TException>(expression, message, expression.Compile());
        }

        private static Actual<TException> Test<TException>(LambdaExpression expression, string message, Action executeExpression) where TException : Exception
        {
            try
            {
                executeExpression();
            }
            catch (TException e)
            {
                return new Actual<TException>(e);
            }
            catch (Exception actual)
            {
                throw EasyAssertion.Failure(FailureMessageFormatter.Current.WrongException(typeof(TException), actual.GetType(), expression, message), actual);
            }

            throw EasyAssertion.Failure(FailureMessageFormatter.Current.NoException(typeof(TException), expression, message));
        }
    }
}
