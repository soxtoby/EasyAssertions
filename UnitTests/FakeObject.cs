namespace EasyAssertions.UnitTests
{
    internal class FakeObject
    {
        private readonly string toString;

        public FakeObject(string toString)
        {
            this.toString = toString;
        }

        public override string ToString()
        {
            return toString;
        }
    }
}