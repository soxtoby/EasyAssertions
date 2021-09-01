using NUnit.Framework;
using System.Collections.Generic;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class TestNodeTests
    {
        [Test]
        public void ConstructedWithNoValues_ValueIsDefault()
        {
            var sutWithNoValue = new TestNode<int>();
            Assert.AreEqual(0, sutWithNoValue.Value);
        }

        [Test]
        public void ConstructedWithValue_ValueIsConstructorValue()
        {
            var sutWithValue = new TestNode<int>(1);
            Assert.AreEqual(1, sutWithValue.Value);
        }

        [Test]
        public void NoChildrenByDefault()
        {
            var sut = new TestNode<int>();
            CollectionAssert.IsEmpty(sut);
        }

        [Test]
        public void Add_AddsAChild()
        {
            var sut = new TestNode<int>();
            var child = new TestNode<int>(1);
            sut.Add(child);
            CollectionAssert.AreEqual(new[] { child }, sut);
        }

        [Test]
        public void AddRange_AddsChildren()
        {
            var sut = new TestNode<int>();
            var child1 = new TestNode<int>(1);
            var child2 = new TestNode<int>(2);

            sut.AddRange(new[] { child1, child2 });

            CollectionAssert.AreEqual(new[] { child1, child2 }, sut);
        }

        [Test]
        public void NodeValueIsNull_ToString_ReturnsEmptyString()
        {
            Assert.AreEqual(string.Empty, new TestNode<object>());
        }

        [Test]
        public void NodeHasValue_ToString_ReturnsValueAsString()
        {
            Assert.AreEqual("1", new TestNode<int>(1).ToString());
        }

        [Test]
        public void ImplicitConversion_ValueIsConvertedValue()
        {
            TestNode<int> sut = 1;
            Assert.AreEqual(1, sut.Value);
        }

        [Test]
        public void NodeExtension_CreatesNodeWithCorrectValue()
        {
            var result = 1.Node();
            Assert.AreEqual(1, result.Value);
        }

        [Test]
        public void NodeExtensions_AddsChildren()
        {
            var child1 = new TestNode<int>(1);
            var child2 = new TestNode<int>(2);

            var result = 1.Node(child1, child2);

            CollectionAssert.AreEqual(new[] { child1, child2 }, result);
        }

        [Test]
        public void ValuesExtension_ReturnsNodeValues()
        {
            var node1 = new TestNode<int>(1);
            var node2 = new TestNode<int>(2);

            var result = new[] { node1, node2 }.Values();

            CollectionAssert.AreEqual(new[] { 1, 2 }, result);
        }
    }
}