namespace EasyAssertions;

/// <summary>
/// Assertions for Enums.
/// </summary>
public static class EnumAssertions
{
    /// <summary>
    /// Asserts that a <c>[Flags]</c> enum has the specified flag set.
    /// </summary>
    public static Actual<T> ShouldHaveFlag<T>(this T actual, T expectedFlag, string? message = null) where T : struct
    {
        return actual.RegisterAssertion(c =>
            {
                if (!typeof(Enum).IsAssignableFrom(typeof(T)))
                    throw c.StandardError.NotEqual(typeof(Enum), actual.GetType(), message);

                if (typeof(T).GetCustomAttributes(typeof(FlagsAttribute), false).None())
                    throw c.StandardError.DoesNotContain(new FlagsAttribute(), typeof(T).GetCustomAttributes(false), message);

                var actualEnum = (actual as Enum)!;
                var expectedFlagEnum = (expectedFlag as Enum)!;
                if (!actualEnum.HasFlag(expectedFlagEnum))
                    throw c.StandardError.DoesNotContain(expectedFlag, Flags<T>(actualEnum), message);
            });
    }

    static IEnumerable<T> Flags<T>(Enum actualEnum) where T : struct
    {
        var zeroValue = Enum.Parse(typeof(T), "0");
        return Enum.GetValues(typeof(T))
            .Cast<Enum>()
            .Where(v => !v.Equals(zeroValue) && actualEnum.HasFlag(v))
            .Cast<T>();
    }
}