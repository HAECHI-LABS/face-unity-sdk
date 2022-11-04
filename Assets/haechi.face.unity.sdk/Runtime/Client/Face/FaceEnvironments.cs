using System;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [JsonObject]
    [Serializable]
    public class FaceEnvironments
    {
        [SerializeField] [JsonProperty("network")]
        internal string network;

        [SerializeField] [JsonProperty("env")] internal string env;

        [SerializeField] [JsonProperty("apiKey")]
        internal string apiKey;

        /// <summary>
        /// Environments used to initialize and connect with Face.
        /// </summary>
        /// <param name="network">Blockchain value such as Ethereum, Polygon etc.</param>
        /// <param name="env">Environment such as Testnet, Mainnet.</param>
        /// <param name="apiKey">Your api key received when register.</param>
        public FaceEnvironments(string network, string env, string apiKey)
        {
            this.network = network;
            this.env = env;
            this.apiKey = apiKey;
        }
    }
}