using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class CollectionFailureMessageTests
    {
        [Test]
        public void ItemType_IsEscaped()
        {
            CollectionFailureMessage sut = new CollectionFailureMessage { ItemType = "{foo}" };

            Assert.AreEqual(@"\{foo\}", sut.ItemType);
        }
    }
}