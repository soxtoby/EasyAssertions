using System.Reflection;

namespace EasyAssertions
{
    class AssertionMethod : AssertionCall
    {
        const string GetterPrefix = "get_";

        public AssertionMethod(MethodBase assertionMethod, SourceAddress callAddress, int callOffset)
            : base(assertionMethod)
        {
            CallAddress = callAddress;
            CallOffset = callOffset;
        }

        public override AssertionFrame CreateFrame(AssertionFrame? outerFrame, string actualSuffix, string expectedSuffix) =>
            new AssertionStatement(this, outerFrame, actualSuffix, expectedSuffix);

        public string AssertionName => IsProperty ? AssertionMethod.Name[GetterPrefix.Length..] : AssertionMethod.Name;

        public bool IsProperty => AssertionMethod.Name.StartsWith(GetterPrefix);

        /// <summary>
        /// Where the method was called from, at the start of the expression (should be the start of the actual variable)
        /// </summary>
        public SourceAddress CallAddress { get; }

        /// <summary>
        /// Offset from the start of the native JIT-compiled code for the method that is being executed.
        /// Only used for differentiating between method calls in the same expression.
        /// </summary>
        public int CallOffset { get; }

        public override string ToString() => AssertionName;
    }
}