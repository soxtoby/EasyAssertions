using System.Linq;

namespace EasyAssertions
{
    internal class BaseGroup : AssertionComponentGroup
    {
        public override SourceAddress Address => MethodCalls.First().SourceAddress;
    }
}