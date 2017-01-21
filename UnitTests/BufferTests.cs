using System;
using System.Linq;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    public class BufferTests
    {
        [Test]
        public void EnumerateTwice_SourceIsEnumeratedOnce()
        {
            TestEnumerable<int> source = new TestEnumerable<int>(new[] { 1 });
            Buffer<int> sut = new Buffer<int>(source);

            CollectionAssert.AreEqual(new[] { 1 }, sut);
            CollectionAssert.AreEqual(new[] { 1 }, sut);

            Assert.AreEqual(1, source.EnumerationCount);
            Assert.IsTrue(source.EnumerationCompleted);
        }

        [Test]
        public void EnumeratePartially_SourceIsEnumeratedPartially()
        {
            TestEnumerable<int> source = new TestEnumerable<int>(new[] { 1, 2 });
            Buffer<int> sut = new Buffer<int>(source);

            Assert.AreEqual(1, sut.First());

            Assert.IsFalse(source.EnumerationCompleted);
        }

        [Test]
        public void Index_BeforeEnd_SourceIsEnumeratedPartially()
        {
            TestEnumerable<int> source = new TestEnumerable<int>(new[] { 1, 2 });
            Buffer<int> sut = new Buffer<int>(source);

            Assert.AreEqual(1, sut[0]);

            Assert.IsFalse(source.EnumerationCompleted);
        }

        [Test]
        public void Index_PastEnd_ThrowsArgumentOutOfRangeException()
        {
            TestEnumerable<int> source = new TestEnumerable<int>(new[] { 1 });
            Buffer<int> sut = new Buffer<int>(source);

            Assert.Throws<ArgumentOutOfRangeException>(() => { int r = sut[1]; });
            Assert.IsTrue(source.EnumerationCompleted);
        }
    }
}