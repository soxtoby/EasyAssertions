using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EasyAssertions
{
    /// <summary>
    /// Helper class for specifying expected tree structures.
    /// Is an enumeration of its children.
    /// </summary>
    /// <example>
    /// var expected = 1.Node(
    ///                     11,
    ///                     12.Node(
    ///                         121,
    ///                         122));
    /// or
    /// var expected = new TestNode&lt;int&gt;(1) {
    ///     11,
    ///     new TestNode&lt;int&gt;(12) {
    ///         121,
    ///         122
    ///     }
    /// };
    /// </example>
    public class TestNode<T> : IEnumerable<TestNode<T>>
    {
        /// <summary>
        /// The value to be compared against the actual node.
        /// </summary>
        public readonly T Value;

        private readonly List<TestNode<T>> children = new();

        /// <summary>
        /// Constructs a root node with no value.
        /// </summary>
        public TestNode()
        {
            Value = default!;
        }

        /// <summary>
        /// Constructs a node with a value.
        /// </summary>
        public TestNode(T value)
            : this()
        {
            Value = value;
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="TestNode{T}"/>'s children collection.
        /// </summary>
        public void AddRange(IEnumerable<TestNode<T>> childNodes)
        {
            foreach (TestNode<T> childNode in childNodes)
                Add(childNode);
        }

        /// <summary>
        /// Adds a single child to the end of the <see cref="TestNode{T}"/>'s children collection.
        /// </summary>
        public TestNode<T> Add(TestNode<T> childNode)
        {
            children.Add(childNode);
            return this;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="TestNode{T}"/>'s children.
        /// </summary>
        public IEnumerator<TestNode<T>> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="TestNode{T}"/>'s children.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a string that represents the <see cref="TestNode{T}"/>'s value.
        /// </summary>
        public override string ToString()
        {
            return string.Empty + Value;
        }

        /// <summary>
        /// Constructs a node with a value.
        /// </summary>
        public static implicit operator TestNode<T>(T value)
        {
            return new TestNode<T>(value);
        }
    }

    /// <summary>
    /// Helper methods for working with <see cref="TestNode{T}"/>.
    /// </summary>
    public static class TestNode
    {
        /// <summary>
        /// Creates a node with the specified value and children.
        /// </summary>
        public static TestNode<T> Node<T>(this T value, params TestNode<T>[] children)
        {
            TestNode<T> node = value;
            node.AddRange(children);
            return node;
        }

        /// <summary>
        /// Returns the <see cref="TestNode{T}.Value"/>s from a collection of <see cref="TestNode{T}"/>s.
        /// </summary>
        public static IEnumerable<T> Values<T>(this IEnumerable<TestNode<T>> nodes)
        {
            return nodes.Select(n => n.Value);
        }
    }
}