using System.Collections.Generic;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Type;
using haechi.face.unity.sdk.Runtime.Utils;
using JetBrains.Annotations;

namespace haechi.face.unity.sdk.Runtime
{
    public class FaceSettings
    {
        public struct Parameters
        {
            /// <value>
            /// Api key.
            /// </value>
            public string ApiKey;
            
            /// <value>
            /// Environment such as Testnet, Mainnet.
            /// </value>
            public Profile? Environment;
            
            /// <value>
            /// Blockchain Network such as Sepolia, Amoy, Ethereum etc.
            /// </value>
            public BlockchainNetwork Network;

            /// <value>
            /// Deeplink scheme used on webview (only used android)
            /// </value>
            public string Scheme;
            
            public struct InternalParameters
            {
                public string IframeUrl;
            }
            public InternalParameters? Internal;
        }

        private struct parameters
        {
            public string _apiKey;
            public Profile _environment;
            public BlockchainNetwork _network;
            public string _scheme;
            [CanBeNull] public string _iframeUrl;
        }
        
        private static FaceSettings instance;

        /// <value>
        /// Returns FaceSettings' instance.
        /// </value>
        public static FaceSettings Instance => instance;

        /// <summary>
        /// Initialize FaceSettings with <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.FaceSettings.Parameters.html">Parameters</a>.
        /// </summary>
        /// <param name="parameters"><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.FaceSettings.Parameters.html">Parameters</a>.</param>
        /// <exception cref="AlreadyInitializedException">Throws if FaceSettings is already initialized.</exception>
        public static void Init(Parameters parameters)
        {
            if (IsInitialized())
            {
                throw new AlreadyInitializedException();
            }
            instance = new FaceSettings(new parameters
            {
                _apiKey = parameters.ApiKey,
                _environment = parameters.Environment ?? _getDefaultProfile(parameters.Network),
                _network = parameters.Network,
                _scheme = parameters.Scheme,
                _iframeUrl = parameters.Internal?.IframeUrl
            });
        }

        private static Profile _getDefaultProfile(BlockchainNetwork blockchainNetwork)
        {
            return Profile.ProdMainnet;
        }

        /// <summary>
        /// Initialize FaceSettings with null instance.
        /// </summary>
        public static void Destruct()
        {
            instance = null;
        }
        
        /// <returns>Returns true if initialized.</returns>
        public static bool IsInitialized()
        {
            return instance != null;
        }

        private FaceSettings(parameters parameters)
        {
            this._parameters = parameters;
        }

        private parameters _parameters;
        
        /// <returns>Return Profile such as Testnet, Mainnet.</returns>
        public Profile Environment()
        {
            return this._parameters._environment;
        }

        /// <returns>Return Api key.</returns>
        public string ApiKey()
        {
            return this._parameters._apiKey;
        }
        
        /// <returns> Return Blockchain such as Ethereum, Polygon etc.</returns>
        public Blockchain Blockchain()
        {
            return Blockchains.OfBlockchainNetwork(this._parameters._network);
        }

        /// <returns> Return Blockchain Network such as Sepolia, Amoy, Ethereum etc.</returns>
        public BlockchainNetwork Network()
        {
            return this._parameters._network;
        }

        /// <returns> Return Deeplink Scheme</returns>
        public string Scheme()
        {
            return this._parameters._scheme;
        }

        public void SetNetwork(BlockchainNetwork network)
        {
            this._parameters._network = network;
        }
        
        private readonly Dictionary<Profile, string> _webviewHostMap = new Dictionary<Profile, string>
        {
            { Profile.Local,  "http://localhost:3333" },
            { Profile.Dev, "https://app.dev.facewallet.xyz" },
            { Profile.StageTest, "https://app.stage-test.facewallet.xyz" },
            { Profile.StageMainnet, "https://app.stage.facewallet.xyz" },
            { Profile.ProdTest, "https://app.test.facewallet.xyz" },
            { Profile.ProdMainnet, "https://app.facewallet.xyz" },
        };

        private readonly Dictionary<Profile, string> _serverHostMap = new Dictionary<Profile, string>
        {
            { Profile.Local, "http://localhost:8881" },
            { Profile.Dev, "https://api.dev.facewallet.xyz" },
            { Profile.StageTest, "https://api.stage-test.facewallet.xyz" },
            { Profile.StageMainnet, "https://api.stage.facewallet.xyz" },
            { Profile.ProdTest, "https://api.test.facewallet.xyz" },
            { Profile.ProdMainnet, "https://api.facewallet.xyz" },
        };
        
        private readonly Dictionary<Profile, string> _iframeHostMap = new Dictionary<Profile, string>
        {
            { Profile.Local, "http://localhost:3333" },
            { Profile.Dev, "https://app.dev.facewallet.xyz" },
            { Profile.StageTest, "https://app.stage-test.facewallet.xyz" },
            { Profile.StageMainnet, "https://app.stage.facewallet.xyz" },
            { Profile.ProdTest, "https://app.test.facewallet.xyz" },
            { Profile.ProdMainnet, "https://app.facewallet.xyz" },
        };
        
        /// <returns>Returns webview client url.</returns>
        public string WebviewHostURL()
        {
            if (this._parameters._iframeUrl != null)
            {
                return this._parameters._iframeUrl;
            }
            return this._webviewHostMap.GetValueOrDefault(this.Environment(), this._webviewHostMap[Profile.StageTest]);
        }
        
        /// <returns>Returns server host url.</returns>
        public string ServerHostURL()
        {
            return this._serverHostMap.GetValueOrDefault(this.Environment(), this._serverHostMap[Profile.StageTest]);
        }
        
        /// <returns>Returns iframe host url.</returns>
        public string IframeURL()
        {
            if (this._parameters._iframeUrl != null)
            {
                return this._parameters._iframeUrl;
            }
            return string.Format(
                $"{this._iframeHostMap.GetValueOrDefault(this.Environment(), this._iframeHostMap[Profile.StageTest])}/?api_key={this.ApiKey()}&blockchain={this.Blockchain()}&env={this.Environment()}&version={SdkInfo.UNITY_SDK_VERSION}&type={SdkInfo.UNITY_SDK_TYPE}");
        }
    }
}