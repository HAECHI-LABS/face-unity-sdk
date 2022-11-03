using System.Collections.Generic;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Type;

namespace haechi.face.unity.sdk.Runtime
{
    public class FaceSettings
    {
        public struct Parameters
        {
            public string ApiKey;
            public string Environment;
            public string Blockchain;
        }
        
        private static FaceSettings instance;

        public static FaceSettings Instance => instance;

        public static void Init(Parameters parameters)
        {
            if (instance != null)
            {
                throw new FaceException(ErrorCodes.ALREADY_INITIALIZED);
            }
            instance = new FaceSettings(parameters);
        }

        public static void Destruct()
        {
            instance = null;
        }

        public static bool IsInitialized()
        {
            return instance != null;
        }

        private FaceSettings(Parameters parameters)
        {
            this._parameters = parameters;
        }

        private Parameters _parameters;
        
        public Profile Environment()
        {
            return Profiles.ValueOf(this._parameters.Environment);
        }

        public string ApiKey()
        {
            return this._parameters.ApiKey;
        }

        public Blockchain Blockchain()
        {
            return Blockchains.ValueOf(this._parameters.Blockchain);
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

        public string WebviewHostURL()
        {
            return this._webviewHostMap.GetValueOrDefault(this.Environment(), this._webviewHostMap[Profile.Dev]);
        }

        public string ServerHostURL()
        {
            return this._serverHostMap.GetValueOrDefault(this.Environment(), this._serverHostMap[Profile.Dev]);
        }
    }
}