using System.Collections.Generic;
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
            public Profile Environment;
            
            /// <value>
            /// Blockchain such as Ethereum, Klaytn, etc.
            /// </value>
            public Blockchain Blockchain;
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
            instance = new FaceSettings(new Parameters
            {
                ApiKey = parameters.ApiKey,
                Environment = parameters.Environment,
                Blockchain = parameters.Blockchain
            });
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

        private FaceSettings(Parameters parameters)
        {
            this._parameters = parameters;
        }

        private Parameters _parameters;
        
        /// <returns>Profile such as Testnet, Mainnet.</returns>
        public Profile Environment()
        {
            return this._parameters.Environment;
        }

        /// <returns>Api key.</returns>
        public string ApiKey()
        {
            return this._parameters.ApiKey;
        }
        
        /// <returns>Blockchain such as Ethereum, Klaytn, etc.</returns>
        public Blockchain Blockchain()
        {
            return this._parameters.Blockchain;
        }
        
        private readonly Dictionary<Profile, string> _webviewHostMap = new Dictionary<Profile, string>
        {
            { Profile.Dev, "https://app.dev.facewallet.xyz" },
            { Profile.StageTest, "https://app.stage-test.facewallet.xyz" },
            { Profile.StageMainnet, "https://app.stage.facewallet.xyz" },
            { Profile.Testnet, "https://app.test.facewallet.xyz" },
            { Profile.Mainnet, "https://app.facewallet.xyz" },
        };

        private readonly Dictionary<Profile, string> _serverHostMap = new Dictionary<Profile, string>
        {
            { Profile.Dev, "https://api.dev.facewallet.xyz" },
            { Profile.StageTest, "https://api.stage-test.facewallet.xyz" },
            { Profile.StageMainnet, "https://api.stage.facewallet.xyz" },
            { Profile.Testnet, "https://api.test.facewallet.xyz" },
            { Profile.Mainnet, "https://api.facewallet.xyz" },
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