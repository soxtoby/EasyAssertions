using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyAssertions
{
    internal class SourceExpressionProvider : TestExpressionProvider
    {
        private readonly List<AssertionComponentGroup> assertionGroupChain = new List<AssertionComponentGroup>();

        private AssertionComponentGroup CurrentGroup => assertionGroupChain.LastOrDefault();

        [ThreadStatic]
        private static SourceExpressionProvider threadInstance;

        public static SourceExpressionProvider ForCurrentThread => threadInstance ?? (threadInstance = new SourceExpressionProvider());

        private SourceExpressionProvider() { }

        public string GetActualExpression()
        {
            return GetActualExpression(assertionGroupChain);
        }

        private static string GetActualExpression(IEnumerable<AssertionComponentGroup> assertionComponentGroups)
        {
            return NormalizeIndentation(assertionComponentGroups.Aggregate(string.Empty, (expression, group) => @group.GetActualExpression(expression)));
        }

        public string GetExpectedExpression()
        {
            if (assertionGroupChain.Count < 2)
                return string.Empty;

            // The last group will be _inside_ the component that has the expected parameter
            AssertionComponentGroup assertionGroupEntryPoint = assertionGroupChain.SkipLast(1).Last();

            // In case the expected expression references the actual value, get the actual expression from outside the component
            string actualExpression = GetActualExpression(assertionGroupChain.SkipLast(2));

            return NormalizeIndentation(assertionGroupEntryPoint.GetExpectedExpression(actualExpression));
        }

        private static string NormalizeIndentation(string input)
        {
            string[] lines = input.Split('\n');

            int minIndent = lines
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

        public void EnterAssertion(int assertionFrameIndex)
        {
            assertionFrameIndex++;

            StackAnalyser analyser = StackAnalyser.ForCurrentStack();

            RegisterComponent((address, methodName) => new AssertionMethod(address, methodName), assertionFrameIndex, analyser);

            EnterNestedAssertion(analyser.GetMethod(assertionFrameIndex), assertionFrameIndex);
        }

        public void RegisterContinuation(int continuationFrameIndex)
        {
            RegisterComponent((address, methodName) => new Continuation(address, methodName), continuationFrameIndex + 1, StackAnalyser.ForCurrentStack());
        }

        private void RegisterComponent(Func<SourceAddress, string, AssertionComponent> createMethod, int assertionFrameIndex, StackAnalyser analyser)
        {
            PopStackBackToAssertion(assertionFrameIndex, analyser);

            SourceAddress callAddress = analyser.GetCallAddress(assertionFrameIndex);
            string assertionName = analyser.GetMethodName(assertionFrameIndex);
            AssertionComponent method = createMethod(callAddress, assertionName);

            CurrentGroup.AddComponent(method);
        }

        private void PopStackBackToAssertion(int assertionFrameIndex, StackAnalyser analyser)
        {
            int groupPosition = analyser.GetParentGroupPosition(assertionFrameIndex, assertionGroupChain);
            assertionGroupChain.RemoveRange(groupPosition, assertionGroupChain.Count - groupPosition);

            if (assertionGroupChain.None())
                assertionGroupChain.Add(new BaseGroup());
        }

        public void EnterNestedAssertion(MethodBase innerAssertionMethod, int assertionFrameIndex)
        {
            assertionFrameIndex++;
            AddGroup(innerAssertionMethod, assertionFrameIndex, (callAddress, expressionAlias) =>
                new NestedAssertionGroup(callAddress, expressionAlias));
        }

        public void EnterIndexedAssertion(MethodInfo innerAssertionMethod, int index, int assertionFrameIndex)
        {
            assertionFrameIndex++;
            AddGroup(innerAssertionMethod, assertionFrameIndex, (callAddress, expressionAlias) =>
                new IndexedAssertionGroup(callAddress, GetExpressionAlias(innerAssertionMethod), index));
        }

        private void AddGroup(MethodBase innerAssertionMethod, int callFrameIndex, Func<SourceAddress, string, AssertionComponentGroup> createGroup)
        {
            SourceAddress callerAddress = StackAnalyser.ForCurrentStack().GetCallAddress(callFrameIndex + 1);
            string expressionAlias = GetExpressionAlias(innerAssertionMethod);
            assertionGroupChain.Add(createGroup(callerAddress, expressionAlias));
        }

        private static string GetExpressionAlias(MethodBase innerAssertion)
        {
            return innerAssertion.GetParameters().FirstOrDefault()?.Name ?? string.Empty;
        }

        public void ExitAssertion()
        {
            assertionGroupChain.Remove(CurrentGroup);
        }

        public void Reset()
        {
            assertionGroupChain.Clear();
        }
    }
}
