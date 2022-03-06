namespace EasyAssertions.UnitTests;

class FakeObject
{
    readonly string toString;

    public FakeObject(string toString)
    {
        this.toString = toString;
    }

    public override string ToString()
    {
        return toString;
    }
}