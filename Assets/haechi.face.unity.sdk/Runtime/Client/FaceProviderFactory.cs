using System;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Module;
using haechi.face.unity.sdk.Runtime.Webview;
using Nethereum.Unity.Rpc;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class FaceProviderFactory : IUnityRpcRequestClientFactory
    {
        public FaceProviderFactory(SafeWebviewController safeWebviewController, string uri, IWallet wallet)
        {
            this._safeWebviewController = safeWebviewController;
            this._uri = new Uri(uri);
            this._wallet = wallet;
        }

        private SafeWebviewController _safeWebviewController { get; }
        private Uri _uri { get; }
        private IWallet _wallet { get; }

        public IUnityRpcRequestClient CreateUnityRpcClient()
        {
            return new FaceRpcProvider(this._safeWebviewController, this._uri, this._wallet);
        }
    }
}