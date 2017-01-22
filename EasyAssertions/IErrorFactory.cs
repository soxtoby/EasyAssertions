using System;

namespace EasyAssertions
{
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