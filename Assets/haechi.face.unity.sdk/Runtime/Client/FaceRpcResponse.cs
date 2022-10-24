using System;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace haechi.face.unity.sdk.Runtime.Client
{
    [Serializable]
    public class FaceRpcResponse : RpcResponseMessage
    {
        [JsonConstructor]
        public FaceRpcResponse()
        {
        }

        public FaceRpcResponse(object id) : base(id)
        {
            this.From = "FACE_IFRAME";
            this.To = "FACE_NATIVE_SDK";
        }
        
        public FaceRpcResponse(object id, RpcError error) : base(id, error)
        {
            this.From = "FACE_IFRAME";
            this.To = "FACE_NATIVE_SDK";
        }

        public FaceRpcResponse(object id, JToken result) : base(id, result)
        {
            this.From = "FACE_IFRAME";
            this.To = "FACE_NATIVE_SDK";
        }

        [JsonProperty("from", Required = Required.Always)]
        public string From { get; private set; }
        
        [JsonProperty("to", Required = Required.Always)]
        public string To { get; private set; }
    }
}