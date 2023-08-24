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

        /// <summary>
        /// Raw transaction data using for send transaction, contract call etc.
        /// </summary>
        /// <param name="from">From address.</param>
        /// <param name="to">To address.</param>
        /// <param name="value">Blockchain value. Should be hex format.</param>
        /// <param name="data">Data string.</param>
        public RawTransaction(string from, string to, string value, string data)
        {
            this.from = string.IsNullOrEmpty(from) ? from : from.ToLower();
            this.to = string.IsNullOrEmpty(to) ? to : to.ToLower();
            this.value = value;
            this.data = data;
        }
    }
}