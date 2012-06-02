using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyAssertions
{
    internal abstract class AssertionComponentGroup
    {
        private readonly List<AssertionComponent> calls = new List<AssertionComponent>();
        public readonly AssertionComponentGroup ParentGroup;

        protected AssertionComponentGroup(AssertionComponentGroup parentGroup)
        {
            ParentGroup = parentGroup;
        }

        public IEnumerable<AssertionComponent> MethodCalls
        {
            get { return calls; }
        }

        public SourceAddress InnerAssertionSourceAddress
        {
            get { return MethodCalls.First().SourceAddress; }
        }

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

        public void AddComponent(AssertionComponent component)
        {
            calls.Add(component);
        }
    }
}