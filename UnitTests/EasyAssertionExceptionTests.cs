﻿using System;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class EasyAssertionExceptionTests
    {
        [Test]
        public void Exception_ToString()
        {
            EasyAssertionException sut = new EasyAssertionException("foo");
            StringAssert.StartsWith("foo" + Environment.NewLine + Environment.NewLine, sut.ToString());
        }
    }
}