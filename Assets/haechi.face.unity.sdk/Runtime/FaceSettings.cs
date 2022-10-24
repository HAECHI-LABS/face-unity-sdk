using System.Collections.Generic;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Settings
{
    public class FaceSettings : MonoBehaviour
    {
        #region Header SETTINGS

        [Space(10)]
        [Header("SETTINGS")]

        #endregion

        #region Tooltip

        [Tooltip("API KEY")]

        #endregion

        public string ApiKey;

        #region Tooltip

        [Tooltip("Environment")]

        #endregion

        [SerializeField]
        private string env;

        #region Tooltip

        [Tooltip("Blockchain")]

        #endregion

        [SerializeField]
        private string blockchain;

        private readonly Dictionary<Profile, string> _webviewHostMap = new Dictionary<Profile, string>
        {
            { Profile.Dev, "https://app.dev.facewallet.xyz" }
            // TODO: Setup other environment webview host
        };

        public static FaceSettings Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

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

        public string WebviewHostURL()
        {
            return this._webviewHostMap.GetValueOrDefault(this.Environment(), "https://app.dev.facewallet.xyz");
        }
    }
}