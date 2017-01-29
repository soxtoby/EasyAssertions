namespace EasyAssertions.UnitTests
{
    class Equatable
    {
        public readonly int Value;

        public Equatable(int value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            Equatable otherEquatable = obj as Equatable;
            return otherEquatable != null
                && otherEquatable.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }

    class SubEquatable : Equatable
    {
        public SubEquatable(int value) : base(value) { }
    }
}