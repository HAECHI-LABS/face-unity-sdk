using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Webview;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.Unity.Rpc;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class FaceRpcProvider : ClientBase, IUnityRpcRequestClient
    {
        private readonly SafeWebviewController _webview;

        private readonly FaceClient _client;

        private readonly MethodHandlers _methodHandlers;

        private readonly IRequestSender _defaultRequestSender;

        public FaceRpcProvider(SafeWebviewController safeWebviewController)
        {
            this._webview = safeWebviewController;
            this._client = new FaceClient(new Uri(FaceSettings.Instance.ServerHostURL()), new HttpClient());
            this._methodHandlers = new MethodHandlers(this);
            this._defaultRequestSender = new WebviewRequestSender(this);
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
            if (!this._methodHandlers.TryGetRequestSender(request.Method, out IRequestSender sender))
            {
                return await this._defaultRequestSender.SendRequest(request);
            }
            
            return await sender.SendRequest(request);
        }

        protected override Task<RpcResponseMessage[]> SendAsync(RpcRequestMessage[] request)
        {
            // TODO: batch rpc sender will be implemented if needed
            throw new NotImplementedException();
        }

        internal async Task<FaceRpcResponse> SendFaceRpcAsync<TParams>(FaceRpcRequest<TParams> request)
        {
            return (FaceRpcResponse)await this.SendAsync(request);
        }

        private class MethodHandlers
        {
            private readonly Dictionary<FaceRpcMethod, IRequestSender> _senders;

            public MethodHandlers(FaceRpcProvider provider)
            {
                this._senders = new Dictionary<FaceRpcMethod, IRequestSender>
                {
                    {FaceRpcMethod.face_logInSignUp, new WebviewRequestSender(provider)},
                    {FaceRpcMethod.face_logOut, new WebviewRequestSender(provider)},
                    {FaceRpcMethod.eth_getBalance, new ServerRequestSender(provider)},
                    {FaceRpcMethod.eth_sendTransaction, new WebviewRequestSender(provider)},
                    // ...
                };
            }

            public bool TryGetRequestSender(string methodValue, out IRequestSender sender)
            {
                if (!FaceRpcMethods.Contains(methodValue))
                {
                    sender = null;
                    return false;
                }

                return this._senders.TryGetValue(FaceRpcMethods.ValueOf(methodValue), out sender);
            }
        }

        private interface IRequestSender
        {
            Task<RpcResponseMessage> SendRequest(RpcRequestMessage request);
        }

        private class WebviewRequestSender : IRequestSender
        {
            private readonly FaceRpcProvider _provider;
            public WebviewRequestSender(FaceRpcProvider provider)
            {
                this._provider = provider;
            }
            
            public async Task<RpcResponseMessage> SendRequest(RpcRequestMessage request)
            {
                TaskCompletionSource<FaceRpcResponse> promise = new TaskCompletionSource<FaceRpcResponse>();
            
                this._provider._webview.SendMessage(request, response => promise.TrySetResult(response));
            
                return await promise.Task;
            }
        }

        private class ServerRequestSender : IRequestSender
        {
            private readonly FaceRpcProvider _provider;
            public ServerRequestSender(FaceRpcProvider provider)
            {
                this._provider = provider;
            }

            public async Task<RpcResponseMessage> SendRequest(RpcRequestMessage request)
            {
                TaskCompletionSource<RpcResponseMessage> promise = new TaskCompletionSource<RpcResponseMessage>();
                FaceRpcResponse response = await this._provider._client.SendRpcRequest(request, "/api/v1/rpc");
                promise.TrySetResult(response);
                return await promise.Task;
            }
        }
    }

}