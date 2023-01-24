using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class RsaSigner : MonoBehaviour
    {
        public static string Sign(string privateKey, string plainTextData)
        {
            try
            {
                RSACryptoServiceProvider rsaPrivateKey = _importPrivateKey(_bytesToPem(privateKey, "PRIVATE KEY"));
                var bytesPlainTextData = Encoding.UTF8.GetBytes(plainTextData);
                var signedData = rsaPrivateKey.SignData(bytesPlainTextData, "SHA256");

                return Convert.ToBase64String(signedData);
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }

        private static RSACryptoServiceProvider _importPrivateKey(string pem) {
            PemReader pr = new PemReader(new StringReader(pem));
            RsaPrivateCrtKeyParameters key = (RsaPrivateCrtKeyParameters)pr.ReadObject();
            
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters(key);

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
            Debug.Log(retval);
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
