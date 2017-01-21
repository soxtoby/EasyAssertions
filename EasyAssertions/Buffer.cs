using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EasyAssertions
{
    public interface IBuffer<out T> : IEnumerable<T>, IDisposable
    {
        T this[int index] { get; }
    }

    public class Buffer<T> : IBuffer<T>
    {
        private readonly List<T> buffer = new List<T>();
        private readonly IEnumerator<T> sourceEnumerator;

        public Buffer(IEnumerable<T> source)
        {
            sourceEnumerator = source.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            int i = -1;

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
                if (!EnumerateTo(index))
                    throw new ArgumentOutOfRangeException(nameof(index));
                return buffer[index];
            }
        }

        private bool EnumerateTo(int index)
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
}