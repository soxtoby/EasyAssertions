using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyAssertions
{
    internal abstract class AssertionComponentGroup
    {
        private readonly List<AssertionComponent> calls = new List<AssertionComponent>();

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

        public string GetExpectedExpression()
        {
            return AggregateMethodCalls(
                    (method, source, nextSegment) => method.GetExpectedSegment(source, nextSegment), 
                    (_, segment) => segment.Expression)
                .Trim();
        }

        private string AggregateMethodCalls(Func<AssertionComponent, string, int, ExpressionSegment> getSegment, Func<string, ExpressionSegment, string> aggregateSegment)
        {
            if (calls.None())
                return string.Empty;

            SourceAddress assertionsAddress = calls.First().SourceAddress;

            string[] sourceLines;
            if (!Utils.TryReadAllLines(assertionsAddress, out sourceLines))
                return string.Empty;

            string expressionSource = sourceLines.Skip(assertionsAddress.LineIndex).Join(Environment.NewLine);

            ExpressionSegment segment = new ExpressionSegment { IndexOfNextSegment = assertionsAddress.ExpressionIndex };
            string currentResult = string.Empty;
            foreach (AssertionComponent method in MethodCalls)
            {
                segment = getSegment(method, expressionSource, segment.IndexOfNextSegment);
                currentResult = aggregateSegment(currentResult, segment);
            }
            return currentResult;
        }
    }
}