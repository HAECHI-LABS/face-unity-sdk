using System;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace haechi.face.unity.sdk.Runtime.Client
{
    [JsonObject]
    [Serializable]
    public class FaceRpcResponse : RpcResponseMessage
    {
        [JsonConstructor]
        public FaceRpcResponse()
        {
            this.From = "FACE_IFRAME";
            this.To = "FACE_NATIVE_SDK";
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

        public T CastResult<T>()
        {
            return (T) Result.ToObject(typeof(T));
        }

        [JsonProperty("from")]
        public string From { get; private set; }
        
        [JsonProperty("to")]
        public string To { get; private set; }
    }
}