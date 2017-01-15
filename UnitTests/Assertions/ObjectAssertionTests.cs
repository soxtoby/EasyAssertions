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
            MockFormatter.NotEqual(obj2, obj1, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => obj1.ShouldBe(obj2, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBe_DifferentStrings_FailsWithStringsNotEqualMessage()
        {
            MockFormatter.NotEqual("foo", "bar", message: "baz").Returns("qux");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => "bar".ShouldBe("foo", "baz"));

            Assert.AreEqual("qux", result.Message);
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
            MockFormatter.NotEqual(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBe(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void NullableShouldBe_ValueIsDifferent_FailsWithObjectsNotEqualMessage()
        {
            int? actual = 1;
            const int expected = 2;
            MockFormatter.NotEqual(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBe(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
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
            MockFormatter.AreEqual(notExpected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotBe(notExpected, "foo"));

            Assert.AreEqual("bar", result.Message);
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
            MockFormatter.NotEqual(null, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeNull("foo"));

            Assert.AreEqual("bar", result.Message);
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
            MockFormatter.IsNull("foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => ((object)null).ShouldNotBeNull("foo"));

            Assert.AreEqual("bar", result.Message);
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
            MockFormatter.NotSame(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldReferTo(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
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
            MockFormatter.AreSame(actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldNotReferTo(actual, "foo"));

            Assert.AreEqual("bar", result.Message);
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
            MockFormatter.NotEqual(typeof(SubEquatable), typeof(Equatable), "foo").Returns("bar");
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeA<SubEquatable>("foo"));

            Assert.AreEqual("bar", result.Message);
        }

        protected class Equatable
        {
            public readonly int Value;

            public Equatable(int value)
            {
                Value = value;
            }

            public override bool Equals(object obj)
            {
                Equatable otherEquatable = obj as Equatable;
                return otherEquatable != null
                    && otherEquatable.Value == Value;
            }

            public override int GetHashCode()
            {
                return Value;
            }
        }

        protected class SubEquatable : Equatable
        {
            public SubEquatable(int value) : base(value) { }
        }
    }
}