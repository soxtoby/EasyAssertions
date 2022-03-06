using System.Collections;

namespace EasyAssertions.UnitTests;

class TestEnumerable<T> : IEnumerable<T>
{
    readonly IEnumerable<T> items;

    public int EnumerationCount { get; private set; }
    public bool EnumerationCompleted { get; private set; }

    public TestEnumerable(IEnumerable<T> items)
    {
        this.items = items;
    }

    public IEnumerator<T> GetEnumerator()
    {
        EnumerationCount++;
        foreach (var item in items)
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