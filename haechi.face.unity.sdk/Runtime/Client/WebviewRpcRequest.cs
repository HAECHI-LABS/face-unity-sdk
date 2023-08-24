using System;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client
{
    /// <summary>
    /// WebviewRpcRequest is RPC request object from the SafeWebview
    /// </summary>
    [JsonObject]
    [Serializable]
    public class WebviewRpcRequest
    {
        [JsonConstructor]
        public WebviewRpcRequest()
        {
        }
        
        [JsonProperty("id")]
        public object Id { get; private set; }

        [JsonProperty("jsonrpc", Required = Required.Always)]
        public string JsonRpcVersion { get; private set; }

        [JsonProperty("method", Required = Required.Always)]
        public string Method { get; private set; }

        [JsonProperty("params")]
        [JsonConverter(typeof (RpcParametersJsonConverter))]
        public object RawParameters { get; private set; }
        
        public override string ToString()
        {
            return $"Id: {Id}, Params: {RawParameters}, Method: {Method}";
        }
    }
}