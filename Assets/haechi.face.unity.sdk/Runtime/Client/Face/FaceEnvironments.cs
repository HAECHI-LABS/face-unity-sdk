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

        [SerializeField] [JsonProperty("profile")]
        internal string profile;

        public FaceEnvironments(string network, string profile)
        {
            this.network = network;
            this.profile = profile;
        }
    }
}