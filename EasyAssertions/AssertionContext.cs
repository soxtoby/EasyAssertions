using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace EasyAssertions
{
    /// <summary>
    /// Helpers for creating custom assertions that behave consistently.
    /// </summary>
    public interface IAssertionContext
    {
        /// <summary>
        /// Provides a set of standard testing functions for implementing assertion logic.
        /// </summary>
        StandardTests Test { get; }

        /// <summary>
        /// The standard set of assertion failure messages.
        /// </summary>
        IStandardErrors StandardError { get; }

        /// <summary>
        /// Provides factory methods for creating custom assertion failure messages.
        /// </summary>
        IErrorFactory Error { get; }

        /// <summary>
        /// Call another method with assertion logic.
        /// </summary>
        /// <param name="callAssertionMethod">Expression that calls a method with assertion logic.</param>
        /// <param name="actualSuffix">String appended to the actual source expression in failure messages.</param>
        /// <param name="expectedSuffix">String appended to the expected source expression in failure messages.</param>
        void Call([InstantHandle] Expression<Action> callAssertionMethod, string actualSuffix = "", string expectedSuffix = "");
    }

    class AssertionContext : IAssertionContext
    {
        public StandardTests Test => StandardTests.Instance;
        public IStandardErrors StandardError => StandardErrors.Current;
        public IErrorFactory Error => ErrorFactory.Instance;

        public void Call(Expression<Action> callAssertionMethod, string actualSuffix = "", string expectedSuffix = "") =>
            SourceExpressionProvider.ForCurrentThread.InvokeAssertion(callAssertionMethod, actualSuffix, expectedSuffix);
    }
}