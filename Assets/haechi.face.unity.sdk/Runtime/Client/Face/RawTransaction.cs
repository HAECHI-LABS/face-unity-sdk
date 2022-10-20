using System;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [JsonObject]
    [Serializable]
    public class RawTransaction
    {
        [SerializeField] [JsonProperty("from")]
        internal string from;

        [SerializeField] [JsonProperty("to")] internal string to;

        [SerializeField] [JsonProperty("value")]
        internal string value;

        [SerializeField] [JsonProperty("data")]
        internal string data;

        [JsonConstructor]
        private RawTransaction()
        {
        }

        private RawTransaction(string from, string to, string value, string data)
        {
            this.from = from;
            this.to = to;
            this.value = value;
            this.data = data;
        }

        public static RawTransaction Of(string from, string to, string value, string data)
        {
            return new RawTransaction(from, to, value, data);
        }
    }
}