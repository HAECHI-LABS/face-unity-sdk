using System;
using System.Collections;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Webview;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.Unity.Rpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class FaceRpcProvider : ClientBase, IUnityRpcRequestClient
    {
        private readonly FakeWebviewController _webview;

        public FaceRpcProvider(string uri)
        {
            this._webview = new FakeWebviewController();
            this.JsonSerializerSettings = DefaultJsonSerializerSettingsFactory.BuildDefaultJsonSerializerSettings();
        }

        public JsonSerializerSettings JsonSerializerSettings { get; }
        public Exception Exception { get; set; }
        public RpcResponseMessage Result { get; set; }

        public IEnumerator SendRequest(RpcRequest request)
        {
            yield return this.SendAsync(new RpcRequestMessage(request.Id, request.Method, request.RawParameters));
        }

        protected override async Task<RpcResponseMessage> SendAsync(RpcRequestMessage request, string route = null)
        {
            TaskCompletionSource<RpcResponseMessage> promise = new TaskCompletionSource<RpcResponseMessage>();
            
            this._webview.SendMessage(request.Id.ToString(), request, response => promise.TrySetResult(response));

            return await promise.Task;
        }

        protected override Task<RpcResponseMessage[]> SendAsync(RpcRequestMessage[] request)
        {
            // TODO: batch rpc sender will be implemented if needed
            throw new NotImplementedException();
        }

        internal async Task<FaceRpcResponse> SendFaceRpcAsync<TParams, TResult>(FaceRpcRequest<TParams> request)
        {
            RpcResponseMessage response = await this.SendAsync(request);
            return new FaceRpcResponse(response.Id.ToString(), request.Method,
                JsonUtility.FromJson<TResult>(response.Result.ToString()), response.Error);
        }

        internal async Task<FaceRpcResponse> SendFaceRpcAsync<TParams>(FaceRpcRequest<TParams> request)
        {
            RpcResponseMessage response = await this.SendAsync(request);
            return new FaceRpcResponse(response.Id.ToString(), request.Method, response.Result, response.Error);
        }
    }
}