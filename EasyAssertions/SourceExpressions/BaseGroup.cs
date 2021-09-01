using System.Linq;

namespace EasyAssertions
{
    class BaseGroup : AssertionComponentGroup
    {
        public override SourceAddress Address => MethodCalls.First().SourceAddress;
    }
}