using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace EasyAssertions
{
    public class FunctionFailureMessage : FailureMessage
    {
        public FunctionFailureMessage(LambdaExpression function)
        {
            Function = function;
        }

        public LambdaExpression Function { get; private set; }

        public override string ActualExpression { get { return CleanFunctionBody(Function); } }

        public Type ExpectedExceptionType { get; set; }

        public string ExpectedExceptionName
        {
            get { return "<" + ExpectedExceptionType.Name + ">"; }
        }

        public Type ActualExceptionType { get; set; }

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