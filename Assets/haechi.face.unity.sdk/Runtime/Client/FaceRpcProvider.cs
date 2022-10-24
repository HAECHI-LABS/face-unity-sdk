using System;
using System.Collections;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Webview;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.Unity.Rpc;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class FaceRpcProvider : ClientBase, IUnityRpcRequestClient
    {
        private readonly SafeWebviewController _webview;
        // HTTP Client

        public FaceRpcProvider(SafeWebviewController safeWebviewController)
        {
            this._webview = safeWebviewController;
            this.JsonSerializerSettings = DefaultJsonSerializerSettingsFactory.BuildDefaultJsonSerializerSettings();
        }

        public JsonSerializerSettings JsonSerializerSettings { get; }
        public System.Exception Exception { get; set; }
        public RpcResponseMessage Result { get; set; }

        public IEnumerator SendRequest(RpcRequest request)
        {
            yield return this.SendAsync(new RpcRequestMessage(request.Id, request.Method, request.RawParameters));
        }

        protected override async Task<RpcResponseMessage> SendAsync(RpcRequestMessage request, string route = null)
        {
            TaskCompletionSource<FaceRpcResponse> promise = new TaskCompletionSource<FaceRpcResponse>();
            
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
            return (FaceRpcResponse) await this.SendAsync(request);
        }

        internal async Task<FaceRpcResponse> SendFaceRpcAsync<TParams>(FaceRpcRequest<TParams> request)
        {
            return (FaceRpcResponse) await this.SendAsync(request);
        }
    }
}