using System;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [JsonObject]
    [Serializable]
    public class FaceLoginResponse
    {
        /// <value>
        /// Unique user ID using on Face Wallet.
        /// </value>
        [JsonProperty("faceUserId")]
        public string faceUserId;
        
        /// <value>
        /// User's wallet info such as unique id, address etc.
        /// </value>
        [JsonProperty("wallet")]
        public Wallet wallet;
        
        [JsonConstructor]
        public FaceLoginResponse()
        {
        }

        /// <returns>
        /// Confirms if the user is newly signed up.
        /// </returns>
        public bool IsNewUser()
        {
            return !string.IsNullOrEmpty(this.wallet.KeyId) && string.IsNullOrEmpty(this.wallet.SignedAddress);
        }
        
        public class Wallet
        {
            /// <value>
            /// Unique wallet id.
            /// </value>
            [JsonProperty("id")]
            public string Id;
            
            /// <value>
            /// Wallet's address.
            /// </value>
            [JsonProperty("address")]
            public string Address;
            
            /// <value>
            /// Encrypted using ecdsa algorithm public key.
            /// </value>
            [JsonProperty("ecdsaPublicKey")]
            public string EcdsaPublicKey;
            
            /// <value>
            /// Encrypted using eddsa algorithm public key.
            /// </value>
            [JsonProperty("eddsaPublicKey")]
            public string EddsaPublicKey;
            
            /// <value>
            /// Signed address to verify the address given is from Face server.
            /// </value>
            [JsonProperty("signedAddress")]
            public string SignedAddress;
            
            /// <value>
            /// Unique key id.
            /// </value>
            [JsonProperty("keyId")]
            public string KeyId;
        }
    }
}