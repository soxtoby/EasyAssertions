using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using static EasyAssertions.UnitTests.EnumerableArg;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class CollectionAssertionTests : AssertionTests
    {
        class ShouldbeEmpty : CollectionAssertionTests
        {
            [Test]
            public void IsEmpty_ReturnsActualValue()
            {
                IEnumerable<object> actual = Enumerable.Empty<object>();

                AssertReturnsActual(actual, () => actual.ShouldBeEmpty());
            }

            [Test]
            public void NonEmpty_FailsWithEnumerableNotEmptyMessage()
            {
                int[] actual = { 1 };
                MockFormatter.NotEmpty(Matches(actual), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeEmpty("foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void NonEmpty_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                MockFormatter.NotEmpty(Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeEmpty());

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<string> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<string>), null, msg => actual.ShouldBeEmpty(msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                IEnumerable<string> actual = Enumerable.Empty<string>();

                actual.ShouldBeEmpty();

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
            }
        }

        class ShouldNotBeEmpty : CollectionAssertionTests
        {
            [Test]
            public void NotEmpty_ReturnsActualValue()
            {
                int[] actual = { 1 };

                AssertReturnsActual(actual, () => actual.ShouldNotBeEmpty());
            }

            [Test]
            public void IsEmpty_FailsWithEnumerableEmptyMessage()
            {
                IEnumerable<object> actual = Enumerable.Empty<object>();
                MockFormatter.IsEmpty("foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotBeEmpty("foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<string> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<string>), null, msg => actual.ShouldNotBeEmpty(msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };

                actual.ShouldNotBeEmpty();

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
            }
        }

        class ShouldBeSingular : CollectionAssertionTests
        {
            [Test]
            public void IsSingular_ReturnsActualValue()
            {
                int[] actual = { 1 };

                AssertReturnsActual(actual, () => actual.ShouldBeSingular());
            }

            [Test]
            public void IsNotSingular_FailsWithLengthMismatchMessage()
            {
                int[] actual = { };
                MockFormatter.LengthMismatch(1, Matches(actual), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeSingular("foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void IsNotSingular_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1, 2);
                MockFormatter.LengthMismatch(Arg.Any<int>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeSingular());

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<string> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<string>), null, msg => actual.ShouldBeSingular(msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };

                actual.ShouldBeSingular();

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
            }
        }

        class ShouldBeLength : CollectionAssertionTests
        {
            [Test]
            public void IsExpectedLength_ReturnsActualValue()
            {
                int[] actual = { 1, 2, 3 };

                AssertReturnsActual(actual, () => actual.ShouldBeLength(3));
            }

            [Test]
            public void IsDifferentLength_FailsWithLengthMismatchMessage()
            {
                int[] actual = { 1, 2, 3 };
                MockFormatter.LengthMismatch(2, Matches(actual), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeLength(2, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void IsDifferentLength_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                MockFormatter.LengthMismatch(Arg.Any<int>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeLength(2));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<string> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<string>), null, msg => actual.ShouldBeLength(1, msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int expected = actual.Length;

                actual.ShouldBeLength(expected);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class ShouldMatch : CollectionAssertionTests
        {
            [Test]
            public void MatchingEnumerable_ReturnsActualValue()
            {
                int[] actual = { 1, 2 };

                AssertReturnsActual(actual, () => actual.ShouldMatch(new[] { 1, 2 }));
            }

            [Test]
            public void NonMatchingEnumerables_FailsWithEnumerablesDoNotMatchMessage()
            {
                int[] expected = { 3, 4 };
                int[] actual = { 1, 2 };
                MockFormatter.DoNotMatch(Matches<int>(expected), Matches<int>(actual), Compare.ObjectsMatch, "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, "foo"));


                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void NonMatchingEnumerables_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                TestEnumerable<int> expected = MakeEnumerable(2);
                MockFormatter.DoNotMatch(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<string> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<string>), null, msg => actual.ShouldMatch(new string[] { }, msg));
            }

            [Test]
            public void ExpectedIsNull_FailsWithArgumentNullException()
            {
                string[] actual = { };

                ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => actual.ShouldMatch((IEnumerable<string>)null, "foo"));

                Assert.AreEqual("expected", result.ParamName);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int[] expected = actual;

                actual.ShouldMatch(expected);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class ShouldMatch_CustomEquality : CollectionAssertionTests
        {
            [Test]
            public void MatchingEnumerable_ReturnsActualValue()
            {
                IEnumerable<int> actual = new[] { 1, 2 };

                AssertReturnsActual(actual, () => actual.ShouldMatch(new[] { 1, 2 }, (a, e) => a == e));
            }

            [Test]
            public void NonMatchingEnumerables_FailsWithEnumerablesDoNotMatch()
            {
                int[] actual = { 1, 2, 4 };
                int[] expected = { 2, 4, 6 };
                Func<int, int, bool> predicate = (a, e) => a == e / 2;
                MockFormatter.DoNotMatch(Matches<int>(expected), Matches<int>(actual), predicate, "foo").Returns("bar");
                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, predicate, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void NonMatchingEnumerables_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                TestEnumerable<int> expected = MakeEnumerable(2);
                MockFormatter.DoNotMatch(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, (a, b) => a == b));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypeNotEqualMessage()
            {
                IEnumerable<string> actual = null;
                MockFormatter.NotEqual(typeof(IEnumerable<string>), null, "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(Enumerable.Empty<string>(), (s, s1) => false, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int[] expected = actual;

                actual.ShouldMatch(expected, (a, b) => a == b);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class ShouldMatch_Floats : CollectionAssertionTests
        {
            [Test]
            public void WithinDelta_ReturnsActualValue()
            {
                float[] actual = { 10f, 20f };

                AssertReturnsActual(actual, () => actual.ShouldMatch(new[] { 11f, 21f }, 1f));
            }

            [Test]
            public void OutsideDelta_FailsWithEnumerablesDoNotMatchMessage()
            {
                float[] actual = { 10f, 20f };
                float[] expected = { 11f, 21f };
                Func<float, float, bool> predicate = null;
                MockFormatter.DoNotMatch(Matches<float>(expected), Matches<float>(actual), Arg.Do<Func<float, float, bool>>(p => predicate = p), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, 0.9f, "foo"));

                Assert.AreEqual("bar", result.Message);
                Assert.IsTrue(predicate(1f, 1.9f));
                Assert.IsFalse(predicate(1f, 2f));
            }

            [Test]
            public void OutsideDelta_OnlyEnumeratesOnce()
            {
                TestEnumerable<float> actual = MakeEnumerable(1f);
                TestEnumerable<float> expected = MakeEnumerable(2f);
                MockFormatter.DoNotMatch(Arg.Any<IEnumerable<float>>(), Arg.Any<IEnumerable<float>>(), Arg.Any<Func<float, float, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, float.Epsilon));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<float> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<float>), null, msg => actual.ShouldMatch(new[] { 1f }, 1f, msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                float[] actual = { 1 };
                float[] expected = actual;

                actual.ShouldMatch(expected, float.Epsilon);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class ShouldMatch_Doubles : CollectionAssertionTests
        {
            [Test]
            public void WithinDelta_ReturnsActualValue()
            {
                double[] actual = { 10d, 20d };

                AssertReturnsActual(actual, () => actual.ShouldMatch(new[] { 11d, 21d }, 1d));
            }

            [Test]
            public void OutsideDelta_FailsWithEnumerablesDoNotMatchMessage()
            {
                double[] actual = { 10d, 20d };
                double[] expected = { 11d, 21d };
                Func<double, double, bool> predicate = null;
                MockFormatter.DoNotMatch(Matches<double>(expected), Matches<double>(actual), Arg.Do<Func<double, double, bool>>(p => predicate = p), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, 0.9d, "foo"));

                Assert.AreEqual("bar", result.Message);
                Assert.IsTrue(predicate(1d, 1.9d));
                Assert.IsFalse(predicate(1d, 2d));
            }

            [Test]
            public void OutsideDelta_OnlyEnumeratesOnce()
            {
                TestEnumerable<double> actual = MakeEnumerable(1d);
                TestEnumerable<double> expected = MakeEnumerable(2d);
                MockFormatter.DoNotMatch(Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<Func<double, double, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, double.Epsilon));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<double> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<double>), null, msg => actual.ShouldMatch(new[] { 1d }, 1d, msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                double[] actual = { 1 };
                double[] expected = actual;

                actual.ShouldMatch(expected, double.Epsilon);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class ShouldStartWith : CollectionAssertionTests
        {
            [Test]
            public void StartsWithExpected_ReturnsActualValue()
            {
                int[] actual = { 1, 2, 3 };

                AssertReturnsActual(actual, () => actual.ShouldStartWith(new[] { 1, 2 }));
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldStartWith(Enumerable.Empty<object>(), msg));
            }

            [Test]
            public void ExpectedIsNull_FailsWithArgumentNullException()
            {
                AssertArgumentNullException("expectedStart", () => new[] { 1 }.ShouldStartWith((IEnumerable<int>)null));
            }

            [Test]
            public void DoesNotStartWithExpected_FailsWithDoesNotStartWithMessage()
            {
                int[] actual = { 1 };
                int[] expectedStart = { 2 };
                MockFormatter.DoesNotStartWith(Matches(expectedStart), Matches(actual), Compare.ObjectsAreEqual, "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldStartWith(expectedStart, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void DoesNotStartWithExpected_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                TestEnumerable<int> expected = MakeEnumerable(2);
                MockFormatter.DoesNotStartWith(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>(), Arg.Any<Func<object, object, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldStartWith(expected));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int[] expected = actual;

                actual.ShouldStartWith(expected);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class ShouldEndWith : CollectionAssertionTests
        {
            [Test]
            public void StartsWithExpected_ReturnsActualValue()
            {
                int[] actual = { 1, 2, 3 };

                AssertReturnsActual(actual, () => actual.ShouldEndWith(new[] { 2, 3 }));
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldEndWith(Enumerable.Empty<object>(), msg));
            }

            [Test]
            public void ExpectedIsNull_FailsWithArgumentNullException()
            {
                AssertArgumentNullException("expectedEnd", () => new[] { 1 }.ShouldEndWith((IEnumerable<int>)null));
            }

            [Test]
            public void DoesNotEndWithExpected_FailsWithDoesNotEndWithMessage()
            {
                int[] actual = { 1 };
                int[] expectedStart = { 2 };
                MockFormatter.DoesNotEndWith(Matches(expectedStart), Matches(actual), Compare.ObjectsAreEqual, "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldEndWith(expectedStart, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void DoesNotEndWithExpected_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                TestEnumerable<int> expected = MakeEnumerable(2);
                MockFormatter.DoesNotEndWith(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>(), Arg.Any<Func<object, object, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldEndWith(expected));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int[] expected = actual;

                actual.ShouldEndWith(expected);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class ShouldContain : CollectionAssertionTests
        {
            [Test]
            public void CollectionContainsExpected_ReturnsActualValue()
            {
                int[] actual = { 1, 2 };

                AssertReturnsActual(actual, () => actual.ShouldContain(2));
            }

            [Test]
            public void CollectionDoesNotContainExpected_FailsWithEnumerableDoesNotContainMessage()
            {
                int[] actual = { 1, 2 };
                const int expected = 3;
                MockFormatter.DoesNotContain(expected, Matches(actual), message: "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldContain(expected, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void CollectionDoesNotContainExpected_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                MockFormatter.DoesNotContain(Arg.Any<object>(), Arg.Any<IEnumerable>(), Arg.Any<string>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldContain(2));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldContain(1, msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int expected = 1;

                actual.ShouldContain(expected);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class ShouldNotContain : CollectionAssertionTests
        {
            [Test]
            public void CollectionDoesNotContainExpected_ReturnsActualValue()
            {
                IEnumerable<int> actual = Enumerable.Empty<int>();

                AssertReturnsActual(actual, () => actual.ShouldNotContain(1));
            }

            [Test]
            public void CollectionContainsItem_FailsWithEnumerableContainsItemMessage()
            {
                int[] actual = { 1 };
                MockFormatter.Contains(1, Matches(actual), message: "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotContain(1, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void CollectionContainsItem_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                MockFormatter.Contains(Arg.Any<object>(), Arg.Any<IEnumerable>(), Arg.Any<string>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotContain(1));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldNotContain(1, msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int notExpected = 2;

                actual.ShouldNotContain(notExpected);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(notExpected), TestExpression.GetExpected());
            }
        }

        class ShouldContainItems : CollectionAssertionTests
        {
            [Test]
            public void CollectionContainsAllItems_ReturnsActualValue()
            {
                int[] actual = { 1, 2, 3 };

                AssertReturnsActual(actual, () => actual.ShouldContainItems(new[] { 2, 3, 1 }));
            }

            [Test]
            public void CollectionDoesNotContainAllItems_FailsWithEnumerableDoesNotContainItemsMessage()
            {
                int[] actual = { 1, 2, 3 };
                int[] expected = { 1, 4 };
                MockFormatter.DoesNotContainItems(Matches(expected), Matches(actual), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldContainItems(expected, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void CollectionDoesNotContainAllItems_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                TestEnumerable<int> expected = MakeEnumerable(2);
                MockFormatter.DoesNotContainItems(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldContainItems(expected));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldContainItems(new[] { 1 }, msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int[] expected = actual;

                actual.ShouldContainItems(expected);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class ItemsShouldBeIn : CollectionAssertionTests
        {
            [Test]
            public void ExpectedContainsAllItems_ReturnsActualValue()
            {
                int[] actual = { 1, 2 };

                AssertReturnsActual(actual, () => actual.ItemsShouldBeIn(new[] { 3, 2, 1 }));
            }

            [Test]
            public void ContainsExtraItem_FailsWithCollectionContainsExtraItemMessage()
            {
                int[] actual = { 1, 2 };
                int[] expected = { 1 };
                MockFormatter.ContainsExtraItem(Matches(expected), Matches(actual), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ItemsShouldBeIn(expected, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void ContainsExtraItem_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                TestEnumerable<int> expected = MakeEnumerable(2);
                MockFormatter.ContainsExtraItem(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ItemsShouldBeIn(expected));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ItemsShouldBeIn(new[] { 1 }, msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expectedSuperset", () => new int[0].ItemsShouldBeIn((IEnumerable<int>)null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int[] expected = actual;

                actual.ItemsShouldBeIn(expected);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class ShouldNotContainItems : CollectionAssertionTests
        {
            [Test]
            public void CollectionDoesNotContainAnyOfTheItems_ReturnsActualValue()
            {
                int[] actual = { 1, 2 };

                AssertReturnsActual(actual, () => actual.ShouldNotContainItems(new[] { 3, 4 }));
            }

            [Test]
            public void CollectionContainsOneOfTheItems_FailsWithSequenceContainsItemMessage()
            {
                int[] actual = { 1, 2 };
                int[] expectedToNotContain = { 3, 2 };
                MockFormatter.Contains(Matches(expectedToNotContain), Matches(actual), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotContainItems(expectedToNotContain, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void CollectionContainsOneOfTheItems_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                TestEnumerable<int> expectedToNotContain = MakeEnumerable(1);
                MockFormatter.Contains(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotContainItems(expectedToNotContain));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expectedToNotContain.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldNotContainItems(new[] { 1 }, msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int[] expectedToNotContain = { 2 };

                actual.ShouldNotContainItems(expectedToNotContain);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedToNotContain), TestExpression.GetExpected());
            }
        }

        class ShouldOnlyContain : CollectionAssertionTests
        {
            [Test]
            public void CollectionsHaveSameItems_ReturnsActualValue()
            {
                int[] actual = { 1, 2 };

                AssertReturnsActual(actual, () => actual.ShouldOnlyContain(new[] { 2, 1 }));
            }

            [Test]
            public void MissingItem_FailsWithCollectionDoesNotOnlyContainMessage()
            {
                int[] actual = { 1, 2 };
                int[] expected = { 1, 3 };
                MockFormatter.DoesNotOnlyContain(Matches(expected), Matches(actual), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldOnlyContain(expected, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void ExtraItem_FailsWithCollectionDoesNotOnlyContainMessage()
            {
                int[] actual = { 1, 2 };
                int[] expected = { 1 };
                MockFormatter.DoesNotOnlyContain(Matches(expected), Matches(actual), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldOnlyContain(expected, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void DifferentItems_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                TestEnumerable<int> expected = MakeEnumerable(2);
                MockFormatter.DoesNotOnlyContain(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldOnlyContain(expected));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void HasDuplicate_FailsWithContainsDuplicateMessage()
            {
                int[] actual = { 1, 1 };
                MockFormatter.ContainsDuplicate(Matches(actual), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldOnlyContain(new[] { 1 }, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void HasDuplicate_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1, 1);
                TestEnumerable<int> expected = MakeEnumerable(1);
                MockFormatter.ContainsDuplicate(Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldOnlyContain(expected));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldOnlyContain(new[] { 1 }, msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int[] expected = actual;

                actual.ShouldOnlyContain(expected);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class ShouldNotOnlyContain : CollectionAssertionTests
        {
            [Test]
            public void ActualHasExpectedPlusMore_ReturnsActualValue()
            {
                int[] actual = { 1, 2 };

                AssertReturnsActual(actual, () => actual.ShouldNotOnlyContain(new[] { 2 }));
            }

            [Test]
            public void ActualOnlyHasItemsInExpected_FailsWithCollectionOnlyContainsMessage()
            {
                int[] actual = { 1, 2 };
                int[] expected = { 2, 1, 3 };
                MockFormatter.OnlyContains(Matches(expected), Matches(actual), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotOnlyContain(expected, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void ActualOnlyhasItemsInExpected_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                TestEnumerable<int> expected = MakeEnumerable(1);
                MockFormatter.OnlyContains(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotOnlyContain(expected));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldNotOnlyContain(new[] { 1 }, msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expected", () => new[] { 1 }.ShouldNotOnlyContain((int[])null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int[] expected = { 2 };

                actual.ShouldNotOnlyContain(expected);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class ShouldBeDistinct : CollectionAssertionTests
        {
            [Test]
            public void NoDuplicates_ReturnsActual()
            {
                int[] actual = { 1, 2, 3 };

                AssertReturnsActual(actual, () => actual.ShouldBeDistinct());
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;

                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldBeDistinct(msg));
            }

            [Test]
            public void ContainsDuplicate_FailsWithContainsDuplicateMessage()
            {
                int[] actual = { 1, 2, 1 };
                MockFormatter.ContainsDuplicate(Matches(actual), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeDistinct("foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void ContainsDuplicate_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1, 1);
                MockFormatter.ContainsDuplicate(Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeDistinct());

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };

                actual.ShouldBeDistinct();

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
            }
        }

        class ShouldMatchReferences : CollectionAssertionTests
        {
            [Test]
            public void SameItems_ReturnsActualValue()
            {
                object a = new object();
                object b = new object();
                object[] actual = { a, b };

                AssertReturnsActual(actual, () => actual.ShouldMatchReferences(new[] { a, b }));
            }

            [Test]
            public void DifferentLength_FailsWithEnumerableLengthMismatchMessage()
            {
                object[] actual = { };
                object[] expected = { new object() };
                MockFormatter.LengthMismatch(1, Matches(actual), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatchReferences(expected, "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void DifferentLength_OnlyEnumeratesOnce()
            {
                TestEnumerable<object> actual = MakeEnumerable(new object());
                TestEnumerable<object> expected = MakeEnumerable(new object(), new object());
                MockFormatter.LengthMismatch(Arg.Any<int>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatchReferences(expected));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void DifferentItems_FailsWithEnumerablesNotSameMessage()
            {
                object a = new object();
                object[] actual = { a, new object() };
                object[] expected = { a, new object() };
                MockFormatter.ItemsNotSame(Matches(expected), Matches(actual), "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatchReferences(expected, "foo"));

                Assert.AreSame("bar", result.Message);
            }

            [Test]
            public void DifferentItems_OnlyEnumeratesOnce()
            {
                TestEnumerable<object> actual = MakeEnumerable(new object());
                TestEnumerable<object> expected = MakeEnumerable(new object());
                MockFormatter.ItemsNotSame(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatchReferences(expected));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<object> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<object>), null, msg => actual.ShouldMatchReferences(new[] { new object() }, msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                object[] actual = { new object() };
                object[] expected = actual;

                actual.ShouldMatchReferences(expected);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class KeyedCollection_ShouldContainKey : CollectionAssertionTests
        {
            [Test]
            public void ContainsKeys_ReturnsActualValue()
            {
                TestKeyedCollection actual = new TestKeyedCollection { "foo", "bar" };

                AssertReturnsActual(actual, () => actual.ShouldContainKey('b'));
            }

            [Test]
            public void DoesNotContainKey_FailsWithDoesNotContainKeyMessage()
            {
                TestKeyedCollection actual = new TestKeyedCollection();
                MockFormatter.DoesNotContain('a', actual, "key", "foo").Returns("bar");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldContainKey('a', "foo"));

                Assert.AreEqual("bar", result.Message);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                TestKeyedCollection actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(KeyedCollection<char, string>), null, msg => actual.ShouldContainKey('a', msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                TestKeyedCollection actual = new TestKeyedCollection { "foo" };
                char expected = 'f';

                actual.ShouldContainKey(expected);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class KeyedCollection_ShouldNotContainKey : CollectionAssertionTests
        {
            [Test]
            public void DoesNotContainKey_ReturnsActualValue()
            {
                TestKeyedCollection actual = new TestKeyedCollection();

                AssertReturnsActual(actual, () => actual.ShouldNotContainKey('a'));
            }

            [Test]
            public void ContainsKey_FailsWithContainsKeyMessage()
            {
                TestKeyedCollection actual = new TestKeyedCollection { "foo" };
                MockFormatter.Contains('f', actual, "key", "bar").Returns("baz");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotContainKey('f', "bar"));

                Assert.AreEqual("baz", result.Message);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                TestKeyedCollection actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(KeyedCollection<char, string>), null, msg => actual.ShouldNotContainKey('a', msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                TestKeyedCollection actual = new TestKeyedCollection { "foo" };
                char notExpected = 'a';

                actual.ShouldNotContainKey(notExpected);

                Assert.AreEqual(nameof(actual), TestExpression.GetActual());
                Assert.AreEqual(nameof(notExpected), TestExpression.GetExpected());
            }
        }

        class ItemsSatisfy : CollectionAssertionTests
        {
            [Test]
            public void ItemsSatisfyAssertions_ReturnsActualValue()
            {
                int[] actual = { 1 };

                AssertReturnsActual(actual, () => actual.ItemsSatisfy(i => { }));
            }

            [Test]
            public void CallsAssertionsWithMatchedItem()
            {
                bool gotFirstItem = false;
                bool gotSecondItem = false;

                new[] { 1, 2 }.ItemsSatisfy(
                    i => gotFirstItem = i == 1,
                    i => gotSecondItem = i == 2);

                Assert.IsTrue(gotFirstItem, "first item");
                Assert.IsTrue(gotSecondItem, "second item");
            }

            [Test]
            public void WrongNumberOfItems_FailsWithEnumerableLengthMismatchMessage()
            {
                int[] actual = { 1 };
                MockFormatter.LengthMismatch(2, Matches(actual)).Returns("foo");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ItemsSatisfy(i => { }, i => { }));

                Assert.AreEqual("foo", result.Message);
            }

            [Test]
            public void WrongNumberOfItems_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1, 2);
                MockFormatter.LengthMismatch(Arg.Any<int>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ItemsSatisfy(i => i.ShouldBe(1)));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void ItemDoesNotSatisfyItsAssertion_FailsWithThrownException()
            {
                int[] actual = { 1, 2 };
                Exception failure = new Exception("foo");

                Exception result = Assert.Throws<Exception>(() => actual.ItemsSatisfy(
                    i => { },
                    i => { throw failure; }));

                Assert.AreSame(failure, result);
            }

            [Test]
            public void ItemDoesNotSatisfyItsAssertion_OnlyEnumeratesOnce()
            {
                TestEnumerable<int> actual = MakeEnumerable(1);
                MockFormatter.NotEqual(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<string>()).Returns(EnumerateArgs);

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ItemsSatisfy(i => i.ShouldBe(2)));

                Assert.AreEqual(EnumerateArgsResult, result.Message);
                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                MockFormatter.NotEqual(typeof(IEnumerable<int>), null).Returns("foo");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ItemsSatisfy(i => { }));

                Assert.AreEqual("foo", result.Message);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int expected = 2;

                Assert.Throws<EasyAssertionException>(() => actual.ItemsSatisfy(item => item.ShouldBe(expected)));

                Assert.AreEqual($"{nameof(actual)}[0]", TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        class AllItemsSatisfy : CollectionAssertionTests
        {
            [Test]
            public void AllItemsSatisfyAssertion_ReturnsActualValue()
            {
                int[] actual = { 1 };

                AssertReturnsActual(actual, () => actual.AllItemsSatisfy(i => { }));
            }

            [Test]
            public void ItemsDoNotSatisfyAssertion_FailsWithThrownException()
            {
                int[] actual = { 1, 2 };
                Exception failure = new Exception("foo");

                Exception result = Assert.Throws<Exception>(() => actual.AllItemsSatisfy(i =>
                {
                    if (i > 1) throw failure;
                }));

                Assert.AreSame(failure, result);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                MockFormatter.NotEqual(typeof(IEnumerable<int>), null).Returns("foo");

                EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.AllItemsSatisfy(i => { }));

                Assert.AreEqual("foo", result.Message);
            }

            [Test]
            public void AssertionIsNull_FailsWithArgumentNullException()
            {
                ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => new int[0].AllItemsSatisfy(null));

                Assert.AreEqual("assertion", result.ParamName);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actual = { 1 };
                int expected = 2;

                Assert.Throws<EasyAssertionException>(() => actual.AllItemsSatisfy(item => item.ShouldBe(expected)));

                Assert.AreEqual($"{nameof(actual)}[0]", TestExpression.GetActual());
                Assert.AreEqual(nameof(expected), TestExpression.GetExpected());
            }
        }

        private static TestEnumerable<T> MakeEnumerable<T>(params T[] items) => new TestEnumerable<T>(items);

        private static readonly string EnumerateArgsResult = "foo";

        private static string EnumerateArgs(CallInfo call)
        {
            foreach (IEnumerable enumerable in call.Args().OfType<IEnumerable>())
                enumerable.Cast<object>().ToList();
            return EnumerateArgsResult;
        }

        private class TestKeyedCollection : KeyedCollection<char, string>
        {
            protected override char GetKeyForItem(string item)
            {
                return item[0];
            }
        }
    }
}