using System.Collections.Generic;
using System.Runtime.CompilerServices;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Settings
{
    public class FaceSettings
    {
        private static FaceSettings instance;

        public static FaceSettings Instance => instance;

        public static void Init(string apiKey, string env, string blockchain)
        {
            instance = new FaceSettings(apiKey, env, blockchain);
        }

        private FaceSettings(string apiKey, string env, string blockchain)
        {
            this.ApiKey = apiKey;
            this.env = env;
            this.blockchain = blockchain;
        }

        public string ApiKey;
        
        private string env;
    
        private string blockchain;

        public void Environment(string env)
        {
            this.env = env;
        }

        public Profile Environment()
        {
            return Profiles.ValueOf(this.env);
        }

        public void Blockchain(string blockchain)
        {
            this.blockchain = blockchain;
        }

        public Blockchain Blockchain()
        {
            return Blockchains.ValueOf(this.blockchain);
        }
        
        private readonly Dictionary<Profile, string> _webviewHostMap = new Dictionary<Profile, string>
        {
            { Profile.Dev, "https://app.dev.facewallet.xyz" },
            // TODO: Setup other environment webview host
        };

        private readonly Dictionary<Profile, string> _serverHostMap = new Dictionary<Profile, string>
        {
            { Profile.Dev, "http://localhost:8881" },
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