using System;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Webview;
using Nethereum.Unity.Rpc;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class FaceProviderFactory : IUnityRpcRequestClientFactory
    {
        public FaceProviderFactory(SafeWebviewController safeWebviewController, string uri)
        {
            this._safeWebviewController = safeWebviewController;
            this._uri = new Uri(uri);
        }

        private SafeWebviewController _safeWebviewController { get; }
        private Uri _uri { get; }

        public IUnityRpcRequestClient CreateUnityRpcClient()
        {
            return new FaceRpcProvider(this._safeWebviewController, this._uri);
        }
    }
}