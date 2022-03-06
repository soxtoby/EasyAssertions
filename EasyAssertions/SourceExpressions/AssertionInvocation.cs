using System.Linq.Expressions;
using System.Reflection;

namespace EasyAssertions;

class AssertionInvocation : AssertionCall
{
    public AssertionInvocation(Expression<Action> callAssertionMethod) : base(GetAssertionMethod(callAssertionMethod)) { }

    static MethodInfo GetAssertionMethod(Expression<Action> callAssertionMethod) =>
        callAssertionMethod.Body switch
            {
                MethodCallExpression methodCall => methodCall.Method,
                InvocationExpression invocation when InvokedMethod(invocation) is MethodInfo method => method,
                _ => throw new ArgumentException("Expression must be a method or delegate call", nameof(callAssertionMethod))
            };

    static MethodInfo? InvokedMethod(InvocationExpression invocation) =>
        Expression.Lambda<Func<Delegate>>(invocation.Expression).Compile()()?.Method;

    public override AssertionFrame CreateFrame(AssertionFrame? outerFrame, string actualSuffix, string expectedSuffix) =>
        new InvocationFrame(this, outerFrame, actualSuffix, expectedSuffix);

    public override string ToString() => $"Invoke({AssertionMethod.Name})";
}