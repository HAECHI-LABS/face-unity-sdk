using System;
using Newtonsoft.Json;
using WalletConnectSharp.Sign.Models.Engine.Methods;

namespace haechi.face.unity.sdk.Runtime.Client.WalletConnect
{
    [Serializable]
    public class WcRequestEvent<T>
    {
        [JsonProperty("method")]
        public string Method { get; set; }
        
        [JsonProperty("id")]
        public long Id { get; set; }
        
        [JsonProperty("params")]
        public SessionRequest<T> Params { get; set;}
    }
}