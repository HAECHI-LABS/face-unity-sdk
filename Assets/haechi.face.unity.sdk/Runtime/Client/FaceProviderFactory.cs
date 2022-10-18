using haechi.face.unity.sdk.Runtime.Client;
using Nethereum.Unity.Rpc;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class FaceProviderFactory : IUnityRpcRequestClientFactory
    {
        public FaceProviderFactory(string uri)
        {
            this.Uri = uri;
        }

        public string Uri { get; }

        public IUnityRpcRequestClient CreateUnityRpcClient()
        {
            return new FaceRpcProvider(this.Uri);
        }
    }
}