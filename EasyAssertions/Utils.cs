using System.Collections;
using System.Text;

namespace EasyAssertions;

static class Utils
{
    public static string Join<T>(this IEnumerable<T> items, string delimiter)
    {
        var sb = new StringBuilder();
        var delimit = false;
        foreach (var item in items)
        {
            if (delimit)
                sb.Append(delimiter);
            else
                delimit = true;

            sb.Append(item);
        }
        return sb.ToString();
    }

    public static bool None<T>(this IEnumerable<T> enumerable)
    {
        return !enumerable.Any();
    }

    public static string? NullIfEmpty(this string value)
    {
        return value == string.Empty
            ? null
            : value;
    }

    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> sequence, int count)
    {
        var queue = new Queue<T>();
        using var enumerator = sequence.GetEnumerator();

        while (enumerator.MoveNext())
        {
            queue.Enqueue(enumerator.Current);
            if (queue.Count > count)
                yield return queue.Dequeue();
        }
    }

    public static IEnumerable<TOut> Zip<TLeft, TRight, TOut>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TRight, TOut> select)
    {
        using var leftEnumerator = left.GetEnumerator();
        using var rightEnumerator = right.GetEnumerator();

        while (leftEnumerator.MoveNext() && rightEnumerator.MoveNext())
            yield return select(leftEnumerator.Current, rightEnumerator.Current);
    }

    public static IBuffer<object> Buffer(this IEnumerable source)
    {
        return source.Cast<object>().Buffer();
    }

    public static IBuffer<T> Buffer<T>(this IEnumerable<T> source)
    {
        return new Buffer<T>(source);
    }

    public static bool TryReadSource(SourceAddress address, out ReadOnlySpan<char> sourceSpan)
    {
        try
        {
            if (address.FilePath is not null)
            {
                sourceSpan = File.ReadAllLines(address.FilePath)
                    .Skip(address.LineNumber - 1)
                    .Join(Environment.NewLine)
                    .AsSpan()
                    .Slice(address.ColumnNumber - 1);
                return true;
            }
        }
        catch
        {
            // ignored
        }

        sourceSpan = default;
        return false;
    }
}