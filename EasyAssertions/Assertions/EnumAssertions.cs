﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyAssertions
{
    /// <summary>
    /// Assertions for Enums.
    /// </summary>
    public static class EnumAssertions
    {
        /// <summary>
        /// Asserts that two value types are equal, using the default equality comparer.
        /// </summary>
        public static Actual<T> ShouldBeValue<T>(this T actual, T expected, string message = null) where T : struct
        {
            return actual.RegisterAssertion(c =>
                {
                    if (!c.Test.ObjectsAreEqual(actual, expected))
                        throw c.StandardError.NotEqual(expected, actual, message);
                });
        }

        /// <summary>
        /// Asserts that a <code>[Flags]</code> enum has the specified flag set.
        /// </summary>
        public static Actual<T> ShouldHaveFlag<T>(this T actual, T expectedFlag, string message = null) where T : struct
        {
            return actual.RegisterAssertion(c =>
                {
                    if (!typeof(Enum).IsAssignableFrom(typeof(T)))
                        throw c.StandardError.NotEqual(typeof(Enum), actual.GetType(), message);

                    if (typeof(T).GetCustomAttributes(typeof(FlagsAttribute), false).None())
                        throw c.StandardError.DoesNotContain(new FlagsAttribute(), typeof(T).GetCustomAttributes(false), message);

                    Enum actualEnum = actual as Enum;
                    Enum expectedFlagEnum = expectedFlag as Enum;
                    if (!actualEnum.HasFlag(expectedFlagEnum))
                        throw c.StandardError.DoesNotContain(expectedFlag, Flags<T>(actualEnum), message);
                });
        }

        private static IEnumerable<T> Flags<T>(Enum actualEnum) where T : struct
        {
            object zeroValue = Enum.Parse(typeof(T), "0");
            return Enum.GetValues(typeof(T))
                .Cast<Enum>()
                .Where(v => !v.Equals(zeroValue) && actualEnum.HasFlag(v))
                .Cast<T>();
        }
    }
}