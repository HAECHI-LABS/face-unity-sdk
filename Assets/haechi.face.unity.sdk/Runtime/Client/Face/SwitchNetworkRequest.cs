using System;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [Serializable]
    [JsonObject]
    public class SwitchNetworkRequest
    {
        [JsonProperty("blockchain", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public string Blockchain { get; private set; }

        public SwitchNetworkRequest(string blockchain)
        {
            this.Blockchain = blockchain;
        }
    }
}