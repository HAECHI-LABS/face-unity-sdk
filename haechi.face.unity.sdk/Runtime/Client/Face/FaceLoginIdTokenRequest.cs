using System;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [Serializable]
    [JsonObject]
    public class FaceLoginIdTokenRequest
    {
        [JsonProperty("idToken")]
        public string IdToken { get; set; }
        
        [JsonProperty("sig")]
        public string Sig { get; set; }

        public FaceLoginIdTokenRequest(string idToken, string sig)
        {
            this.IdToken = idToken;
            this.Sig = sig;
        }
    }
}