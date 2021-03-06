﻿using System;
using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class ObjectAssertionTests : AssertionTests
    {
        [Test]
        public void ShouldBe_SameValueReturnsActualValue()
        {
            Equatable actual = new Equatable(1);
            Equatable expected = new Equatable(1);
            Actual<Equatable> result = actual.ShouldBe(expected);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldBe_DifferentObjects_FailsWithObjectsNotEqualMessage()
        {
            object obj1 = new object();
            object obj2 = new object();
            Error.NotEqual(obj2, obj1, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => obj1.ShouldBe(obj2, "foo"));
        }

        [Test]
        public void ShouldBe_DifferentStrings_FailsWithStringsNotEqualMessage()
        {
            Error.NotEqual("foo", "bar", message: "baz").Returns(ExpectedException);

            AssertThrowsExpectedError(() => "bar".ShouldBe("foo", "baz"));
        }

        [Test]
        public void ShouldBe_CorrectlyRegistersAssertion()
        {
            Equatable actualExpression = new Equatable(1);
            Equatable expectedExpression = new Equatable(2);
            Error.NotEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldBe(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
        }

        [Test]
        public void NullableShouldBe_ValueEqualsExpected_ReturnsActualValue()
        {
            int? actual = 1;

            Actual<int> result = actual.ShouldBe(1);

            Assert.AreEqual(1, result.And);
        }

        [Test]
        public void NullableShouldBe_NoValue_FailsWithObjectsNotEqualMessage()
        {
            int? actual = null;
            const int expected = 1;
            Error.NotEqual(expected, actual, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldBe(expected, "foo"));
        }

        [Test]
        public void NullableShouldBe_ValueIsDifferent_FailsWithObjectsNotEqualMessage()
        {
            int? actual = 1;
            const int expected = 2;
            Error.NotEqual(expected, actual, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldBe(expected, "foo"));
        }

        [Test]
        public void NullableShouldBe_CorrectlyRegistersAssertion()
        {
            int? actualExpression = 1;
            const int expectedExpression = 2;
            Error.NotEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

            Assert.Throws<Exception>(() => actualExpression.ShouldBe(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldNotBe_DifferentValue_ReturnsActualValue()
        {
            Equatable actual = new Equatable(1);

            Actual<Equatable> result = actual.ShouldNotBe(new Equatable(2));

            Assert.AreSame(actual, result.Value);
        }

        [Test]
        public void ShouldNotBe_EqualValue_FailsWithObjectsEqualMessage()
        {
            Equatable actual = new Equatable(1);
            Equatable notExpected = new Equatable(1);
            Error.AreEqual(notExpected, actual, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldNotBe(notExpected, "foo"));
        }

        [Test]
        public void ShouldNotBe_CorrectlyRegistersAssertion()
        {
            Equatable actualExpression = new Equatable(1);
            Equatable expectedExpression = new Equatable(1);
            Error.AreEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldNotBe(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldBeNull_IsNull_Passes()
        {
            ((object)null).ShouldBeNull();
        }

        [Test]
        public void ShouldBeNull_NotNull_FailsWithNotEqualToNullMessage()
        {
            object actual = new object();
            Error.NotEqual(null, actual, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldBeNull("foo"));
        }

        [Test]
        public void ShouldBeNull_CorrectlyRegistersAssertion()
        {
            object actualExpression = null;

            actualExpression.ShouldBeNull();

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldNotBeNull_NotNull_ReturnsActualValue()
        {
            object actual = new object();

            Actual<object> result = actual.ShouldNotBeNull();

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldNotBeNull_IsNull_FailsWithIsNullMessage()
        {
            Error.IsNull("foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => ((object)null).ShouldNotBeNull("foo"));
        }

        [Test]
        public void ShouldNotBeNull_CorrectlyRegistersAssertion()
        {
            object actualExpression = new object();

            actualExpression.ShouldNotBeNull();

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldReferTo_SameObject_ReturnsActualValue()
        {
            object obj = new object();
            Actual<object> result = obj.ShouldReferTo(obj);

            Assert.AreSame(obj, result.And);
        }

        [Test]
        public void ShouldReferTo_DifferentObject_FailsWithObjectsNotSameMessage()
        {
            Equatable actual = new Equatable(1);
            Equatable expected = new Equatable(1);
            Error.NotSame(expected, actual, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldReferTo(expected, "foo"));
        }

        [Test]
        public void ShouldReferTo_CorrectlyRegistersAssertion()
        {
            object actualExpression = new object();
            object expectedExpression = new object();
            Error.NotSame(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldReferTo(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldNotReferTo_DifferentObject_ReturnsActualValue()
        {
            object actual = new object();

            Actual<object> result = actual.ShouldNotReferTo(new object());

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldNotReferTo_SameObject_FailsWithObjectsAreSameMessage()
        {
            object actual = new object();
            Error.AreSame(actual, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldNotReferTo(actual, "foo"));
        }

        [Test]
        public void ShouldNotReferTo_CorrectlyRegistersAssertion()
        {
            object actualExpression = new object();
            object expectedExpression = actualExpression;
            Error.AreSame(Arg.Any<object>(), Arg.Any<string>()).Returns(ExpectedException);

            AssertThrowsExpectedError(() => actualExpression.ShouldNotReferTo(expectedExpression));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
            Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldBeA_SubType_ReturnsTypedActual()
        {
            object actual = new SubEquatable(1);
            Actual<Equatable> result = actual.ShouldBeA<Equatable>();

            Assert.AreSame(actual, result.And);
            Assert.AreEqual(1, result.And.Value);
        }

        [Test]
        public void ShouldBeA_SuperType_FailsWithTypesNotEqualMessage()
        {
            object actual = new Equatable(1);
            Error.NotEqual(typeof(SubEquatable), typeof(Equatable), "foo").Returns(ExpectedException);
            AssertThrowsExpectedError(() => actual.ShouldBeA<SubEquatable>("foo"));
        }

        [Test]
        public void ShouldBeA_CorrectlyRegistersAssertion()
        {
            object actualExpression = new SubEquatable(1);

            actualExpression.ShouldBeA<Equatable>();

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }
    }
}