using System;
using haechi.face.unity.sdk.Runtime.Utils;
using NUnit.Framework;

namespace haechi.face.unity.sdk.Tests.Runtime.Utils
{
    public class NumberFormatterTests
    {
        [Test]
        public void DivideHexWithDecimalsTest()
        {
            Assert.Throws<FormatException>(
                () => NumberFormatter.DivideHexWithDecimals("0.009210999998593", 18));
        }
    }
}