using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyAssertions
{
    internal abstract class AssertionComponentGroup : AssertionComponent
    {
        private readonly List<AssertionComponent> calls = new List<AssertionComponent>();

        protected AssertionComponentGroup(SourceAddress sourceAddress, string methodName) : base(sourceAddress, methodName) { }

        public IEnumerable<AssertionComponent> MethodCalls { get { return calls; } }

        public SourceAddress InnerAssertionSourceAddress { get { return MethodCalls.First().SourceAddress; } }

        public virtual string GetExpression()
        {
            string[] sourceLines = File.ReadAllLines(InnerAssertionSourceAddress.FileName);
            string expressionSource = sourceLines.Skip(InnerAssertionSourceAddress.LineIndex).Join(Environment.NewLine);

            string expression = string.Empty;
            ExpressionSegment segment = new ExpressionSegment { IndexOfNextSegment = InnerAssertionSourceAddress.ExpressionIndex };
            foreach (AssertionComponent method in MethodCalls)
            {
                segment = method.GetSegment(expressionSource, segment.IndexOfNextSegment);
                expression += segment.Expression.Trim();
            }

            return expression;
        }

        public void AddMethod(AssertionComponent component)
        {
            component.ParentGroup = this;
            calls.Add(component);
        }
    }
}