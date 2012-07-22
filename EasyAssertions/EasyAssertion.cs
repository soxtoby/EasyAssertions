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

        /// <summary>
        /// Asserts that two objects are equal, using the default equality comparer.
        /// </summary>
        public static Actual<TActual> ShouldBe<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
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

        /// <summary>
        /// Asserts that two objects are not equal, using the default equality comparer.
        /// </summary>
        public static Actual<TActual> ShouldNotBe<TActual, TNotExpected>(this TActual actual, TNotExpected notExpected, string message = null) where TNotExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (Compare.ObjectsAreEqual(actual, notExpected))
                        throw Failure(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that two float values are within a specified tolerance of eachother.
        /// </summary>
        public static Actual<float> ShouldBe(this float actual, float expected, float tolerance, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (!Compare.AreWithinTolerance(actual, expected, tolerance))
                        throw Failure(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that two float values are not within a specified tolerance of eachother.
        /// </summary>
        public static Actual<float> ShouldNotBe(this float actual, float notExpected, float tolerance, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (Compare.AreWithinTolerance(actual, notExpected, tolerance))
                        throw Failure(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that two double values are within a specified tolerance of eachother.
        /// </summary>
        public static Actual<double> ShouldBe(this double actual, double expected, double delta, string message = null)
        {
            return actual.RegisterAssert(() =>
            {
                if (!Compare.AreWithinTolerance(actual, expected, delta))
                    throw Failure(FailureMessageFormatter.Current.NotEqual(expected, actual, message));
            });
        }

        /// <summary>
        /// Asserts that two double values are not within a specified tolerance of eachother.
        /// </summary>
        public static Actual<double> ShouldNotBe(this double actual, double notExpected, double delta, string message = null)
        {
            return actual.RegisterAssert(() =>
            {
                if (Compare.AreWithinTolerance(actual, notExpected, delta))
                    throw Failure(FailureMessageFormatter.Current.AreEqual(notExpected, actual, message));
            });
        }

        /// <summary>
        /// Asserts that the given object is a null reference.
        /// </summary>
        public static void ShouldBeNull<TActual>(this TActual actual, string message = null)
        {
            actual.RegisterAssert(() =>
                {
                    if (!Equals(actual, null))
                        throw Failure(FailureMessageFormatter.Current.NotEqual(null, actual, message));
                });
        }

        /// <summary>
        /// Asserts that the given object is not a null reference.
        /// </summary>
        public static Actual<TActual> ShouldNotBeNull<TActual>(this TActual actual, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (Equals(actual, null))
                        throw Failure(FailureMessageFormatter.Current.IsNull(message));
                });
        }

        /// <summary>
        /// Asserts that two object instances are the same instance.
        /// </summary>
        public static Actual<TActual> ShouldBeThis<TActual, TExpected>(this TActual actual, TExpected expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (!ReferenceEquals(actual, expected))
                        throw Failure(FailureMessageFormatter.Current.NotSame(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that two object instances are different instances.
        /// </summary>
        public static Actual<TActual> ShouldNotBeThis<TActual, TNotExpected>(this TActual actual, TNotExpected notExpected, string message = null) where TNotExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (ReferenceEquals(actual, notExpected))
                        throw Failure(FailureMessageFormatter.Current.AreSame(actual, message));
                });
        }

        /// <summary>
        /// Asserts that a sequence has no elements in it.
        /// </summary>
        public static Actual<TActual> ShouldBeEmpty<TActual>(this TActual actual, string message = null) where TActual : IEnumerable
        {
            return actual.RegisterAssert(() =>
                {
                    if (!Compare.IsEmpty(actual))
                        throw Failure(FailureMessageFormatter.Current.NotEmpty(actual, message));
                });
        }

        /// <summary>
        /// Asserts that a sequence contains at least one element.
        /// </summary>
        public static Actual<TActual> ShouldNotBeEmpty<TActual>(this TActual actual, string message = null) where TActual : IEnumerable
        {
            return actual.RegisterAssert(() =>
                {
                    if (Compare.IsEmpty(actual))
                        throw Failure(FailureMessageFormatter.Current.IsEmpty(message));
                });
        }

        /// <summary>
        /// Asserts that two sequences contain the same items in the same order.
        /// Non-IEnumerable items are compared using the default equality comparer.
        /// IEnumerable items are compared recursively.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() => AssertMatch(actual, expected, (a, e) => Compare.ObjectsMatch(a, e), message));
        }

        /// <summary>
        /// Asserts that two sequences contain the same float values (within a specified tolerance), in the same order.
        /// </summary>
        public static Actual<IEnumerable<float>> ShouldMatch(this IEnumerable<float> actual, IEnumerable<float> expected, float delta, string message = null)
        {
            return actual.RegisterAssert(() => AssertMatch(actual, expected, (a, e) => Compare.AreWithinTolerance(a, e, delta), message));
        }

        /// <summary>
        /// Asserts that two sequences contain the same double values (within a specified tolerance), in the same order.
        /// </summary>
        public static Actual<IEnumerable<double>> ShouldMatch(this IEnumerable<double> actual, IEnumerable<double> expected, double delta, string message = null)
        {
            return actual.RegisterAssert(() => AssertMatch(actual, expected, (a, e) => Compare.AreWithinTolerance(a, e, delta), message));
        }

        /// <summary>
        /// Asserts that two sequences contain the same items in the same order, using a custom equality function.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldMatch<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> predicate, string message = null)
        {
            return actual.RegisterAssert(() => AssertMatch(actual, expected, predicate, message));
        }

        private static void AssertMatch<TActual, TExpected>(IEnumerable<TActual> actual, IEnumerable<TExpected> expected, Func<TActual, TExpected, bool> predicate, string message)
        {
            List<TActual> actualList = actual.ToList();
            List<TExpected> expectedList = expected.ToList();

            if (!Compare.CollectionsMatch(actualList, expectedList, (a, e) => predicate((TActual)a, (TExpected)e)))
                throw Failure(FailureMessageFormatter.Current.DoNotMatch(expected, actual, message));
        }

        /// <summary>
        /// Asserts that a sequence contains a specified element, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldContain<TActual, TExpected>(this IEnumerable<TActual> actual, TExpected expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (!actual.Contains(expected))
                        throw Failure(FailureMessageFormatter.Current.DoesNotContain(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a sequence contains all specified elements, in any order, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldContainItems<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (!Compare.ContainsAllItems(actual, expected))
                        throw Failure(FailureMessageFormatter.Current.DoesNotContainItems(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts thats a sequence only contains the specified elements, in any order, using the default equality comparer.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldOnlyContain<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null) where TExpected : TActual
        {
            return actual.RegisterAssert(() =>
                {
                    if (!Compare.ContainsOnlyExpectedItems(actual, expected))
                        throw Failure(FailureMessageFormatter.Current.DoesNotOnlyContain(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that two sequences contain the same object instances in the same order.
        /// </summary>
        public static Actual<IEnumerable<TActual>> ShouldBeThese<TActual, TExpected>(this IEnumerable<TActual> actual, IEnumerable<TExpected> expected, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    List<TActual> actualList = actual.ToList();
                    List<TExpected> expectedList = expected.ToList();

                    if (actualList.Count != expectedList.Count)
                        throw Failure(FailureMessageFormatter.Current.LengthMismatch(expectedList.Count, actual, message));

                    if (!Compare.CollectionsMatch(actual, expected, ReferenceEquals))
                        throw Failure(FailureMessageFormatter.Current.ItemsNotSame(expected, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a sequence of items satisfies a matched sequence of assertions.
        /// </summary>
        public static Actual<IEnumerable<TItem>> ItemsSatisfy<TItem>(this IEnumerable<TItem> actual, params Action<TItem>[] assertions)
        {
            return actual.RegisterAssert(() =>
                {
                    List<TItem> actualList = actual.ToList();
                    if (actualList.Count != assertions.Length)
                        throw Failure(FailureMessageFormatter.Current.LengthMismatch(assertions.Length, actual));

                    for (int i = 0; i < assertions.Length; i++)
                        RegisterIndexedAssert(i, assertions[i].Method, () => assertions[i](actualList[i]));
                });
        }

        /// <summary>
        /// Asserts that all items in a sequence satisfy a specified assertion.
        /// </summary>
        public static Actual<IEnumerable<TItem>> AllItemsSatisfy<TItem>(this IEnumerable<TItem> actual, Action<TItem> assertion)
        {
            return actual.RegisterAssert(() =>
                {
                    int i = 0;
                    foreach (TItem item in actual)
                        RegisterIndexedAssert(i++, assertion.Method, () => assertion(item));
                });
        }

        /// <summary>
        /// Asserts that an object is assignable to a specified type.
        /// </summary>
        public static Actual<TExpected> ShouldBeA<TExpected>(this object actual, string message = null)
        {
            actual.RegisterAssert(() =>
                {
                    if (!(actual is TExpected))
                        throw Failure(FailureMessageFormatter.Current.NotEqual(typeof(TExpected),
                            actual == null ? null : actual.GetType(), message));
                });

            return new Actual<TExpected>((TExpected)actual);
        }

        /// <summary>
        /// Asserts that a string contains a specified substring.
        /// </summary>
        public static Actual<string> ShouldContain(this string actual, string expectedToContain, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (!actual.Contains(expectedToContain))
                        throw Failure(FailureMessageFormatter.Current.DoesNotContain(expectedToContain, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a string does not contain a specified substring.
        /// </summary>
        public static Actual<string> ShouldNotContain(this string actual, string expectedToNotContain, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (actual.Contains(expectedToNotContain))
                        throw Failure(FailureMessageFormatter.Current.Contains(expectedToNotContain, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a string begins with a specified substring.
        /// </summary>
        public static Actual<string> ShouldStartWith(this string actual, string expectedStart, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (!actual.StartsWith(expectedStart))
                        throw Failure(FailureMessageFormatter.Current.DoesNotStartWith(expectedStart, actual, message));
                });
        }

        /// <summary>
        /// Asserts that a string ends with a specified substring.
        /// </summary>
        public static Actual<string> ShouldEndWith(this string actual, string expectedEnd, string message = null)
        {
            return actual.RegisterAssert(() =>
                {
                    if (!actual.EndsWith(expectedEnd))
                        throw Failure(FailureMessageFormatter.Current.DoesNotEndWith(expectedEnd, actual, message));
                });
        }

        /// <summary>
        /// Provides access to an object's child properties without changing the method chaining context.
        /// </summary>
        public static Actual<TActual> And<TActual>(this Actual<TActual> actual, Action<TActual> assert)
        {
            actual.RegisterAssert(() => RegisterInnerAssert(assert.Method, () => assert(actual.Value)));
            return actual;
        }

        /// <summary>
        /// Registers an assertion action for purposes of tracking the current position in the assertion expression's source code.
        /// Use this for custom assertions that act directly on the actual value.
        /// </summary>
        public static Actual<TActual> RegisterAssert<TActual>(this TActual actual, Action assert)
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod(1);
            assert();
            return new Actual<TActual>(actual);
        }

        /// <summary>
        /// Registers an assertion action with a named parameter for following the assertion expression into the action.
        /// Use this for custom assertions that call other existing assertions.
        /// </summary>
        public static Actual<TActual> RegisterAssert<TActual>(this TActual actual, Action<TActual> assert)
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod(1);
            RegisterInnerAssert(assert.Method, () => assert(actual));
            return new Actual<TActual>(actual);
        }

        /// <summary>
        /// Registers an assertion function with a named parameter for following the assertion expression into the function.
        /// Use this for custom assertions that call other existing assertions.
        /// </summary>
        /// <returns>
        /// The return value of the registered assertion function.
        /// </returns>
        public static Actual<TActual> RegisterAssert<TActual>(this TActual actual, Func<TActual, Actual<TActual>> assert)
        {
            SourceExpressionProvider.Instance.RegisterAssertionMethod(1);
            Actual<TActual> ret = null;
            RegisterInnerAssert(assert.Method, () => ret = assert(actual));
            return ret;
        }

        /// <summary>
        /// Registers an assertion action that is associated with an index into the actual object.
        /// The first parameter of the item assertion method is assumed to be the actual value.
        /// Use this when executing a user-provided assertion on an item in a sequence.
        /// </summary>
        public static void RegisterIndexedAssert(int index, MethodInfo itemAssertionMethod, Action assert)
        {
            SourceExpressionProvider.Instance.EnterIndexedAssertion(itemAssertionMethod, index);
            assert();
            SourceExpressionProvider.Instance.ExitNestedAssertion();
        }

        /// <summary>
        /// Registers an assertion action to be executed from within another assertion method,
        /// for purposes of following the assertion expression into the inner assertion method.
        /// The first parameter of the item assertion method is assumed to be the actual value.
        /// Use this when executing a user-provided assertion.
        /// </summary>
        public static void RegisterInnerAssert(MethodInfo innerAssertionMethod, Action assert)
        {
            SourceExpressionProvider.Instance.EnterNestedAssertion(innerAssertionMethod);
            assert();
            SourceExpressionProvider.Instance.ExitNestedAssertion();
        }

        /// <summary>
        /// Overrides the exceptions used when assertions fail.
        /// Test frameworks will detect their own exception types and display the correct assertion failure messages.
        /// </summary>
        public static void UseFrameworkExceptions(Func<string, Exception> messageExceptionFactory, Func<string, Exception, Exception> innerExceptionExceptionFactory)
        {
            createMessageException = messageExceptionFactory;
            createInnerExceptionException = innerExceptionExceptionFactory;
        }

        /// <summary>
        /// Throw EasyAssertionExceptions when assertions fail.
        /// </summary>
        public static void UseEasyAssertionExceptions()
        {
            createMessageException = null;
            createInnerExceptionException = null;
        }

        /// <summary>
        /// Creates an exception to be thrown for a failed assertion.
        /// </summary>
        public static Exception Failure(string failureMessage)
        {
            return createMessageException != null
                ? createMessageException(failureMessage)
                : new EasyAssertionException(failureMessage);
        }

        /// <summary>
        /// Creates an exception to be thrown for a failed assertion.
        /// </summary>
        public static Exception Failure(string failureMessage, Exception innerException)
        {
            return createInnerExceptionException != null
                ? createInnerExceptionException(failureMessage, innerException)
                : new EasyAssertionException(failureMessage, innerException);
        }
    }

    public static class Should
    {
        /// <summary>
        /// Assert that a function will throw an exception.
        /// </summary>
        public static Actual<TException> Throw<TException>(Expression<Func<object>> expression, string message = null) where TException : Exception
        {
            return Test<TException>(expression, message, () => expression.Compile()());
        }

        /// <summary>
        /// Assert that an action will throw an exception.
        /// </summary>
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
