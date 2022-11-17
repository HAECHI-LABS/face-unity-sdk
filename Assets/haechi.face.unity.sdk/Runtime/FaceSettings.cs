using System.Collections.Generic;
using System.Diagnostics;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Type;

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
            /// Blockchain Network such as Goerli, Mumbai, Ethereum etc.
            /// </value>
            public BlockchainNetwork Network;
        }

        private struct parameters
        {
            public string _apiKey;
            public Profile _environment;
            public BlockchainNetwork _network;
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
        /// <exception cref="FaceException">Throws if FaceSettings is already initialized.</exception>
        public static void Init(Parameters parameters)
        {
            if (instance != null)
            {
                throw new FaceException(ErrorCodes.ALREADY_INITIALIZED);
            }
            instance = new FaceSettings(new parameters
            {
                _apiKey = parameters.ApiKey,
                _environment = parameters.Environment ?? _getDefaultProfile(parameters.Network),
                _network = parameters.Network
            });
        }

        private static Profile _getDefaultProfile(BlockchainNetwork blockchainNetwork)
        {
            if (blockchainNetwork.Equals(BlockchainNetwork.BAOBAB) || 
                blockchainNetwork.Equals(BlockchainNetwork.GOERLI) || 
                blockchainNetwork.Equals(BlockchainNetwork.MUMBAI) || 
                blockchainNetwork.Equals(BlockchainNetwork.BNB_SMART_CHAIN_TESTNET))
            {
                return Profile.ProdTest;
            }

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
        
        /// <returns> Return Blockchain Network such as Goerli, Mumbai, Ethereum etc.</returns>
        public Blockchain Blockchain()
        {
            return BlockchainNetworks.OfBlockchain(this._parameters._network);
        }

        public BlockchainNetwork Network()
        {
            return this._parameters._network;
        }
        
        private readonly Dictionary<Profile, string> _webviewHostMap = new Dictionary<Profile, string>
        {
            { Profile.Local, "http://localhost:3333" },
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
        
        /// <returns>Returns webview client url.</returns>
        public string WebviewHostURL()
        {
            return this._webviewHostMap.GetValueOrDefault(this.Environment(), this._webviewHostMap[Profile.Dev]);
        }
        
        /// <returns>Returns server host url.</returns>
        public string ServerHostURL()
        {
            return this._serverHostMap.GetValueOrDefault(this.Environment(), this._serverHostMap[Profile.Dev]);
        }
    }
}