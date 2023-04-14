using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Utils
{
    public class RSASignatureVerifier
    {
        public static bool Verify(string plain, string signature, string pubKey)
        {
            // TODO: Remove this if statements after SecretKey issue fixed
            if (String.IsNullOrEmpty(signature))
            {
                return true;
            }
            try
            {
                byte[] sig = Convert.FromBase64String(RSAUtils.ToValidBase64(signature));
                string pem = RSAUtils.PublicKeyToPem(pubKey);
                RSACryptoServiceProvider rsa = RSAUtils.ImportPublicKey(pem);
                return rsa.VerifyData(Encoding.UTF8.GetBytes(plain), sig, HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);
            }
            catch (System.Exception e)
            {
                Debug.Log($"{e.Message}:\n{e.StackTrace}");
                return false;
            }
        }
    }
}