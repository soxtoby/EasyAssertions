using NUnit.Framework;

namespace EasyAssertions.UnitTests;

public class UtilsTests
{
    [Test]
    public void SkipLast_SkipEntireSequence_ReturnsEmpty()
    {
        CollectionAssert.IsEmpty(new[] { 1, 2, 3 }.SkipLast(4));
    }

    [Test]
    public void SkipLast_SkipPartial_ReturnsAllButSkipped()
    {
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, new[] { 1, 2, 3, 4, 5 }.SkipLast(2));
    }
}