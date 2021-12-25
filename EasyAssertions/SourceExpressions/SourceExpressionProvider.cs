using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace EasyAssertions
{
    class SourceExpressionProvider : ITestExpressionProvider
    {
        List<AssertionFrame> stack = new();
        int currentStackIndex;
        int lastStackIndex;

        static readonly AsyncLocal<SourceExpressionProvider> LocalInstance = new();
        public static SourceExpressionProvider Current => LocalInstance.Value ??= new SourceExpressionProvider();

        SourceExpressionProvider() { }

        public void RunAssertion(Action<IAssertionContext> assert, string actualSuffix = "", string expectedSuffix = "") =>
            TrackAssertion(assert, StackAnalyser.MethodCall(2), actualSuffix, expectedSuffix);

        public void InvokeAssertion(Expression<Action> callAssertionMethod, string actualSuffix = "", string expectedSuffix = "") =>
            TrackAssertion(c => callAssertionMethod.Compile()(), new AssertionInvocation(callAssertionMethod), actualSuffix, expectedSuffix);

        void TrackAssertion(Action<IAssertionContext> assert, AssertionCall assertion, string actualSuffix, string expectedSuffix)
        {
            try
            {
                EnterAssertion(assertion, actualSuffix, expectedSuffix);
                assert(new AssertionContext());
                ExitAssertion(true);
            }
            catch
            {
                ExitAssertion(false);
                throw;
            }
        }

        void EnterAssertion(AssertionCall assertion, string actualSuffix, string expectedSuffix)
        {
            if (CurrentAssertionFrame?.TryChainAssertion(assertion) == true)
                DiscardPastCurrentFrame();
            else
                Push(assertion, actualSuffix, expectedSuffix);

            currentStackIndex++;
            lastStackIndex = currentStackIndex - 1;
        }

        void DiscardPastCurrentFrame()
        {
            stack = stack.Take(currentStackIndex + 1).ToList();
        }

        void Push(AssertionCall assertion, string actualSuffix, string expectedSuffix)
        {
            stack = stack.Take(currentStackIndex).ToList();
            stack.Add(assertion.CreateFrame(stack.LastOrDefault(), actualSuffix, expectedSuffix));
        }

        void ExitAssertion(bool success)
        {
            currentStackIndex--;
            if (success)
                lastStackIndex--;
        }

        public string GetActualExpression() => NormalizeIndentation(LastAssertionFrame?.GetActualExpression() ?? string.Empty);
        public string GetExpectedExpression() => NormalizeIndentation(LastAssertionFrame?.GetExpectedExpression() ?? string.Empty);

        static string NormalizeIndentation(string input)
        {
            var lines = input.Split('\n');

            var minIndent = lines
                .Skip(1)
                .Select(l => l.Length - l.TrimStart(' ').Length)
                .Aggregate(int.MaxValue, Math.Min);

            return minIndent <= 4
                ? input
                : lines.Take(1)
                    .Concat(lines
                        .Skip(1)
                        .Select(l => l.Substring(minIndent - 4)))
                    .Join("\n");
        }

        AssertionFrame? CurrentAssertionFrame => stack.ElementAtOrDefault(currentStackIndex);
        AssertionFrame? LastAssertionFrame => stack.ElementAtOrDefault(lastStackIndex);
    }
}