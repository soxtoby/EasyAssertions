using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyAssertions
{
    internal abstract class AssertionComponentGroup
    {
        private readonly List<AssertionComponent> calls = new List<AssertionComponent>();

        public IEnumerable<AssertionComponent> MethodCalls
        {
            get { return calls; }
        }

        public abstract SourceAddress Address { get; }

        public virtual string GetExpression(string parentExpression)
        {
            SourceAddress assertionsAddress = calls.First().SourceAddress;

            string[] sourceLines = File.ReadAllLines(assertionsAddress.FileName);
            string expressionSource = sourceLines.Skip(assertionsAddress.LineIndex).Join(Environment.NewLine);

            string expression = string.Empty;
            ExpressionSegment segment = new ExpressionSegment { IndexOfNextSegment = assertionsAddress.ExpressionIndex };
            foreach (AssertionComponent method in MethodCalls)
            {
                segment = method.GetSegment(expressionSource, segment.IndexOfNextSegment);
                expression += segment.Expression.Trim();
            }

            return expression;
        }

        public void AddComponent(AssertionComponent component)
        {
            if (MethodCalls.Any() && !Equals(MethodCalls.First().SourceAddress, component.SourceAddress))
                calls.Clear();
            calls.Add(component);
        }
    }
}