using System.Diagnostics;

namespace EasyAssertions;

static class StackAnalyser
{
    public static AssertionCall MethodCall(int methodFrameIndex)
    {
        methodFrameIndex++; // To account for this method

        var currentStack = new StackTrace(true);
        var methodFrame = currentStack.GetFrame(methodFrameIndex);
        var callerFrame = currentStack.GetFrame(methodFrameIndex + 1);

        var method = methodFrame.GetMethod();
        var callingExpressionAddress = new SourceAddress(callerFrame.GetFileName(), callerFrame.GetFileLineNumber(), callerFrame.GetFileColumnNumber());

        return new AssertionMethod(method, callingExpressionAddress, callerFrame.GetNativeOffset());
    }
}