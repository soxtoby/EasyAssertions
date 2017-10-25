using System;

namespace EasyAssertions
{
    /// <summary>
    /// Provides factory methods for creating custom assertion failure messages.
    /// </summary>
    public interface IErrorFactory
    {
        /// <summary>
        /// Creates an <see cref="Exception"/> to be thrown for a failed assertion that includes the source representation of the actual value.
        /// </summary>
        Exception WithActualExpression(string message);

        /// <summary>
        /// Creates an <see cref="Exception"/> to be thrown for a failed assertion that includes the source representation of the actual value.
        /// </summary>
        Exception WithActualExpression(string message, Exception innerException);

        /// <summary>
        /// Creates an <see cref="Exception"/> to be thrown for a failed assertion.
        /// </summary>
        Exception Custom(string message);

        /// <summary>
        /// Creates an <see cref="Exception"/> to be thrown for a failed assertion.
        /// </summary>
        Exception Custom(string message, Exception innerException);
    }
}