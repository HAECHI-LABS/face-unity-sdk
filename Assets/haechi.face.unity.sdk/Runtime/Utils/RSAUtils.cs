using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Utils
{
    public class RSAUtils
    {
        private const string PUBLIC_KEY_HEADER = "PUBLIC KEY";
        private const string PRIVATE_KEY_HEADER = "PRIVATE KEY";
        
        public static RSACryptoServiceProvider ImportPublicKey(string pem) {
            PemReader pr = new PemReader(new StringReader(pem));
            AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKey);

            RSACryptoServiceProvider rsaPublicKey = new RSACryptoServiceProvider();
            rsaPublicKey.ImportParameters(rsaParams);
            return rsaPublicKey;
        }
        
        public static RSACryptoServiceProvider ImportPrivateKey(string pem) {
            PemReader pr = new PemReader(new StringReader(pem));
            RsaPrivateCrtKeyParameters key = (RsaPrivateCrtKeyParameters)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters(key);

            RSACryptoServiceProvider rsaPrivateKey = CreateRsaProviderFromPrivateKey(pem);
            rsaPrivateKey.ImportParameters(rsaParams);
            return rsaPrivateKey;
        } 
        
        public static RSACryptoServiceProvider ImportRSAPrivateKey(string pem) {
            RSACryptoServiceProvider rsaPrivateKey = CreateRsaProviderFromPrivateKey(pem);
            return rsaPrivateKey;
        }
        
        private static RSACryptoServiceProvider CreateRsaProviderFromPrivateKey(string privateKey)
        {
            var privateKeyBits = System.Convert.FromBase64String(privateKey);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters parameters = new RSAParameters();

            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort uInt16 = 0;
                uInt16 = binaryReader.ReadUInt16();
                if (uInt16 == 0x8130)
                    binaryReader.ReadByte();
                else if (uInt16 == 0x8230)
                    binaryReader.ReadInt16();
                else
                    throw new System.Exception("Unexpected value read binr.ReadUInt16()");

                uInt16 = binaryReader.ReadUInt16();
                if (uInt16 != 0x0102)
                    throw new System.Exception("Unexpected version");

                bt = binaryReader.ReadByte();
                if (bt != 0x00)
                    throw new System.Exception("Unexpected value read binr.ReadByte()");

                parameters.Modulus = binaryReader.ReadBytes(GetIntegerSize(binaryReader));
                parameters.Exponent = binaryReader.ReadBytes(GetIntegerSize(binaryReader));
                parameters.D = binaryReader.ReadBytes(GetIntegerSize(binaryReader));
                parameters.P = binaryReader.ReadBytes(GetIntegerSize(binaryReader));
                parameters.Q = binaryReader.ReadBytes(GetIntegerSize(binaryReader));
                parameters.DP = binaryReader.ReadBytes(GetIntegerSize(binaryReader));
                parameters.DQ = binaryReader.ReadBytes(GetIntegerSize(binaryReader));
                parameters.InverseQ = binaryReader.ReadBytes(GetIntegerSize(binaryReader));
            }

            rsa.ImportParameters(parameters);
            return rsa;
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowByte = 0x00;
            byte highByte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
            if (bt == 0x82)
            {
                highByte = binr.ReadByte();
                lowByte = binr.ReadByte();
                byte[] modInt = { lowByte, highByte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modInt, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        public static string PublicKeyToPem(string key)
        {
            return KeyToPem(key, PUBLIC_KEY_HEADER);
        }

        public static string PrivateKeyToPem(string key)
        {
            return KeyToPem(key, PRIVATE_KEY_HEADER);
        } 
        
        public static string RSAPrivateKeyToPem(string key)
        {
            return RSAKeyToPem(key);
        }
        
        public static string ToValidBase64(string str)
        {
            return str.Replace('-', '+').Replace('_', '/');
        }

        private static string KeyToPem(string key, string pemHeader)
        {
            key = ToValidBase64(key);
            key = Regex.Replace(key, @"(\S{64}(?!$))", "$1\n");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"-----BEGIN {pemHeader}-----");
            foreach (var line in key.Split("\n"))
            {
                sb.AppendLine(line);
            }

            sb.AppendLine($"-----END {pemHeader}-----");
            return sb.ToString();
        }
        
        private static string RSAKeyToPem(string key)
        {
            key = ToValidBase64(key);
            key = Regex.Replace(key, @"(\S{64}(?!$))", "$1\n");
            StringBuilder sb = new StringBuilder();
            // sb.AppendLine($"-----BEGIN {pemHeader}-----");
            foreach (var line in key.Split("\n"))
            {
                sb.AppendLine(line);
            }

            // sb.AppendLine($"-----END {pemHeader}-----");
            return sb.ToString();
        }
    }
}