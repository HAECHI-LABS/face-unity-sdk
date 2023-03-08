using System;
using System.Globalization;
using System.Numerics;
using Nethereum.Util;
using UnityEngine;

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
            hex = hex.Length % 2 != 0 ? $"0{hex}" : $"00{hex}";
            if (string.IsNullOrEmpty(hex))
            {
                return Decimal.Zero;
            }
            return decimal.Parse(BigInteger.Parse(hex, NumberStyles.HexNumber).ToString());
        }

        public static string DivideHexWithDecimals(string hexadecimal, int decimals)
        {
            decimal number = HexadecimalToDecimal(hexadecimal);
            return decimal.Divide(number, decimal.Parse(BigInteger.Pow(10, decimals).ToString()))
                .ToStringInvariant();
        }
    }
}