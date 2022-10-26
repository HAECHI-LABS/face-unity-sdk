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
        internal FaceRpcProvider provider;
        internal ContractDataFactory dataFactory;
        
        private WalletProxy _walletProxy;

        public void Initialize(FaceSettings.Parameters parameters)
        {
            FaceSettings.NewInstance(parameters);
            
            this.safeWebviewController = this.GetComponent<SafeWebviewController>();
            
            this._walletProxy = new WalletProxy();
            
            // Inject walletProxy instead of real Wallet. Because Wallet still not instantiated
            FaceProviderFactory factory = new FaceProviderFactory(safeWebviewController, 
                FaceSettings.Instance.ServerHostURL(), this._walletProxy);
            this.provider = (FaceRpcProvider)factory.CreateUnityRpcClient();
            
            Web3 web3 = new Web3(this.provider);
            this.wallet = new Wallet(this.provider);
            
            // Now register real wallet
            this._walletProxy.Register(this.wallet);
            this.dataFactory = new ContractDataFactory(web3);
        }
        
        public void Disconnect()
        {
            FaceSettings.Destruct();
            
            this.provider = null;
            this.dataFactory = null;
            this.wallet = null;
        }
    }
}