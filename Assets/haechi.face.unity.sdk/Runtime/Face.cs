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
        internal WalletProxy walletProxy;
        internal FaceRpcProvider provider;

        public void Initialize(FaceSettings.Parameters parameters)
        {
            FaceSettings.NewInstance(parameters);
            
            SafeWebviewController safeWebviewController = this.GetComponent<SafeWebviewController>();
            
            this.walletProxy = new WalletProxy();
            
            // Inject walletProxy instead of real Wallet. Because Wallet still not instantiated
            FaceProviderFactory factory = new FaceProviderFactory(safeWebviewController, 
                FaceSettings.Instance.ServerHostURL(), this.walletProxy);
            this.provider = (FaceRpcProvider)factory.CreateUnityRpcClient();
            
            Web3 web3 = new Web3(this.provider);
            this.wallet = new Wallet(this.provider, web3);
            
            // Now register real wallet
            this.walletProxy.Register(this.wallet);
            this.dataFactory = new ContractDataFactory(web3);
            
        }
    }
}