namespace EasyAssertions;

/// <summary>
/// A wrapper around the value returned by an assertion, used for chaining further assertions.
/// </summary>
public class Actual<T>
{
    internal readonly T Value;

    /// <inheritdoc cref="Actual{T}" />
    public Actual(T actual)
    {
        Value = actual;
    }

    /// <summary>
    /// Provides access to the actual value, to allow assertions to be chained.
    /// </summary>
    public T And
    {
        get
        {
            Value.RegisterAssertion(c => { });
            return Value;
        }
    }
}