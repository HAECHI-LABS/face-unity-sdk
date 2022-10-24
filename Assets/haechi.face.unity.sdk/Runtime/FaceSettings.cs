using System.Collections.Generic;
using System.Runtime.CompilerServices;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Settings
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
            instance = new FaceSettings(parameters);
        }

        private FaceSettings(Parameters parameters)
        {
            this._parameters = parameters;
        }

        private Parameters _parameters;
        
        public void Environment(string env)
        {
            this._parameters.Environment = env;
        }

        public Profile Environment()
        {
            return Profiles.ValueOf(this._parameters.Environment);
        }

        public void Blockchain(string blockchain)
        {
            this._parameters.Blockchain = blockchain;
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
            // TODO: Setup other environment webview host
        };

        private readonly Dictionary<Profile, string> _serverHostMap = new Dictionary<Profile, string>
        {
            { Profile.Dev, "https://dev.facewallet.xyz" },
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