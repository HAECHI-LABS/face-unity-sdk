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

        public static void NewInstance(Parameters parameters)
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
            { Profile.ProdTest, "https://app.test.facewallet.xyz" },
            { Profile.ProdMainnet, "https://app.facewallet.xyz" },
        };

        private readonly Dictionary<Profile, string> _serverHostMap = new Dictionary<Profile, string>
        {
            { Profile.Dev, "https://dev.facewallet.xyz" },
            { Profile.StageTest, "https://stage-test.facewallet.xyz" },
            { Profile.StageMainnet, "https://stage.facewallet.xyz" },
            { Profile.ProdTest, "https://test.facewallet.xyz" },
            { Profile.ProdMainnet, "https://facewallet.xyz" },
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