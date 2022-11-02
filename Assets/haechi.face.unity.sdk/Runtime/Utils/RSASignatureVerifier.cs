using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace haechi.face.unity.sdk.Runtime.Utils
{
    public static class RSASignatureVerifier
    {
        public static bool Verify(string plain, string signature, string pubKey)
        {
            byte[] sig = Convert.FromBase64String(_intoValidBase64(signature));
                
            RSACryptoServiceProvider rsa = _importPublicKey(_bytesToPem(_intoValidBase64(pubKey), "PUBLIC KEY"));
            return rsa.VerifyData(Encoding.UTF8.GetBytes(plain), sig, HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }

        private static string _intoValidBase64(string str)
        {
            return str.Replace('-', '+').Replace('_', '/');
        }

        private static RSACryptoServiceProvider _importPublicKey(string pem) {
            PemReader pr = new PemReader(new StringReader(pem));
            AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKey);

            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.ImportParameters(rsaParams);
            return csp;
        }
        
        private static string _bytesToPem(string key, string pemHeader)
        {
            key = key.Replace('-', '+').Replace('_', '/');
            string retval = key;
            retval = _formatString(retval, 64, 0);
            retval = "-----BEGIN "+ pemHeader +"-----\r\n" +
                     retval +
                     "\r\n-----END "+ pemHeader +"-----\r\n";
            return retval;
        }
        
        private static string _formatString(string inStr, int lineLen, int groupLen)
        {
            char[] tmpCh = new char[inStr.Length*2];
            int i, c = 0, linec = 0;
            int gc = 0;
            for (i=0; i<inStr.Length; i++)
            {
                tmpCh[c++] = inStr[i];
                gc++;
                linec++;
                if (gc >= groupLen && groupLen > 0)
                {
                    tmpCh[c++] = ' ';
                    gc = 0;
                }
                if (linec >= lineLen)
                {
                    tmpCh[c++] = '\r';
                    tmpCh[c++] = '\n';
                    linec = 0;
                }
            }
            string retval = new string(tmpCh);
            retval = retval.TrimEnd('\0');
            retval = retval.TrimEnd('\n');
            retval = retval.TrimEnd('\r');
            return retval;
        }
    }
}