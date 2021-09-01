using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyAssertions
{
    abstract class AssertionComponentGroup
    {
        readonly List<AssertionComponent> calls = new();

        protected IEnumerable<AssertionComponent> MethodCalls => calls;

        public abstract SourceAddress Address { get; }

        public virtual string GetActualExpression(string parentExpression)
        {
            return AggregateMethodCalls(
                (method, source, nextSegment) => method.GetActualSegment(source, nextSegment),
                (expression, segment) => expression + segment.Expression.Trim());
        }

        public void AddComponent(AssertionComponent component)
        {
            if (MethodCalls.Any() && !Equals(MethodCalls.First().SourceAddress, component.SourceAddress))
                calls.Clear();
            calls.Add(component);
        }

        public virtual string GetExpectedExpression(string actualExpression, string parentExpression)
        {
            return AggregateMethodCalls(
                    (method, source, nextSegment) => method.GetExpectedSegment(source, nextSegment),
                    (_, segment) => segment.Expression)
                .Trim();
        }

        string AggregateMethodCalls(Func<AssertionComponent, string, int, ExpressionSegment> getSegment, Func<string, ExpressionSegment, string> aggregateSegment)
        {
            if (calls.None())
                return string.Empty;

            var assertionsAddress = calls.First().SourceAddress;

            if (!Utils.TryReadAllLines(assertionsAddress, out var sourceLines))
                return string.Empty;

            var expressionSource = sourceLines.Skip(assertionsAddress.LineIndex).Join(Environment.NewLine);

            var segment = new ExpressionSegment { IndexOfNextSegment = assertionsAddress.ExpressionIndex };
            var currentResult = string.Empty;
            foreach (var method in MethodCalls)
            {
                segment = getSegment(method, expressionSource, segment.IndexOfNextSegment);
                currentResult = aggregateSegment(currentResult, segment);
            }
            return currentResult;
        }
    }
}