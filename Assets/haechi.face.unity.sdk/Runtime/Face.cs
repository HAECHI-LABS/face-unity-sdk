using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.WalletConnect;
using haechi.face.unity.sdk.Runtime.Contract;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Module;
using haechi.face.unity.sdk.Runtime.Webview;
using Nethereum.Web3;
using UnityEngine;
using WalletConnectSharpV1.Unity.Network;

namespace haechi.face.unity.sdk.Runtime
{
    [RequireComponent(typeof(SafeWebviewController))]
    public class Face : MonoBehaviour
    {
        
        private SafeWebviewController _safeWebviewController;

        /// <value>
        /// This value indicates the version of webview client.
        /// </value>
        public const int WEBVIEW_VERSION = 1;
        
        private Auth _auth;
        private Wallet _wallet;
        private WalletConnect _walletConnect;
        private Bora _bora;
        internal FaceRpcProvider provider;
        internal ContractDataFactory dataFactory;

        private WalletProxy _walletProxy;
        private AuthProxy _authProxy;
        
        /// <param name="parameters">Initialize face with registered environments.&#10; This method makes ready to use Face Wallet(<a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Module.Wallet.html">Module.Wallet</a>).</param>
        public void Initialize(FaceSettings.Parameters parameters)
        {
            FaceSettings.Init(parameters);
#if UNITY_WEBGL
            Iframe.CreateIframe();
#endif

            this._safeWebviewController = this.GetComponent<SafeWebviewController>();
            
            this._walletProxy = new WalletProxy();
            this._authProxy = new AuthProxy();
            
            // Inject walletProxy instead of real Wallet. Because Wallet still not instantiated
            FaceProviderFactory factory = new FaceProviderFactory(this._safeWebviewController, 
                FaceSettings.Instance.ServerHostURL(),
                this,
                this._walletProxy);
            this.provider = (FaceRpcProvider)factory.CreateUnityRpcClient();
#if !UNITY_WEBGL
            this._registryFaceUnityScripts();
#endif
            Web3 web3 = new Web3(this.provider);
            this._auth = new Auth(this.provider);
            this._wallet = new Wallet(this.provider);
            this._walletConnect = new WalletConnect(this.provider, this._wallet);
            this._bora = new Bora(this.provider, this._auth);

            // Now register real wallet
            this._walletProxy.Register(this._wallet);
            this._authProxy.Register(this._auth);
            this.dataFactory = new ContractDataFactory(web3);
        }
        
        /// <summary>
        /// Disconnect Face Wallet.&#10; If this method be called, need to initialize again to connect with Face Wallet.
        /// </summary>
        public async void Disconnect()
        {
            await this._walletConnect.DisconnectWalletConnectV1();
            FaceSettings.Destruct();
            this.provider = null;
            this.dataFactory = null;
            this._auth = null;
            this._wallet = null;
            this._walletConnect = null;
#if !UNITY_WEBGL
            if (this.gameObject.GetComponent<NativeWebSocketTransport>() != null)
            {
                Destroy(this.gameObject.GetComponent<NativeWebSocketTransport>());
            }
            if (this.gameObject.GetComponent<WalletConnectV1Client>() != null)
            {
                Destroy(this.gameObject.GetComponent<WalletConnectV1Client>());
            }
            // For iOS AppDelegate plugin, need to add WalletConnectV1Client as a GameObject
            if (GameObject.Find("WalletConnectV1Client") != null)
            {
                Destroy(GameObject.Find("WalletConnectV1Client"));
            }

            if (this.gameObject.GetComponent<WalletConnectV2Client>() != null)
            {
                Destroy(this.gameObject.GetComponent<WalletConnectV2Client>());
            }
#endif
        }
        
        /// <summary>
        /// Check Face initialization and returns <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Module.Auth.html">Auth</a>.
        /// </summary>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Module.Auth.html">Module.Wallet</a></returns>
        /// <exception cref="NotInitializedException">Throws if FaceSettings is not initialized.</exception>
        public Auth Auth()
        {
            if (!FaceSettings.IsInitialized())
            {
                throw new NotInitializedException();
            }

            return this._auth;
        }

        /// <summary>
        /// Check Face initialization and returns <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Module.Wallet.html">Wallet</a>.
        /// </summary>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Module.Wallet.html">Module.Wallet</a></returns>
        /// <exception cref="NotInitializedException">Throws if FaceSettings is not initialized.</exception>
        public Wallet Wallet()
        {
            if (!FaceSettings.IsInitialized())
            {
                throw new NotInitializedException();
            }

            return this._wallet;
        }
        
        /// <summary>
        /// Check Face initialization and returns <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Module.WalletConnect.html">Wallet</a>.
        /// </summary>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Module.WalletConnect.html">Module.Wallet</a></returns>
        /// <exception cref="NotInitializedException">Throws if FaceSettings is not initialized.</exception>
        public WalletConnect WalletConnect()
        {
            if (!FaceSettings.IsInitialized())
            {
                throw new NotInitializedException();
            }

            return this._walletConnect;
        }
        
        /// <summary>
        /// Check Face initialization and returns <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Module.Bora.html">Bora</a>.
        /// </summary>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Module.Bora.html">Module.Bora</a></returns>
        /// <exception cref="NotInitializedException">Throws if FaceSettings is not initialized.</exception>
        public Bora Bora()
        {
            if (!FaceSettings.IsInitialized())
            {
                throw new NotInitializedException();
            }

            return this._bora;
        }

        private void _registryFaceUnityScripts()
        {
            if (this.gameObject.GetComponent<NativeWebSocketTransport>() == null)
            {
                this.gameObject.AddComponent<NativeWebSocketTransport>();
            }
            // For iOS AppDelegate plugin, need to add WalletConnectV1Client as a GameObject
            if (GameObject.Find("WalletConnectV1Client") == null)
            {
                GameObject nativeWebSocketTransport = new GameObject();
                nativeWebSocketTransport.name = "WalletConnectV1Client";
                nativeWebSocketTransport.AddComponent<WalletConnectV1Client>();
            }
            if (this.gameObject.GetComponent<WalletConnectV2Client>() == null)
            {
                this.gameObject.AddComponent<WalletConnectV2Client>();
            }
        }
    }
}