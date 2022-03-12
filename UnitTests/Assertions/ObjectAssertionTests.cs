using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests;

[TestFixture]
public class ObjectAssertionTests : AssertionTests
{
    /* // None of the lines in the following method should compile
    public void ShouldNotCompile()
    {
        1.ShouldBeNull();
        1.ShouldNotBeNull();
    }/**/

    [Test]
    public void ShouldBe_SameValueReturnsActualValue()
    {
        var actual = new Equatable(1);
        var expected = new Equatable(1);
        var result = actual.ShouldBe(expected);

        Assert.AreSame(actual, result.And);
    }

    [Test]
    public void ShouldBe_DifferentObjects_FailsWithObjectsNotEqualMessage()
    {
        var obj1 = new object();
        var obj2 = new object();
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
        var actualExpression = new Equatable(1);
        var expectedExpression = new Equatable(2);
        Error.NotEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

        AssertThrowsExpectedError(() => actualExpression.ShouldBe(expectedExpression));

        Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
    }

    [Test]
    public void ShouldBeValue_SameValueReturnsActualValue()
    {
        var actual = 1;
        var expected = 1;
        var result = actual.ShouldBeValue(expected);

        Assert.AreEqual(actual, result.And);
    }

    [Test]
    public void ShouldBeValue_DifferentValues_FailsWithObjectsNotEqualMessage()
    {
        var value1 = 1;
        var value2 = 2;
        Error.NotEqual(value2, value1, "foo").Returns(ExpectedException);

        AssertThrowsExpectedError(() => value1.ShouldBeValue(value2, "foo"));
    }

    [Test]
    public void ShouldBeValue_CorrectlyRegistersAssertion()
    {
        var actualExpression = 1;
        var expectedExpression = 2;
        Error.NotEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

        AssertThrowsExpectedError(() => actualExpression.ShouldBeValue(expectedExpression));

        Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
    }

    [Test]
    public void ShouldBeValue_Nullable_ValueEqualsExpected_ReturnsActualValue()
    {
        int? actual = 1;

        var result = actual.ShouldBeValue(1);

        Assert.AreEqual(1, result.And);
    }

    [Test]
    public void ShouldBeValue_Nullable_NoValue_FailsWithObjectsNotEqualMessage()
    {
        int? actual = null;
        const int expected = 1;
        Error.NotEqual(expected, actual, "foo").Returns(ExpectedException);

        AssertThrowsExpectedError(() => actual.ShouldBeValue(expected, "foo"));
    }

    [Test]
    public void ShouldBeValue_Nullable_ValueIsDifferent_FailsWithObjectsNotEqualMessage()
    {
        int? actual = 1;
        const int expected = 2;
        Error.NotEqual(expected, actual, "foo").Returns(ExpectedException);

        AssertThrowsExpectedError(() => actual.ShouldBeValue(expected, "foo"));
    }

    [Test]
    public void ShouldBeValue_Nullable_CorrectlyRegistersAssertion()
    {
        int? actualExpression = 1;
        const int expectedExpression = 2;
        Error.NotEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

        Assert.Throws<Exception>(() => actualExpression.ShouldBeValue(expectedExpression));

        Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
    }

    [Test]
    public void ShouldNotBe_DifferentValue_ReturnsActualValue()
    {
        var actual = new Equatable(1);

        var result = actual.ShouldNotBe(new Equatable(2));

        Assert.AreSame(actual, result.Value);
    }

    [Test]
    public void ShouldNotBe_EqualValue_FailsWithObjectsEqualMessage()
    {
        var actual = new Equatable(1);
        var notExpected = new Equatable(1);
        Error.AreEqual(notExpected, actual, "foo").Returns(ExpectedException);

        AssertThrowsExpectedError(() => actual.ShouldNotBe(notExpected, "foo"));
    }

    [Test]
    public void ShouldNotBe_CorrectlyRegistersAssertion()
    {
        var actualExpression = new Equatable(1);
        var expectedExpression = new Equatable(1);
        Error.AreEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

        AssertThrowsExpectedError(() => actualExpression.ShouldNotBe(expectedExpression));

        Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
    }

    [Test]
    public void ShouldNotBeValue_DifferentValue_ReturnsActualValue()
    {
        var actual = 1;

        var result = actual.ShouldNotBeValue(2);

        Assert.AreEqual(actual, result.Value);
    }

    [Test]
    public void ShouldNotBeValue_EqualValue_FailsWithObjectsEqualMessage()
    {
        var actual = 1;
        var notExpected = 1;
        Error.AreEqual(notExpected, actual, "foo").Returns(ExpectedException);

        AssertThrowsExpectedError(() => actual.ShouldNotBeValue(notExpected, "foo"));
    }

    [Test]
    public void ShouldNotBeValue_CorrectlyRegistersAssertion()
    {
        var actualExpression = 1;
        var expectedExpression = 1;
        Error.AreEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

        AssertThrowsExpectedError(() => actualExpression.ShouldNotBeValue(expectedExpression));

        Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
    }

    [Test]
    public void ShouldNotBeValue_Nullable_DifferentValue_ReturnsActualValue()
    {
        int? actual = 1;

        var result = actual.ShouldNotBeValue(2);

        Assert.AreEqual(actual, result.Value);
    }

    [Test]
    public void ShouldNotBeValue_Nullable_EqualValue_FailsWithObjectsEqualMessage()
    {
        int? actual = 1;
        var notExpected = 1;
        Error.AreEqual(notExpected, actual, "foo").Returns(ExpectedException);

        AssertThrowsExpectedError(() => actual.ShouldNotBeValue(notExpected, "foo"));
    }

    [Test]
    public void ShouldNotBeValue_Nullable_CorrectlyRegistersAssertion()
    {
        int? actualExpression = 1;
        var expectedExpression = 1;
        Error.AreEqual(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

        AssertThrowsExpectedError(() => actualExpression.ShouldNotBeValue(expectedExpression));

        Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
    }

    [Test]
    public void ShouldBeNull_Class_IsNull_Passes()
    {
        ((object?)null).ShouldBeNull();
    }

    [Test]
    public void ShouldBeNull_Class_NotNull_FailsWithNotEqualToNullMessage()
    {
        var actual = new object();
        Error.NotEqual(null, actual, "foo").Returns(ExpectedException);

        AssertThrowsExpectedError(() => actual.ShouldBeNull("foo"));
    }

    [Test]
    public void ShouldBeNull_Class_CorrectlyRegistersAssertion()
    {
        var actualExpression = new object();
        Error.NotEqual(Arg.Any<object?>(), Arg.Any<object?>(), Arg.Any<string?>()).Returns(ExpectedException);

        AssertThrowsExpectedError(() => actualExpression.ShouldBeNull());

        Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
    }

    [Test]
    public void ShouldBeNull_Struct_IsNull_Passes()
    {
        ((int?)null).ShouldBeNull();
    }

    [Test]
    public void ShouldBeNull_Struct_NotNull_FailsWithNotEqualToNullMessage()
    {
        int? actual = 1;
        Error.NotEqual(null, actual, "foo").Returns(ExpectedException);

        AssertThrowsExpectedError(() => actual.ShouldBeNull("foo"));
    }

    [Test]
    public void ShouldBeNull_Struct_CorrectlyRegistersAssertion()
    {
        int? actualExpression = 1;
        Error.NotEqual(Arg.Any<object?>(), Arg.Any<object?>(), Arg.Any<string?>()).Returns(ExpectedException);

        AssertThrowsExpectedError(() => actualExpression.ShouldBeNull());

        Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
    }

    [Test]
    public void ShouldNotBeNull_Class_NotNull_ReturnsActualValue()
    {
        var actual = new object();

        var result = actual.ShouldNotBeNull();

        Assert.AreSame(actual, result.And);
    }

    [Test]
    public void ShouldNotBeNull_Class_IsNull_FailsWithIsNullMessage()
    {
        Error.IsNull("foo").Returns(ExpectedException);

        AssertThrowsExpectedError(() => ((object?)null).ShouldNotBeNull("foo"));
    }

    [Test]
    public void ShouldNotBeNull_Class_CorrectlyRegistersAssertion()
    {
        object? actualExpression = null;
        Error.IsNull(Arg.Any<string?>()).Returns(ExpectedException);

        AssertThrowsExpectedError(() => actualExpression.ShouldNotBeNull());

        Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
    }

    [Test]
    public void ShouldNotBeNull_NullableStruct_NotNull_ReturnsActualValue()
    {
        int? actual = 1;

        var result = actual.ShouldNotBeNull();

        Assert.AreEqual(actual, result.And);
    }

    [Test]
    public void ShouldNotBeNull_NullableStruct_IsNull_FailsWithIsNullMessage()
    {
        Error.IsNull("foo").Returns(ExpectedException);

        AssertThrowsExpectedError(() => ((int?)null).ShouldNotBeNull("foo"));
    }

    [Test]
    public void ShouldNotBeNull_NullableStruct_CorrectlyRegistersAssertion()
    {
        int? actualExpression = null;
        Error.IsNull(Arg.Any<string?>()).Returns(ExpectedException);

        AssertThrowsExpectedError(() => actualExpression.ShouldNotBeNull());

        Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
    }

    [Test]
    public void ShouldReferTo_SameObject_ReturnsActualValue()
    {
        var obj = new object();
        var result = obj.ShouldReferTo(obj);

        Assert.AreSame(obj, result.And);
    }

    [Test]
    public void ShouldReferTo_DifferentObject_FailsWithObjectsNotSameMessage()
    {
        var actual = new Equatable(1);
        var expected = new Equatable(1);
        Error.NotSame(expected, actual, "foo").Returns(ExpectedException);

        AssertThrowsExpectedError(() => actual.ShouldReferTo(expected, "foo"));
    }

    [Test]
    public void ShouldReferTo_CorrectlyRegistersAssertion()
    {
        var actualExpression = new object();
        var expectedExpression = new object();
        Error.NotSame(Arg.Any<object>(), Arg.Any<object>()).Returns(ExpectedException);

        AssertThrowsExpectedError(() => actualExpression.ShouldReferTo(expectedExpression));

        Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
    }

    [Test]
    public void ShouldNotReferTo_DifferentObject_ReturnsActualValue()
    {
        var actual = new object();

        var result = actual.ShouldNotReferTo(new object());

        Assert.AreSame(actual, result.And);
    }

    [Test]
    public void ShouldNotReferTo_SameObject_FailsWithObjectsAreSameMessage()
    {
        var actual = new object();
        Error.AreSame(actual, "foo").Returns(ExpectedException);

        AssertThrowsExpectedError(() => actual.ShouldNotReferTo(actual, "foo"));
    }

    [Test]
    public void ShouldNotReferTo_CorrectlyRegistersAssertion()
    {
        var actualExpression = new object();
        var expectedExpression = actualExpression;
        Error.AreSame(Arg.Any<object>(), Arg.Any<string>()).Returns(ExpectedException);

        AssertThrowsExpectedError(() => actualExpression.ShouldNotReferTo(expectedExpression));

        Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        Assert.AreEqual(nameof(expectedExpression), TestExpression.GetExpected());
    }

    [Test]
    public void ShouldBeA_SubType_ReturnsTypedActual()
    {
        object actual = new SubEquatable(1);
        var result = actual.ShouldBeA<Equatable>();

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
        object actualExpression = new Equatable(1);
        Error.NotEqual(Arg.Any<object?>(), Arg.Any<object?>(), Arg.Any<string?>()).Returns(ExpectedException);

        AssertThrowsExpectedError(() => actualExpression.ShouldBeA<SubEquatable>());

        Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
    }
}