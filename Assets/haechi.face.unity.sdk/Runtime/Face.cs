using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Contract;
using haechi.face.unity.sdk.Runtime.Exception;
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

        /// <value>
        /// This value indicates the version of webview client.
        /// </value>
        public const int WEBVIEW_VERSION = 1;
        
        private Wallet _wallet;
        internal FaceRpcProvider provider;
        internal ContractDataFactory dataFactory;
        
        private WalletProxy _walletProxy;
        
        /// <param name="parameters">Initialize face with registered environments.&#10; This method makes ready to use Face Wallet(<a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Module.Wallet.html">Module.Wallet</a>).</param>
        public void Initialize(FaceSettings.Parameters parameters)
        {
            FaceSettings.Init(parameters);
            
            this.safeWebviewController = this.GetComponent<SafeWebviewController>();
            
            this._walletProxy = new WalletProxy();
            
            // Inject walletProxy instead of real Wallet. Because Wallet still not instantiated
            FaceProviderFactory factory = new FaceProviderFactory(safeWebviewController, 
                FaceSettings.Instance.ServerHostURL(), this._walletProxy);
            this.provider = (FaceRpcProvider)factory.CreateUnityRpcClient();
            
            Web3 web3 = new Web3(this.provider);
            this._wallet = new Wallet(this.provider);
            
            // Now register real wallet
            this._walletProxy.Register(this._wallet);
            this.dataFactory = new ContractDataFactory(web3);
        }
        
        /// <summary>
        /// Disconnect Face Wallet.&#10; If this method be called, need to initialize again to connect with Face Wallet.
        /// </summary>
        public void Disconnect()
        {
            FaceSettings.Destruct();
            
            this.provider = null;
            this.dataFactory = null;
            this._wallet = null;
        }

        /// <summary>
        /// Check Face initialization and returns <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Module.Wallet.html">Wallet</a>.
        /// </summary>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Module.Wallet.html">Module.Wallet</a></returns>
        /// <exception cref="FaceException">Throws if FaceSettings is not initialized.</exception>
        public Wallet Wallet()
        {
            if (!FaceSettings.IsInitialized())
            {
                throw new FaceException(ErrorCodes.NOT_INITIALIZED);
            }

            return this._wallet;
        }
    }
}