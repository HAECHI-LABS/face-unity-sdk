using System;
using haechi.face.unity.sdk.Runtime.Module;
using haechi.face.unity.sdk.Runtime.Webview;
using Nethereum.Unity.Rpc;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class FaceProviderFactory : IUnityRpcRequestClientFactory
    {
        public FaceProviderFactory(SafeWebviewController safeWebviewController, string uri, MonoBehaviour face, IWallet wallet)
        {
            this._safeWebviewController = safeWebviewController;
            this._uri = new Uri(uri);
            this._face = face;
            this._wallet = wallet;
        }

        private SafeWebviewController _safeWebviewController { get; }
        private Uri _uri;
        private MonoBehaviour _face { get; }
        private IWallet _wallet { get; }

        public IUnityRpcRequestClient CreateUnityRpcClient()
        {
            return new FaceRpcProvider(this._safeWebviewController, this._uri, this._face, this._wallet);
        }
    }
}