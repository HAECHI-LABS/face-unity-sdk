using System;
using Newtonsoft.Json;

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

        public bool IsNewUser()
        {
            return !string.IsNullOrEmpty(this.wallet.KeyId) && string.IsNullOrEmpty(this.wallet.SignedAddress);
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
            
            [JsonProperty("signedAddress")]
            public string SignedAddress;
            
            [JsonProperty("keyId")]
            public string KeyId;
        }
    }
}