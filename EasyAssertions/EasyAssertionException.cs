namespace EasyAssertions;

/// <summary>
/// The default exception type thrown by failed assertions.
/// The type of exceptions thrown can be changed using <see cref="EasyAssertion.UseFrameworkExceptions">EasyAssertion.UseFrameworkExceptions</see>.
/// </summary>
public class EasyAssertionException : Exception
{
    /// <inheritdoc />
    public override string ToString()
    {
        return Message + Environment.NewLine + Environment.NewLine + StackTrace;
    }

    internal EasyAssertionException(string message)
        : base(message)
    {
    }

    internal EasyAssertionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}