using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

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

            RSACryptoServiceProvider rsaPrivateKey = new RSACryptoServiceProvider();
            rsaPrivateKey.ImportParameters(rsaParams);
            return rsaPrivateKey;
        }
        
        public static string PublicKeyToPem(string key)
        {
            return KeyToPem(key, PUBLIC_KEY_HEADER);
        }

        public static string PrivateKeyToPem(string key)
        {
            return KeyToPem(key, PRIVATE_KEY_HEADER);
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
    }
}