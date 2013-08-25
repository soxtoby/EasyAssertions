using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EasyAssertions
{
    public class TestNode<T> : IEnumerable<TestNode<T>>
    {
        public readonly T Value;

        private readonly List<TestNode<T>> children = new List<TestNode<T>>();

        public TestNode()
        {
        }

        public TestNode(T value)
            : this()
        {
            Value = value;
        }

        public void AddRange(IEnumerable<TestNode<T>> childNodes)
        {
            foreach (TestNode<T> childNode in childNodes)
                Add(childNode);
        }

        public TestNode<T> Add(TestNode<T> childNode)
        {
            children.Add(childNode);
            return this;
        }

        public IEnumerator<TestNode<T>> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Empty + Value;
        }

        public static implicit operator TestNode<T>(T value)
        {
            return new TestNode<T>(value);
        }
    }

    public static class TestNode
    {
        public static TestNode<T> Node<T>(this T value, params TestNode<T>[] children)
        {
            TestNode<T> node = new TestNode<T>(value);
            foreach (TestNode<T> child in children)
                node.Add(child);
            return node;
        }

        public static IEnumerable<T> Values<T>(this IEnumerable<TestNode<T>> nodes)
        {
            return nodes.Select(n => n.Value);
        }
    }
}