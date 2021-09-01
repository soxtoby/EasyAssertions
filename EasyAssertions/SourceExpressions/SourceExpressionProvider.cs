using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyAssertions
{
    internal class SourceExpressionProvider : ITestExpressionProvider
    {
        private readonly List<AssertionComponentGroup> assertionGroupChain = new();

        private AssertionComponentGroup CurrentGroup => assertionGroupChain.Last();

        [ThreadStatic]
        private static SourceExpressionProvider? threadInstance;

        public static SourceExpressionProvider ForCurrentThread => threadInstance ??= new SourceExpressionProvider();

        private SourceExpressionProvider() { }

        public string GetActualExpression()
        {
            return GetActualExpression(assertionGroupChain);
        }

        private static string GetActualExpression(IEnumerable<AssertionComponentGroup> assertionComponentGroups)
        {
            return NormalizeIndentation(assertionComponentGroups.Aggregate(string.Empty, (expression, group) => group.GetActualExpression(expression)));
        }

        public string GetExpectedExpression()
        {
            if (assertionGroupChain.Count < 2)
                return string.Empty;

            // The last group will be _inside_ the component that has the expected parameter
            var groupsUpToAssertion = assertionGroupChain.SkipLast(1);

            // In case the expected expression references the actual value, get the actual expression from outside the component
            var actualExpression = GetActualExpression(assertionGroupChain.SkipLast(2));

            return NormalizeIndentation(groupsUpToAssertion.Aggregate(string.Empty, (expression, group) => group.GetExpectedExpression(actualExpression, expression)));
        }

        private static string NormalizeIndentation(string input)
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

        public void EnterAssertion(int assertionFrameIndex)
        {
            assertionFrameIndex++;

            var analyser = StackAnalyser.ForCurrentStack();

            RegisterComponent((address, methodName) => new AssertionMethod(address, methodName), assertionFrameIndex, analyser);

            EnterNestedAssertion(analyser.GetMethod(assertionFrameIndex), assertionFrameIndex, analyser);
        }

        public void EnterAssertion(MethodInfo method, int assertionFrameIndex)
        {
            assertionFrameIndex++;

            var analyser = StackAnalyser.ForCurrentStack();

            RegisterComponent((address, methodName) => new AssertionMethod(address, methodName), assertionFrameIndex, analyser);

            EnterNestedAssertion(method, assertionFrameIndex, analyser);
        }

        private void EnterNestedAssertion(MethodBase innerAssertionMethod, int assertionFrameIndex, StackAnalyser analyser)
        {
            AddGroup(innerAssertionMethod, assertionFrameIndex, (callAddress, actualAlias, expectedAlias) =>
                new NestedAssertionGroup(callAddress, actualAlias, expectedAlias), analyser);
        }

        public void EnterIndexedAssertion(int index, int assertionFrameIndex)
        {
            assertionFrameIndex++;

            var analyser = StackAnalyser.ForCurrentStack();

            RegisterComponent((address, methodName) => new AssertionMethod(address, methodName), assertionFrameIndex, analyser);

            AddGroup(analyser.GetMethod(assertionFrameIndex), assertionFrameIndex, (callAddress, actualAlias, expectedAlias) =>
                new IndexedAssertionGroup(callAddress, actualAlias, expectedAlias, index), analyser);
        }

        private void AddGroup(MethodBase innerAssertionMethod, int callFrameIndex, Func<SourceAddress, string, string, AssertionComponentGroup> createGroup, StackAnalyser analyser)
        {
            var callerAddress = analyser.GetCallAddress(callFrameIndex);
            var actualAlias = GetExpressionAlias(innerAssertionMethod, 0);
            var expectedAlias = GetExpressionAlias(innerAssertionMethod, 1);
            assertionGroupChain.Add(createGroup(callerAddress, actualAlias, expectedAlias));
        }

        public void RegisterContinuation(int continuationFrameIndex)
        {
            RegisterComponent((address, methodName) => new Continuation(address, methodName), continuationFrameIndex + 1, StackAnalyser.ForCurrentStack());
        }

        private void RegisterComponent(Func<SourceAddress, string, AssertionComponent> createMethod, int assertionFrameIndex, StackAnalyser analyser)
        {
            PopStackBackToAssertion(assertionFrameIndex, analyser);

            var callAddress = analyser.GetCallAddress(assertionFrameIndex);
            var assertionName = analyser.GetMethodName(assertionFrameIndex);
            var method = createMethod(callAddress, assertionName);

            CurrentGroup.AddComponent(method);
        }

        private void PopStackBackToAssertion(int assertionFrameIndex, StackAnalyser analyser)
        {
            var groupPosition = analyser.GetParentGroupPosition(assertionFrameIndex, assertionGroupChain);
            assertionGroupChain.RemoveRange(groupPosition, assertionGroupChain.Count - groupPosition);

            if (assertionGroupChain.None())
                assertionGroupChain.Add(new BaseGroup());
        }

        private static string GetExpressionAlias(MethodBase innerAssertion, int parameterIndex)
        {
            return innerAssertion.GetParameters().ElementAtOrDefault(parameterIndex)?.Name ?? string.Empty;
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
