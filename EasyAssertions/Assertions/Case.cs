namespace EasyAssertions;

/// <summary>
/// Specifies case sensitivity for string assertions.
/// </summary>
public enum Case
{
    /// <summary>
    /// Strings must have the same casing.
    /// </summary>
    Sensitive,
    /// <summary>
    /// Ignore string casing.
    /// </summary>
    Insensitive
}