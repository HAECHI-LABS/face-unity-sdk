using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Contract;
using haechi.face.unity.sdk.Runtime.Module;
using haechi.face.unity.sdk.Runtime.Client;
using Nethereum.Web3;

namespace haechi.face.unity.sdk.Runtime
{
    public class Face
    {
        private readonly FaceRpcProvider _client;
        private readonly FaceProviderFactory _factory;
        private readonly Web3 _web3;
        internal ContractDataFactory dataFactory;
        internal Wallet wallet;

        public Face(string webviewUri)
        {
            this._factory = new FaceProviderFactory(webviewUri);
            this._client = (FaceRpcProvider)this._factory.CreateUnityRpcClient();
            this._web3 = new Web3(this._client);
            this.dataFactory = new ContractDataFactory(this._web3);
            this.wallet = new Wallet(this._client);
        }
    }
}