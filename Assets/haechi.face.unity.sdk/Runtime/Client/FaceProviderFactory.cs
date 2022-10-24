using haechi.face.unity.sdk.Runtime.Webview;
using Nethereum.Unity.Rpc;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class FaceProviderFactory : IUnityRpcRequestClientFactory
    {
        public FaceProviderFactory(SafeWebviewController safeWebviewController)
        {
            this._safeWebviewController = safeWebviewController;
        }

        private SafeWebviewController _safeWebviewController { get; }

        public IUnityRpcRequestClient CreateUnityRpcClient()
        {
            return new FaceRpcProvider(this._safeWebviewController);
        }
    }
}