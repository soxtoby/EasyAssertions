using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EasyAssertions
{
    /// <summary>
    /// A helper class for building function and exception assertion failure messages in a consistent format.
    /// </summary>
    public class FunctionFailureMessage : FailureMessage
    {
        public FunctionFailureMessage(LambdaExpression function)
        {
            Function = function;
        }

        /// <summary>
        /// The function being asserted on.
        /// </summary>
        protected LambdaExpression Function { get; private set; }

        /// <summary>
        /// Outputs the source representation of the <see cref="Function"/>.
        /// </summary>
        public override string ActualExpression { get { return CleanFunctionBody(Function); } }

        /// <summary>
        /// The <see cref="Type"/> of exception that the <see cref="Function"/> was expected to throw.
        /// </summary>
        public Type ExpectedExceptionType { get; set; }

        /// <summary>
        /// The <see cref="MemberInfo.Name"/> of the <see cref="ExpectedExceptionType"/>, wrapped in &lt; &gt;.
        /// </summary>
        public string ExpectedExceptionName
        {
            get { return "<" + ExpectedExceptionType.Name + ">"; }
        }

        /// <summary>
        /// The <see cref="Type"/> of exception that the <see cref="Function"/> threw.
        /// </summary>
        public Type ActualExceptionType { get; set; }

        /// <summary>
        /// The <see cref="MemberInfo.Name"/> of the <see cref="ActualExceptionType"/>, wrapped in &lt; &gt;.
        /// </summary>
        public string ActualExceptionName
        {
            get { return "<" + ActualExceptionType.Name + ">"; }
        }

        private static readonly Regex MemberPattern = new Regex(@"value\(.*?\)\.", RegexOptions.Compiled);

        private static string CleanFunctionBody(LambdaExpression function)
        {
            return MemberPattern.Replace(function.Body.ToString(), string.Empty);
        }
    }
}