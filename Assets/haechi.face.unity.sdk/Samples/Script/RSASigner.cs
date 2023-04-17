using System;
using System.Security.Cryptography;
using System.Text;
using haechi.face.unity.sdk.Runtime.Utils;
using UnityEngine;

namespace haechi.face.unity.sdk.Samples.Script
{
    public class RSASigner : MonoBehaviour
    {
        public static string Sign(string prvKey, string plainTextData)
        {
            try
            {
                string pem = RSAUtils.RSAPrivateKeyToPem(prvKey);
                RSACryptoServiceProvider rsaPrivateKey = RSAUtils.ImportRSAPrivateKey(pem);
                byte[] bytesPlainTextData = Encoding.UTF8.GetBytes(plainTextData);
                byte[] signedData = rsaPrivateKey.SignData(bytesPlainTextData, "SHA256");

                return Convert.ToBase64String(signedData);
            }
            catch (Exception e)
            {
                Debug.Log($"{e.Message}:\n{e.StackTrace}");
                return null;
            }
        }
    }
}
