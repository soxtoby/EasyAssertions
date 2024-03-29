﻿using System.Collections;

namespace EasyAssertions;

/// <summary>
/// Represents a buffered <see cref="IEnumerable{T}"/>.
/// The buffer can be enumerated multiple times without enumerating the source enumerable more than once.
/// </summary>
public interface IBuffer<out T> : IReadOnlyList<T>, IDisposable { }

/// <summary>
/// Wraps an <see cref="IEnumerable{T}"/> in a buffer.
/// The buffer can be enumerated multiple times without enumerating the source enumerable more than once.
/// </summary>
public static class Buffer
{
    /// <summary>
    /// Creates a buffer based on the specified source.
    /// </summary>
    public static IBuffer<T> Create<T>(IEnumerable<T> source)
    {
        return new Buffer<T>(source);
    }
}

class Buffer<T> : IBuffer<T>
{
    readonly List<T> buffer = new();
    readonly IEnumerator<T> sourceEnumerator;

    public Buffer(IEnumerable<T> source)
    {
        sourceEnumerator = source.GetEnumerator();
    }

    public IEnumerator<T> GetEnumerator()
    {
        var i = -1;

        while (EnumerateTo(++i))
            yield return buffer[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public T this[int index]
    {
        get
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (!EnumerateTo(index))
                throw new ArgumentOutOfRangeException(nameof(index));
            return buffer[index];
        }
    }

    public int Count
    {
        get
        {
            EnumerateTo(int.MaxValue);
            return buffer.Count;
        }
    }

    bool EnumerateTo(int index)
    {
        while (index >= buffer.Count)
        {
            if (!sourceEnumerator.MoveNext())
                return false;
            buffer.Add(sourceEnumerator.Current);
        }
        return true;
    }

    public void Dispose()
    {
        sourceEnumerator.Dispose();
    }
}