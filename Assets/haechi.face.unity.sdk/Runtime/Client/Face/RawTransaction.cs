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

        [SerializeField] [JsonProperty("gas")] internal string gas;

        [JsonConstructor]
        private RawTransaction()
        {
        }

        public RawTransaction(string from, string to, string value, string data, string gas)
        {
            this.from = from;
            this.to = to;
            this.value = value;
            this.data = data;
            this.gas = gas == "" ? null : gas;
        }
    }
}