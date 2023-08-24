using System;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client.WalletConnect
{
    [Serializable]
    [JsonObject]
    public class DappSession
    {
        [JsonProperty("dapp_name")]
        public string DappName { get; private set; }
        
        [JsonProperty("dapp_url")]
        public string DappUrl { get; private set; }
        

        [JsonConstructor]
        public DappSession() {}
        public DappSession(string dappName, string dappUrl)
        {
            this.DappName = dappName;
            this.DappUrl = dappUrl;
        }
    }
}