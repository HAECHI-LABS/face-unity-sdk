using System;
using haechi.face.unity.sdk.Runtime.Exception;
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
        }

        public FaceRpcResponse(object id) : base(id)
        {
        }
        
        public FaceRpcResponse(object id, RpcError error) : base(id, error)
        {
        }

        public FaceRpcResponse(object id, JToken result) : base(id, result)
        {
        }

        public FaceRpcResponse(string method)
        {
            this.Method = method;
        }

        public FaceRpcResponse(WebviewRpcRequest request)
        {
            this.Request = request;
        }

        public static FaceRpcResponse WebviewClosed()
        {
            FaceRpcResponse result = new FaceRpcResponse
            {
                _isWebviewClosed = true
            };
            return result;
        }

        [JsonProperty("id")]
        public object Id { get; private set; }

        [JsonProperty("from")] public string From { get; private set; } = "FACE_IFRAME";

        [JsonProperty("to")] public string To { get; private set; } = "FACE_NATIVE_SDK";
        
        [JsonProperty("result")]
        public JToken Result { get; private set; }
        
        [JsonProperty("method")]
        public string Method { get; private set; }

        private bool _isWebviewClosed = false;
        
        public T CastResult<T>()
        {
            if (this._isWebviewClosed)
            {
                throw new WebviewClosedException();
            }
            return (T) Result.ToObject(typeof(T));
        }

        public bool IsWebviewClosed()
        {
            return this._isWebviewClosed;
        }

        public override string ToString()
        {
            return $"Id: {Id}, From: {From}, To: {To}, Result: {Result}, Method: {Method}, IsWebviewClosed: {this.IsWebviewClosed()}";
        }

        public WebviewRpcRequest Request { get; private set; }
    }
}