using System;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [JsonObject]
    [Serializable]
    public class FaceLoginResponse
    {
        public string faceUserId;
        public Wallet wallet;
        
        [JsonConstructor]
        public FaceLoginResponse()
        {
        }
        
        public class Wallet
        {
            public string Id;
            public string Address;
            public string EcdsaPublicKey;
            public string EddsaPublicKey;
        }
    }
}