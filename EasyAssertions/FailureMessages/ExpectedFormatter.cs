using System;
using SmartFormat.Core.Extensions;
using SmartFormat.Core.Output;
using SmartFormat.Core.Parsing;

namespace EasyAssertions
{
    public class ExpectedFormatter : IFormatter
    {
        private ExpectedFormatter() { }

        public static readonly ExpectedFormatter Instance = new ExpectedFormatter();

        public void EvaluateFormat(object current, Format format, ref bool handled, IOutput output, FormatDetails formatDetails)
        {
            FailureMessage.ExpectedWrapper expected = current as FailureMessage.ExpectedWrapper;
            if (expected != null && format != null)
            {
                handled = true;

                if (expected.Expression != null)
                {
                    string currentOutput = output.ToString();
                    int indexIntoLine = currentOutput.Length - Environment.NewLine.Length - currentOutput.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);
                    string indent = new string(' ', indexIntoLine);
                    output.Write(expected.Expression + Environment.NewLine + indent, formatDetails);
                    formatDetails.Formatter.Format(output, format, current, formatDetails);
                }
                else
                {
                    formatDetails.Formatter.Format(output, format, current, formatDetails);
                }
            }
        }
    }
}