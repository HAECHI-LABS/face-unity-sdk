using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using UnityEngine;

namespace haechi.face.unity.sdk.Samples.Script
{
    public class RsaSigner : MonoBehaviour
    {
        private const string PRIVATE_KEY_HEADER = "PRIVATE KEY";

        public static string Sign(string privateKey, string plainTextData)
        {
            try
            {
                string pem = _privateKeyToPem(privateKey, PRIVATE_KEY_HEADER);
                RSACryptoServiceProvider rsaPrivateKey = _importPrivateKey(pem);
                byte[] bytesPlainTextData = Encoding.UTF8.GetBytes(plainTextData);
                byte[] signedData = rsaPrivateKey.SignData(bytesPlainTextData, "SHA256");

                return Convert.ToBase64String(signedData);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }

        private static RSACryptoServiceProvider _importPrivateKey(string pem) {
            PemReader pr = new PemReader(new StringReader(pem));
            RsaPrivateCrtKeyParameters key = (RsaPrivateCrtKeyParameters)pr.ReadObject();
            
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters(key);

            RSACryptoServiceProvider rsaPrivateKey = new RSACryptoServiceProvider();
            rsaPrivateKey.ImportParameters(rsaParams);
            return rsaPrivateKey;
        }
        
        private static string _privateKeyToPem(string key, string pemHeader)
        {
            key = key.Replace("-", "+").Replace("_", "/");
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
