using System;
using haechi.face.unity.sdk.Runtime.Client.BoraPortal;
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
        /// User verification JWT Token.
        /// </value>
        [JsonProperty("userVerificationToken")]
        public string userVerificationToken;
        
        /// <value>
        /// User's wallet info such as unique id, address etc.
        /// </value>
        [JsonProperty("wallet")]
        public Wallet wallet;

        /// <summary>
        /// This field is used when you use `BoraLogin`, `BoraDirectSocialLogin`, or `BoraLoginWithIdToken`
        /// </summary>
        [JsonProperty("boraConnectResponse", NullValueHandling = NullValueHandling.Include)]
        public BoraPortalConnectStatusResponse boraConnectStatusResponse;
        
        [JsonConstructor]
        public FaceLoginResponse()
        {
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

        public override string ToString()
        {
            return
                $"faceUserId: {this.faceUserId}, wallet.id: {this.wallet.Id}, wallet.address: {this.wallet.Address}, wallet.signedAddress: {this.wallet.SignedAddress}";
        }
    }
}