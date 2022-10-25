using System;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

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

        public FaceRpcResponse(string method)
        {
            this.From = "FACE_IFRAME";
            this.To = "FACE_NATIVE_SDK";
            this.Method = method;
        }

        public T CastResult<T>()
        {
            return (T) Result.ToObject(typeof(T));
        }
        
        [JsonProperty("id")]
        public object Id { get; private set; }

        [JsonProperty("from")]
        public string From { get; private set; }
        
        [JsonProperty("to")]
        public string To { get; private set; }
        
        [JsonProperty("result")]
        public JToken Result { get; private set; }
        
        [JsonProperty("method")]
        public string Method { get; private set; }

        public override string ToString()
        {
            return $"Id: {Id}, From: {From}, To: {To}, Result: {Result}, Method: {Method}";
        }
    }
}