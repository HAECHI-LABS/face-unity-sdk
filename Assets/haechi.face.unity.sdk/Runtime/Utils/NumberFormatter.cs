using System.Globalization;
using System.Numerics;
using Nethereum.Util;

namespace haechi.face.unity.sdk.Runtime.Utils
{
    public static class NumberFormatter
    {
        public static string DecimalStringToIntegerString(string decimalNumber, int decimals)
        {
            return (BigDecimal.Parse(decimalNumber) * BigInteger.Pow(10, decimals)).ToString();
        }

        public static string DecimalStringToHexadecimal(string decimalNumber)
        {
            return BigInteger.Parse(BigDecimal.Parse(decimalNumber).ToString()).ToString("x8");
        }

        public static decimal HexadecimalToDecimal(string hexadecimal)
        {
            string hex = hexadecimal.StartsWith("0x") ? hexadecimal.Substring("0x".Length) : hexadecimal;
            decimal hexNumber = decimal.Parse(BigInteger.Parse(hex, NumberStyles.AllowHexSpecifier).ToString());
            return hexNumber;
        }

        public static string DivideHexWithDecimals(string hexadecimal, int decimals)
        {
            decimal number = HexadecimalToDecimal(hexadecimal);
            return decimal.Divide(number, decimal.Parse(BigInteger.Pow(10, decimals).ToString()))
                .ToStringInvariant();
        }
    }
}