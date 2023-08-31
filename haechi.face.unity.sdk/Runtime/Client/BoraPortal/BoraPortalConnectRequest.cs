using System;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client.BoraPortal
{
    [Serializable]
    [JsonObject]
    public class BoraPortalConnectRequest
    {
        [JsonProperty("bappUsn", Required = Required.Always)]
        public string BappUsn { get; private set; }
        
        [JsonProperty("signature", Required = Required.Always)]
        public string Signature { get; private set; }

        public BoraPortalConnectRequest(string bappUsn, string signature)
        {
            this.BappUsn = bappUsn;
            this.Signature = signature;
        }
    }
}