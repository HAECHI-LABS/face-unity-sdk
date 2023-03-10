using System;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client.BoraPortal
{
    [Serializable]
    [JsonObject]
    public class BoraPortalConnectStatusResponse
    {
        [JsonProperty("status", Required = Required.Always)]
        public BoraPortalConnectStatus Status { get; private set; }
        
        [JsonProperty("bappUsn", Required = Required.AllowNull)]
        public string BappUsn { get; private set; }

        [JsonProperty("boraPortalUsn", Required = Required.AllowNull)]
        public string BoraPortalUsn { get; private set; }
        
        [JsonProperty("walletAddressHash", Required = Required.AllowNull)]
        public string WalletAddressHash { get; private set; }
    }
    
    public enum BoraPortalConnectStatus
    {
        CONNECTED,
        UNCONNECTED
    }
}