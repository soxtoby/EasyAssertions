﻿using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class EasyAssertionTests
    {
        private IFailureMessageFormatter mockFormatter;

        [SetUp]
        public void SetUp()
        {
            mockFormatter = Substitute.For<IFailureMessageFormatter>();
            FailureMessageFormatter.Override(mockFormatter);
        }

        [TearDown]
        public void TearDown()
        {
            FailureMessageFormatter.Default();
        }

        [Test]
        public void ShouldEqual_SameValue_Passes()
        {
            1.ShouldEqual(1);
        }

        [Test]
        public void ShouldEqual_ReturnsActualValue()
        {
            Equatable actual = new Equatable(1);
            Equatable expected = new Equatable(1);
            Actual<Equatable> result = actual.ShouldEqual(expected);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldEqual_DifferentObjects_FailsWithObjectsNotEqualMessage()
        {
            object obj1 = new object();
            object obj2 = new object();
            mockFormatter.NotEqual(obj2, obj1, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => obj1.ShouldEqual(obj2, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldEqual_DifferentStrings_FailsWithStringsNotEqualMessage()
        {
            mockFormatter.NotEqual("foo", "bar", "baz").Returns("qux");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => "bar".ShouldEqual("foo", "baz"));

            Assert.AreEqual("qux", result.Message);
        }

        [Test]
        public void ShouldBe_SameObject_Passes()
        {
            object obj = new object();
            obj.ShouldBe(obj);
        }

        [Test]
        public void ShouldBe_ReturnsActualValue()
        {
            object obj = new object();
            Actual<object> result = obj.ShouldBe(obj);

            Assert.AreSame(obj, result.And);
        }

        [Test]
        public void ShouldBe_DifferentObject_FailsWithObjectsNotSameMessage()
        {
            Equatable actual = new Equatable(1);
            Equatable expected = new Equatable(1);
            mockFormatter.NotSame(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBe(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldMatch_MatchingEnumerable_Passes()
        {
            new[] { 1, 2, 3 }.ShouldMatch(new[] { 1, 2, 3 });
        }

        [Test]
        public void ShouldMatch_ReturnsActualValue()
        {
            int[] actual = new[] { 1, 2 };
            int[] expected = new[] { 1, 2 };
            Actual<int[]> result = actual.ShouldMatch(expected);

            Assert.AreSame(actual, result.And);
        }

        [Test]
        public void ShouldMatch_NonMatchingEnumerables_FailsWithEnumerablesDoNotMatchMessage()
        {
            int[] expected = new[] { 3, 4 };
            int[] actual = new[] { 1, 2 };
            mockFormatter.DoNotMatch(expected, actual, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldMatch(expected, "foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldBeA_SubType_Passes()
        {
            object actual = new SubEquatable(1);
            actual.ShouldBeA<Equatable>();
        }

        [Test]
        public void ShouldBeA_ReturnsTypedActual()
        {
            object actual = new Equatable(1);
            Actual<Equatable> result = actual.ShouldBeA<Equatable>();

            Assert.AreSame(actual, result.And);
            Assert.AreEqual(1, result.And.Value);
        }

        [Test]
        public void ShouldBeA_SuperType_FailsWithTypesNotEqualMessage()
        {
            object actual = new Equatable(1);
            mockFormatter.NotEqual(typeof(SubEquatable), typeof(Equatable), "foo").Returns("bar");
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => actual.ShouldBeA<SubEquatable>("foo"));

            Assert.AreEqual("bar", result.Message);
        }

        [Test]
        public void ShouldContain_StringDoesContainsSubstring_Passes()
        {
            "1234".ShouldContain("23");
        }

        [Test]
        public void ShouldContain_ReturnsActualValue()
        {
            Actual<string> result = "foo".ShouldContain("oo");
            Assert.AreEqual("foo", result.And);
        }

        [Test]
        public void ShouldContain_StringDoesNotContainSubstring_FailsWithStringDoesNotContainMessage()
        {
            mockFormatter.DoesNotContain("bar", "foo", "baz").Returns("qux");
            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => "foo".ShouldContain("bar", "baz"));

            Assert.AreEqual("qux", result.Message);
        }

        private class Equatable
        {
            public readonly int Value;

            public Equatable(int value)
            {
                this.Value = value;
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

        private class SubEquatable : Equatable
        {
            public SubEquatable(int value) : base(value) { }
        }
    }
}