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
        public static Actual<TActual> ShouldBe<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TExpected : TActual
        {
            return actual.Assert(() =>
                {
                    if (!Compare.ObjectsAreEqual(actual, expected))
                    {
                        string actualString = actual as string;
                        string expectedString = expected as string;
                        if (actualString != null && expectedString != null)
                            throw new EasyAssertionException(FailureMessageFormatter.Current.NotEqual(expectedString, actualString, message));

                        throw new EasyAssertionException(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
                    }
                });
        }

        public static Actual<TActual> ShouldNotBe<TActual, TNotExpected>(this TActual actual, TNotExpected notExpected, string message = null) where TNotExpected : TActual
        {
            return actual.Assert(() =>
                {
                    if (Compare.ObjectsAreEqual(actual, notExpected))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
                });
        }

        public static Actual<float> ShouldBe(this float actual, float expected, float delta, string message = null)
        {
            return actual.Assert(() =>
                {
                    if (!Compare.AreWithinDelta(actual, expected, delta))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
                });
        }

        public static Actual<float> ShouldNotBe(this float actual, float notExpected, float delta, string message = null)
        {
            return actual.Assert(() =>
                {
                    if (Compare.AreWithinDelta(actual, notExpected, delta))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
                });
        }

        public static Actual<double> ShouldBe(this double actual, double expected, double delta, string message = null)
        {
            return actual.Assert(() =>
            {
                if (!Compare.AreWithinDelta(actual, expected, delta))
                    throw new EasyAssertionException(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
            });
        }

        public static Actual<double> ShouldNotBe(this double actual, double notExpected, double delta, string message = null)
        {
            return actual.Assert(() =>
            {
                if (Compare.AreWithinDelta(actual, notExpected, delta))
                    throw new EasyAssertionException(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
            });
        }

        public static void ShouldBeNull<TActual>(this TActual actual, string message = null)
        {
            actual.Assert(() =>
                {
                    if (!Equals(actual, null))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.NotEqual(null, actual, message));
                });
        }

        public static Actual<TActual> ShouldNotBeNull<TActual>(this TActual actual, string message = null)
        {
            return actual.Assert(() =>
                {
                    if (Equals(actual, null))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.IsNull(message));
                });
        }

        public static Actual<TActual> ShouldBeThis<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TExpected : TActual
        {
            return actual.Assert(() =>
                {
                    if (!ReferenceEquals(actual, expected))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.NotSame(expected, actual, message));
                });
        }

        public static Actual<TActual> ShouldNotBeThis<TActual, TNotExpected>(this TActual actual, TNotExpected notExpected, string message = null) where TNotExpected : TActual
        {
            return actual.Assert(() =>
                {
                    if (ReferenceEquals(actual, notExpected))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.AreSame(actual, message));
                });
        }

        public static Actual<TActual> ShouldBeEmpty<TActual>(this TActual actual, string message = null) where TActual : IEnumerable
        {
            return actual.Assert(() =>
                {
                    if (!Compare.IsEmpty(actual))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.NotEmpty(actual, message));
                });
        }

        public static Actual<TActual> ShouldNotBeEmpty<TActual>(this TActual actual, string message = null) where TActual : IEnumerable
        {
            return actual.Assert(() =>
                {
                    if (Compare.IsEmpty(actual))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.IsEmpty(message));
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
                throw new EasyAssertionException(FailureMessageFormatter.Current.DoNotMatch(expected, actual, message));
        }

        public static Actual<IEnumerable<TActual>> ShouldContain<TActual, TExpected>(this IEnumerable<TActual> actual, TExpected expected, string message = null) where TExpected : TActual
        {
            return actual.Assert(() =>
                {
                    if (!actual.Contains(expected))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.DoesNotContain(expected, actual, message));
                });
        }

        public static Actual<IEnumerable<TActual>> ShouldContainItems<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.Assert(() =>
                {
                    HashSet<TActual> actualList = new HashSet<TActual>(actual);

                    foreach (TExpected expectedItem in expected)
                    {
                        if (!actualList.Contains(expectedItem))
                            throw new EasyAssertionException(FailureMessageFormatter.Current.DoesNotContainItems(expected, actual, message));
                    }
                });
        }

        public static Actual<IEnumerable<TActual>> ShouldBeThese<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null)
        {
            return actual.Assert(() =>
                {
                    List<TActual> actualList = actual.ToList();
                    List<TExpected> expectedList = expected.ToList();

                    if (actualList.Count != expectedList.Count)
                        throw new EasyAssertionException(FailureMessageFormatter.Current.LengthMismatch(expectedList.Count, actual, message));

                    if (!Compare.CollectionsMatch(actual, expected, ReferenceEquals))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.ItemsNotSame(expected, actual, message));
                });
        }

        public static Actual<IEnumerable<TItem>> ItemsSatisfy<TItem>(this IEnumerable<TItem> actual, params Action<TItem>[] assertions)
        {
            return actual.Assert(() =>
                {
                    List<TItem> actualList = actual.ToList();
                    if (actualList.Count != assertions.Length)
                        throw new EasyAssertionException(FailureMessageFormatter.Current.LengthMismatch(assertions.Length, actual));

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
                        throw new EasyAssertionException(FailureMessageFormatter.Current.NotEqual(typeof(TExpected),
                            actual == null ? null : actual.GetType(), message));
                });

            return new Actual<TExpected>((TExpected)actual);
        }

        public static Actual<string> ShouldContain(this string actual, string expectedToContain, string message = null)
        {
            return actual.Assert(() =>
                {
                    if (!actual.Contains(expectedToContain))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.DoesNotContain(expectedToContain, actual, message));
                });
        }

        public static Actual<string> ShouldStartWith(this string actual, string expectedStart, string message = null)
        {
            return actual.Assert(() =>
                {
                    if (!actual.StartsWith(expectedStart))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.DoesNotStartWith(expectedStart, actual, message));
                });
        }

        public static Actual<string> ShouldEndWith(this string actual, string expectedEnd, string message = null)
        {
            return actual.Assert(() =>
                {
                    if (!actual.EndsWith(expectedEnd))
                        throw new EasyAssertionException(FailureMessageFormatter.Current.DoesNotEndWith(expectedEnd, actual, message));
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
