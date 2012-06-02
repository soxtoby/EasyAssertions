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

        private SourceExpressionProvider() { }

        public string GetExpression()
        {
            return currentGroup.GetExpression();
        }

        public void RegisterAssertionMethod()
        {
            RegisterMethod((address, methodName) => new AssertionMethod(address, methodName));
        }

        public void RegisterContinuation()
        {
            RegisterMethod((address, methodName) => new Continuation(address, methodName));
        }

        private void RegisterMethod(Func<SourceAddress, string, AssertionComponent> createMethod)
        {
            AssertionComponent component = BuildComponent(createMethod);
            PopOldFrames(component.SourceAddress);
            EnsureCurrentFrame(component.SourceAddress);
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

        private void EnsureCurrentFrame(SourceAddress sourceAddress)
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

        private static TComponent BuildComponent<TComponent>(Func<SourceAddress, string, TComponent> createComponent) where TComponent : AssertionComponent
        {
            StackTrace stack = new StackTrace(true);
            StackFrame[] frames = stack.GetFrames();

            if (frames == null)
                throw new Exception("Couldn't get stack trace.");

            int testFrameIndex = frames.IndexOf(f => f.GetMethod().DeclaringType.Namespace != "EasyAssertions"); // FIXME use attribute
            StackFrame testFrame = frames[testFrameIndex];
            StackFrame assertionFrame = frames[testFrameIndex - 1];

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