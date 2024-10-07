using System;
using haechi.face.unity.sdk.Runtime.Type;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [Serializable]
    [JsonObject]
    public class SwitchNetworkRequest
    {
        [JsonProperty("network", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public string Network { get; private set; }

        public SwitchNetworkRequest(BlockchainNetwork network)
        {
            this.Network = network.ToString();
        }
    }
}