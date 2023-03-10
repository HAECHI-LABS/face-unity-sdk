using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;


namespace haechi.face.unity.sdk.Runtime.Utils
{
    public class BoraSignatureGenerator
    {
        public static string CreateSignature(string rawMessage, string prvKey)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(rawMessage);
            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(messageBytes);
            string privateKeyPem = _createPemFromPrivateKey(prvKey);
            RSA rsa = _createRsa(privateKeyPem);
            byte[] signature = rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
        }
        
        private static string _createPemFromPrivateKey(string privateKey)
        {
            privateKey = privateKey.Replace("-", "+").Replace("_", "/");
            privateKey = Regex.Replace(privateKey, @"(\S{64}(?!$))", "$1\n");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-----BEGIN PRIVATE KEY-----");
            foreach (var line in privateKey.Split("\n"))
            {
                sb.AppendLine(line);
            }

            sb.AppendLine("-----END PRIVATE KEY-----");
            return sb.ToString();
        }

        private static RSA _createRsa(string privateKeyPem)
        {
            PemReader pemReader = new PemReader(new StringReader(privateKeyPem));
            var keyPair = pemReader.ReadObject() as RsaPrivateCrtKeyParameters;
            if (keyPair == null)
            {
                throw new ArgumentException("Invalid PEM-encoded RSA key");
            }
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters(keyPair);
            RSA rsa = RSA.Create();
            rsa.ImportParameters(rsaParams);
            return rsa;
        }
    }
}