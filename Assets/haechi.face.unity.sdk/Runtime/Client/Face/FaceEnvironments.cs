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

        public FaceEnvironments(string network, string env, string apiKey)
        {
            this.network = network;
            this.env = env;
            this.apiKey = apiKey;
        }
    }
}