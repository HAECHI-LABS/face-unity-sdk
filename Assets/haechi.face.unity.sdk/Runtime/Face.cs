using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Contract;
using haechi.face.unity.sdk.Runtime.Module;
using haechi.face.unity.sdk.Runtime.Webview;
using Nethereum.Web3;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime
{
    [RequireComponent(typeof(SafeWebviewController))]
    public class Face : MonoBehaviour
    {
        [SerializeField] internal SafeWebviewController safeWebviewController;
        internal Wallet wallet;
        private ContractDataFactory _dataFactory;
        private FaceRpcProvider _client;

        public void Initialize(FaceSettings.Parameters parameters)
        {
            FaceSettings.NewInstance(parameters);
            
            FaceProviderFactory factory = new FaceProviderFactory(this.safeWebviewController);
            this._client = (FaceRpcProvider)factory.CreateUnityRpcClient();
            this._dataFactory = new ContractDataFactory(new Web3(this._client));
            this.wallet = new Wallet(this._client);
        }

        public void Disconnect()
        {
            FaceSettings.Destruct();
            
            this._client = null;
            this._dataFactory = null;
            this.wallet = null;
        }
    }
}