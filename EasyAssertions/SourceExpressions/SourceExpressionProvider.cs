using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace EasyAssertions
{
    internal class SourceExpressionProvider : TestExpressionProvider
    {
        private AssertionComponentGroup currentGroup;

        public static readonly SourceExpressionProvider Instance = new SourceExpressionProvider();

        static SourceExpressionProvider() { }

        private SourceExpressionProvider() { }

        public string GetExpression()
        {
            return currentGroup.GetExpression();
        }

        public void RegisterAssertionMethod(int testFrameIndex, int assertionFrameIndex)
        {
            RegisterMethod((address, methodName) => new AssertionMethod(address, methodName), testFrameIndex + 1, assertionFrameIndex + 1);
        }

        public void RegisterContinuation(int testFrameIndex, int continuationFrameIndex)
        {
            RegisterMethod((address, methodName) => new Continuation(address, methodName), testFrameIndex + 1, continuationFrameIndex + 1);
        }

        private void RegisterMethod(Func<SourceAddress, string, AssertionComponent> createMethod, int testFrameIndex, int assertionFrameIndex)
        {
            AssertionComponent component = BuildComponent(createMethod, testFrameIndex + 1, assertionFrameIndex + 1);
            PopOldFrames(component.SourceAddress);
            EnsureCurrentFrame();
            AddToCurrentFrame(component);
        }

        private void PopOldFrames(SourceAddress sourceAddress)
        {
            while (SourceIsFromADifferentFrame(sourceAddress))
                PopFrame();
        }

        private bool SourceIsFromADifferentFrame(SourceAddress sourceAddress)
        {
            return CurrentFrameIsPopulated
                && !sourceAddress.Equals(currentGroup.InnerAssertionSourceAddress);
        }

        private bool CurrentFrameIsPopulated
        {
            get
            {
                return currentGroup != null
                    && currentGroup.MethodCalls.Any();
            }
        }

        private void PopFrame()
        {
            currentGroup = currentGroup.ParentGroup;
        }

        private void EnsureCurrentFrame()
        {
            if (currentGroup == null)
                currentGroup = new BaseGroup();
        }

        private void AddToCurrentFrame(AssertionComponent component)
        {
            currentGroup.AddComponent(component);
        }

        public void EnterNestedAssertion(MethodInfo innerAssertion)
        {
            currentGroup = new NestedAssertionGroup(currentGroup, GetExpressionAlias(innerAssertion));
        }

        public void EnterIndexedAssertion(MethodInfo innerAssertion, int index)
        {
            currentGroup = new IndexedAssertionGroup(currentGroup, GetExpressionAlias(innerAssertion), index);
        }

        private static string GetExpressionAlias(MethodInfo innerAssertion)
        {
            return innerAssertion.GetParameters().First().Name;
        }

        public void ExitNestedAssertion()
        {
            currentGroup = currentGroup.ParentGroup;
        }

        private static TComponent BuildComponent<TComponent>(Func<SourceAddress, string, TComponent> createComponent, int testFrameIndex, int assertionFrameIndex) where TComponent : AssertionComponent
        {
            StackTrace stack = new StackTrace(true);
            StackFrame[] frames = stack.GetFrames();

            StackFrame testFrame = frames[testFrameIndex + 1];
            StackFrame assertionFrame = frames[assertionFrameIndex + 1];

            SourceAddress componentAddress = new SourceAddress
                {
                    FileName = testFrame.GetFileName(),
                    LineIndex = testFrame.GetFileLineNumber() - 1,
                    ExpressionIndex = testFrame.GetFileColumnNumber() - 1
                };

            return createComponent(componentAddress, GetComponentName(assertionFrame));
        }

        private static string GetComponentName(StackFrame assertionFrame)
        {
            string methodName = assertionFrame.GetMethod().Name;
            return methodName.StartsWith("get_")
                ? methodName.Substring(4)
                : methodName;
        }
    }
}