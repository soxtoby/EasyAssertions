﻿using System;
using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    public class NumberAssertionTests : AssertionTests
    {
        /*// None of the lines in the following method should compile
        public void ShouldNotCompile()
        {
            1f.ShouldBe(1d);
            1f.ShouldBe(1d, "message");
            1f.ShouldBe(1);
            1f.ShouldBe(1, "message");
            1d.ShouldBe(1f);
            1d.ShouldBe(1f, "message");
            1d.ShouldBe(1);
            1d.ShouldBe(1, "message");
            1.ShouldBe(1f);
            1.ShouldBe(1f, "message");
            1.ShouldBe(1d);
            1.ShouldBe(1d, "message");
            1.ShouldBe(1f, 1d);
            1.ShouldBe(1f, 1d, "message");
            1.ShouldBe(1d, 1d);
            1.ShouldBe(1d, 1d, "message");
            1.ShouldBe(1, 1d);
            1.ShouldBe(1, 1d, "message");
        }/**/


        class ShouldBe_Float_Float : NumberAssertionTests
        {
            [Test]
            public void WithinTolerance_ReturnsActualValue()
            {
                const float actual = 1f;

                var result = ((Func<Actual<float>>)(() => ShouldBe(actual, 1f, 1f)))();

                Assert.AreEqual(actual, result.And);
            }

            [Test]
            public void OutsideTolerance_FailsWithObjectsNotEqualMessage()
            {
                const float expected = 10f;
                const float actual = 1f;
                Error.NotEqual(expected, actual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => ShouldBe(actual, expected, 1f, "foo"));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                const float expectedExpression = 10f;
                const float actualExpression = 1f;
                Error.NotEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldBe(expectedExpression, 1f));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }

            static Actual<float> ShouldBe(float actual, float expected, double tolerance, string? message = null)
            {
                return actual.ShouldBe(expected, tolerance, message);
            }
        }

        class ShouldBe_Float_Double : NumberAssertionTests
        {
            [Test]
            public void WithinTolerance_ReturnsActualValue()
            {
                const float actual = 1f;

                var result = ((Func<Actual<float>>)(() => ShouldBe(actual, 1f, 1d)))();

                Assert.AreEqual(actual, result.And);
            }

            [Test]
            public void OutsideTolerance_FailsWithObjectsNotEqualMessage()
            {
                const double expected = 10d;
                const float actual = 1f;
                Error.NotEqual(expected, actual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldBe(expected, 1d, "foo"));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                const float actualExpression = 1;
                const double expectedExpression = 2;
                Error.NotEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldBe(expectedExpression, double.Epsilon));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }

            static Actual<float> ShouldBe(float actual, double expected, double tolerance, string? message = null)
            {
                return actual.ShouldBe(expected, tolerance, message);
            }
        }

        class ShouldBe_Double_Double : NumberAssertionTests
        {
            [Test]
            public void WithinTolerance_ReturnsActualValue()
            {
                const double actual = 1d;

                var result = ((Func<Actual<double>>)(() => ShouldBe(actual, 1d, 0)))();

                Assert.AreEqual(actual, result.And);
            }

            [Test]
            public void OutsideTolerance_FailsWithObjectsNotEqualMessage()
            {
                const double expected = 10d;
                const double actual = 1d;
                Error.NotEqual(expected, actual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => ShouldBe(actual, expected, 1d, "foo"));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                const double actualExpression = 1;
                const double expectedExpression = 2;
                Error.NotEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldBe(expectedExpression, double.Epsilon));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }

            static Actual<double> ShouldBe(double actual, double expected, double tolerance, string? message = null)
            {
                return actual.ShouldBe(expected, tolerance, message);
            }
        }

        class ShouldBe_Double_Float : NumberAssertionTests
        {
            [Test]
            public void WithinTolerance_ReturnsActualValue()
            {
                const double actual = 1d;

                var result = ((Func<Actual<double>>)(() => ShouldBe(actual, 1f, 0)))();

                Assert.AreEqual(actual, result.And);
            }

            [Test]
            public void OutsideTolerance_FailsWithObjectsNotEqualMessage()
            {
                const float expected = 10f;
                const double actual = 1d;
                Error.NotEqual(expected, actual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => ShouldBe(actual, expected, 1d, "foo"));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                const double actualExpression = 1;
                const float expectedExpression = 2;
                Error.NotEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldBe(expectedExpression, double.Epsilon));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }

            static Actual<double> ShouldBe(double actual, float expected, double tolerance, string? message = null)
            {
                return actual.ShouldBe(expected, tolerance, message);
            }
        }

        class ShouldNotBe_Float : NumberAssertionTests
        {
            [Test]
            public void OutsideTolerance_ReturnsActualValue()
            {
                const float actual = 1f;

                var result = actual.ShouldNotBe(2f, 0);

                Assert.AreEqual(actual, result.And);
            }

            [Test]
            public void WithinTolerance_FailsWithObjectsEqualMessage()
            {
                const float actual = 1f;
                const float notExpected = 2f;
                Error.AreEqual(notExpected, actual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotBe(notExpected, 1f, "foo"));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                const float actualExpression = 1;
                const float expectedExpression = 1;
                Error.AreEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldNotBe(expectedExpression, float.Epsilon));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldNotBe_Double : NumberAssertionTests
        {
            [Test]
            public void OutsideTolerance_ReturnsActualValue()
            {
                const double actual = 1d;

                var result = actual.ShouldNotBe(2d, 0);

                Assert.AreEqual(actual, result.And);
            }

            [Test]
            public void WithinTolerance_FailsWithObjectsEqualMessage()
            {
                const double actual = 1d;
                const double notExpected = 2d;
                Error.AreEqual(notExpected, actual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotBe(notExpected, 1d, "foo"));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                const double actualExpression = 1;
                const double expectedExpression = 1;
                Error.AreEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldNotBe(expectedExpression, double.Epsilon));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldBeGreaterThan : NumberAssertionTests
        {
            [Test]
            public void IsGreaterThan_ReturnsActualValue()
            {
                const int actual = 2;

                var result = actual.ShouldBeGreaterThan(1);

                Assert.AreEqual(actual, result.And);
            }

            [Test]
            public void NotGreaterThan_FailsWithNotGreaterThanMessage()
            {
                const int actual = 1;
                const int expected = 2;
                Error.NotGreaterThan(expected, actual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldBeGreaterThan(expected, "foo"));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                const int actualExpression = 1;
                const int expectedExpression = 2;
                Error.NotGreaterThan(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldBeGreaterThan(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldBeLessThan : NumberAssertionTests
        {
            [Test]
            public void IsLessThan_ReturnsActualValue()
            {
                const int actual = 1;

                var result = actual.ShouldBeLessThan(2);

                Assert.AreEqual(actual, result.And);
            }

            [Test]
            public void NotLessThan_FailsWithNotLessThanMessage()
            {
                const int actual = 2;
                const int expected = 1;
                Error.NotLessThan(expected, actual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldBeLessThan(expected, "foo"));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                const int actualExpression = 1;
                const int expectedExpression = 0;
                Error.NotLessThan(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldBeLessThan(expectedExpression));

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
                Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
            }
        }

        class ShouldBeNaN_Float : NumberAssertionTests
        {
            [Test]
            public void IsNaN_Passes()
            {
                const float actual = float.NaN;

                actual.ShouldBeNaN();
            }

            [Test]
            public void IsNotNaN_FailsWithObjectsNotEqualMessage()
            {
                const float actual = 1f;
                Error.NotEqual(float.NaN, actual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldBeNaN("foo"));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                const float actualExpression = 1f;
                Error.NotEqual(Arg.Any<object?>(), Arg.Any<object?>(), Arg.Any<string?>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldBeNaN());

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            }
        }

        class ShouldNotBeNaN_Float : NumberAssertionTests
        {
            [Test]
            public void IsNotNaN_ReturnsActualValue()
            {
                const float actual = 1f;

                var result = actual.ShouldNotBeNaN();

                Assert.AreEqual(actual, result.And);
            }

            [Test]
            public void IsNaN_FailsWithObjectsEqualMessage()
            {
                const float actual = float.NaN;
                Error.AreEqual(float.NaN, actual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotBeNaN("foo"));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                const float actualExpression = float.NaN;
                Error.AreEqual(Arg.Any<object?>(), Arg.Any<object?>(), Arg.Any<string?>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldNotBeNaN());

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            }
        }

        class ShouldBeNaN_Double : NumberAssertionTests
        {
            [Test]
            public void IsNaN_Passes()
            {
                const double actual = double.NaN;

                actual.ShouldBeNaN();
            }

            [Test]
            public void IsNotNaN_FailsWithObjectsNotEqualMessage()
            {
                const double actual = 1d;
                Error.NotEqual(double.NaN, actual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldBeNaN("foo"));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                const double actualExpression = 1d;
                Error.NotEqual(Arg.Any<object?>(), Arg.Any<object?>(), Arg.Any<string?>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldBeNaN());

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            }
        }

        class ShouldNotBeNaN_Double : NumberAssertionTests
        {
            [Test]
            public void IsNotNaN_ReturnsActualValue()
            {
                const double actual = 1f;

                var result = actual.ShouldNotBeNaN();

                Assert.AreEqual(actual, result.And);
            }

            [Test]
            public void IsNaN_FailsWithObjectsEqualMessage()
            {
                const double actual = double.NaN;
                Error.AreEqual(double.NaN, actual, "foo").Returns(ExpectedException);

                AssertThrowsExpectedError(() => actual.ShouldNotBeNaN("foo"));
            }

            [Test]
            public void CorrectlyRegistersAssertion()
            {
                const double actualExpression = double.NaN;
                Error.AreEqual(Arg.Any<object?>(), Arg.Any<object?>(), Arg.Any<string?>()).Returns(ExpectedException);

                AssertThrowsExpectedError(() => actualExpression.ShouldNotBeNaN());

                Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            }
        }
    }
}
