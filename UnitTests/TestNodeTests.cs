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
            TestNode<int> sutWithNoValue = new TestNode<int>();
            Assert.AreEqual(0, sutWithNoValue.Value);
        }

        [Test]
        public void ConstructedWithValue_ValueIsConstructorValue()
        {
            TestNode<int> sutWithValue = new TestNode<int>(1);
            Assert.AreEqual(1, sutWithValue.Value);
        }

        [Test]
        public void NoChildrenByDefault()
        {
            TestNode<int> sut = new TestNode<int>();
            CollectionAssert.IsEmpty(sut);
        }

        [Test]
        public void Add_AddsAChild()
        {
            TestNode<int> sut = new TestNode<int>();
            TestNode<int> child = new TestNode<int>(1);
            sut.Add(child);
            CollectionAssert.AreEqual(new[] { child }, sut);
        }

        [Test]
        public void AddRange_AddsChildren()
        {
            TestNode<int> sut = new TestNode<int>();
            TestNode<int> child1 = new TestNode<int>(1);
            TestNode<int> child2 = new TestNode<int>(2);

            sut.AddRange(new[] { child1, child2 });

            CollectionAssert.AreEqual(new[] { child1, child2 }, sut);
        }

        [Test]
        public void NodeValueIsNull_ToString_ReturnsEmptyString()
        {
            Assert.AreEqual(string.Empty, new TestNode<object>(null));
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
            TestNode<int> result = 1.Node();
            Assert.AreEqual(1, result.Value);
        }

        [Test]
        public void NodeExtensions_AddsChildren()
        {
            TestNode<int> child1 = new TestNode<int>(1);
            TestNode<int> child2 = new TestNode<int>(2);

            TestNode<int> result = 1.Node(child1, child2);

            CollectionAssert.AreEqual(new[] { child1, child2 }, result);
        }

        [Test]
        public void ValuesExtension_ReturnsNodeValues()
        {
            TestNode<int> node1 = new TestNode<int>(1);
            TestNode<int> node2 = new TestNode<int>(2);

            IEnumerable<int> result = new[] { node1, node2 }.Values();

            CollectionAssert.AreEqual(new[] { 1, 2 }, result);
        }
    }
}