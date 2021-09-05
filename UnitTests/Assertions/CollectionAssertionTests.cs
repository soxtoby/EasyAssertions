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
        /* // None of the lines in the following method should compile
        public void ShouldNotCompile()
        {
            new Dictionary<string, string>().ShouldMatch(new Dictionary<string, string>().AsEnumerable());
            new Dictionary<string, string>().ShouldMatch(new Dictionary<string, string>().AsEnumerable(), "message");
            new Dictionary<string, string>().AsEnumerable().ShouldMatch(new Dictionary<string, string>());
            new Dictionary<string, string>().AsEnumerable().ShouldMatch(new Dictionary<string, string>(), "message");
            new HashSet<string>().ShouldMatch(new[] { "foo" });
            new HashSet<string>().ShouldMatch(new[] { "foo" }, "message");
            new[] { "foo" }.ShouldMatch(new HashSet<string>());
            new[] { "foo" }.ShouldMatch(new HashSet<string>(), "message");
            new Dictionary<string, string>().ShouldMatchReferences(new Dictionary<string, string>().AsEnumerable());
            new Dictionary<string, string>().ShouldMatchReferences(new Dictionary<string, string>().AsEnumerable(), "message");
            new Dictionary<string, string>().AsEnumerable().ShouldMatchReferences(new Dictionary<string, string>());
            new Dictionary<string, string>().AsEnumerable().ShouldMatchReferences(new Dictionary<string, string>(), "message");
            new HashSet<string>().ShouldMatchReferences(new[] { "foo" });
            new HashSet<string>().ShouldMatchReferences(new[] { "foo" }, "message");
            new[] { "foo" }.ShouldMatchReferences(new HashSet<string>());
            new[] { "foo" }.ShouldMatchReferences(new HashSet<string>(), "message");
            new[] { "foo" }.ItemsSatisfy(v => v == "foo");
            new[] { "foo" }.AllItemsSatisfy(v => v == "foo");
        }/**/

        class ShouldBeEmpty : CollectionAssertionTests
        {
            [Test]
            public void IsEmpty_ReturnsActualValue()
            {
                var actual = Enumerable.Empty<object>();

                AssertReturnsActual(actual, () => actual.ShouldBeEmpty());
            }

            [Test]
            public void NonEmpty_FailsWithEnumerableNotEmptyMessage()
            {
                int[] actual = { 1 };
                Error.NotEmpty(Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldBeEmpty("foo"));
            }

            [Test]
            public void NonEmpty_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1);
                Error.NotEmpty(Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldBeEmpty());

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
                var actualExpression = Enumerable.Empty<string>();

                actualExpression.ShouldBeEmpty();

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
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
                var actual = Enumerable.Empty<object>();
                Error.IsEmpty("foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotBeEmpty("foo"));
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
                int[] actualExpression = { 1 };

                actualExpression.ShouldNotBeEmpty();

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
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
                Error.LengthMismatch(1, Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldBeSingular("foo"));
            }

            [Test]
            public void IsNotSingular_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1, 2);
                Error.LengthMismatch(Arg.Any<int>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldBeSingular());

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
                int[] actualExpression = { 1 };

                actualExpression.ShouldBeSingular();

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            }
        }

        class ShouldBeASingular : CollectionAssertionTests
        {
            [Test]
            public void IsSingularItemWithExpectedType_ReturnsActualValue()
            {
                Equatable actualItem = new SubEquatable(1);
                AssertReturnsActual(actualItem, () => new[] { actualItem }.ShouldBeASingular<Equatable>());
            }

            [Test]
            public void IsNotSingular_FailsWithLengthMismatchMessage()
            {
                IEnumerable actual = new[] { 1, 2 };
                Error.LengthMismatch(1, Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldBeASingular<int>("foo"));
            }

            [Test]
            public void IsSingularItemWithWrongType_FailsWithTypesNotEqualMessage()
            {
                IEnumerable actual = new[] { 1.2 };

                AssertFailsWithTypesNotEqualMessage(typeof(int), typeof(double), msg => actual.ShouldBeASingular<int>(msg));
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable actual = null;

                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable), null, msg => actual.ShouldBeASingular<int>(msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                IEnumerable actualExpression = new[] { 1, 2 };
                Error.LengthMismatch(Arg.Any<int>(), Arg.Any<IEnumerable>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldBeASingular<int>());

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            }
        }

        class ShouldAllBeA : CollectionAssertionTests
        {
            [Test]
            public void AllItemsAreCorrectType_ReturnsTypedEnumerable()
            {
                IEnumerable actual = new[] { new SubEquatable(1), new SubEquatable(2) };

                var result = actual.ShouldAllBeA<Equatable>();

                CollectionAssert.AreEqual(actual, result.And);
            }

            [Test]
            public void NotAllItemsAreCorrectType_FailsWithTypesNotEqualMessage()
            {
                IEnumerable actual = new[] { Equatable(1), new object() };

                AssertFailsWithTypesNotEqualMessage(typeof(Equatable), typeof(object), msg => actual.ShouldAllBeA<Equatable>(msg));
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable actual = null;

                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable), null, msg => actual.ShouldAllBeA<Equatable>(msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                IEnumerable actualExpression = new[] { Equatable(1), new object() };

                AssertFailsWithTypesNotEqualMessage(typeof(Equatable), typeof(object), msg => actualExpression.ShouldAllBeA<Equatable>(msg));

                Assert.AreEqual($"{nameof(actualExpression)}[1]", TestExpression.GetActual());
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
                Error.LengthMismatch(2, Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldBeLength(2, "foo"));
            }

            [Test]
            public void IsDifferentLength_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1);
                Error.LengthMismatch(Arg.Any<int>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldBeLength(2));

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
                int[] actualExpression = { 1, 2, 3 };
                var expectedExpression = 2;
                Error.LengthMismatch(Arg.Any<int>(), Arg.Any<IEnumerable>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldBeLength(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
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
                Error.DoNotMatch(Matches<int>(expected), Matches<int>(actual), StandardTests.Instance.ObjectsMatch, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldMatch(expected, "foo"));
            }

            [Test]
            public void NonMatchingEnumerables_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1);
                var expected = MakeEnumerable(2);
                Error.DoNotMatch(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldMatch(expected));

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
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                string[] actual = { };

                var result = Assert.Throws<ArgumentNullException>(() => actual.ShouldMatch((IEnumerable<string>)null, "foo"));

                Assert.AreEqual("expected", result.ParamName);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] expectedExpression = { 3, 4 };
                int[] actualExpression = { 1, 2 };
                Error.DoNotMatch(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldMatch(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
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
                Error.DoNotMatch(Matches<int>(expected), Matches<int>(actual), predicate, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldMatch(expected, predicate, "foo"));
            }

            [Test]
            public void NonMatchingEnumerables_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1);
                var expected = MakeEnumerable(2);
                Error.DoNotMatch(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldMatch(expected, (a, b) => a == b));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<string>), null, msg => ((IEnumerable<string>)null).ShouldMatch(Enumerable.Empty<string>(), (s, s1) => false, msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expected", () => new Equatable[0].ShouldMatch((IEnumerable<int>)null, ValueEquals));
            }

            [Test]
            public void PredicateIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("predicate", () => new Equatable[0].ShouldMatch(new int[0], (Func<Equatable, int, bool>)null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actualExpression = { 1, 2, 4 };
                int[] expectedExpression = { 2, 4, 6 };
                Func<int, int, bool> predicate = (a, e) => a == e / 2;
                Error.DoNotMatch(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldMatch(expectedExpression, predicate));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldMatch_Floats : CollectionAssertionTests
        {
            [Test]
            public void WithinTolerance_ReturnsActualValue()
            {
                float[] actual = { 10f, 20f };

                AssertReturnsActual(actual, () => actual.ShouldMatch(new[] { 11f, 21f }, 1f));
            }

            [Test]
            public void OutsideTolerance_FailsWithEnumerablesDoNotMatchMessage()
            {
                float[] actual = { 10f, 20f };
                float[] expected = { 11f, 21f };
                Func<float, float, bool> predicate = null;
                Error.DoNotMatch(Matches<float>(expected), Matches<float>(actual), Arg.Do<Func<float, float, bool>>(p => predicate = p), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldMatch(expected, 0.9f, "foo"));

                Assert.IsTrue(predicate(1f, 1.9f));
                Assert.IsFalse(predicate(1f, 2f));
            }

            [Test]
            public void OutsideTolerance_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1f);
                var expected = MakeEnumerable(2f);
                Error.DoNotMatch(Arg.Any<IEnumerable<float>>(), Arg.Any<IEnumerable<float>>(), Arg.Any<Func<float, float, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldMatch(expected, float.Epsilon));

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
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expected", () => new float[0].ShouldMatch(null, 1));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                float[] actualExpression = { 10f, 20f };
                float[] expectedExpression = { 11f, 21f };
                Error.DoNotMatch(Arg.Any<IEnumerable<float>>(), Arg.Any<IEnumerable<float>>(), Arg.Any<Func<float, float, bool>>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldMatch(expectedExpression, 0.9f));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldMatch_Doubles : CollectionAssertionTests
        {
            [Test]
            public void WithinTolerance_ReturnsActualValue()
            {
                double[] actual = { 10d, 20d };

                AssertReturnsActual(actual, () => actual.ShouldMatch(new[] { 11d, 21d }, 1d));
            }

            [Test]
            public void OutsideTolerance_FailsWithEnumerablesDoNotMatchMessage()
            {
                double[] actual = { 10d, 20d };
                double[] expected = { 11d, 21d };
                Func<double, double, bool> predicate = null;
                Error.DoNotMatch(Matches<double>(expected), Matches<double>(actual), Arg.Do<Func<double, double, bool>>(p => predicate = p), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldMatch(expected, 0.9d, "foo"));

                Assert.IsTrue(predicate(1d, 1.9d));
                Assert.IsFalse(predicate(1d, 2d));
            }

            [Test]
            public void OutsideTolerance_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1d);
                var expected = MakeEnumerable(2d);
                Error.DoNotMatch(Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<Func<double, double, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldMatch(expected, double.Epsilon));

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
            public void ExpectedIsNull_ThrowArgumentNullException()
            {
                AssertArgumentNullException("expected", () => new double[0].ShouldMatch(null, 1));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                double[] actualExpression = { 10d, 20d };
                double[] expectedExpression = { 11d, 21d };
                Error.DoNotMatch(Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<Func<double, double, bool>>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldMatch(expectedExpression, 0.9d));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldMatch_Assertion : CollectionAssertionTests
        {
            [Test]
            public void MatchingEnumerable_ReturnsActualValue()
            {
                IEnumerable<int> actual = new[] { 1, 2 };

                AssertReturnsActual(actual, () => actual.ShouldMatch(new[] { 1, 2 }, (a, e) => a.ShouldBe(e)));
            }

            [Test]
            public void NonMatchingEnumerables_FailsWithSpecificError()
            {
                int[] actual = { 1, 2, 4 };
                int[] expected = { 2, 4, 6 };
                Error.NotEqual(3, 4).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldMatch(expected, (a, e) => a.ShouldBe(e / 2)));
            }

            [Test]
            public void MatchingEnumerables_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1);
                var expected = MakeEnumerable(1);

                actual.ShouldMatch(expected, (a, b) => a.ShouldBe(b));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                Error.NotEqual(typeof(IEnumerable<string>), null).Returns(ExpectedException);

                AssertThrowsExpectedError(() => ((IEnumerable<string>?)null).ShouldMatch(Enumerable.Empty<string>(), (a, e) => a.ShouldBe(e)));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expected", () => new Equatable[0].ShouldMatch((IEnumerable<int>)null, (a, e) => a.Value.ShouldBe(e)));
            }

            [Test]
            public void AssertionIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("assertion", () => new Equatable[0].ShouldMatch(new int[0], (Action<Equatable, int>?)null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actualExpression = { 1, 2, 4 };
                int[] expectedExpression = { 2, 4, 6 };
                Error.NotEqual(3, 4).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldMatch(expectedExpression, (a, e) => a.ShouldBe(e / 2)));

                Assert.AreEqual($"{nameof(actualExpression)}[2]", TestExpression.GetActual());
                Assert.AreEqual($"{nameof(expectedExpression)}[2] / 2", TestExpression.GetExpected());
            }
        }

        class ShouldNotMatch : CollectionAssertionTests
        {
            [Test]
            public void NonMatchingEnumerables_ReturnsActual()
            {
                IEnumerable<Equatable> actual = new[] { Equatable(1), Equatable(2), Equatable(3) };
                IEnumerable<Equatable> notExpected = new[] { Equatable(1), Equatable(2), Equatable(4) };

                AssertReturnsActual(actual, () => actual.ShouldNotMatch(notExpected));
            }

            [Test]
            public void MatchingEnumerables_FailsWithCollectionsMatchMessage()
            {
                IEnumerable<Equatable> actual = new[] { Equatable(1), Equatable(2), Equatable(3) };
                var notExpected = actual;
                Error.Matches(Matches(notExpected), Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotMatch(notExpected, "foo"));
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;

                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldNotMatch(new[] { 1 }, msg));
            }

            [Test]
            public void NotExpectedIsNull_ThrowsArgumentNullException()
            {
                IEnumerable<int> actual = new[] { 1 };

                AssertArgumentNullException("notExpected", () => actual.ShouldNotMatch((IEnumerable<int>)null));
            }

            [Test]
            public void MatchingEnumerables_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1);
                var notExpected = MakeEnumerable(1);
                Error.Matches(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldNotMatch(notExpected));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, notExpected.EnumerationCount);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                IEnumerable<int> actualExpression = new[] { 1, 2, 3 };
                var expectedExpression = actualExpression;
                Error.Matches(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldNotMatch(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldNotMatch_Floats : CollectionAssertionTests
        {
            [Test]
            public void OutsideTolerance_ReturnsActual()
            {
                IEnumerable<float> actual = new[] { 1f, 2f };
                IEnumerable<float> notExpected = new[] { 1f, 5f };

                AssertReturnsActual(actual, () => actual.ShouldNotMatch(notExpected, 1));
            }

            [Test]
            public void WithinTolerance_FailsWithCollectionsMatchMessage()
            {
                IEnumerable<float> actual = new[] { 10f, 20f };
                IEnumerable<float> notExpected = new[] { 9f, 21f };
                Error.Matches(Matches(notExpected), Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotMatch(notExpected, 1, "foo"));
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<float> actual = null;

                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<float>), null, msg => actual.ShouldNotMatch(new[] { 1f }, 1, msg));
            }

            [Test]
            public void NotExpectedIsNull_ThrowsArgumentNullException()
            {
                IEnumerable<float> actual = new[] { 1f };

                AssertArgumentNullException("notExpected", () => actual.ShouldNotMatch(null, 1));
            }

            [Test]
            public void MatchingEnumerables_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1f);
                var notExpected = MakeEnumerable(1f);
                Error.Matches(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldNotMatch(notExpected, 1));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, notExpected.EnumerationCount);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                IEnumerable<float> actualExpression = new[] { 1f };
                var expectedExpression = actualExpression;
                Error.Matches(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldNotMatch(expectedExpression, 1));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldNotMatch_Doubles : CollectionAssertionTests
        {
            [Test]
            public void OutsideTolerance_ReturnsActual()
            {
                IEnumerable<double> actual = new[] { 1d, 2d };
                IEnumerable<double> notExpected = new[] { 1d, 5d };

                AssertReturnsActual(actual, () => actual.ShouldNotMatch(notExpected, 1));
            }

            [Test]
            public void WithinTolerance_FailsWithCollectionsMatchMessage()
            {
                IEnumerable<double> actual = new[] { 10d, 20d };
                IEnumerable<double> notExpected = new[] { 9d, 21d };
                Error.Matches(Matches(notExpected), Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotMatch(notExpected, 1, "foo"));
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<double> actual = null;

                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<double>), null, msg => actual.ShouldNotMatch(new[] { 1d }, 1, msg));
            }

            [Test]
            public void NotExpectedIsNull_ThrowsArgumentNullException()
            {
                IEnumerable<double> actual = new[] { 1d };

                AssertArgumentNullException("notExpected", () => actual.ShouldNotMatch(null, 1));
            }

            [Test]
            public void MatchingEnumerables_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1d);
                var notExpected = MakeEnumerable(1d);
                Error.Matches(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldNotMatch(notExpected, 1));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, notExpected.EnumerationCount);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                IEnumerable<double> actualExpression = new[] { 1d };
                var expectedExpression = actualExpression;
                Error.Matches(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldNotMatch(expectedExpression, 1));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldNotMatch_CustomEquality : CollectionAssertionTests
        {
            [Test]
            public void NonMatchingEnumerables_ReturnsActual()
            {
                IEnumerable<Equatable> actual = new[] { Equatable(1), Equatable(2), Equatable(3) };
                IEnumerable<int> notExpected = new[] { 1, 2, 4 };

                AssertReturnsActual(actual, () => actual.ShouldNotMatch(notExpected, ValueEquals));
            }

            [Test]
            public void MatchingEnumerables_FailsWithCollectionsMatchMessage()
            {
                IEnumerable<Equatable> actual = new[] { Equatable(1), Equatable(2), Equatable(3) };
                IEnumerable<int> notExpected = new[] { 1, 2, 3 };
                Error.Matches(Matches(notExpected), Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotMatch(notExpected, ValueEquals, "foo"));
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;

                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldNotMatch(new[] { 1 }, (a, e) => a == e, msg));
            }

            [Test]
            public void NotExpectedIsNull_ThrowsArgumentNullException()
            {
                IEnumerable<int> actual = new[] { 1 };

                AssertArgumentNullException("notExpected", () => actual.ShouldNotMatch((IEnumerable<int>)null, (a, e) => a == e));
            }

            [Test]
            public void PredicateIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("predicate", () => new int[0].ShouldNotMatch(new[] { 1 }, (Func<int, int, bool>)null));
            }

            [Test]
            public void MatchingEnumerables_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1);
                var notExpected = MakeEnumerable(1);
                Error.Matches(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldNotMatch(notExpected, (a, e) => a == e));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, notExpected.EnumerationCount);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                IEnumerable<int> actualExpression = new[] { 1, 2, 3 };
                var expectedExpression = actualExpression;
                Error.Matches(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldNotMatch(expectedExpression, (a, e) => a == e));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
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
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldStartWith(Enumerable.Empty<int>(), msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expectedStart", () => new[] { 1 }.ShouldStartWith((IEnumerable<int>)null));
            }

            [Test]
            public void DoesNotStartWithExpected_FailsWithDoesNotStartWithMessage()
            {
                int[] actual = { 1 };
                int[] expectedStart = { 2 };
                Error.DoesNotStartWith(Matches<int>(expectedStart), Matches<int>(actual), StandardTests.Instance.ObjectsAreEqual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldStartWith(expectedStart, "foo"));
            }

            [Test]
            public void DoesNotStartWithExpected_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1);
                var expected = MakeEnumerable(2);
                Error.DoesNotStartWith(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldStartWith(expected));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actualExpression = { 1 };
                int[] expectedExpression = { 2 };
                Error.DoesNotStartWith(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldStartWith(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldStartWith_CustomEquality : CollectionAssertionTests
        {
            [Test]
            public void StartsWithExpected_ReturnsActualValue()
            {
                Equatable[] actual = { Equatable(1), Equatable(2), Equatable(3) };

                AssertReturnsActual(actual, () => actual.ShouldStartWith(new[] { 1, 2 }, ValueEquals));
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldStartWith(Enumerable.Empty<int>(), (a, e) => a == e, msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expectedStart", () => new[] { 1 }.ShouldStartWith((IEnumerable<int>)null, (a, e) => a == e));
            }

            [Test]
            public void PredicateIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("predicate", () => new[] { 1 }.ShouldStartWith(new[] { 1 }, (Func<int, int, bool>)null));
            }

            [Test]
            public void DoesNotStartWithExpected_FailsWithDoesNotStartWithMessage()
            {
                Equatable[] actual = { Equatable(1), };
                int[] expectedStart = { 2 };
                Func<Equatable, int, bool> predicate = ValueEquals;
                Error.DoesNotStartWith(Matches<int>(expectedStart), Matches<Equatable>(actual), predicate, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldStartWith(expectedStart, predicate, "foo"));
            }

            [Test]
            public void DoesNotStartWithExpected_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1);
                var expected = MakeEnumerable(2);
                Error.DoesNotStartWith(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldStartWith(expected, (a, e) => a == e));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actualExpression = { 1 };
                int[] expectedExpression = { 2 };
                Error.DoesNotStartWith(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldStartWith(expectedExpression, (a, e) => a == e));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
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
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldEndWith(Enumerable.Empty<int>(), msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expectedEnd", () => new[] { 1 }.ShouldEndWith((IEnumerable<int>)null));
            }

            [Test]
            public void DoesNotEndWithExpected_FailsWithDoesNotEndWithMessage()
            {
                int[] actual = { 1 };
                int[] expectedStart = { 2 };
                Error.DoesNotEndWith(Matches<int>(expectedStart), Matches<int>(actual), StandardTests.Instance.ObjectsAreEqual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldEndWith(expectedStart, "foo"));
            }

            [Test]
            public void DoesNotEndWithExpected_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1);
                var expected = MakeEnumerable(2);
                Error.DoesNotEndWith(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldEndWith(expected));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actualExpression = { 1 };
                int[] expectedExpression = { 2 };
                Error.DoesNotEndWith(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldEndWith(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldEndWith_CustomEquality : CollectionAssertionTests
        {
            [Test]
            public void StartsWithExpected_ReturnsActualValue()
            {
                Equatable[] actual = { Equatable(1), Equatable(2), Equatable(3) };

                AssertReturnsActual(actual, () => actual.ShouldEndWith(new[] { 2, 3 }, ValueEquals));
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<int>), null, msg => actual.ShouldEndWith(Enumerable.Empty<int>(), (a, e) => a == e, msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expectedEnd", () => new[] { 1 }.ShouldEndWith((IEnumerable<int>)null, (a, e) => a == e));
            }

            [Test]
            public void PredicateIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("predicate", () => new[] { 1 }.ShouldEndWith(new[] { 1 }, (Func<int, int, bool>)null));
            }

            [Test]
            public void DoesNotEndWithExpected_FailsWithDoesNotEndWithMessage()
            {
                Equatable[] actual = { Equatable(1) };
                int[] expectedStart = { 2 };
                Func<Equatable, int, bool> predicate = ValueEquals;
                Error.DoesNotEndWith(Matches<int>(expectedStart), Matches<Equatable>(actual), predicate, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldEndWith(expectedStart, predicate, "foo"));
            }

            [Test]
            public void DoesNotEndWithExpected_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1);
                var expected = MakeEnumerable(2);
                Error.DoesNotEndWith(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldEndWith(expected, (a, e) => a == e));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actualExpression = { 1 };
                int[] expectedExpression = { 2 };
                Error.DoesNotEndWith(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldEndWith(expectedExpression, (a, e) => a == e));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldContain : CollectionAssertionTests
        {
            [Test]
            public void CollectionContainsExpected_ReturnsActualValue()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };

                AssertReturnsActual(actual, () => actual.ShouldContain(Equatable(2)));
            }

            [Test]
            public void CollectionDoesNotContainExpected_FailsWithEnumerableDoesNotContainMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };
                var expected = Equatable(3);
                Error.DoesNotContain(expected, Matches(actual), message: "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldContain(expected, "foo"));
            }

            [Test]
            public void CollectionDoesNotContainExpected_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1);
                Error.DoesNotContain(Arg.Any<object>(), Arg.Any<IEnumerable>(), Arg.Any<string>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldContain(2));

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
                int[] actualExpression = { 1, 2 };
                const int expectedExpression = 3;
                Error.DoesNotContain(Arg.Any<object>(), Arg.Any<IEnumerable>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldContain(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldContain_CustomEquality : CollectionAssertionTests
        {
            [Test]
            public void CollectionContainsExpected_ReturnsActualValue()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };

                AssertReturnsActual(actual, () => actual.ShouldContain(2, ValueEquals));
            }

            [Test]
            public void CollectionDoesNotContainExpected_FailsWithEnumerableDoesNotContainMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };
                var expected = 3;
                Error.DoesNotContain(expected, Matches<Equatable>(actual), message: "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldContain(expected, ValueEquals, "foo"));
            }

            [Test]
            public void CollectionDoesNotContainExpected_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(Equatable(1));
                Error.DoesNotContain(Arg.Any<object>(), Arg.Any<IEnumerable>(), Arg.Any<string>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldContain(2, ValueEquals));

                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<Equatable> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<Equatable>), null, msg => actual.ShouldContain(1, ValueEquals, msg));
            }

            [Test]
            public void PredicateIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("predicate", () => new Equatable[0].ShouldContain(1, (Func<Equatable, int, bool>)null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                Equatable[] actualExpression = { Equatable(1), Equatable(2) };
                const int expectedExpression = 3;
                Error.DoesNotContain(Arg.Any<object>(), Arg.Any<IEnumerable>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldContain(expectedExpression, ValueEquals));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldNotContain : CollectionAssertionTests
        {
            [Test]
            public void CollectionDoesNotContainExpected_ReturnsActualValue()
            {
                var actual = Enumerable.Empty<Equatable>();

                AssertReturnsActual(actual, () => actual.ShouldNotContain(Equatable(1)));
            }

            [Test]
            public void CollectionContainsItem_FailsWithEnumerableContainsItemMessage()
            {
                Equatable[] actual = { Equatable(1) };
                Error.Contains(Equatable(1), Matches(actual), message: "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotContain(Equatable(1), "foo"));
            }

            [Test]
            public void CollectionContainsItem_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(Equatable(1));
                Error.Contains(Arg.Any<object>(), Arg.Any<IEnumerable>(), Arg.Any<string>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldNotContain(Equatable(1)));

                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<Equatable> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<Equatable>), null, msg => actual.ShouldNotContain(Equatable(1), msg));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                Equatable[] actualExpression = { Equatable(1) };
                var expectedExpression = Equatable(1);
                Error.Contains(Arg.Any<object>(), Arg.Any<IEnumerable>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldNotContain(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldNotContain_CustomEquality : CollectionAssertionTests
        {
            [Test]
            public void CollectionDoesNotContainExpected_ReturnsActualValue()
            {
                var actual = Enumerable.Empty<Equatable>();

                AssertReturnsActual(actual, () => actual.ShouldNotContain(1, ValueEquals));
            }

            [Test]
            public void CollectionContainsItem_FailsWithEnumerableContainsItemMessage()
            {
                Equatable[] actual = { Equatable(1) };
                Error.Contains(1, Matches(actual), message: "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotContain(1, ValueEquals, "foo"));
            }

            [Test]
            public void CollectionContainsItem_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(Equatable(1));
                Error.Contains(Arg.Any<object>(), Arg.Any<IEnumerable>(), Arg.Any<string>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldNotContain(1, ValueEquals));

                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<Equatable> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<Equatable>), null, msg => actual.ShouldNotContain(1, ValueEquals, msg));
            }

            [Test]
            public void PredicateIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("predicate", () => new Equatable[0].ShouldNotContain(1, (Func<Equatable, int, bool>)null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                Equatable[] actualExpression = { Equatable(1) };
                var expectedExpression = 1;
                Error.Contains(Arg.Any<object>(), Arg.Any<IEnumerable>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldNotContain(expectedExpression, ValueEquals));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldContainItems : CollectionAssertionTests
        {
            [Test]
            public void CollectionContainsAllItems_ReturnsActualValue()
            {
                Equatable[] actual = { Equatable(1), Equatable(2), Equatable(3) };

                AssertReturnsActual(actual, () => actual.ShouldContainItems(new[] { Equatable(2), Equatable(3), Equatable(1) }));
            }

            [Test]
            public void CollectionDoesNotContainAllItems_FailsWithEnumerableDoesNotContainItemsMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(2), Equatable(3) };
                Equatable[] expected = { Equatable(1), Equatable(4) };
                Error.DoesNotContainItems(Matches<Equatable>(expected), Matches<Equatable>(actual), StandardTests.Instance.ObjectsAreEqual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldContainItems(expected, "foo"));
            }

            [Test]
            public void CollectionDoesNotContainEnoughDuplicates_FailsWithEnumerableDoesNotContainItemsMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(1), Equatable(3) };
                Equatable[] expected = { Equatable(1), Equatable(1), Equatable(1) };
                Error.DoesNotContainItems(Matches<Equatable>(expected), Matches<Equatable>(actual), StandardTests.Instance.ObjectsAreEqual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldContainItems(expected, "foo"));
            }

            [Test]
            public void CollectionDoesNotContainAllItems_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(Equatable(1));
                var expected = MakeEnumerable(Equatable(2));
                Error.DoesNotContainItems(Arg.Any<IEnumerable<Equatable>>(), Arg.Any<IEnumerable<Equatable>>(), Arg.Any<Func<Equatable, Equatable, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldContainItems(expected));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<Equatable> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<Equatable>), null, msg => actual.ShouldContainItems(new[] { Equatable(1) }, msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expected", () => new Equatable[0].ShouldContainItems((IEnumerable<Equatable>)null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                Equatable[] actualExpression = { Equatable(1), Equatable(2), Equatable(3) };
                Equatable[] expectedExpression = { Equatable(1), Equatable(4) };
                Error.DoesNotContainItems(Arg.Any<IEnumerable<Equatable>>(), Arg.Any<IEnumerable<Equatable>>(), Arg.Any<Func<Equatable, Equatable, bool>>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldContainItems(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldContainItems_CustomEquality : CollectionAssertionTests
        {
            [Test]
            public void CollectionContainsAllItems_ReturnsActualValue()
            {
                Equatable[] actual = { Equatable(1), Equatable(2), Equatable(3) };

                AssertReturnsActual(actual, () => actual.ShouldContainItems(new[] { 2, 3, 1 }, ValueEquals));
            }

            [Test]
            public void CollectionDoesNotContainAllItems_FailsWithEnumerableDoesNotContainItemsMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(2), Equatable(3) };
                int[] expected = { 1, 4 };
                Error.DoesNotContainItems(Matches<int>(expected), Matches<Equatable>(actual), ValueEquals, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldContainItems(expected, ValueEquals, "foo"));
            }

            [Test]
            public void CollectionDoesNotContainEnoughDuplicates_FailsWithEnumerableDoesNotContainItemsMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(1), Equatable(3) };
                int[] expected = { 1, 1, 1 };
                Error.DoesNotContainItems(Matches<int>(expected), Matches<Equatable>(actual), ValueEquals, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldContainItems(expected, ValueEquals, "foo"));
            }

            [Test]
            public void CollectionDoesNotContainAllItems_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(Equatable(1));
                var expected = MakeEnumerable(2);
                Error.DoesNotContainItems(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<Equatable>>(), Arg.Any<Func<Equatable, int, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldContainItems(expected, ValueEquals));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<Equatable> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<Equatable>), null, msg => actual.ShouldContainItems(new[] { 1 }, ValueEquals, msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expected", () => new Equatable[0].ShouldContainItems((IEnumerable<int>)null, ValueEquals));
            }

            [Test]
            public void PredicateIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("predicate", () => new Equatable[0].ShouldContainItems(new int[0], (Func<Equatable, int, bool>)null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                Equatable[] actualExpression = { Equatable(1), Equatable(2), Equatable(3) };
                int[] expectedExpression = { 1, 4 };
                Error.DoesNotContainItems(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<Equatable>>(), ValueEquals).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldContainItems(expectedExpression, ValueEquals));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ItemsShouldBeIn : CollectionAssertionTests
        {
            [Test]
            public void ExpectedContainsAllItems_ReturnsActualValue()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };

                AssertReturnsActual(actual, () => actual.ItemsShouldBeIn(new[] { Equatable(3), Equatable(2), Equatable(1) }));
            }

            [Test]
            public void ContainsExtraItem_FailsWithCollectionContainsExtraItemMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };
                Equatable[] expected = { Equatable(1) };
                Error.ContainsExtraItem(Matches<Equatable>(expected), Matches<Equatable>(actual), StandardTests.Instance.ObjectsAreEqual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ItemsShouldBeIn(expected, "foo"));
            }

            [Test]
            public void ContainsExtraDuplicate_FailsWithCollectionContainsExtraItemMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(1), Equatable(1) };
                Equatable[] expected = { Equatable(1), Equatable(1) };
                Error.ContainsExtraItem(Matches<Equatable>(expected), Matches<Equatable>(actual), StandardTests.Instance.ObjectsAreEqual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ItemsShouldBeIn(expected, "foo"));
            }

            [Test]
            public void ContainsExtraItem_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(Equatable(1));
                var expected = MakeEnumerable(Equatable(2));
                Error.ContainsExtraItem(Arg.Any<IEnumerable<Equatable>>(), Arg.Any<IEnumerable<Equatable>>(), StandardTests.Instance.ObjectsAreEqual, Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ItemsShouldBeIn(expected));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<Equatable> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<Equatable>), null, msg => actual.ItemsShouldBeIn(new[] { Equatable(1) }, msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expectedSuperset", () => new Equatable[0].ItemsShouldBeIn((IEnumerable<Equatable>)null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                Equatable[] actualExpression = { Equatable(1), Equatable(2) };
                Equatable[] expectedExpression = { Equatable(1) };
                Error.ContainsExtraItem(Arg.Any<IEnumerable<Equatable>>(), Arg.Any<IEnumerable<Equatable>>(), StandardTests.Instance.ObjectsAreEqual).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ItemsShouldBeIn(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ItemsShouldBeIn_CustomEquality : CollectionAssertionTests
        {
            [Test]
            public void ExpectedContainsAllItems_ReturnsActualValue()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };

                AssertReturnsActual(actual, () => actual.ItemsShouldBeIn(new[] { 3, 2, 1 }, ValueEquals));
            }

            [Test]
            public void ContainsExtraItem_FailsWithCollectionContainsExtraItemMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };
                int[] expected = { 1 };
                Error.ContainsExtraItem(Matches<int>(expected), Matches<Equatable>(actual), ValueEquals, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ItemsShouldBeIn(expected, ValueEquals, "foo"));
            }

            [Test]
            public void ContainsExtraDuplicate_FailsWithCollectionContainsExtraItemMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(1), Equatable(1) };
                int[] expected = { 1, 1 };
                Error.ContainsExtraItem(Matches<int>(expected), Matches<Equatable>(actual), ValueEquals, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ItemsShouldBeIn(expected, ValueEquals, "foo"));
            }

            [Test]
            public void ContainsExtraItem_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(Equatable(1));
                var expected = MakeEnumerable(2);
                Error.ContainsExtraItem(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<Equatable>>(), Arg.Any<Func<Equatable, int, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ItemsShouldBeIn(expected, ValueEquals));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<Equatable> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<Equatable>), null, msg => actual.ItemsShouldBeIn(new[] { 1 }, ValueEquals, msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expectedSuperset", () => new Equatable[0].ItemsShouldBeIn((IEnumerable<int>)null, ValueEquals));
            }

            [Test]
            public void PredicateIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("predicate", () => new Equatable[0].ItemsShouldBeIn(new int[0], (Func<Equatable, int, bool>)null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                Equatable[] actualExpression = { Equatable(1), Equatable(2) };
                int[] expectedExpression = { 1 };
                Error.ContainsExtraItem(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<Equatable>>(), ValueEquals).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ItemsShouldBeIn(expectedExpression, ValueEquals));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldNotContainItems : CollectionAssertionTests
        {
            [Test]
            public void CollectionDoesNotContainAnyOfTheItems_ReturnsActualValue()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };

                AssertReturnsActual(actual, () => actual.ShouldNotContainItems(new[] { Equatable(3), Equatable(4) }));
            }

            [Test]
            public void CollectionContainsOneOfTheItems_FailsWithSequenceContainsItemMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };
                Equatable[] expectedToNotContain = { Equatable(3), Equatable(2) };
                Error.Contains(Matches(expectedToNotContain), Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotContainItems(expectedToNotContain, "foo"));
            }

            [Test]
            public void CollectionContainsOneOfTheItems_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(Equatable(1));
                var expectedToNotContain = MakeEnumerable(Equatable(1));
                Error.Contains(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldNotContainItems(expectedToNotContain));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expectedToNotContain.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<Equatable> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<Equatable>), null, msg => actual.ShouldNotContainItems(new[] { Equatable(1) }, msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expectedToNotContain", () => new Equatable[0].ShouldNotContainItems((IEnumerable<Equatable>)null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                Equatable[] actualExpression = { Equatable(1), Equatable(2) };
                Equatable[] expectedExpression = { Equatable(3), Equatable(2) };
                Error.Contains(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldNotContainItems(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldNotContainItems_CustomEquality : CollectionAssertionTests
        {
            [Test]
            public void CollectionDoesNotContainAnyOfTheItems_ReturnsActualValue()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };

                AssertReturnsActual(actual, () => actual.ShouldNotContainItems(new[] { 3, 4 }, ValueEquals));
            }

            [Test]
            public void CollectionContainsOneOfTheItems_FailsWithSequenceContainsItemMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };
                int[] expectedToNotContain = { 3, 2 };
                Error.Contains(Matches(expectedToNotContain), Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotContainItems(expectedToNotContain, ValueEquals, "foo"));
            }

            [Test]
            public void CollectionContainsOneOfTheItems_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(Equatable(1));
                var expectedToNotContain = MakeEnumerable(1);
                Error.Contains(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldNotContainItems(expectedToNotContain, ValueEquals));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expectedToNotContain.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<Equatable> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<Equatable>), null, msg => actual.ShouldNotContainItems(new[] { 1 }, ValueEquals, msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expectedToNotContain", () => new Equatable[0].ShouldNotContainItems((IEnumerable<int>)null, ValueEquals));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                Equatable[] actualExpression = { Equatable(1), Equatable(2) };
                int[] expectedExpression = { 3, 2 };
                Error.Contains(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldNotContainItems(expectedExpression, ValueEquals));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldOnlyContain : CollectionAssertionTests
        {
            [Test]
            public void CollectionsHaveSameItems_ReturnsActualValue()
            {
                Equatable[] actual = { Equatable(1), Equatable(2), Equatable(1) };

                AssertReturnsActual(actual, () => actual.ShouldOnlyContain(new[] { Equatable(2), Equatable(1), Equatable(1) }));
            }

            [Test]
            public void MissingItem_FailsWithCollectionDoesNotOnlyContainMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };
                Equatable[] expected = { Equatable(1), Equatable(3) };
                Error.DoesNotOnlyContain(Matches<Equatable>(expected), Matches<Equatable>(actual), StandardTests.Instance.ObjectsAreEqual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldOnlyContain(expected, "foo"));
            }

            [Test]
            public void ExtraItem_FailsWithCollectionDoesNotOnlyContainMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };
                Equatable[] expected = { Equatable(1) };
                Error.DoesNotOnlyContain(Matches<Equatable>(expected), Matches<Equatable>(actual), StandardTests.Instance.ObjectsAreEqual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldOnlyContain(expected, "foo"));
            }

            [Test]
            public void ExtraCopyOfItem_FailsWithCollectionDoesNotOnlyContainMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(1), Equatable(1) };
                Equatable[] expected = { Equatable(1), Equatable(1) };
                Error.DoesNotOnlyContain(Matches<Equatable>(expected), Matches<Equatable>(actual), StandardTests.Instance.ObjectsAreEqual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldOnlyContain(expected, "foo"));
            }

            [Test]
            public void DifferentItems_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(Equatable(1));
                var expected = MakeEnumerable(Equatable(2));
                Error.DoesNotOnlyContain(Arg.Any<IEnumerable<Equatable>>(), Arg.Any<IEnumerable<Equatable>>(), Arg.Any<Func<Equatable, Equatable, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldOnlyContain(expected));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<Equatable> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<Equatable>), null, msg => actual.ShouldOnlyContain(new[] { Equatable(1) }, msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expected", () => new Equatable[0].ShouldOnlyContain((IEnumerable<Equatable>)null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                Equatable[] actualExpression = { Equatable(1), Equatable(2) };
                Equatable[] expectedExpression = { Equatable(1) };
                Error.DoesNotOnlyContain(Arg.Any<IEnumerable<Equatable>>(), Arg.Any<IEnumerable<Equatable>>(), Arg.Any<Func<Equatable, Equatable, bool>>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldOnlyContain(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldOnlyContain_CustomEquality : CollectionAssertionTests
        {
            [Test]
            public void CollectionsHaveSameItems_ReturnsActualValue()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };

                AssertReturnsActual(actual, () => actual.ShouldOnlyContain(new[] { 2, 1 }, ValueEquals));
            }

            [Test]
            public void MissingItem_FailsWithCollectionDoesNotOnlyContainMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };
                int[] expected = { 1, 3 };
                Error.DoesNotOnlyContain(Matches<int>(expected), Matches<Equatable>(actual), ValueEquals, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldOnlyContain(expected, ValueEquals, "foo"));
            }

            [Test]
            public void ExtraItem_FailsWithCollectionDoesNotOnlyContainMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(2) };
                int[] expected = { 1 };
                Error.DoesNotOnlyContain(Matches<int>(expected), Matches<Equatable>(actual), ValueEquals, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldOnlyContain(expected, ValueEquals, "foo"));
            }

            [Test]
            public void DifferentItems_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(Equatable(1));
                var expected = MakeEnumerable(2);
                Error.DoesNotOnlyContain(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<Equatable>>(), Arg.Any<Func<Equatable, int, bool>>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldOnlyContain(expected, ValueEquals));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void HasDuplicate_FailsWithCollectionDoesNotOnlyContainMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(1) };
                int[] expected = { 1 };
                Error.DoesNotOnlyContain(Matches<int>(expected), Matches<Equatable>(actual), ValueEquals, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldOnlyContain(expected, ValueEquals, "foo"));
            }

            [Test]
            public void HasDuplicate_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(Equatable(1), Equatable(1));
                var expected = MakeEnumerable(1);
                Error.DoesNotOnlyContain(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<Equatable>>(), Arg.Any<Func<Equatable, int, bool>>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldOnlyContain(expected, ValueEquals));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<Equatable> actual = null;
                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<Equatable>), null, msg => actual.ShouldOnlyContain(new[] { 1 }, ValueEquals, msg));
            }

            [Test]
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expected", () => new Equatable[0].ShouldOnlyContain((IEnumerable<int>)null, ValueEquals));
            }


            [Test]
            public void PredicateIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("predicate", () => new Equatable[0].ShouldOnlyContain(new int[0], (Func<Equatable, int, bool>)null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                Equatable[] actualExpression = { Equatable(1), Equatable(2) };
                int[] expectedExpression = { 1 };
                Error.DoesNotOnlyContain(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<Equatable>>(), Arg.Any<Func<Equatable, int, bool>>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldOnlyContain(expectedExpression, ValueEquals));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldBeDistinct : CollectionAssertionTests
        {
            [Test]
            public void NoDuplicates_ReturnsActual()
            {
                Equatable[] actual = { Equatable(1), Equatable(2), Equatable(3) };

                AssertReturnsActual(actual, () => actual.ShouldBeDistinct());
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<Equatable> actual = null;

                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<Equatable>), null, msg => actual.ShouldBeDistinct(msg));
            }

            [Test]
            public void ContainsDuplicate_FailsWithContainsDuplicateMessage()
            {
                Equatable[] actual = { Equatable(1), Equatable(2), Equatable(1) };
                Error.ContainsDuplicate(Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldBeDistinct("foo"));
            }

            [Test]
            public void ContainsDuplicate_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(Equatable(1), Equatable(1));
                Error.ContainsDuplicate(Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldBeDistinct());

                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                Equatable[] actualExpression = { Equatable(1) };

                actualExpression.ShouldBeDistinct();

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            }
        }

        class ShouldBeDistinct_CustomEquality : CollectionAssertionTests
        {
            [Test]
            public void NoDuplicates_ReturnsActual()
            {
                Equatable[] actual = { Equatable(1), Equatable(1) };

                AssertReturnsActual(actual, () => actual.ShouldBeDistinct(ReferenceEquals));
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<Equatable> actual = null;

                AssertFailsWithTypesNotEqualMessage(typeof(IEnumerable<Equatable>), null, msg => actual.ShouldBeDistinct(Equals, msg));
            }

            [Test]
            public void ContainsDuplicate_FailsWithContainsDuplicateMessage()
            {
                int[] actual = { 1, 2, 3 };
                Error.ContainsDuplicate(Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldBeDistinct((a, b) => a % 2 == b % 2, "foo"));
            }

            [Test]
            public void ContainsDuplicate_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1, 2, 3);
                Error.ContainsDuplicate(Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldBeDistinct((a, b) => a % 2 == b % 2));

                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actualExpression = { 1, 2, 3 };
                Error.ContainsDuplicate(Arg.Any<IEnumerable>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldBeDistinct((a, b) => a % 2 == b % 2));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            }
        }

        class ShouldMatchReferences : CollectionAssertionTests
        {
            [Test]
            public void SameItems_ReturnsActualValue()
            {
                var a = new object();
                var b = new object();
                object[] actual = { a, b };

                AssertReturnsActual(actual, () => actual.ShouldMatchReferences(new[] { a, b }));
            }

            [Test]
            public void DifferentLength_FailsWithEnumerableLengthMismatchMessage()
            {
                object[] actual = { };
                object[] expected = { new object() };
                Error.LengthMismatch(1, Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldMatchReferences(expected, "foo"));
            }

            [Test]
            public void DifferentLength_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(new object());
                var expected = MakeEnumerable(new object(), new object());
                Error.LengthMismatch(Arg.Any<int>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldMatchReferences(expected));

                Assert.AreEqual(1, actual.EnumerationCount);
                Assert.AreEqual(1, expected.EnumerationCount);
            }

            [Test]
            public void DifferentItems_FailsWithEnumerablesNotSameMessage()
            {
                var a = new object();
                object[] actual = { a, new object() };
                object[] expected = { a, new object() };
                Error.ItemsNotSame(Matches(expected), Matches(actual), "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldMatchReferences(expected, "foo"));
            }

            [Test]
            public void DifferentItems_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(new object());
                var expected = MakeEnumerable(new object());
                Error.ItemsNotSame(Arg.Any<IEnumerable>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ShouldMatchReferences(expected));

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
            public void ExpectedIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("expected", () => new Equatable[0].ShouldMatchReferences((IEnumerable<Equatable>)null));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] expectedExpression = { 3, 4 };
                int[] actualExpression = { 1, 2 };
                Error.DoNotMatch(Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<Func<int, int, bool>>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldMatch(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class KeyedCollection_ShouldContainKey : CollectionAssertionTests
        {
            [Test]
            public void ContainsKeys_ReturnsActualValue()
            {
                var actual = new TestKeyedCollection { "foo", "bar" };

                AssertReturnsActual(actual, () => actual.ShouldContainKey('b'));
            }

            [Test]
            public void DoesNotContainKey_FailsWithDoesNotContainKeyMessage()
            {
                var actual = new TestKeyedCollection();
                Error.DoesNotContain('a', actual, "key", "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldContainKey('a', "foo"));
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
                var actualExpression = new TestKeyedCollection();
                var expectedExpression = 'a';
                Error.DoesNotContain(Arg.Any<object>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldContainKey(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class KeyedCollection_ShouldNotContainKey : CollectionAssertionTests
        {
            [Test]
            public void DoesNotContainKey_ReturnsActualValue()
            {
                var actual = new TestKeyedCollection();

                AssertReturnsActual(actual, () => actual.ShouldNotContainKey('a'));
            }

            [Test]
            public void ContainsKey_FailsWithContainsKeyMessage()
            {
                var actual = new TestKeyedCollection { "foo" };
                Error.Contains('f', actual, "key", "bar").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotContainKey('f', "bar"));
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
                var actualExpression = new TestKeyedCollection { "foo" };
                var expectedExpression = 'f';
                Error.Contains(Arg.Any<object>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldNotContainKey(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
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
                var gotFirstItem = false;
                var gotSecondItem = false;

                new[] { 1, 2 }.ItemsSatisfy(
                    i => { gotFirstItem = i == 1; },
                    i => { gotSecondItem = i == 2; });

                Assert.IsTrue(gotFirstItem, "first item");
                Assert.IsTrue(gotSecondItem, "second item");
            }

            [Test]
            public void WrongNumberOfItems_FailsWithEnumerableLengthMismatchMessage()
            {
                int[] actual = { 1 };
                Error.LengthMismatch(2, Matches(actual)).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ItemsSatisfy(i => { }, i => { }));
            }

            [Test]
            public void WrongNumberOfItems_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1, 2);
                Error.LengthMismatch(Arg.Any<int>(), Arg.Any<IEnumerable>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ItemsSatisfy(i => i.ShouldBe(1)));

                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void ItemDoesNotSatisfyItsAssertion_FailsWithThrownException()
            {
                int[] actual = { 1, 2 };

                AssertThrowsExpectedError(() => actual.ItemsSatisfy(
                    i => { },
                    i => { throw ExpectedException; }));
            }

            [Test]
            public void ItemDoesNotSatisfyItsAssertion_OnlyEnumeratesOnce()
            {
                var actual = MakeEnumerable(1);
                Error.NotEqual(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<string>()).Returns(EnumerateArgs);

                AssertThrowsExpectedError(() => actual.ItemsSatisfy(i => i.ShouldBe(2)));

                Assert.AreEqual(1, actual.EnumerationCount);
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                Error.NotEqual(typeof(IEnumerable<int>), null).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ItemsSatisfy(i => { }));
            }

            [Test]
            public void AssertionsArrayIsNull_ThrowsArgumentNullException()
            {
                AssertArgumentNullException("assertions", () => new int[0].ItemsSatisfy((Action<int>[])null));
            }

            [Test]
            public void AssertionIsNull_ThrowsArgumentException()
            {
                var result = Assert.Throws<ArgumentException>(() => new int[0].ItemsSatisfy(i => { }, null));
                Assert.AreEqual("assertions", result.ParamName);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actualExpression = { 1 };
                var expectedExpression = 2;
                Error.NotEqual(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<string>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ItemsSatisfy(item => item.ShouldBe(expectedExpression)));

                Assert.AreEqual($"{nameof(actualExpression)}[0]", TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
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

                AssertThrowsExpectedError(() => actual.AllItemsSatisfy(i =>
                    {
                        if (i > 1) throw ExpectedException;
                    }));
            }

            [Test]
            public void ActualIsNull_FailsWithTypesNotEqualMessage()
            {
                IEnumerable<int> actual = null;
                Error.NotEqual(typeof(IEnumerable<int>), null).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.AllItemsSatisfy(i => { }));
            }

            [Test]
            public void AssertionIsNull_ThrowsArgumentNullException()
            {
                var result = Assert.Throws<ArgumentNullException>(() => new int[0].AllItemsSatisfy((Action<int>?)null));

                Assert.AreEqual("assertion", result.ParamName);
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                int[] actualExpression = { 1 };
                var expectedExpression = 2;
                Error.NotEqual(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<string>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.AllItemsSatisfy(item => item.ShouldBe(expectedExpression)));

                Assert.AreEqual($"{nameof(actualExpression)}[0]", TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        static TestEnumerable<T> MakeEnumerable<T>(params T[] items) => new TestEnumerable<T>(items);

        static Equatable Equatable(int value) => new Equatable(value);

        static bool ValueEquals(Equatable actual, int expected) => actual.Value == expected;

        Exception EnumerateArgs(CallInfo call)
        {
            foreach (var enumerable in call.Args().OfType<IEnumerable>())
                enumerable.Cast<object>().ToList();
            return ExpectedException;
        }

        class TestKeyedCollection : KeyedCollection<char, string>
        {
            protected override char GetKeyForItem(string item)
            {
                return item[0];
            }
        }
    }
}
