using System.Collections;
using System.Collections.Generic;

namespace EasyAssertions.UnitTests
{
    internal class TestEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> items;

        public int EnumerationCount { get; private set; }
        public bool EnumerationCompleted { get; private set; }

        public TestEnumerable(IEnumerable<T> items)
        {
            this.items = items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            EnumerationCount++;
            foreach (T item in items)
            {
                yield return item;
            }
            EnumerationCompleted = true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
