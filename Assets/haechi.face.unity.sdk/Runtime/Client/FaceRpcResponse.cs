using System;
using haechi.face.unity.sdk.Runtime.Exception;
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
        }
        
        /// <param name="id">Rpc ID.</param>
        public FaceRpcResponse(object id) : base(id)
        {
        }
        
        /// <param name="id">Rpc ID.</param>
        /// <param name="error">Error message.</param>
        public FaceRpcResponse(object id, RpcError error) : base(id, error)
        {
        }
        
        /// <param name="id">Rpc ID.</param>
        /// <param name="result">Rpc result.</param>
        public FaceRpcResponse(object id, JToken result) : base(id, result)
        {
        }
        
        /// <param name="method">Rpc method.</param>
        public FaceRpcResponse(string method)
        {
            this.Method = method;
        }
        
        /// <param name="request">Raw request includes id, method, and params</param>
        public FaceRpcResponse(WebviewRpcRequest request)
        {
            this.Request = request;
        }

        /// <summary>
        /// Check if webview page closed.
        /// </summary>
        /// <returns>Rpc response.</returns>
        public static FaceRpcResponse WebviewClosed()
        {
            FaceRpcResponse result = new FaceRpcResponse
            {
                _isWebviewClosed = true
            };
            return result;
        }

        /// <value>
        /// Rpc ID.
        /// </value>
        [JsonProperty("id")]
        public object Id { get; private set; }

        /// <value>
        /// From value that indicates where this rpc request made.
        /// </value>
        [JsonProperty("from")] public string From { get; private set; } = "FACE_IFRAME";

        /// <value>
        /// To value that indicates where this rpc request received.
        /// </value>
#if UNITY_WEBGL
        [JsonProperty("to")] public string To { get; private set; } = "FACE_SDK";
#else
        [JsonProperty("to")] public string To { get; private set; } = "FACE_NATIVE_SDK";
#endif
        
        /// <value>
        /// Rpc method name.
        /// </value>
        [JsonProperty("method")]
        public string Method { get; private set; }

        private bool _isWebviewClosed = false;
        
        /// <summary>
        /// Cast result to generic type.
        /// </summary>
        /// <typeparam name="T">Object you want to cast to.</typeparam>
        /// <returns>Object.</returns>
        /// <exception cref="WebviewClosedException">Throws if unwanted webview closing event invoked.</exception>
        public T CastResult<T>()
        {
            if (this._isWebviewClosed)
            {
                throw new WebviewClosedException();
            }
            return (T) Result.ToObject(typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns true if webview closed.</returns>
        public bool IsWebviewClosed()
        {
            return this._isWebviewClosed;
        }

        /// <summary>
        /// Overrides ToString() method to customize.
        /// </summary>
        /// <returns>Customized string value.</returns>
        public override string ToString()
        {
            return $"Id: {Id}, From: {From}, To: {To}, Result: {Result}, Method: {Method}, IsWebviewClosed: {this.IsWebviewClosed()}";
        }

        /// <value>
        /// Webview sent rpc request message.
        /// </value>
        public WebviewRpcRequest Request { get; private set; }
    }
}