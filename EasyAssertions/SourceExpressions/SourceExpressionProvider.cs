using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyAssertions
{
    internal class SourceExpressionProvider : TestExpressionProvider
    {
        private readonly List<AssertionComponentGroup> assertionGroupChain = new List<AssertionComponentGroup>();

        private AssertionComponentGroup CurrentGroup { get { return assertionGroupChain.LastOrDefault(); } }

        [ThreadStatic]
        private static SourceExpressionProvider threadInstance;

        public static SourceExpressionProvider ForCurrentThread
        {
            get { return threadInstance ?? (threadInstance = new SourceExpressionProvider()); }
        }

        private SourceExpressionProvider() { }

        public string GetActualExpression()
        {
            return assertionGroupChain.Aggregate(string.Empty, (expression, group) => group.GetActualExpression(expression));
        }

        public string GetExpectedExpression()
        {
            return assertionGroupChain.Last().GetExpectedExpression();
        }

        public void RegisterAssertionMethod(int assertionFrameIndex)
        {
            RegisterComponent((address, methodName) => new AssertionMethod(address, methodName), assertionFrameIndex + 1);
        }

        public void RegisterContinuation(int continuationFrameIndex)
        {
            RegisterComponent((address, methodName) => new Continuation(address, methodName), continuationFrameIndex + 1);
        }

        private void RegisterComponent(Func<SourceAddress, string, AssertionComponent> createMethod, int assertionFrameIndex)
        {
            assertionFrameIndex++;

            StackAnalyser analyser = StackAnalyser.ForCurrentStack();

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

        public void EnterNestedAssertion(MethodInfo innerAssertionMethod)
        {
            AddGroup(innerAssertionMethod, 2, (callAddress, expressionAlias) =>
                new NestedAssertionGroup(callAddress, expressionAlias));
        }

        public void EnterIndexedAssertion(MethodInfo innerAssertionMethod, int index)
        {
            AddGroup(innerAssertionMethod, 2, (callAddress, expressionAlias) =>
                new IndexedAssertionGroup(callAddress, GetExpressionAlias(innerAssertionMethod), index));
        }

        private void AddGroup(MethodInfo innerAssertionMethod, int callFrameIndex, Func<SourceAddress, string, AssertionComponentGroup> createGroup)
        {
            SourceAddress callerAddress = StackAnalyser.ForCurrentStack().GetCallAddress(callFrameIndex + 1);
            string expressionAlias = GetExpressionAlias(innerAssertionMethod);
            assertionGroupChain.Add(createGroup(callerAddress, expressionAlias));
        }

        private static string GetExpressionAlias(MethodInfo innerAssertion)
        {
            return innerAssertion.GetParameters().First().Name;
        }

        public void ExitNestedAssertion()
        {
            assertionGroupChain.Remove(CurrentGroup);
        }
    }
}
