using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace EasyAssertions
{
    internal class StackAnalyser
    {
        private readonly List<StackFrame> frames;

        public static StackAnalyser ForCurrentStack()
        {
            var currentStack = new StackTrace(true).GetFrames() ?? Enumerable.Empty<StackFrame>();
            return new StackAnalyser(currentStack.Skip(1));  // Skip top frame so index arguments are relative to caller, not this method
        }

        private StackAnalyser(IEnumerable<StackFrame> stackFrames)
        {
            frames = stackFrames.ToList();
        }

        public SourceAddress GetCallAddress(int methodFrameIndex)
        {
            var frame = frames[methodFrameIndex + 1];

            return new SourceAddress
                {
                    FileName = frame.GetFileName(),
                    LineIndex = frame.GetFileLineNumber() - 1,
                    ExpressionIndex = frame.GetFileColumnNumber() - 1
                };
        }

        public int GetParentGroupPosition(int assertionFrameIndex, IList<AssertionComponentGroup> assertionGroupChain)
        {
            if (assertionGroupChain.None())
                return 0;

            // Most of an assertion stack trace is usually from the test framework, which we don't care about,
            // so search for the first group from the top of the stack, and then we'll work our way back for the rest of the groups.
            var firstGroupFrameIndex = FirstGroupFrameIndex(assertionFrameIndex, assertionGroupChain.First());
            return firstGroupFrameIndex < 0 ? 0
                : assertionGroupChain.Count == 1 ? 1
                : ParentGroupPosition(assertionFrameIndex, assertionGroupChain, firstGroupFrameIndex);
        }

        private int ParentGroupPosition(int assertionFrameIndex, IList<AssertionComponentGroup> assertionGroupChain, int firstGroupFrameIndex)
        {
            var groupAddress = assertionGroupChain[1].Address;

            var nextGroupIndex = 1;

            for (var i = firstGroupFrameIndex; i > assertionFrameIndex; i--)
            {
                var frame = frames[i];
                if (AreSameAddress(groupAddress, frame))
                {
                    nextGroupIndex++;

                    if (nextGroupIndex == assertionGroupChain.Count)
                        break;

                    groupAddress = assertionGroupChain[nextGroupIndex].Address;
                }
            }

            return nextGroupIndex;
        }

        private int FirstGroupFrameIndex(int assertionFrameIndex, AssertionComponentGroup firstGroup)
        {
            for (var i = assertionFrameIndex; i < frames.Count; i++)
                if (AreSameAddress(firstGroup.Address, frames[i]))
                    return i;

            return -1;
        }

        private static bool AreSameAddress(SourceAddress groupAddress, StackFrame frame)
        {
            return frame.GetFileLineNumber() - 1 == groupAddress.LineIndex
                && frame.GetFileColumnNumber() - 1 == groupAddress.ExpressionIndex
                && frame.GetFileName() == groupAddress.FileName;
        }

        public string GetMethodName(int frameIndex)
        {
            var methodName = GetMethod(frameIndex).Name;
            return methodName.StartsWith("get_")
                ? methodName.Substring(4)
                : methodName;
        }

        public MethodBase GetMethod(int frameIndex)
        {
            var assertionFrame = frames[frameIndex];
            return assertionFrame.GetMethod();
        }
    }
}