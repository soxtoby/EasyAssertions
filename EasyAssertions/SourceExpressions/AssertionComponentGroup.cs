using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyAssertions
{
    internal abstract class AssertionComponentGroup
    {
        private readonly List<AssertionComponent> calls = new List<AssertionComponent>();

        protected IEnumerable<AssertionComponent> MethodCalls
        {
            get { return calls; }
        }

        public abstract SourceAddress Address { get; }

        public virtual string GetActualExpression(string parentExpression)
        {
            string expression = string.Empty;
            AggregateMethodCalls(
                (method, source, nextSegment) => method.GetActualSegment(source, nextSegment),
                segment => expression += segment.Expression.Trim());

            return expression;
        }

        public void AddComponent(AssertionComponent component)
        {
            if (MethodCalls.Any() && !Equals(MethodCalls.First().SourceAddress, component.SourceAddress))
                calls.Clear();
            calls.Add(component);
        }

        public string GetExpectedExpression()
        {
            string lastExpression = string.Empty;
            AggregateMethodCalls(
                (method, source, nextSegment) => method.GetExpectedSegment(source, nextSegment),
                segment => lastExpression = segment.Expression);

            return lastExpression.Trim();
        }

        private void AggregateMethodCalls(Func<AssertionComponent, string, int, ExpressionSegment> getSegment, Action<ExpressionSegment> useSegment)
        {
            SourceAddress assertionsAddress = calls.First().SourceAddress;

            string[] sourceLines = File.ReadAllLines(assertionsAddress.FileName);
            string expressionSource = sourceLines.Skip(assertionsAddress.LineIndex).Join(Environment.NewLine);

            ExpressionSegment segment = new ExpressionSegment { IndexOfNextSegment = assertionsAddress.ExpressionIndex };
            foreach (AssertionComponent method in MethodCalls)
            {
                segment = getSegment(method, expressionSource, segment.IndexOfNextSegment);
                useSegment(segment);
            }
        }
    }
}