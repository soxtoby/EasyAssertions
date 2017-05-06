using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using static EasyAssertions.UnitTests.EnumerableArg;

namespace EasyAssertions.UnitTests
{
    public class EnumAssertionTests : AssertionTests
    {
        [Test]
        public void ShouldBeValue_SameValue_ReturnsActualValue()
        {
            TestEnum actual = TestEnum.One;
            TestEnum expected = TestEnum.One;

            Actual<TestEnum> result = actual.ShouldBeValue(expected);

            Assert.AreEqual(actual, result.Value);
        }

        [Test]
        public void ShouldBeValue_DifferentValue_FailsWithObjectsNotEqualMessage()
        {
            TestEnum actual = TestEnum.One;
            TestEnum expected = TestEnum.Two;
            Error.NotEqual(expected, actual, "foo").Returns(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldBe(expected, "foo"));
        }

        [Test]
        public void ShouldHaveFlag_HasFlag_ReturnsActualValue()
        {
            FlagsEnum actual = FlagsEnum.One | FlagsEnum.Two;
            FlagsEnum expectedFlag = FlagsEnum.Two;

            Actual<FlagsEnum> result = actual.ShouldHaveFlag(expectedFlag);

            Assert.AreEqual(actual, result.Value);
        }

        [Test]
        public void ShouldHaveFlag_NotAnEnum_FailsWithTypesNotEqualMessage()
        {
            AssertFailsWithTypesNotEqualMessage(typeof(Enum), typeof(int), msg => 1.ShouldHaveFlag(1, msg));
        }

        [Test]
        public void ShouldHaveFlag_NotAFlagsEnum_FailsWithDoesNotContainFlagsAttributeMessage()
        {
            TestEnum actual = TestEnum.One;
            TestEnum expectedFlag = TestEnum.One;
            Error.DoesNotContain(Arg.Any<FlagsAttribute>(), Matches<object>(typeof(TestEnum).GetCustomAttributes(false)), "foo").Throws(ExpectedException);
            
            AssertThrowsExpectedError(() => actual.ShouldHaveFlag(expectedFlag, "foo"));
        }

        [Test]
        public void ShouldHaveFlag_DoesNotHaveFlag_FailsWithDoesNotContainFlagMessage()
        {
            FlagsEnum actual = FlagsEnum.Two;
            FlagsEnum expected = FlagsEnum.One;
            Error.DoesNotContain(expected, Matches<FlagsEnum>(new[] { FlagsEnum.Two }), "foo").Throws(ExpectedException);

            AssertThrowsExpectedError(() => actual.ShouldHaveFlag(expected, "foo"));
        }

        [NotFlags]
        enum TestEnum
        {
            One,
            Two
        }

        [Flags]
        enum FlagsEnum
        {
            None = 0,
            One = 1,
            Two = 2
        }

        class NotFlagsAttribute : Attribute { }
    }
}