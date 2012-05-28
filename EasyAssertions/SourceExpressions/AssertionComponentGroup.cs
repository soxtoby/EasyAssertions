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

        public override ExpressionSegment GetSegment(string expressionSource, int fromIndex)
        {
            ExpressionSegment lastSegment = null;
            EvaluateSubComponents(segment => lastSegment = segment);

            return new ExpressionSegment
                {
                    Expression = String.Empty,
                    IndexOfNextSegment = lastSegment.IndexOfNextSegment
                };
        }

        public virtual string GetExpression()
        {
            string expression = string.Empty;
            EvaluateSubComponents(segment => expression += segment.Expression.Trim());
            return expression;
        }

        private void EvaluateSubComponents(Action<ExpressionSegment> func)
        {
            string[] sourceLines = File.ReadAllLines(InnerAssertionSourceAddress.FileName);
            string expressionSource = sourceLines.Skip(InnerAssertionSourceAddress.LineIndex).Join(Environment.NewLine);

            ExpressionSegment segment = new ExpressionSegment { IndexOfNextSegment = InnerAssertionSourceAddress.ExpressionIndex };
            foreach (AssertionComponent method in MethodCalls)
            {
                segment = method.GetSegment(expressionSource, segment.IndexOfNextSegment);
                func(segment);
            }
        }

        public void AddMethod(AssertionComponent component)
        {
            component.ParentGroup = this;
            calls.Add(component);
        }
    }
}