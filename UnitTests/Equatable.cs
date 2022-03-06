namespace EasyAssertions.UnitTests;

class Equatable
{
    public readonly int Value;

    public Equatable(int value)
    {
        Value = value;
    }

    public override bool Equals(object obj)
    {
        return obj is Equatable otherEquatable
            && otherEquatable.Value == Value;
    }

    public override int GetHashCode()
    {
        return Value;
    }

    public override string ToString()
    {
        return $"Eq({Value})";
    }
}

class SubEquatable : Equatable
{
    public SubEquatable(int value) : base(value) { }
}