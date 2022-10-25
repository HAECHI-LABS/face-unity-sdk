using System;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [JsonObject]
    [Serializable]
    public class FaceLoginResponse
    {
        [JsonProperty("faceUserId")]
        public string faceUserId;
        
        [JsonProperty("wallet")]
        public Wallet wallet;
        
        [JsonConstructor]
        public FaceLoginResponse()
        {
        }
        
        public class Wallet
        {
            [JsonProperty("id")]
            public string Id;
            
            [JsonProperty("address")]
            public string Address;
            
            [JsonProperty("ecdsaPublicKey")]
            public string EcdsaPublicKey;
            
            [JsonProperty("eddsaPublicKey")]
            public string EddsaPublicKey;
        }
    }
}